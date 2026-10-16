using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Security;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api/categories")]
public class CategoriesController(RaceDayDbContext db,CurrentUser current):ControllerBase {
 [HttpGet] public async Task<IActionResult> All()=>Ok(await db.Categories.OrderBy(c=>c.DistanceKm).ToListAsync());
 [HttpPost] public async Task<IActionResult> Create(CreateCategoryRequest r){
  if(!current.IsAuthenticated)return Unauthorized();if(!current.IsRole("Organiser"))return Forbid();
  if(r.MaximumAge.HasValue&&r.MaximumAge<r.MinimumAge)return BadRequest(new{message="Maximum age cannot be below minimum age."});
  if(await db.Categories.AnyAsync(x=>x.CategoryName==r.CategoryName.Trim()))return Conflict(new{message="Category name already exists."});
  var c=new Category{CategoryName=r.CategoryName.Trim(),DistanceKm=r.DistanceKm,MinimumAge=r.MinimumAge,MaximumAge=r.MaximumAge};
  db.Categories.Add(c);await db.SaveChangesAsync();return Created("/api/categories",c);
 }
}
