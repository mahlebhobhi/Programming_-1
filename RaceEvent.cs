using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.Models;
public class RaceEvent {
 public int EventID {get;set;}
 public int OrganiserID {get;set;}
 public Organiser? Organiser {get;set;}
 [Required,MaxLength(120)] public string EventName {get;set;}="";
 public DateTime EventDate {get;set;}
 [Required,MaxLength(150)] public string Location {get;set;}="";
 [MaxLength(500)] public string? Description {get;set;}
 [Required,MaxLength(20)] public string EventStatus {get;set;}="Open";
 public DateTime CreatedAt {get;set;}=DateTime.UtcNow;
 public List<EventCategory> EventCategories {get;set;}=new();
}
