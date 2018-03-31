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
        private void UpdateRoomStudents(Session session, Room room, Exam exam1, List<Student> stud1, int capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                RoomStudent temp = new RoomStudent();
                temp.Exam = exam1;
                temp.Session = session;
                temp.Student = stud1.First();
                temp.Room = room;
                room.RoomStudents.Add(temp);
                if (room.Exams.Contains(exam1) == false)
                    room.Exams.Add(exam1); 
                db.SaveChanges();
                stud1.Remove(stud1.First());

            }
        }
        private void RandomAllot(Session session)
        {
            List<Exam> exams = session.Exams.ToList();
            List<Student> stud1 = new List<Student>();
            int? a = null;
            int check = 0;
            foreach (var room in db.Rooms.ToList())
            {
                while(room.RoomStatus != 0)
                {
                    int capacity = (room.Capacity.GetValueOrDefault() - db.RoomStudents.Where(x => x.Room_Id == room.Id && x.Session_Id == session.Id).ToList().Count());
                    foreach (var exam in exams)
                    {
                        stud1 = exam.Students.ToList();
                        List<Student> temp = db.Students.Where(x => x.RoomStudents.Where(t => t.exam_id == exam.Id && t.Session_Id == session.Id).Count() == 0).ToList();
                        stud1.RemoveAll(x => temp.Contains(x) == false);
                        if (room.Capacity != capacity)
                        {
                            if (room.RoomStudents.Where(x => x.Exam.Code == exam.Code).Count() < room.Capacity / 2)
                            {
                                if (stud1.Count() >= capacity)
                                {
                                    UpdateRoomStudents(session, room, exam, stud1, capacity);
                                    capacity = 0;
                                    break;
                                }
                                if (capacity >= stud1.Count())
                                {
                                    UpdateRoomStudents(session, room, exam, stud1, stud1.Count());
                                    capacity = capacity - stud1.Count();
                                    a = exam.Id;
                                    break;
                                }
                            }
                            else
                                check = 1;
                        }
                        else
                        {
                            if (room.RoomStudents.Where(x => x.Exam.Code == exam.Code).Count() < room.Capacity / 2)
                            {
                                if (stud1.Count() >= capacity/2)
                                {
                                    UpdateRoomStudents(session, room, exam, stud1, capacity/2);
                                    capacity = 0;
                                    break;
                                }
                                if (capacity/2 >= stud1.Count())
                                {
                                    UpdateRoomStudents(session, room, exam, stud1, stud1.Count());
                                    capacity = capacity - stud1.Count();
                                    a = exam.Id;
                                    break;
                                }
                            }
                            else
                                check = 1;
                        }
                    }
                    if (capacity == 0)
                        room.RoomStatus = 0;
                    if (stud1.Count() == 0)
                    {
                        exams.RemoveAll(x => x.Id == a);
                    }
                    if (exams.Count() == 0)
                        break;
                    if (check == 1)
                        break;
                }
            }
        }
       
        private void AllotStudentsInSessionWritingExamToRoom(Session session, Exam exam1 , Exam exam2 , List<Department> DeptList1 , List<Department> DeptList2 )
        {
            foreach (var room in db.Rooms.ToList())
            {
                List<Student> stud1 = new List<Student>();
                List<Student> stud2 = new List<Student>();
                if (exam1 != null)
                {
                    stud1 = exam1.Students.ToList();
                    List<Student> temp = db.Students.Where(x => x.RoomStudents.Where(t => t.exam_id == exam1.Id && t.Session_Id == session.Id).Count() == 0).ToList();
                    stud1.RemoveAll(x => temp.Contains(x) == false);
                }
                if (exam2 != null)
                {
                    stud2 = exam2.Students.ToList();
                    List<Student> temp = db.Students.Where(x => x.RoomStudents.Where(t => t.exam_id == exam2.Id && t.Session_Id == session.Id).Count() == 0).ToList();
                    stud2.RemoveAll(x => temp.Contains(x) == false);
                }
                int capacity = (room.Capacity.GetValueOrDefault() - db.RoomStudents.Where(x => x.Room_Id == room.Id && x.Session_Id == session.Id).ToList().Count()) / 2;
                if (room.RoomStatus == 1 && capacity != 0  )
                {
                    if (stud1.Count() != 0)
                    {
                        if (room.Exams.Where(x => x.Code == exam1.Code).Count() == 0)
                        {
                            foreach (var dept in DeptList1)
                            {
                                if (stud1.Where(x => x.Department.Id == dept.Id).ToList().Count() >= capacity)
                                {
                                    UpdateRoomStudents(session, room, exam1, stud1.Where(x => x.Department.Id == dept.Id).ToList(), capacity);
                                    break;
                                }
                            }
                        }
                    }
                    if (stud2.Count() != 0)
                    {
                        if (room.Exams.Where(x => x.Code == exam2.Code).Count() == 0)
                        {
                            foreach (var dept in DeptList2)
                            {
                                if (stud2.Where(x => x.Department.Id == dept.Id).ToList().Count() >= capacity)
                                {
                                    UpdateRoomStudents(session, room, exam2, stud2.Where(x => x.Department.Id == dept.Id).ToList(), capacity);
                                    break;
                                }
                            }
                        }
                    }
                }
                capacity = (room.Capacity.GetValueOrDefault() - db.RoomStudents.Where(x => x.Room_Id == room.Id && x.Session_Id == session.Id).ToList().Count());
                if (capacity == 0)
                    room.RoomStatus = 0;
                if (stud1.Count()==0 && stud2.Count()==0)
                {
                    return;
                }
            }

       }
       private void AllotStudentsInSessionWritingExam (Session session, Exam exam1 ,Exam exam2)
        {
            List<Department> DeptList1 = new List<Department>();
            List<Department> DeptList2 = new List<Department>();
            List<Student> stud2 = new List<Student>();
            List<Student> stud1 = new List<Student>();
            if (exam1 != null)
            {
                stud1 = exam1.Students.ToList();
            }
            if (exam2 != null)
            {
                stud2 = exam2.Students.ToList();
            }
            foreach(var dept in db.Departments.ToList())
            {
                if(stud1.Where(x => x.Department.Id == dept.Id).Count() != 0 )
                {
                    DeptList1.Add(dept);
                }
                if (stud2.Where(x => x.Department.Id == dept.Id).Count() != 0)
                {
                    DeptList2.Add(dept);
                }
            }
            AllotStudentsInSessionWritingExamToRoom(session, exam1, exam2,  DeptList1, DeptList2);
            return;
                
        }
        private void AllotStudentsInSession(Session session)
        {
            List<Exam> ExamGroup1 = new List<Exam>();
            List<Exam> ExamGroup2 = new List<Exam>();
            int count = 0;
            foreach(var exam in session.Exams.ToList())
            {
                if(count % 2==0)
                {
                    ExamGroup1.Add(exam);
                }
                else
                {
                    ExamGroup2.Add(exam);
                }
                count++;
            }
            while (true)
            {
                Exam exam1 = ExamGroup1.FirstOrDefault();
                Exam exam2 = ExamGroup2.FirstOrDefault();
                AllotStudentsInSessionWritingExam(session, exam1, exam2);
                ExamGroup1.Remove(exam1);
                ExamGroup2.Remove(exam2);
                if (ExamGroup1.Count() == 0 && ExamGroup2.Count() == 0)
                    break;
            }
            RandomAllot(session);
            return;
        }
        public int Allot()
        {
            foreach(var session in db.Sessions.ToList())
            {
                AllotStudentsInSession(session);
                foreach(var room in db.Rooms.ToList())
                {
                    room.RoomStatus = 1;
                }
                db.SaveChanges();
            }
            return 0;
        }
           

        
    }
}