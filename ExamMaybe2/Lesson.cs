using System.ComponentModel.DataAnnotations;

namespace MaybeExam
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime LeaveTime { get; set; }
        public Teacher Teacher { get; set; }
    }
}
