using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.DTOs;
public class EventRequest {
 [Required,MaxLength(120)] public string EventName {get;set;}="";
 public DateTime EventDate {get;set;}
 [Required,MaxLength(150)] public string Location {get;set;}="";
 [MaxLength(500)] public string? Description {get;set;}
 [Required] public string EventStatus {get;set;}="Open";
}
public class CategoryRequest {
 [Range(1,int.MaxValue)] public int CategoryID {get;set;}
}
public class CreateCategoryRequest {
 [Required,MaxLength(80)] public string CategoryName {get;set;}="";
 [Range(typeof(decimal),"0.01","999.99")] public decimal DistanceKm {get;set;}
 [Range(0,120)] public int MinimumAge {get;set;}
 [Range(0,120)] public int? MaximumAge {get;set;}
}
public class EnrollmentRequest { [Range(1,int.MaxValue)] public int CategoryId {get;set;} }
public class ResultRequest {
 public int? FinishPosition {get;set;}
 public int? FinishTimeSeconds {get;set;}
 [Required] public string ResultStatus {get;set;}="Finished";
}
