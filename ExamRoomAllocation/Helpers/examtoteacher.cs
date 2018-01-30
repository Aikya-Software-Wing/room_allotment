using System;
using ExamRoomAllocation.Models;
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

                return db.Sessions.Where(s => s.Name.Remove(s.Name.Length - 1) != temp).ToList();
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
                return db.Exam.Where(c => c.SessionId == session.Id).ToList();
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
                return db.Teachers.Where(t => t.Department_Id != exam.Id).ToList();
            }
            catch (NullReferenceException)
            {
                
                // to insert alert no teacher available
                throw;
            }
        }



        public int Index()
        {
            db.Exam.OrderBy(e => e.Date);
           
            DateTime startdate = db.Exam.Min(e => e.Date).GetValueOrDefault();
            DateTime enddate = db.Exam.Max(e => e.Date).GetValueOrDefault();
            
            while (DateTime.Compare(startdate, enddate) < 0)
            {
                
                List<Teacher> teacherassignedinthesamedate = new List<Teacher>(); 
                List<Session> sessions = listofsessions(startdate);
                foreach (var session in sessions.ToList())
                {
                    List<Exam> ExamInSession = examinsession(session);
                    foreach (var exam in ExamInSession.ToList())
                    {
                        List<Room> RoomconductingExam = exam.Rooms.ToList();
                       foreach (var room in RoomconductingExam.ToList())
                        {
                            List<Teacher> TeacherNotInSamedept = teachernotinsamedept(exam);
                            TeacherNotInSamedept.OrderByDescending(e => e.TeacherPriority);
                          foreach (var teacher in TeacherNotInSamedept.ToList() )
                          {
                            
                                if (!(teacherassignedinthesamedate.Contains(teacher)))
                                {
                                    int count = teacher.Exams.Count();
                                    if (count <= 8)
                                    {
                                        teacher.Exams.Add(exam);
                                    db.SaveChanges();
                                    teacherassignedinthesamedate.Add(teacher);
                                    break;
                                    }
                                }
                            
                          }
                            RoomconductingExam.RemoveAll(r => r.No == room.No);
                       }
                        ExamInSession.RemoveAll(e => e.Code == exam.Code);
                    }
                    sessions.RemoveAll(s => s.Id == session.Id);
                }
                startdate = startdate.AddDays(1);
            }
            return 0;
        }
    }
}