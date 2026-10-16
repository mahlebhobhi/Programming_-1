using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Security;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api")]
public class EnrollmentsController(RaceDayDbContext db,CurrentUser current):ControllerBase {
 [HttpPost("events/{id:int}/enrolments")] public async Task<IActionResult> Enrol(int id,EnrollmentRequest r){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Participant"))return Forbid();
  var p=await db.Participants.SingleOrDefaultAsync(x=>x.UserID==current.Id);if(p==null)return Forbid();
  var e=await db.Events.FindAsync(id);if(e==null)return NotFound(new{message="Event not found."});
  if(e.EventStatus!="Open")return Conflict(new{message="This event is not open for enrolment."});
  if(!await db.EventCategories.AnyAsync(x=>x.EventID==id&&x.CategoryID==r.CategoryId))return BadRequest(new{message="Category is not available for this event."});
  if(await db.Enrolments.AnyAsync(x=>x.ParticipantID==p.ParticipantID&&x.EventID==id&&x.CategoryID==r.CategoryId))return Conflict(new{message="Already enrolled in this category."});
  var en=new Enrollment{ParticipantID=p.ParticipantID,EventID=id,CategoryID=r.CategoryId,EnrolmentStatus="Registered"};
  db.Enrolments.Add(en);await db.SaveChangesAsync();return Created($"/api/enrolments/{en.EnrolmentID}",en);
 }
 [HttpGet("users/me/enrolments")] public async Task<IActionResult> Mine(){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Participant"))return Forbid();
  var p=await db.Participants.SingleOrDefaultAsync(x=>x.UserID==current.Id);if(p==null)return Forbid();
  return Ok(await db.Enrolments.Include(x=>x.EventCategory).ThenInclude(x=>x!.Category).Include(x=>x.EventCategory).ThenInclude(x=>x!.Event).Where(x=>x.ParticipantID==p.ParticipantID).ToListAsync());
 }
 [HttpGet("events/{id:int}/enrolments")] public async Task<IActionResult> ForEvent(int id){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Organiser"))return Forbid();
  var e=await db.Events.FindAsync(id);if(e==null)return NotFound();
  var org=await db.Organisers.SingleOrDefaultAsync(x=>x.UserID==current.Id);if(org==null||org.OrganiserID!=e.OrganiserID)return Forbid();
  return Ok(await db.Enrolments.Include(x=>x.Participant).ThenInclude(x=>x!.UserAccount).Include(x=>x.EventCategory).ThenInclude(x=>x!.Category).Where(x=>x.EventID==id).Select(x=>new{x.EnrolmentID,x.EnrolmentStatus,x.EnrolmentDate,Participant=x.Participant!.UserAccount!.FirstName+" "+x.Participant.UserAccount.LastName,Category=x.EventCategory!.Category!.CategoryName}).ToListAsync());
 }
 [HttpDelete("enrolments/{id:int}")] public async Task<IActionResult> Cancel(int id){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Participant"))return Forbid();
  var p=await db.Participants.SingleOrDefaultAsync(x=>x.UserID==current.Id);if(p==null)return Forbid();
  var en=await db.Enrolments.Include(x=>x.Result).FirstOrDefaultAsync(x=>x.EnrolmentID==id&&x.ParticipantID==p.ParticipantID);
  if(en==null)return NotFound();if(en.Result!=null||en.EnrolmentStatus=="Completed")return Conflict(new{message="Completed enrolments cannot be cancelled."});
  en.EnrolmentStatus="Cancelled";await db.SaveChangesAsync();return NoContent();
 }
}
