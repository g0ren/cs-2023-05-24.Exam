using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace MaybeExam
{

    public class Program
    {
        static void ListTeachers()
        {
            using (MaybeDbContext db = new MaybeDbContext())
            {
                foreach (var line in
                    db.Teachers
                    .Select(p => new ValueTuple<int,string>(p.Id, p.Name))
                    .ToList())
                {
                    Console.WriteLine($"{line.Item1} {line.Item2}");
                }
            }
        }

        static void AddLesson(int teacherID, DateTime arrived, DateTime left)
        {
            using (MaybeDbContext db = new MaybeDbContext())
            {
                Teacher teacher = db.Teachers.First(t => t.Id == teacherID);
                Lesson lesson = new Lesson
                {
                    ArrivalTime = arrived,
                    LeaveTime = left,
                    Teacher = teacher
                };
                db.Lessons.Add(lesson);
                db.SaveChanges();
            }
        }

        static void AddLesssonToTeacher()
        {
            int teacherId;
            DateTime arrived;
            DateTime left;
            ListTeachers();
            Console.WriteLine("Which teacher you want to add a lesson to?");
            Console.WriteLine("Enter the teacher's ID");
            teacherId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter the teacher's arrival time");
            Console.WriteLine("In YYYY-MM-DD hh:mm or any other parseable format");
            arrived = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Enter the teacher's leave time");
            Console.WriteLine("In YYYY-MM-DD hh:mm or any other parseable format");
            left = DateTime.Parse(Console.ReadLine());
            AddLesson(teacherId, arrived, left);
        }

        static bool IsThisWeek(DateTime date)
        {
            DateTime today = DateTime.Today;
            int currentWeekNumber = GetWeekNumber(today);
            int targetWeekNumber = GetWeekNumber(date);
            return (today.Year == date.Year) && (currentWeekNumber == targetWeekNumber);
        }

        static int GetWeekNumber(DateTime date)
        {
            int dayOfWeek = (int)date.DayOfWeek;

            if (dayOfWeek == 0)
            {
                dayOfWeek = 7;
            }

            int weekNumber = (date.DayOfYear + (7 - dayOfWeek + 1)) / 7;
            return weekNumber;
        }

        static void ReportByWeek()
        {
            using (MaybeDbContext db = new MaybeDbContext())
            {
                var teachers = db.Teachers.ToList();
                foreach(var teacher in teachers)
                {
                    string teacherName = teacher.Name;
                    var lessons = db.Lessons
                        .ToList();
                    int minutes = lessons                        
                        .Where(v => v.Teacher == teacher)
                        .Where(v => 
                            IsThisWeek(v.ArrivalTime) && IsThisWeek(v.LeaveTime))
                        .Aggregate(0, (sum, val) =>
                            sum + (int)(val.LeaveTime - val.ArrivalTime).TotalMinutes);
                    Console.WriteLine($"{teacherName}\t{minutes}");
                }
            }
        }
        
        static void ReportByShift(List<Teacher> teachers, List<Lesson> lessons, int shiftStart, int shiftEnd)
        {
            foreach (var teacher in teachers)
            {
                var teacherName = teacher.Name;
                var minutes = lessons
                    .Where(v => v.Teacher == teacher)
                    .Where(v =>
                        IsThisWeek(v.ArrivalTime) && IsThisWeek(v.LeaveTime))
                    .Where(v => v.ArrivalTime.Hour >= shiftStart && v.ArrivalTime.Hour < shiftEnd)
                    .Aggregate(0, (sum, val) =>
                        sum + (int)(val.LeaveTime - val.ArrivalTime).TotalMinutes);
                Console.WriteLine($"{teacherName}\t{minutes}");
            }
        }

        static void ReportByShifts(List<Teacher> teachers, List<Lesson> lessons)
        {
            Console.WriteLine($"Shift from 0 to 12");
            ReportByShift(teachers, lessons, 0, 12);
            Console.WriteLine($"Shift from 12 to 18");
            ReportByShift(teachers, lessons, 12, 18);
            Console.WriteLine($"Shift from 18 to 24");
            ReportByShift(teachers, lessons, 18, 24);
        }

        static void ReportByWeekAndTimes()
        {
            using (MaybeDbContext db = new MaybeDbContext())
            {
                var teachers = db.Teachers.ToList();
                var lessons = db.Lessons.ToList();
                ReportByShifts(teachers, lessons);
            }
        }

        static void ReportByDayAndTimes()
        {
            using (MaybeDbContext db = new MaybeDbContext())
            {
                var teachers = db.Teachers.ToList();
                var lessons = db.Lessons.ToList();
                var monday = lessons
                    .Where(x => x.LeaveTime.DayOfWeek == DayOfWeek.Monday)
                    .ToList();
                var tuesday = lessons
                    .Where(x => x.LeaveTime.DayOfWeek == DayOfWeek.Tuesday)
                    .ToList();
                var wednesday = lessons
                    .Where(x => x.LeaveTime.DayOfWeek == DayOfWeek.Wednesday)
                    .ToList();
                var thursday = lessons
                    .Where(x => x.LeaveTime.DayOfWeek == DayOfWeek.Thursday)
                    .ToList();
                var friday = lessons
                    .Where(x => x.LeaveTime.DayOfWeek == DayOfWeek.Friday)
                    .ToList();
                var saturday = lessons
                    .Where(x => x.LeaveTime.DayOfWeek == DayOfWeek.Saturday)
                    .ToList();
                var sunday = lessons
                    .Where(x => x.LeaveTime.DayOfWeek == DayOfWeek.Sunday)
                    .ToList();
                Console.WriteLine("Monday");
                ReportByShifts(teachers, monday);
                Console.WriteLine("Tuesday");
                ReportByShifts(teachers, tuesday);
                Console.WriteLine("Wednesday");
                ReportByShifts(teachers, wednesday);
                Console.WriteLine("Thursday");
                ReportByShifts(teachers, thursday);
                Console.WriteLine("Friday");
                ReportByShifts(teachers, friday);
                Console.WriteLine("Saturday");
                ReportByShifts(teachers, saturday);
                Console.WriteLine("Sunday");
                ReportByShifts(teachers, sunday);
            }
        }

        static void TotalTimeThisWeek()
        {
            using (MaybeDbContext db = new MaybeDbContext())
            {
                var lessons = db.Lessons.ToList();
                int minutes =
                    lessons
                    .Where(v =>
                            IsThisWeek(v.ArrivalTime) && IsThisWeek(v.LeaveTime))
                    .Aggregate(0, (sum, val) => sum +
                    (int)(val.LeaveTime - val.ArrivalTime).TotalMinutes);
                Console.WriteLine($"Total minutes teaching this week: {minutes}");
            }
        }

        static void Menu()
        {
            int command = 0;
            while (true)
            {
                Console.WriteLine("Enter command");
                Console.WriteLine("1. List all teachers");
                Console.WriteLine("2. Add a lesson to teacher");
                Console.WriteLine("3. List teachers by total minutes spent teaching this week");
                Console.WriteLine("4. List teachers by total minutes spent teaching this week by shift");
                Console.WriteLine("5. List teachers by total minutes spent teaching this week by day and shift");
                Console.WriteLine("0. Exit");
                command = Convert.ToInt32(Console.ReadLine());
                switch (command)
                {
                    case 0:
                        return;
                    case 1:
                        ListTeachers();
                        break;
                    case 2:
                        AddLesssonToTeacher();
                        break;
                    case 3:
                        ReportByWeek();
                        TotalTimeThisWeek();
                        break;
                    case 4:
                        ReportByWeekAndTimes();
                        TotalTimeThisWeek();
                        break;
                    case 5:
                        ReportByDayAndTimes();
                        TotalTimeThisWeek();
                        break;
                    default:
                        Console.WriteLine("Unknown command!");
                        break;
                }
            }
        }

        public static void Main()
        {
            Menu();
        }
    }
}
