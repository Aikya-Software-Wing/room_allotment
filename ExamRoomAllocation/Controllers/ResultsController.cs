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

        public ActionResult AllocateFirstYear()
        {
            TeacherToRoom assignExam = new TeacherToRoom();
            StudentHelpher stud = new StudentHelpher();
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
            foreach (var t in teacherDetails)
            {
                SessionList.Add(t.Session.Name);
            }
            teacherViewModel.SessionList = SessionList;
            teacherViewModel.TeacherId = id;
            return View(teacherViewModel);
        }

        public ActionResult EditTeacherDetails(string Teacher_id)
        {
            if (Teacher_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(Teacher_id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTeacherDetails([Bind(Include = "RoomId,SessionId")] TeacherRoom teacher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teacher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SessionId = new SelectList(db.Streams, "Id", "Name", teacher.Session_Id);
            return View(teacher);
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
    }
}