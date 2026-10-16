using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.Models;
public class UserAccount {
 public int UserID {get;set;}
 [Required,MaxLength(50)] public string FirstName {get;set;}="";
 [Required,MaxLength(50)] public string LastName {get;set;}="";
 [Required,EmailAddress,MaxLength(100)] public string Email {get;set;}="";
 [Required,MaxLength(255)] public string PasswordHash {get;set;}="";
 [MaxLength(20)] public string? PhoneNumber {get;set;}
 [Required,MaxLength(20)] public string UserRole {get;set;}="Participant";
 public DateTime CreatedAt {get;set;}=DateTime.UtcNow;
 public Organiser? Organiser {get;set;}
 public Participant? Participant {get;set;}
}
