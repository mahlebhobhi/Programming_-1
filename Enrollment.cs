using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.Models;
public class Enrollment {
 public int EnrolmentID {get;set;}
 public int ParticipantID {get;set;}
 public Participant? Participant {get;set;}
 public int EventID {get;set;}
 public int CategoryID {get;set;}
 public EventCategory? EventCategory {get;set;}
 public DateTime EnrolmentDate {get;set;}=DateTime.UtcNow;
 [Required,MaxLength(20)] public string EnrolmentStatus {get;set;}="Registered";
 public RaceResult? Result {get;set;}
}
