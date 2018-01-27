using ExamRoomAllocation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamRoomAllocation.Helpers

{
    public class StudentHelpher
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        private List<Session> ListOfSessions()
        {
            try
            {
                return db.Sessions.ToList();
            }
            catch (NullReferenceException)
            {
                throw;
                // to insert alert no session available
            }
        }
        private List<Exam> ExamInSession(Session session)
        {
            try
            {
                Student student = new Student();
                return db.Exam.Where(c => c.SessionId == session.Id).ToList();
            }
            catch (NullReferenceException)
            {
                throw;
                // to insert alert no exam in session available
            }
        }

        private List<Room> Room()

        {
            try
            {
                db.Rooms.OrderBy(t => t.No);
                return db.Rooms.ToList();
            }
            catch (NullReferenceException)
            {
                throw;
                // to insert alert no exam in session available
            }
        }

        private int RoomToStudent(int i,List<Student> stud1,Room room,Session session)
        {
            int k=0;
            RoomStudent r1 = new RoomStudent();
            while (k < i)
            {
                Student student = stud1.FirstOrDefault();
                List<RoomStudent> roomstudent = db.RoomStudents.ToList();
                foreach(var rs in roomstudent)
                {
                    if (rs.Student.Id == student.Id && rs.Session_Id == session.Id)
                    {
                        goto Skip;
                    }
                }
                    r1.Room_Id = room.Id;
                    r1.Session_Id = session.Id;
                //string jhi = student.Id;
                    r1.Student_Id = student.Id;
                    room.RoomStudents.Add(r1);
                    db.SaveChanges();
                    k++;
                Skip: stud1.RemoveAll(e => e.Id == student.Id);

            }
            return 0;
        }




        public int Index()
        {

            List<Session> sessions = ListOfSessions();
            List<Student> students = db.Students.ToList();


            foreach (var session in sessions)
            {
                List<Exam> examgroup1 = ExamInSession(session).Where(e => e.Id % 2 == 0).ToList();
                List<Exam> examgroup2 = ExamInSession(session).Where(e => e.Id % 2 != 0).ToList();
                List<Room> rooms = Room();
                while (true)
                {
                    Exam exam = examgroup1.FirstOrDefault();
                    Exam exam1 = examgroup2.FirstOrDefault();
                    Room room = rooms.FirstOrDefault();
                    int seats = room.Capacity.GetValueOrDefault(); 
                    if (examgroup1.Count() != 0 && examgroup2.Count() != 0 && rooms.Count != 0)
                    {
                        int i = exam.Students.Count();
                        int j = exam1.Students.Count();
                        List<Student> stud1 = exam.Students.ToList();
                        List<Student> stud2 = exam1.Students.ToList();
                        
                        if (room.RoomStatus != 0)
                        {
                            int seatsinone = seats / 2;
                            int seatsintwo = seats / 2;
                            if (i > seatsinone)
                            {
                                RoomToStudent(seatsinone, stud1, room, session);
                                room.Exams.Add(exam);
                                db.SaveChanges();
                                i = i - seatsinone;
                                seatsinone = 0;
                                seats = seats - seatsinone;
                            }
                            if (j > seatsintwo)
                            {
                                RoomToStudent(seatsintwo, stud2, room, session);
                                room.Exams.Add(exam1);
                                db.SaveChanges();
                                j = j - seatsintwo;
                                seatsintwo = 0;
                                seats = seats - seatsintwo;
                            }
                            if (i <= seatsinone)
                            {
                                RoomToStudent(i, stud1, room, session);
                                room.Exams.Add(exam);
                                db.SaveChanges();
                                seatsinone = seatsinone - i;
                                seats = seats - i;
                                i = 0;
                            }
                            if (j <= seatsintwo)
                            {
                                RoomToStudent(j, stud2, room, session);
                                room.Exams.Add(exam);
                                db.SaveChanges();
                                seatsintwo = seatsintwo - j;
                                seats = seats - j;
                                j = 0;
                            }
                            
                            if (i == 0)
                            {
                                examgroup1.RemoveAll(e => e.Code == exam.Code);
                            }
                            if (j == 0)
                            {
                                examgroup2.RemoveAll(e => e.Code == exam1.Code);
                            }
                        }
                    }
                    if (examgroup1.Count() == 0 && examgroup2.Count() != 0 && rooms.Count != 0)
                    {
                        int i = exam1.Students.Count();
                        List<Student> stud1 = exam1.Students.ToList();
                       
                        if (exam1.Students.Count() <= seats)
                        {
                            RoomToStudent(exam1.Students.Count(), stud1, room, session);
                            room.Exams.Add(exam1);
                            db.SaveChanges();
                            break;
                        }
                        if (exam1.Students.Count() > seats)
                        {
                            RoomToStudent(seats, stud1, room, session);
                            room.Exams.Add(exam1);
                            db.SaveChanges();
                            room.RoomStatus = 0;
                            i = i - room.Capacity.GetValueOrDefault();
                            seats = 0;
                        }
                    }
                    if (examgroup2.Count() == 0 && examgroup1.Count() != 0 && rooms.Count != 0)
                    {
                        int i = exam.Students.Count();
                        List<Student> stud1 = exam.Students.ToList();
                        if (exam.Students.Count() <= seats)
                        {
                            RoomToStudent(exam.Students.Count(), stud1, room, session);
                            room.Exams.Add(exam);
                            db.SaveChanges();
                            break;
                        }
                        if (exam1.Students.Count() > seats)
                        {
                            RoomToStudent(seats, stud1, room, session);
                            room.Exams.Add(exam);
                            db.SaveChanges();
                            room.RoomStatus = 0;
                            i = i - room.Capacity.GetValueOrDefault();
                            seats = 0;
                        }
                    }
                    if (examgroup1.Count() == 0 && examgroup2.Count() == 0)
                    {
                        break;
                    }
                    if (seats == 0)
                    {
                        room.RoomStatus = 0;
                        rooms.RemoveAll(r => r.No == room.No);
                    }
                    if (rooms.Count == 0)
                    {
                        //retrun error message no rooms available
                        break;
                    }
                }
            }
            return 0;
        }
    }
}