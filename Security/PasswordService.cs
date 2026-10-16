using Microsoft.AspNetCore.Identity;
using RaceDay.Api.Models;
namespace RaceDay.Api.Security;
public class PasswordService {
 private readonly PasswordHasher<UserAccount> hasher=new();
 public string Hash(UserAccount user,string password)=>hasher.HashPassword(user,password);
 public bool Verify(UserAccount user,string password)=>hasher.VerifyHashedPassword(user,user.PasswordHash,password)!=PasswordVerificationResult.Failed;
}
