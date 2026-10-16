using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.Models;
public class Organiser {
 public int OrganiserID {get;set;}
 public int UserID {get;set;}
 public UserAccount? UserAccount {get;set;}
 [Required,MaxLength(100)] public string OrganisationName {get;set;}="";
 public List<RaceEvent> Events {get;set;}=new();
}
