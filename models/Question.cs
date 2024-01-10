using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Me.models
{
    public class Question
    {
        [Key]
        public int Quid { get; set; } // Primary key

        [ForeignKey("Quiz")]
        public int Qid { get; set; } // Foreign key referencing the Quiz table
        public string Qutext { get; set; }
        public string Firstop { get; set; }
        public string Secondop { get; set; }
        public string Thirdop { get; set; }
        public string Fourthop { get; set; }
        public string Qurightanswer { get; set; }
        public int Qumark { get; set; }
        public bool Qutype { get; set; } // "MultipleChoice" or "SimpleAnswer"
        public Quiz Quiz { get; set; }


    }
}