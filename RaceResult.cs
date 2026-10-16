using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.Models;
public class RaceResult {
 public int ResultID {get;set;}
 public int EnrolmentID {get;set;}
 public Enrollment? Enrolment {get;set;}
 public int? FinishPosition {get;set;}
 public int? FinishTimeSeconds {get;set;}
 [Required,MaxLength(20)] public string ResultStatus {get;set;}="Finished";
 public DateTime RecordedAt {get;set;}=DateTime.UtcNow;
}
