using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Test.Me.models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Quiz> Quiz { get; set; }
    public DbSet<Question> Question { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student_Question> Student_Question { get; set; }
    public DbSet<Student_Quiz> Student_Quiz { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure composite primary key for Student_Question
        modelBuilder.Entity<Student_Question>()
            .HasKey(sq => new { sq.Sid, sq.Quid });

        // Configure composite primary key for Student_Quiz
        modelBuilder.Entity<Student_Quiz>()
            .HasKey(sq => new { sq.Sid, sq.Qid });
    }

}
