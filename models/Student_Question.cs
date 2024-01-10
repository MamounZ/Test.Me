using System.ComponentModel.DataAnnotations;
using Test.Me.models;

public class Student_Question
{
    [Key]
    public int Sid { get; set; }  // Foreign key referencing the Students table
    [Key]
    public int Quid { get; set; }  // Foreign key referencing the Question table
    public string Sanswer { get; set; }
    public string Answrestate { get; set; }

    // Navigation properties if needed
    public Student Student { get; set; }
    public Question Question { get; set; }
}