using System.ComponentModel.DataAnnotations;

namespace MaybeExam
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Lesson> Lessons;
    }
}
