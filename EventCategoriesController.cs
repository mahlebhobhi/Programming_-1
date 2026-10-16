using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Security;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api/events/{id:int}/categories")]
public class EventCategoriesController(RaceDayDbContext db,CurrentUser current):ControllerBase {
 [HttpGet] public async Task<IActionResult> Get(int id){
  if(!await db.Events.AnyAsync(x=>x.EventID==id))return NotFound();
  return Ok(await db.EventCategories.Include(x=>x.Category).Where(x=>x.EventID==id).Select(x=>x.Category).ToListAsync());
 }
 [HttpPost] public async Task<IActionResult> Add(int id,CategoryRequest r){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Organiser"))return Forbid();
  var e=await db.Events.FindAsync(id);if(e==null)return NotFound();
  var org=await db.Organisers.SingleOrDefaultAsync(x=>x.UserID==current.Id);if(org==null||org.OrganiserID!=e.OrganiserID)return Forbid();
  if(!await db.Categories.AnyAsync(x=>x.CategoryID==r.CategoryID))return NotFound(new{message="Category not found."});
  if(await db.EventCategories.AnyAsync(x=>x.EventID==id&&x.CategoryID==r.CategoryID))return Conflict(new{message="Category already added to event."});
  db.EventCategories.Add(new EventCategory{EventID=id,CategoryID=r.CategoryID});await db.SaveChangesAsync();
  return CreatedAtAction(nameof(Get),new{id},new{EventID=id,CategoryID=r.CategoryID});
 }
 [HttpDelete("{categoryId:int}")] public async Task<IActionResult> Remove(int id,int categoryId){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Organiser"))return Forbid();
  var e=await db.Events.FindAsync(id);if(e==null)return NotFound();
  var org=await db.Organisers.SingleOrDefaultAsync(x=>x.UserID==current.Id);if(org==null||org.OrganiserID!=e.OrganiserID)return Forbid();
  var link=await db.EventCategories.Include(x=>x.Enrolments).FirstOrDefaultAsync(x=>x.EventID==id&&x.CategoryID==categoryId);
  if(link==null)return NotFound();if(link.Enrolments.Any())return Conflict(new{message="Cannot remove a category that has enrolment records."});
  db.EventCategories.Remove(link);await db.SaveChangesAsync();return NoContent();
 }
}
