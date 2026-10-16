using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Security;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api/events")]
public class EventsController(RaceDayDbContext db,CurrentUser current):ControllerBase {
 [HttpGet] public async Task<IActionResult> GetAll()=>Ok(await db.Events.Include(e=>e.EventCategories).ThenInclude(ec=>ec.Category).OrderBy(e=>e.EventDate).ToListAsync());
 [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id){var e=await db.Events.Include(x=>x.EventCategories).ThenInclude(x=>x.Category).FirstOrDefaultAsync(x=>x.EventID==id);return e==null?NotFound():Ok(e);}
 [HttpPost] public async Task<IActionResult> Create(EventRequest r){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Organiser"))return Forbid();
  if(!new[]{"Open","Closed","Completed"}.Contains(r.EventStatus))return BadRequest(new{message="EventStatus must be Open, Closed or Completed."});
  var org=await db.Organisers.SingleOrDefaultAsync(x=>x.UserID==current.Id);if(org==null)return Forbid();
  var e=new RaceEvent{OrganiserID=org.OrganiserID,EventName=r.EventName.Trim(),EventDate=r.EventDate.Date,Location=r.Location.Trim(),Description=r.Description,EventStatus=r.EventStatus};
  db.Events.Add(e);await db.SaveChangesAsync();return CreatedAtAction(nameof(Get),new{id=e.EventID},e);
 }
 [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id,EventRequest r){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Organiser"))return Forbid();
  var e=await db.Events.FindAsync(id);if(e==null)return NotFound();
  var org=await db.Organisers.SingleOrDefaultAsync(x=>x.UserID==current.Id);if(org==null||e.OrganiserID!=org.OrganiserID)return Forbid();
  if(!new[]{"Open","Closed","Completed"}.Contains(r.EventStatus))return BadRequest(new{message="Invalid event status."});
  e.EventName=r.EventName.Trim();e.EventDate=r.EventDate.Date;e.Location=r.Location.Trim();e.Description=r.Description;e.EventStatus=r.EventStatus;
  await db.SaveChangesAsync();return Ok(e);
 }
 [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Organiser"))return Forbid();
  var e=await db.Events.FirstOrDefaultAsync(x=>x.EventID==id);if(e==null)return NotFound();
  var org=await db.Organisers.SingleOrDefaultAsync(x=>x.UserID==current.Id);if(org==null||e.OrganiserID!=org.OrganiserID)return Forbid();
  if(e.EventStatus=="Completed"||await db.Enrolments.AnyAsync(x=>x.EventID==id))return Conflict(new{message="Cannot delete a completed event or an event that has enrolment records."});
  db.Events.Remove(e);await db.SaveChangesAsync();return NoContent();
 }
}
