using ExamRoomAllocation.Models;
using System.Linq;
using System.Collections.Generic;
using System;

namespace ExamRoomAllocation.Helpers
{
    public class StudentHelper
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        private Allotment FindBestFitForExam(Exam exam)
        {
            int temp = db.RoomStudents.Where(x => x.exam_id == exam.Id && x.Session_Id == exam.SessionId).Count();
            // int temp = db.Students.Where(x => x.RoomStudents.Where(y => y.Room.Exams.Where(z => z.Id == exam.Id && z.SessionId == exam.SessionId).Count() != 0).Count() != 0).Count();
            int studentsWritingExam = exam.Students.Count - temp;
            List<Allotment> allotments = new List<Allotment>();
            System.Diagnostics.Debug.WriteLine("exam id = " + exam.Id + " students writing = " + studentsWritingExam + " temp = " + temp);

            foreach (var room in db.Rooms.ToList())
            {
                int numberOfExamsInRoom = room.Exams.Where(x => x.SessionId == exam.SessionId).Count();
                int studentsInRoom = db.RoomStudents.Where(x => x.Room_Id == room.Id && x.Session_Id == exam.SessionId).Count();
                int roomCapacity = (numberOfExamsInRoom == 0) ? (room.Capacity.Value / 2) : (room.Capacity.Value - studentsInRoom);
                System.Diagnostics.Debug.WriteLine("room id = " + room.Id + " room capacity = " + roomCapacity + " students in room = " + studentsInRoom);

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

            System.Diagnostics.Debug.WriteLine("----------");

            return allotments.OrderByDescending(x => x.NumberOfStudents).ThenByDescending(x => x.ExamIndexInRoom).First();
        }

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

        private void AllotExamToRoom(Room room, Exam exam)
        {
            db.Rooms.Where(x => x.Id == room.Id).First().Exams.Add(exam);
            db.SaveChanges();
        }

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

        public void AllotStduentsToRooms()
        {
            foreach (var session in db.Sessions.ToList())
            {
                AllotStudentsForSession(session);
            }
        }
    }

    class RoomModelWithAlgorithmParameters
    {
        public int Id { get; set; }

        public int Capacity { get; set; }

        public int NumberOfDepartments { get; set; }
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