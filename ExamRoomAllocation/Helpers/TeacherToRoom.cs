using System;
using ExamRoomAllocation.Models;
using System.Collections.Generic;
using System.Linq;

namespace ExamRoomAllocation.Helpers
{
    public class TeacherToRoom
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        private List<Session> ListOfSessions(DateTime CurrentDate)
        {
            try
            {
                string Temp = SessionHelper.TimeHelper(CurrentDate);

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
                List<Teacher> temp = db.Teachers.Where(t=>t.TeacherPriority != 0).ToList();
                List<Teacher> temp1 = db.Teachers.Where(t => t.TeacherPriority != 0).ToList();
                foreach (var exam in Examsinroom)
                {
                    temp.RemoveAll(t => t.Department_Id == exam.DepartmentId);
                }
                if (temp.Count()!=0)
                {
                    return temp;
                }
                else
                    return temp1;
            }
            catch (NullReferenceException)
            {

                // to insert alert no teacher available
                throw;
            }
        }

        private int Assist(List<Teacher> TeacherNotInSamedept, List<Teacher> TeacherAssignedInTheSameDate, List<Exam> ExaminRoom, Session session, Room room,double duties)
        {
            foreach (var Teacher in TeacherNotInSamedept)
            {
                if (Teacher.TeacherRooms.Where(t=>t.Session.Id == session.Id).Count() == 0 &&Teacher.TeacherRooms.Where(t=>t.Room.Id== room.Id).Count() ==0 && Teacher.TeacherPriority ==1)
                {
                    //  if (!(TeacherAssignedInTheSameDate.Contains(Teacher)))
                    // {
                    int Count = Teacher.TeacherRooms.Count();
                    if (Count <= duties)
                    {
                        TeacherRoom Tr1 = new TeacherRoom();
                        foreach (var exam in ExaminRoom)
                        {
                            Teacher.Exams.Add(exam);
                        }
                        Tr1.Session_Id = session.Id;
                        Tr1.Room_Id = room.Id;
                        Tr1.Teacher_Id = Teacher.Id;
                        Teacher.TeacherRooms.Add(Tr1);
                        db.SaveChanges();
                        TeacherAssignedInTheSameDate.Add(Teacher);
                        return 0;
                    }
                    // }
                }
            }
            return 1;
        }

        private void Relieve(Session session,Room room,List<Teacher> TeacherNotInSamedept,List<Teacher> TeacherAssignedInTheSameDate,List<Exam> ExaminRoom)
        {
            foreach(var Teacher in TeacherNotInSamedept)
            {
                if(Teacher.TeacherPriority == -1 )
                {
                    int Count = Teacher.TeacherRooms.Where(t=>t.Session.Id == session.Id).ToList().Count();
                    if (Count < 5)
                    {
                        TeacherRoom Tr1 = new TeacherRoom();
                        foreach (var exam in ExaminRoom)
                        {
                            Teacher.Exams.Add(exam);
                        }
                        Tr1.Session_Id = session.Id;
                        Tr1.Room_Id = room.Id;
                        Tr1.Teacher_Id = Teacher.Id;
                        Teacher.TeacherRooms.Add(Tr1);
                        db.SaveChanges();
                        TeacherAssignedInTheSameDate.Add(Teacher);
                        break;
                    }
                }
            }
        }
        public int Index()
        {
            db.Exams.OrderBy(e => e.Date);

            DateTime StartDate = db.Exams.Min(e => e.Date).GetValueOrDefault();
            DateTime EndDate = db.Exams.Max(e => e.Date).GetValueOrDefault();

            while (DateTime.Compare(StartDate, EndDate) <= 0)
            {
                int check;
                List<Teacher> TeacherAssignedInTheSameDate = new List<Teacher>();
                List<Session> Sessions = ListOfSessions(StartDate);
                double duties = Math.Floor((double)(db.Rooms.Count() * db.Sessions.ToList().Count()) / db.Teachers.Where(t => t.TeacherPriority == 1).ToList().Count());
                foreach (var session in Sessions.ToList())
                {
                    List<Room> RoomConductingExamInSession = db.Rooms.Where(x => x.RoomStudents.Where(t=>t.Session_Id == session.Id).Count() !=0).ToList();
                    foreach (var room in RoomConductingExamInSession.ToList())
                    {
                        
                        List<Exam> ExaminRoom = ExamInRoom(room.Id,session.Id);
                        List<Teacher> TeacherNotInSamedept = TeacherNotInSameDept(ExaminRoom);
                        Relieve(session, room, TeacherNotInSamedept,TeacherAssignedInTheSameDate,ExaminRoom);
                        check = Assist(TeacherNotInSamedept, TeacherAssignedInTheSameDate, ExaminRoom, session, room,duties);
                        if (check == 1)
                            Assist(TeacherNotInSamedept, TeacherAssignedInTheSameDate, ExaminRoom, session, room, duties + 1);
                        int studentsInRoom = db.RoomStudents.Where(r => r.Session_Id == session.Id && r.Room_Id == room.Id).Count();
                        if (studentsInRoom > 32)
                        {
                          check = Assist(TeacherNotInSamedept, TeacherAssignedInTheSameDate, ExaminRoom, session, room,duties);
                            if (check == 1)
                                Assist(TeacherNotInSamedept, TeacherAssignedInTheSameDate, ExaminRoom, session, room, duties + 1);
                        }
                        RoomConductingExamInSession.RemoveAll(r => r.No == room.No);
                    }
   
                    Sessions.RemoveAll(s => s.Id == session.Id);
                }
                StartDate = StartDate.AddDays(1);
            }
            return 0;
        }
    }
}