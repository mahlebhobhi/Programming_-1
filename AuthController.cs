using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Security;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api/auth")]
public class AuthController(RaceDayDbContext db,PasswordService passwords):ControllerBase {
 [HttpPost("register")]
 public async Task<IActionResult> Register(RegisterRequest request) {
  string role=request.Role.Trim();
  if(role!="Organiser" && role!="Participant") return BadRequest(new{message="Role must be Organiser or Participant."});
  string email=request.Email.Trim().ToLowerInvariant();
  if(await db.UserAccounts.AnyAsync(x=>x.Email==email)) return Conflict(new{message="Email already registered."});
  var user=new UserAccount {FirstName=request.FirstName.Trim(),LastName=request.LastName.Trim(),Email=email,PhoneNumber=request.PhoneNumber?.Trim(),UserRole=role};
  user.PasswordHash=passwords.Hash(user,request.Password);
  db.UserAccounts.Add(user);
  await db.SaveChangesAsync();
  if(role=="Organiser") db.Organisers.Add(new Organiser {UserID=user.UserID,OrganisationName=string.IsNullOrWhiteSpace(request.OrganisationName)?$"{user.FirstName} {user.LastName} Organisation":request.OrganisationName.Trim()});
  else db.Participants.Add(new Participant {UserID=user.UserID,EmergencyContactName=request.EmergencyContactName,EmergencyContactPhone=request.EmergencyContactPhone});
  await db.SaveChangesAsync();
  return Created("/api/users/me",new UserResponse(user.UserID,user.FirstName,user.LastName,user.Email,user.PhoneNumber,user.UserRole));
 }
 [HttpPost("login")]
 public async Task<IActionResult> Login(LoginRequest request) {
  var user=await db.UserAccounts.SingleOrDefaultAsync(x=>x.Email==request.Email.Trim().ToLowerInvariant());
  if(user==null || !passwords.Verify(user,request.Password)) return Unauthorized(new{message="Invalid email or password."});
  HttpContext.Session.Clear(); HttpContext.Session.SetInt32("UserID",user.UserID); HttpContext.Session.SetString("UserRole",user.UserRole);
  return Ok(new UserResponse(user.UserID,user.FirstName,user.LastName,user.Email,user.PhoneNumber,user.UserRole));
 }
 [HttpPost("logout")] public IActionResult Logout(){HttpContext.Session.Clear();return Ok(new{message="Logged out successfully."});}
}
