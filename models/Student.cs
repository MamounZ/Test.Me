using System.ComponentModel.DataAnnotations;

namespace Test.Me.models
{
    public class Student
    {
        [Key]
        public int Sid { get; set; }
        public string Sname { get; set; }
        public string Spassword { get; set; }
        public string Semail { get; set; }



    }

}
