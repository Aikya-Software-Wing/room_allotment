using ExamRoomAllocation.Helpers;
using ExamRoomAllocation.Interfaces;
using ExamRoomAllocation.Models;
using ExamRoomAllocation.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.IO;
using System.Web;
using ClosedXML.Excel;
using System.Data;

namespace ExamRoomAllocation.Controllers
{
    [Authorize]
    public class ResultsController : Controller
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        // GET: Results
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Modify()
        {
            return View();
        }

        public ActionResult AllocateFirstYear()
        {
            TeacherToRoom assignExam = new TeacherToRoom();
            StudentHelper stud = new StudentHelper();
            stud.Allot();
            assignExam.Index();
            TempData["notice"] = "Success!";
            return RedirectToAction("Index");
        }

        public ActionResult AllocateSeniorYear()
        {
            TeacherToRoom assignExam = new TeacherToRoom();
            IAllotmentDriver driver = new StudentCountBasedAllotmentDriver();
            driver.DriveAllotmentAsync(db, new BestFitRoomAllotment(), new GreedyResultOptimizer()).Wait();
            assignExam.Index();
            TempData["notice"] = "Success!";
            return RedirectToAction("Index");
        }

        public ActionResult RoomsIndex()
        {
            var rooms = db.Rooms.ToList();
            return View(rooms);
        }

        public ActionResult RoomSession(int id)
        {
            int RoomId = id;
            TempData["ID"] = RoomId;
            var sessions = db.Sessions.ToList();
            return View(sessions);
        }

        public ActionResult RoomDetails(int sessionId)
        {
            RoomViewModel roomViewModel = new RoomViewModel();

            int RoomId = Convert.ToInt32(TempData["ID"]);
            Room room = db.Rooms.Find(RoomId);
            try
            {
                roomViewModel.BlockName = room.Block;
            }
            catch (NullReferenceException)
            {
                return View("Error");
            }
            roomViewModel.RoomNumber = room.No;
            roomViewModel.RoomId = room.Id;

            List<Student> studentRaw = new List<Student>();
            List<string> departments = new List<string>();
            var students = db.RoomStudents.Where(r => r.Room_Id == RoomId && r.Session_Id == sessionId).ToList();
            foreach (var student in students)
            {
                var studentInDb = db.Students.Where(s => s.Id == student.Student_Id).FirstOrDefault();
                studentRaw.Add(studentInDb);
                departments.Add(studentInDb.Department.Name.ToString());
            }
            var uniqueDepartments = new HashSet<string>(departments);
            roomViewModel.Students = studentRaw;
            roomViewModel.Departments = new List<string>(uniqueDepartments);

            var exams = db.Exams.Where(e => e.SessionId == sessionId).ToList();
            List<string> examsList = new List<string>();
            foreach (var exam in exams)
            {
                if (exam.Rooms.Contains(room))
                {
                    examsList.Add(exam.Code);
                }
            }
            roomViewModel.ExamCode = new List<string>(examsList);

            Exam examForDate = exams.First();
            roomViewModel.Date = examForDate.Date.Value.Date;
            roomViewModel.SessionTime = examForDate.ExamTime;

            var teachers = new List<Teacher>();
            var teacherRoom = db.TeacherRooms.Where(r => r.Room_Id == RoomId && r.Session_Id == sessionId).ToList();
            try
            {
                foreach (var enitity in teacherRoom)
                {
                    var id = enitity.Teacher_Id;
                    teachers.Add(db.Teachers.Find(id));
                }
            }
            catch (NullReferenceException)
            {
                return View("Error");
            }
            var uniqueTeachers = new HashSet<Teacher>(teachers);
            var temp = new List<Teacher>();
            foreach (var teacher in uniqueTeachers)
            {
                temp.Add(teacher);
            }
            roomViewModel.Teachers = temp;
            return View(roomViewModel);
        }

        public ActionResult TeacherIndex()
        {
            TeacherViewModel teacherViewModel = new TeacherViewModel();
            var teachers = db.TeacherRooms.OrderBy(s => s.Session.Name).ToList();
            var SessionList = new List<string>();
            var RoomsList = new List<string>();
            var NamesList = new List<string>();
            var examTimes = new List<string>();
            foreach (var t in teachers)
            {
                SessionList.Add(t.Session.Name);
                NamesList.Add(t.Teacher.Name);
                var roomString = t.Room.Block + '-' + t.Room.No;
                RoomsList.Add(roomString);
            }
            teacherViewModel.Sessions = SessionList;
            teacherViewModel.Rooms = RoomsList;
            teacherViewModel.Names = NamesList;
            return View(teacherViewModel);
        }

        public ActionResult AllocationDetails()
        {
            var exams = db.Exams.ToList();
            var rooms = db.Rooms.ToList();
            int totalDuties = 0;
            foreach (var exam in exams)
            {
                totalDuties += exam.Rooms.Count;
            }
            ViewData["totalDuties"] = totalDuties;
            return View(exams);
        }

        public FileResult Export()
        {
            ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[4] { new DataColumn("Teacher Name"),
                                            new DataColumn("Room No."),
                                            new DataColumn("Block"),
                                            new DataColumn("Session") });
            var TeacherRooms = db.TeacherRooms.ToList().OrderBy(t => t.Teacher_Id);
            foreach (var tr in TeacherRooms)
            {
                Teacher teacher = db.Teachers.Where(t => t.Id == tr.Teacher_Id).First();
                Room room = db.Rooms.Where(r => r.Id == tr.Room_Id).First();
                Session session = db.Sessions.Where(s => s.Id == tr.Session_Id).First();
                dt.Rows.Add(teacher.Name, room.No, room.Block, session.Name);
            }
            int i = 0;
            List<DataTable> dt1 = new List<DataTable>();
            foreach (var session in db.Sessions.ToList())
            {
                DataTable f1 = new DataTable("Session" + i);
                f1.Columns.AddRange(new DataColumn[2] { new DataColumn("StudentUSN"),
                                            new DataColumn("Room No."),
                                             });
                var RoomStud = db.RoomStudents.ToList().OrderBy(t => t.Room_Id).Where(t => t.Session_Id == session.Id);

                foreach (var rs in RoomStud)
                {
                    f1.Rows.Add(rs.Student_Id, rs.Room_Id);
                }
                dt1.Add(f1);
                i++;
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                foreach (var temp in dt1)
                {
                    wb.Worksheets.Add(temp);


                }
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
                }
            }

        }

    }
}