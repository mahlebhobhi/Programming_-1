namespace RaceDay.Api.Models;
public class EventCategory {
 public int EventID {get;set;}
 public RaceEvent? Event {get;set;}
 public int CategoryID {get;set;}
 public Category? Category {get;set;}
 public List<Enrollment> Enrolments {get;set;}=new();
}
