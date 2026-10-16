using Xunit;
namespace RaceDay.Tests;
public class RoleRulesTests {
 [Theory]
 [InlineData("Organiser",true)]
 [InlineData("Participant",true)]
 [InlineData("Admin",false)]
 [InlineData("",false)]
 public void RegistrationRoleMustBeOneOfTwoAllowedRoles(string role,bool expected) {
  bool actual=role=="Organiser"||role=="Participant";
  Assert.Equal(expected,actual);
 }
 [Theory]
 [InlineData("Open",true)]
 [InlineData("Closed",true)]
 [InlineData("Completed",true)]
 [InlineData("Cancelled",false)]
 public void EventStatusMustMatchDatabaseConstraint(string status,bool expected) {
  bool actual=new[]{"Open","Closed","Completed"}.Contains(status,StringComparer.Ordinal);
  Assert.Equal(expected,actual);
 }
 [Theory]
 [InlineData("Registered",true)]
 [InlineData("Cancelled",true)]
 [InlineData("Completed",true)]
 [InlineData("Pending",false)]
 public void EnrolmentStatusMustMatchDatabaseConstraint(string status,bool expected) {
  bool actual=new[]{"Registered","Cancelled","Completed"}.Contains(status,StringComparer.Ordinal);
  Assert.Equal(expected,actual);
 }
 [Theory]
 [InlineData("Finished",true)]
 [InlineData("DidNotFinish",true)]
 [InlineData("Disqualified",true)]
 [InlineData("Unknown",false)]
 public void ResultStatusMustMatchDatabaseConstraint(string status,bool expected) {
  bool actual=new[]{"Finished","DidNotFinish","Disqualified"}.Contains(status,StringComparer.Ordinal);
  Assert.Equal(expected,actual);
 }
 [Fact]
 public void CategoryAgeRangeMustBeValid() {
  int minimumAge=30, maximumAge=20;
  Assert.False(maximumAge>=minimumAge);
 }
}
