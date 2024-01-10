using System.ComponentModel.DataAnnotations;

namespace Test.Me.models
{
    public class Teacher
    {
        [Key]
        public int Tid { get; set; }
        public string Tname { get; set; }
        public string Tpassword { get; set; }
        public string Temail { get; set; }
    }
}
