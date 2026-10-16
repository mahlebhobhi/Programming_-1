using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.DTOs;
public class RegisterRequest {
 [Required,MaxLength(50)] public string FirstName {get;set;}="";
 [Required,MaxLength(50)] public string LastName {get;set;}="";
 [Required,EmailAddress,MaxLength(100)] public string Email {get;set;}="";
 [Required,MinLength(8)] public string Password {get;set;}="";
 [MaxLength(20)] public string? PhoneNumber {get;set;}
 [Required] public string Role {get;set;}="Participant";
 [MaxLength(100)] public string? OrganisationName {get;set;}
 [MaxLength(100)] public string? EmergencyContactName {get;set;}
 [MaxLength(20)] public string? EmergencyContactPhone {get;set;}
}
public class LoginRequest {
 [Required,EmailAddress] public string Email {get;set;}="";
 [Required] public string Password {get;set;}="";
}
public record UserResponse(int UserID,string FirstName,string LastName,string Email,string? PhoneNumber,string UserRole);
public class UpdateProfileRequest {
 [Required,MaxLength(50)] public string FirstName {get;set;}="";
 [Required,MaxLength(50)] public string LastName {get;set;}="";
 [MaxLength(20)] public string? PhoneNumber {get;set;}
}
