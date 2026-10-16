using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Security;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api")]
public class ResultsController(RaceDayDbContext db,CurrentUser current):ControllerBase {
 [HttpPost("enrolments/{id:int}/result")] public async Task<IActionResult> Record(int id,ResultRequest r){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Organiser"))return Forbid();
  var en=await db.Enrolments.Include(x=>x.EventCategory).ThenInclude(x=>x!.Event).FirstOrDefaultAsync(x=>x.EnrolmentID==id);
  if(en==null)return NotFound(new{message="Enrolment not found."});
  var org=await db.Organisers.SingleOrDefaultAsync(x=>x.UserID==current.Id);if(org==null||org.OrganiserID!=en.EventCategory!.Event!.OrganiserID)return Forbid();
  if(!new[]{"Finished","DidNotFinish","Disqualified"}.Contains(r.ResultStatus))return BadRequest(new{message="Invalid result status."});
  if(r.FinishPosition.HasValue&&r.FinishPosition<=0)return BadRequest(new{message="Finish position must be positive."});
  if(r.FinishTimeSeconds.HasValue&&r.FinishTimeSeconds<=0)return BadRequest(new{message="Finish time must be positive."});
  if(await db.Results.AnyAsync(x=>x.EnrolmentID==id))return Conflict(new{message="A result already exists. Use PUT to correct it."});
  var result=new RaceResult{EnrolmentID=id,FinishPosition=r.FinishPosition,FinishTimeSeconds=r.FinishTimeSeconds,ResultStatus=r.ResultStatus};
  db.Results.Add(result);en.EnrolmentStatus="Completed";await db.SaveChangesAsync();return Created($"/api/enrolments/{id}/result",result);
 }
 [HttpPut("enrolments/{id:int}/result")] public async Task<IActionResult> Update(int id,ResultRequest r){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Organiser"))return Forbid();
  var result=await db.Results.Include(x=>x.Enrolment).ThenInclude(x=>x!.EventCategory).ThenInclude(x=>x!.Event).FirstOrDefaultAsync(x=>x.EnrolmentID==id);
  if(result==null)return NotFound();
  var org=await db.Organisers.SingleOrDefaultAsync(x=>x.UserID==current.Id);if(org==null||org.OrganiserID!=result.Enrolment!.EventCategory!.Event!.OrganiserID)return Forbid();
  if(!new[]{"Finished","DidNotFinish","Disqualified"}.Contains(r.ResultStatus))return BadRequest(new{message="Invalid result status."});
  result.FinishPosition=r.FinishPosition;result.FinishTimeSeconds=r.FinishTimeSeconds;result.ResultStatus=r.ResultStatus;await db.SaveChangesAsync();return Ok(result);
 }
 [HttpGet("events/{id:int}/results")] public async Task<IActionResult> EventResults(int id){
  if(!await db.Events.AnyAsync(x=>x.EventID==id))return NotFound();
  return Ok(await db.Results.Include(x=>x.Enrolment).ThenInclude(x=>x!.Participant).ThenInclude(x=>x!.UserAccount).Include(x=>x.Enrolment).ThenInclude(x=>x!.EventCategory).ThenInclude(x=>x!.Category)
   .Where(x=>x.Enrolment!.EventID==id).OrderBy(x=>x.FinishPosition).Select(x=>new{x.ResultID,Participant=x.Enrolment!.Participant!.UserAccount!.FirstName+" "+x.Enrolment.Participant.UserAccount.LastName,Category=x.Enrolment.EventCategory!.Category!.CategoryName,x.FinishPosition,x.FinishTimeSeconds,x.ResultStatus}).ToListAsync());
 }
 [HttpGet("events/{id:int}/results/{categoryId:int}")] public async Task<IActionResult> CategoryResults(int id,int categoryId){
  if(!await db.EventCategories.AnyAsync(x=>x.EventID==id&&x.CategoryID==categoryId))return NotFound();
  return Ok(await db.Results.Include(x=>x.Enrolment).ThenInclude(x=>x!.Participant).ThenInclude(x=>x!.UserAccount).Where(x=>x.Enrolment!.EventID==id&&x.Enrolment.CategoryID==categoryId).OrderBy(x=>x.FinishPosition).Select(x=>new{x.ResultID,Participant=x.Enrolment!.Participant!.UserAccount!.FirstName+" "+x.Enrolment.Participant.UserAccount.LastName,x.FinishPosition,x.FinishTimeSeconds,x.ResultStatus}).ToListAsync());
 }
 [HttpGet("users/me/results")] public async Task<IActionResult> Mine(){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Participant"))return Forbid();
  var p=await db.Participants.SingleOrDefaultAsync(x=>x.UserID==current.Id);if(p==null)return Forbid();
  return Ok(await db.Results.Include(x=>x.Enrolment).ThenInclude(x=>x!.EventCategory).ThenInclude(x=>x!.Event).Include(x=>x.Enrolment).ThenInclude(x=>x!.EventCategory).ThenInclude(x=>x!.Category).Where(x=>x.Enrolment!.ParticipantID==p.ParticipantID).Select(x=>new{x.ResultID,Event=x.Enrolment!.EventCategory!.Event!.EventName,Category=x.Enrolment.EventCategory.Category!.CategoryName,x.FinishPosition,x.FinishTimeSeconds,x.ResultStatus}).ToListAsync());
 }
}
