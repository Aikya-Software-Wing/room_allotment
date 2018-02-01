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

            List<string> studentsList = new List<string>();
            var students = db.RoomStudents.Where(r => r.Room_Id == RoomId && r.Session_Id == sessionId).ToList();
            foreach (var student in students)
            {
                studentsList.Add(student.Student_Id);
            }
            roomViewModel.Students = studentsList;

            var exams = db.Exam.Where(e=>e.SessionId == sessionId).ToList();
            List<string> examsList = new List<string>();
            foreach (var exam in exams)
            {
                if (exam.Rooms.Contains(room))
                {
                    examsList.Add(exam.Code);
                }
            }
            roomViewModel.ExamCode = new List<string>(examsList);

            Session session = db.Sessions.Find(sessionId);
            roomViewModel.SessionName = session.Name;

            Teacher teacher = new Teacher();
            TeacherRoom teacherId = db.TeacherRooms.Where(r => r.Room_Id == RoomId && r.Session_Id == sessionId).FirstOrDefault();
            teacher = db.Teachers.Find(teacherId.Teacher_Id);
            roomViewModel.TeacherName = teacher.Name;

            return View(roomViewModel);
        }
    }
}