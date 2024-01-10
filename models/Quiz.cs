using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Me.models
{
    public class Quiz
    {
        [Key]
        public int Qid { get; set; } // Primary key

        [ForeignKey("Teacher")]
        public int Tid { get; set; } // Foreign key referencing the Teacher table

        public string Qname { get; set; }
        public DateTime Qdate { get; set; }
        public int Qtime { get; set; }


        // Navigation property to the Teacher table
        public Teacher Teacher { get; set; }
        public List<Question> Questions { get; set; }

    }
}
