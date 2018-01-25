using System;
using ExamRoomAllocation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExamRoomAllocation.Controllers;

namespace ExamRoomAllocation.Helpers
{
    public class examtoteacher
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        private List<Session> listofsessions(DateTime currendate)
        {
            try
            {
                string temp = SessionHelper.TimeHelpher(currendate);
                return db.Sessions.Where(s => s.Name.Remove(0, s.Name.Length - 1) != temp).ToList();
            }
            catch (NullReferenceException)
            {

                // to insert alert no session available
                throw;
            }
        }
        private List<Exam> examinsession(Session session)
        {
            try
            {
                return db.Exams.Where(c => c.SessionId == session.Id).ToList();
            }
            catch (NullReferenceException)
            {

                // to insert alert no exam in session available
                throw;
            }
        }

        private List<Teacher> teachernotinsamedept(Exam exam)
        {
            try
            {
                return db.Teachers.Where(t => t.Department != exam.Department).ToList();
            }
            catch (NullReferenceException)
            {
                // to insert alert no teacher available
                throw;
            }
        }



        public int Index()
        {
            db.Exams.OrderBy(e => e.Date);
            Exam last = db.Exams.LastOrDefault();
            Exam first = db.Exams.FirstOrDefault();
            DateTime startdate = first.Date.GetValueOrDefault();
            DateTime enddate = last.Date.GetValueOrDefault();

            while (DateTime.Compare(startdate, enddate) < 0)
            {
                List<Teacher> teacherassignedinthesamedate = new List<Teacher>(); 
                List<Session> sessions = listofsessions(startdate);
                foreach (var session in sessions)
                {
                    List<Exam> ExamInSession = examinsession(session);
                    foreach (var exam in ExamInSession)
                    {
                        List<Room> RoomconductingExam = exam.Rooms.ToList();
                        foreach (var room in RoomconductingExam)
                        {
                            List<Teacher> TeacherNotInSamedept = teachernotinsamedept(exam);
                            TeacherNotInSamedept.OrderByDescending(e => e.TeacherPriority);
                            foreach (var teacher in TeacherNotInSamedept)
                            {
                                
                                int count = teacher.Exams.Count();
                                if (count <= 8)
                                {
                                    teacher.Exams.Add(exam);
                                    break;
                                }
                            }
                            RoomconductingExam.RemoveAll(r => r.No == room.No);
                        }
                        ExamInSession.RemoveAll(e => e.Code == exam.Code);
                    }
                    sessions.RemoveAll(s => s.Id == session.Id);
                }
                startdate.AddDays(1);
            }
            return 0;
        }
    }
}