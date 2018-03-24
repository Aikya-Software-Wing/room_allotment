using ExamRoomAllocation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamRoomAllocation.Helpers

{
    public class StudentHelper
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
                return db.Exams.Where(c => c.SessionId == session.Id).ToList();
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

        private int RoomToStudent(int SplitSeats,List<Student> ListOfStudents,Room room,Session session)
        {
            int Check=0;
            
            while (Check < SplitSeats)
            {
                RoomStudent r1 = new RoomStudent();
                Student student = ListOfStudents.FirstOrDefault();
                if(ListOfStudents.Count()==0)
                {
                    return 0;
                }
                List<RoomStudent> RoomStudent = db.RoomStudents.ToList();
                foreach(var rs in RoomStudent)
                {
                    if (rs.Student.Id == student.Id && rs.Session_Id == session.Id)
                    {
                        goto Skip;
                    }
                }
                    r1.Room_Id = room.Id;
                    r1.Session_Id = session.Id;
                    r1.Student_Id = student.Id;
                    room.RoomStudents.Add(r1);
                    db.SaveChanges();
                ListOfStudents.RemoveAll(e => e.Id == student.Id);
                Check++;
                Skip: ListOfStudents.RemoveAll(e => e.Id == student.Id);

            }
            return 0;
        }

        public int Index()
        {

            List<Session> Sessions = ListOfSessions();
            List<Student> Students = db.Students.ToList();
            foreach (var session in Sessions)
            {
                List<Exam> ExamGroup = ExamInSession(session).ToList();
                List<Exam> ExamGroup1 = new List<Exam>() ;
                List<Exam> ExamGroup2 = new List<Exam>() ;
                int ITemp = 0;
                foreach(var exam in ExamGroup)
                {if (ITemp % 2 == 0)
                    {
                        ExamGroup1.Add(exam);
                    }
                    else
                    { ExamGroup2.Add(exam); }
                    ITemp++;
                }
                
                List<Room> Rooms = Room();
                while (true)
                {
                    List<RoomStudent> Temp = db.RoomStudents.ToList();
                    Exam exam = ExamGroup1.FirstOrDefault();
                    Exam exam1 = ExamGroup2.FirstOrDefault();
                    Room room = Rooms.FirstOrDefault();
                    int Seats = 0;
                    if (Rooms.Count == 0)
                    {
                        //retrun error message no rooms available
                        break;
                    }
                    if (room.RoomStatus != 0)
                    {
                       Seats = room.Capacity.GetValueOrDefault()- room.RoomStudents.Where(e =>e.Session_Id== session.Id).Count() ;
                    }
                    else
                    {
                        Rooms.RemoveAll(r => r.No == room.No);
                        continue;
                    }
                    /*if (exam1 != null)
                    {
                        List<Exam> exams = room.Exams.Where(e => e.Code == exam1.Code && e.SessionId == session.Id).ToList();
                        if (exams.Count() != 0)
                        {
                            exam1 = ExamGroup1.Find(e => e.Id != exam1.Id);
                           
                        }
                    }
                    if (exam != null)
                    {
                        List<Exam> exams1 = room.Exams.Where(e => e.Code == exam.Code && e.SessionId == session.Id).ToList();
                        if (exams1.Count() != 0)
                        {
                            exam = ExamGroup2.Find(e => e.Id != exam.Id);
                           
                        }
                    }*/
                    if (exam != null && exam1 != null && Rooms.Count != 0)
                    {
                        int Group1Count = exam.Students.Count();
                        int Group2Count = exam1.Students.Count();
                        List<Student> StudentGroup1 = exam.Students.OrderBy(d => d.DepartmentId).ToList();
                        List<Student> StudentGroup2 = exam1.Students.OrderBy(d => d.DepartmentId).ToList();
                        foreach (var Temp1 in Temp)
                        {
                            
                            StudentGroup1.RemoveAll(s => s.Id == Temp1.Student_Id && Temp1.Session_Id == session.Id);
                            StudentGroup2.RemoveAll(s => s.Id == Temp1.Student_Id && Temp1.Session_Id == session.Id);
                            
                        }
                        Group1Count = StudentGroup1.Count();
                        Group2Count = StudentGroup2.Count();
                        if (room.RoomStatus != 0)//have to reset to default values
                        {
                            int SeatsInOne = 0;
                            int SeatsInTwo = 0;
                            if (Seats % 2 == 0)
                            {
                                 SeatsInOne = Seats / 2;
                                 SeatsInTwo = Seats / 2;
                            }
                            else if(Seats % 2 !=0)
                            {
                                SeatsInOne = Seats / 2 + 1;
                                SeatsInTwo = Seats / 2;
                            }
                            
                            
                            if (Group1Count > SeatsInOne)
                            {
                                RoomToStudent(SeatsInOne, StudentGroup1, room, session);
                                room.Exams.Add(exam);
                                db.SaveChanges();
                                Group1Count = Group1Count - SeatsInOne;
                                Seats = Seats - SeatsInOne;
                                SeatsInOne = 0;
                                
                            }
                            if (Group2Count > SeatsInTwo)
                            {
                                RoomToStudent(SeatsInTwo, StudentGroup2, room, session);
                                room.Exams.Add(exam1);
                                db.SaveChanges();
                                Group2Count = Group2Count - SeatsInTwo;
                                Seats = Seats - SeatsInTwo;
                                SeatsInTwo = 0;
                                
                            }
                            if (Group1Count <= SeatsInOne)
                            {
                                RoomToStudent(Group1Count, StudentGroup1, room, session);
                                room.Exams.Add(exam);
                                db.SaveChanges();
                                SeatsInOne = SeatsInOne - Group1Count;
                                Seats = Seats - Group1Count;
                                Group1Count = 0;
                            }
                            if (Group2Count <= SeatsInTwo)
                            {
                                RoomToStudent(Group2Count, StudentGroup2, room, session);
                                room.Exams.Add(exam1);
                                db.SaveChanges();
                                SeatsInTwo = SeatsInTwo - Group2Count;
                                Seats = Seats - Group2Count;
                                Group2Count = 0;
                            }
                            if (Seats == 0)
                            {
                                room.RoomStatus = 0;
                            }

                            if (Group1Count == 0 && Group2Count==0 )
                            {
                                ExamGroup1.RemoveAll(e => e.Code == exam.Code);
                                ExamGroup2.RemoveAll(e => e.Code == exam1.Code);
                                continue;
                            }
                            else if (Group2Count == 0)
                            {
                                ExamGroup2.RemoveAll(e => e.Code == exam1.Code);
                                continue;
                            }
                            else if( Group1Count==0 )
                            {
                                ExamGroup1.RemoveAll(e => e.Code == exam.Code);
                                continue;
                            }
                        }
                       
                    }
                    if (exam == null && exam1 != null && Rooms.Count != 0)
                    {
                        int GroupCount = exam1.Students.Count();
                        Seats = Seats / 2;
                        List<Student> StudentGroup = exam1.Students.ToList();
                        foreach (var Temp1 in Temp)
                        {
                            StudentGroup.RemoveAll(s => s.Id == Temp1.Student_Id && Temp1.Session_Id == session.Id);
                        }
                        
                        GroupCount = StudentGroup.Count();
                        if (GroupCount <= Seats && GroupCount != 0)
                        {
                            RoomToStudent(GroupCount, StudentGroup, room, session);
                            room.Exams.Add(exam1);
                            db.SaveChanges();
                            room.RoomStatus = 0;
                            GroupCount = 0;
                            Seats = 0;
                        }
                        if (GroupCount > Seats && GroupCount != 0)
                        {
                            RoomToStudent(Seats, StudentGroup, room, session);
                            room.Exams.Add(exam1);
                            db.SaveChanges();
                            room.RoomStatus = 0;
                            GroupCount = GroupCount - room.Capacity.GetValueOrDefault();
                            Seats = 0;
                        }
                        if (Seats == 0)
                        {
                            room.RoomStatus = 0;

                        }
                        if(GroupCount ==0)
                        {
                            ExamGroup2.RemoveAll(e => e.Id == exam1.Id);
                        }
                    }
                    if (exam1 == null && exam != null && Rooms.Count != 0)
                    {
                        Seats = Seats / 2;
                        int GroupCount = exam.Students.Count();
                        List<Student> StudentGroup = exam.Students.ToList();
                        foreach (var Temp1 in Temp)
                        {
                            StudentGroup.RemoveAll(s => s.Id == Temp1.Student_Id && Temp1.Session_Id == session.Id);
                        }
                        GroupCount = StudentGroup.Count();
                        if (GroupCount <= Seats && GroupCount != 0)
                        {
                            RoomToStudent(GroupCount, StudentGroup, room, session);
                            room.Exams.Add(exam);
                            db.SaveChanges();
                            room.RoomStatus = 0 ;
                            GroupCount = 0;
                            Seats = 0 ;
                        }
                        if (GroupCount > Seats && GroupCount != 0)
                        {
                            RoomToStudent(Seats, StudentGroup, room, session);
                            room.Exams.Add(exam);
                            db.SaveChanges();
                            room.RoomStatus = 0;
                            GroupCount = GroupCount - room.Capacity.GetValueOrDefault();
                            Seats = 0;
                        }
                        if (Seats == 0)
                        {
                            room.RoomStatus = 0;

                        } 
                        if(GroupCount==0)
                        {
                            ExamGroup1.RemoveAll(e => e.Id == exam.Id);
                        }
                    }
                    if (exam == null && exam1 == null)
                    {
                        break;
                    }
                   
                   
                   
                }
                List<Room> RoomTemp = db.Rooms.ToList();
                foreach(var room in RoomTemp)
                {
                    room.RoomStatus = 1;
                    db.SaveChanges();
                }
            }
            return 0;
        }
    }
}