using System;
using ExamRoomAllocation.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExamRoomAllocation.Controllers;

namespace ExamRoomAllocation.Helpers
{
    public class TeacherToRoom
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        private List<Session> ListOfSessions(DateTime CurrentDate)
        {
            try
            {
                string Temp = SessionHelper.TimeHelpher(CurrentDate);

                return db.Sessions.Where(s => s.Name.Remove(s.Name.Length - 1) == Temp).ToList();
            }
            catch (NullReferenceException)
            {

                // to insert alert no session available
                throw;
            }
        }
        private List<Exam> ExamInRoom(Int32 roomid, Int32 sessionid)
        {
            try
            {
                //here i wamt to return the exams conducted in the particular room in the particular session. 
                List<Exam> ExamInRoom = db.Exams.Where(x => x.SessionId == sessionid).ToList();
                List<Exam> FinalList = new List<Exam>();
                Room room = db.Rooms.Find(roomid);
                foreach (var exam in ExamInRoom)
                {
                    if (exam.Rooms.Contains(room))
                    {
                        FinalList.Add(exam);
                    }
                    
                }
                return FinalList;
            }
            catch (NullReferenceException)
            {

                // to insert alert no exam in session available
                throw;
            }
        }

        private List<Teacher> TeacherNotInSameDept(List<Exam> Examsinroom)
        {
            try
            {
                List<Teacher> temp = db.Teachers.Where(t=>t.TeacherPriority > 0).ToList();                
                foreach (var exam in Examsinroom)
                {
                    temp.RemoveAll(t => t.Department_Id == exam.Id);
                }
                return temp;
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

            DateTime StartDate = db.Exams.Min(e => e.Date).GetValueOrDefault();
            DateTime EndDate = db.Exams.Max(e => e.Date).GetValueOrDefault();

            while (DateTime.Compare(StartDate, EndDate) <= 0)
            {

                List<Teacher> TeacherAssignedInTheSameDate = new List<Teacher>();
                List<Session> Sessions = ListOfSessions(StartDate);
                foreach (var session in Sessions.ToList())
                {
                    List<RoomStudent> r1 = db.RoomStudents.Where(e => e.Session_Id == session.Id).ToList();
                    List<Room> RoomConductingExamInSession = new List<Room>();
                    foreach (var roomstudent in r1 )                           
                    {
                        Room temp = new Room();
                        temp.Id = roomstudent.Room_Id;
                        Room temp1 = RoomConductingExamInSession.Find(r => r.Id == temp.Id);
                        if (temp1 == null)
                        { RoomConductingExamInSession.Add(temp); }
                    }

                    foreach (var Room in RoomConductingExamInSession.ToList())
                    {
                        List<Exam> ExaminRoom = ExamInRoom(Room.Id,session.Id);
                        List<Teacher> TeacherNotInSamedept = TeacherNotInSameDept(ExaminRoom);
                        TeacherNotInSamedept.OrderByDescending(e => e.TeacherPriority).OrderByDescending(e => e.Experience);
                        foreach (var Teacher in TeacherNotInSamedept)
                        {

                            if (!(TeacherAssignedInTheSameDate.Contains(Teacher)))
                            {
                                int Count = Teacher.Exams.Count();
                                if (Count <= 8)
                                {
                                    TeacherRoom Tr1 = new TeacherRoom();
                                    foreach (var exam in ExaminRoom)
                                    {
                                        Teacher.Exams.Add(exam);
                                    }
                                    Tr1.Session_Id = session.Id;
                                    Tr1.Room_Id = Room.Id;
                                    Tr1.Teacher_Id = Teacher.Id;
                                    Teacher.TeacherRooms.Add(Tr1);
                                    db.SaveChanges();
                                    TeacherAssignedInTheSameDate.Add(Teacher);
                                    break;
                                }
                            }
                        }
                            foreach (var Teacher1 in TeacherNotInSamedept)
                            {

                                if (!(TeacherAssignedInTheSameDate.Contains(Teacher1)))
                                {
                                    int Count = Teacher1.Exams.Count();
                                    if (Count <= 8)
                                    {
                                        TeacherRoom Tr1 = new TeacherRoom();
                                        foreach (var exam in ExaminRoom)
                                        {
                                            Teacher1.Exams.Add(exam);
                                        }
                                        Tr1.Session_Id = session.Id;
                                        Tr1.Room_Id = Room.Id;
                                        Tr1.Teacher_Id = Teacher1.Id;
                                        Teacher1.TeacherRooms.Add(Tr1);
                                        db.SaveChanges();
                                        TeacherAssignedInTheSameDate.Add(Teacher1);
                                        break;
                                    }
                                }

                            }
                        RoomConductingExamInSession.RemoveAll(r => r.No == Room.No);
                    }
                    Sessions.RemoveAll(s => s.Id == session.Id);
                }
                StartDate = StartDate.AddDays(1);
            }
            return 0;
        }
    }
}