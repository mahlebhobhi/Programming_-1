using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.Models;
public class Participant {
 public int ParticipantID {get;set;}
 public int UserID {get;set;}
 public UserAccount? UserAccount {get;set;}
 [MaxLength(100)] public string? EmergencyContactName {get;set;}
 [MaxLength(20)] public string? EmergencyContactPhone {get;set;}
 public List<Enrollment> Enrolments {get;set;}=new();
}
