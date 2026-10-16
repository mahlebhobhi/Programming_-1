using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
namespace RaceDay.Api.Data;
public class RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : DbContext(options) {
 public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
 public DbSet<Organiser> Organisers => Set<Organiser>();
 public DbSet<Participant> Participants => Set<Participant>();
 public DbSet<RaceEvent> Events => Set<RaceEvent>();
 public DbSet<Category> Categories => Set<Category>();
 public DbSet<EventCategory> EventCategories => Set<EventCategory>();
 public DbSet<Enrollment> Enrolments => Set<Enrollment>();
 public DbSet<RaceResult> Results => Set<RaceResult>();
 protected override void OnModelCreating(ModelBuilder b) {
  b.Entity<UserAccount>().ToTable("UserAccount").HasKey(x=>x.UserID);
  b.Entity<UserAccount>().HasIndex(x=>x.Email).IsUnique();
  b.Entity<Organiser>().ToTable("Organiser").HasKey(x=>x.OrganiserID);
  b.Entity<Organiser>().HasIndex(x=>x.UserID).IsUnique();
  b.Entity<Participant>().ToTable("Participant").HasKey(x=>x.ParticipantID);
  b.Entity<Participant>().HasIndex(x=>x.UserID).IsUnique();
  b.Entity<RaceEvent>().ToTable("Event").HasKey(x=>x.EventID);
  b.Entity<Category>().ToTable("Category").HasKey(x=>x.CategoryID);
  b.Entity<Category>().HasIndex(x=>x.CategoryName).IsUnique();
  b.Entity<EventCategory>().ToTable("EventCategory").HasKey(x=>new{x.EventID,x.CategoryID});
  b.Entity<Enrollment>().ToTable("Enrolment").HasKey(x=>x.EnrolmentID);
  b.Entity<Enrollment>().HasIndex(x=>new{x.ParticipantID,x.EventID,x.CategoryID}).IsUnique();
  b.Entity<RaceResult>().ToTable("Result").HasKey(x=>x.ResultID);
  b.Entity<RaceResult>().HasIndex(x=>x.EnrolmentID).IsUnique();
  b.Entity<UserAccount>().HasOne(x=>x.Organiser).WithOne(x=>x.UserAccount).HasForeignKey<Organiser>(x=>x.UserID).OnDelete(DeleteBehavior.Cascade);
  b.Entity<UserAccount>().HasOne(x=>x.Participant).WithOne(x=>x.UserAccount).HasForeignKey<Participant>(x=>x.UserID).OnDelete(DeleteBehavior.Cascade);
  b.Entity<Organiser>().HasMany(x=>x.Events).WithOne(x=>x.Organiser).HasForeignKey(x=>x.OrganiserID).OnDelete(DeleteBehavior.Restrict);
  b.Entity<EventCategory>().HasOne(x=>x.Event).WithMany(x=>x.EventCategories).HasForeignKey(x=>x.EventID);
  b.Entity<EventCategory>().HasOne(x=>x.Category).WithMany(x=>x.EventCategories).HasForeignKey(x=>x.CategoryID);
  b.Entity<Enrollment>().HasOne(x=>x.Participant).WithMany(x=>x.Enrolments).HasForeignKey(x=>x.ParticipantID).OnDelete(DeleteBehavior.Restrict);
  b.Entity<Enrollment>().HasOne(x=>x.EventCategory).WithMany(x=>x.Enrolments).HasForeignKey(x=>new{x.EventID,x.CategoryID});
  b.Entity<RaceResult>().HasOne(x=>x.Enrolment).WithOne(x=>x.Result).HasForeignKey<RaceResult>(x=>x.EnrolmentID).OnDelete(DeleteBehavior.Cascade);
  b.Entity<RaceEvent>().Property(x=>x.EventDate).HasColumnType("date");
  b.Entity<RaceEvent>().Property(x=>x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
  b.Entity<UserAccount>().Property(x=>x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
  b.Entity<Enrollment>().Property(x=>x.EnrolmentDate).HasDefaultValueSql("SYSDATETIME()");
  b.Entity<RaceResult>().Property(x=>x.RecordedAt).HasDefaultValueSql("SYSDATETIME()");
  b.Entity<Category>().Property(x=>x.DistanceKm).HasColumnType("decimal(5,2)");
 }
}
