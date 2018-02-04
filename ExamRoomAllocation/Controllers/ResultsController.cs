using ExamRoomAllocation.Helpers;
using ExamRoomAllocation.Models;
using ExamRoomAllocation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExamRoomAllocation.Controllers
{
    public class ResultsController : Controller
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        // GET: Results
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Allocate()
        {
            TeacherToRoom assignExam = new TeacherToRoom();
            StudentHelper stud = new StudentHelper();
            stud.Index();
            assignExam.Index();
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
            roomViewModel.BlockName = room.Block;
            roomViewModel.RoomNumber = room.No;
            roomViewModel.RoomId = room.Id;

            List<Student> studentRaw = new List<Student>();
            var students = db.RoomStudents.Where(r => r.Room_Id == RoomId && r.Session_Id == sessionId).ToList();
            foreach (var student in students)
            {
                var studentInDb = db.Students.Where(s => s.Id == student.Student_Id).FirstOrDefault();
                studentRaw.Add(studentInDb);
            }
            roomViewModel.Students = studentRaw;

            var exams = db.Exam.Where(e=>e.SessionId == sessionId).ToList();
            List<string> examsList = new List<string>();
            List<string> departments = new List<string>();
            foreach (var exam in exams)
            {
                if (exam.Rooms.Contains(room))
                {
                    examsList.Add(exam.Code);
                    departments.Add(exam.Department.Name.ToString());
                }
            }
            roomViewModel.ExamCode = new List<string>(examsList);
            roomViewModel.Departments = new List<string>(departments);

            Exam examForDate = exams.First();
            roomViewModel.Date = examForDate.Date.Value.Date;
            roomViewModel.SessionTime = examForDate.ExamTime;

            Teacher teacher = new Teacher();
            TeacherRoom teacherId = db.TeacherRooms.Where(r => r.Room_Id == RoomId && r.Session_Id == sessionId).FirstOrDefault();
            try
            {
                teacher = db.Teachers.Find(teacherId.Teacher_Id);
            }
            catch(NullReferenceException)
            {
                return View("Error");
            }
            roomViewModel.TeacherName = teacher.Name;
            roomViewModel.TeacherDepartment = teacher.Department.Name.ToString();

            return View(roomViewModel);
        }

        public ActionResult TeacherIndex()
        {
            var teachers = db.Teachers.ToList();
            return View(teachers);
        }

        public ActionResult TeacherDetails(string id)
        {
            TeacherViewModel teacherViewModel = new TeacherViewModel();
            var teacher = db.Teachers.Find(id);
            var teacherDetails = db.TeacherRooms.Where(t => t.Teacher_Id == id).ToList();
            teacherViewModel.TeacherName = teacher.Name;
            var SessionList = new List<string>();
            foreach ( var t in teacherDetails)
            {
                SessionList.Add(t.Session.Name);
            }
            teacherViewModel.SessionList = SessionList;
            teacherViewModel.TeacherId = id;
            return View(teacherViewModel);
        }
    }
}