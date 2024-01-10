using System.ComponentModel.DataAnnotations;
using Test.Me.models;

public class Student_Quiz
{
    [Key]
    public int Sid { get; set; }  // Foreign key referencing the Students table
    [Key]
    public int Qid { get; set; }  // Foreign key referencing the Quiz table
    public int Smark { get; set; }

    // Navigation properties if needed
    public Student Student { get; set; }
    public Quiz Quiz { get; set; }
}