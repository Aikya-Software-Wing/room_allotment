using ExamRoomAllocation.Models;
using System.Linq;
using System.Collections.Generic;

namespace ExamRoomAllocation.Helpers
{
    public class StudentHelper
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        /// <summary>
        /// This method find the best room that the exam can be held in.
        /// The algorithm will look for the room that can fit the maximum number of students
        /// </summary>
        /// <param name="exam">The exam for which a room should be found</param>
        /// <returns>The allotment that fits best</returns>
        private Allotment FindBestFitForExam(Exam exam)
        {
            int studentsWritingExam = exam.Students.Count - db.RoomStudents.Where(x => x.exam_id == exam.Id && x.Session_Id == exam.SessionId).Count();
            List<Allotment> allotments = new List<Allotment>();

            foreach (var room in db.Rooms.ToList())
            {
                int numberOfExamsInRoom = room.Exams.Where(x => x.SessionId == exam.SessionId).Count();
                int studentsInRoom = db.RoomStudents.Where(x => x.Room_Id == room.Id && x.Session_Id == exam.SessionId).Count();
                int roomCapacity = (numberOfExamsInRoom == 0) ? (room.Capacity.Value / 2) : (room.Capacity.Value - studentsInRoom);

                if (roomCapacity > 0)
                {
                    if (!room.Exams.Contains(exam))
                    {
                        allotments.Add(new Allotment
                        {
                            ExamId = exam.Id,
                            RoomId = room.Id,
                            NumberOfStudents = (studentsWritingExam > roomCapacity) ? (roomCapacity) : (studentsWritingExam),
                            SessionId = exam.SessionId,
                            ExamIndexInRoom = room.Exams.Where(x => x.SessionId == exam.SessionId).Count() + 1
                        });
                    }
                }
            }

            return allotments.OrderByDescending(x => x.NumberOfStudents).ThenByDescending(x => x.ExamIndexInRoom).First();
        }

        /// <summary>
        /// This method will update the database table "RoomStudents"
        /// </summary>
        /// <param name="allotment">The allotment that has to be added</param>
        private void AllotStudentsToRoom(Allotment allotment)
        {
            List<Student> students = db.Students.Where(x => x.Exams.Where(y => y.Id == allotment.ExamId).Count() != 0 && x.RoomStudents.Where(y => y.exam_id == allotment.ExamId).Count() == 0).Take(allotment.NumberOfStudents).ToList();

            foreach (var student in students)
            {
                db.RoomStudents.Add(new RoomStudent
                {
                    Room_Id = allotment.RoomId,
                    Student_Id = student.Id,
                    Session_Id = allotment.SessionId,
                    exam_id = allotment.ExamId
                });
            }
            db.SaveChanges();
        }

        /// <summary>
        /// This method will update the table RoomExams
        /// </summary>
        /// <param name="room">The room that needs to be updated</param>
        /// <param name="exam">The exam that is held in the room</param>
        private void AllotExamToRoom(Room room, Exam exam)
        {
            db.Rooms.Where(x => x.Id == room.Id).First().Exams.Add(exam);
            db.SaveChanges();
        }

        /// <summary>
        /// This method will find the best fit rooms for all the exams in a session
        /// </summary>
        /// <param name="session">The session for which rooms have to be alloted</param>
        private void AllotStudentsForSession(Session session)
        {
            int studentsToAllot = session.Exams.Sum(x => x.Students.Count());
            do
            {
                foreach (var exam in session.Exams.ToList())
                {
                    Allotment allotment = FindBestFitForExam(exam);
                    AllotStudentsToRoom(allotment);
                    AllotExamToRoom(db.Rooms.Where(x => x.Id == allotment.RoomId).First(), exam);

                    studentsToAllot -= allotment.NumberOfStudents;
                }
            } while (studentsToAllot > 0);
        }

        /// <summary>
        /// This method is the starting point of the algorithm
        /// </summary>
        public void AllotStudentsToRooms()
        {
            foreach (var session in db.Sessions.ToList())
            {
                AllotStudentsForSession(session);
            }
        }
    }

    class Allotment
    {
        public int ExamId { get; set; }

        public int RoomId { get; set; }

        public int SessionId { get; set; }

        public int NumberOfStudents { get; set; }

        public int ExamIndexInRoom { get; set; }
    }
}