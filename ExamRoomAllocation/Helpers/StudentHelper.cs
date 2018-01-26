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
                    if (examgroup1.Count() != 0 && examgroup2.Count() != 0 && rooms.Count != 0)
                    {
                        int i = exam.Students.Count();
                        int j = exam1.Students.Count();
                        if (room.RoomStatus != 0)
                        {
                            int seatsinone = room.Capacity.GetValueOrDefault() / 2;
                            int seatsintwo = room.Capacity.GetValueOrDefault() / 2;
                            if (i > seatsinone)
                            {
                                room.Exams.Add(exam);
                                db.SaveChanges();
                                i = i - seatsinone;
                                seatsinone = 0;
                            }
                            if (j > seatsintwo)
                            {
                                room.Exams.Add(exam1);
                                db.SaveChanges();
                                j = j - seatsintwo;
                                seatsintwo = 0;
                            }
                            if (i < seatsinone)
                            {
                                room.Exams.Add(exam);
                                db.SaveChanges();
                                seatsinone = seatsinone - i;
                                i = 0;
                            }
                            if (j < seatsintwo)
                            {
                                room.Exams.Add(exam);
                                db.SaveChanges();
                                seatsintwo = seatsintwo - j;
                                j = 0;
                            }
                            if (seatsinone == 0 && seatsintwo == 0)
                            {
                                room.RoomStatus = 0;
                                rooms.RemoveAll(r => r.No == room.No);
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
                        if (exam1.Students.Count() < room.Capacity)
                        {
                            room.Exams.Add(exam1);
                            db.SaveChanges();
                            break;
                        }
                        if (exam1.Students.Count() > room.Capacity)
                        {
                            room.Exams.Add(exam1);
                            db.SaveChanges();
                            room.RoomStatus = 0;
                            i = i - room.Capacity.GetValueOrDefault();
                        }
                    }
                    if (examgroup2.Count() == 0 && examgroup1.Count() != 0 && rooms.Count != 0)
                    {
                        int i = exam.Students.Count();
                        if (exam.Students.Count() < room.Capacity)
                        {
                            room.Exams.Add(exam);
                            db.SaveChanges();
                            break;
                        }
                        if (exam1.Students.Count() > room.Capacity)
                        {
                            room.Exams.Add(exam);
                            db.SaveChanges();
                            room.RoomStatus = 0;
                            i = i - room.Capacity.GetValueOrDefault();
                        }
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