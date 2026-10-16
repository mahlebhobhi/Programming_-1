using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Security;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api/users")]
public class UsersController(RaceDayDbContext db,CurrentUser current):ControllerBase {
 [HttpGet("me")] public async Task<IActionResult> GetMe(){
  if(!current.IsAuthenticated)return Unauthorized();
  var u=await db.UserAccounts.FindAsync(current.Id); if(u==null)return NotFound();
  return Ok(new UserResponse(u.UserID,u.FirstName,u.LastName,u.Email,u.PhoneNumber,u.UserRole));
 }
 [HttpPut("me")] public async Task<IActionResult> UpdateMe(UpdateProfileRequest req){
  if(!current.IsAuthenticated)return Unauthorized();
  var u=await db.UserAccounts.FindAsync(current.Id);if(u==null)return NotFound();
  u.FirstName=req.FirstName.Trim();u.LastName=req.LastName.Trim();u.PhoneNumber=req.PhoneNumber?.Trim();
  await db.SaveChangesAsync();return Ok(new UserResponse(u.UserID,u.FirstName,u.LastName,u.Email,u.PhoneNumber,u.UserRole));
 }
}
