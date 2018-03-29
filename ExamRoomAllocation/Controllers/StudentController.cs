using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExamRoomAllocation.Helpers;
using ExamRoomAllocation.Models;
using ExamRoomAllocation.ViewModel;

namespace ExamRoomAllocation.Controllers
{
    public class StudentController : Controller
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        // GET: Student
        public ActionResult Index()
        {
            var students = db.Students.Include(s => s.Department);
            return View(students.ToList());
        }

        // GET: Student/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Student/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name");
            ViewBag.ExamId = new MultiSelectList(db.Exams, "Code", "Name");
            return View();
        }

        //POST: Student/Create
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentExam studentExam)
        {
            Student student = new Student();
            if (ModelState.IsValid)
            {
                if (studentExam.SelectedExams != null)
                {
                    foreach (var code in studentExam.SelectedExams)
                    {
                        Exam exam = db.Exams.Find(code);
                        student.Exams.Add(exam);                        
                    }
                }
                student.DepartmentId = studentExam.DepartmentId;
                student.Id = studentExam.Id;
                student.Name = studentExam.Name;
                student.Sem = studentExam.Sem;
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", student.DepartmentId);
            ViewBag.ExamId = new MultiSelectList(db.Exams, "Code", "Name", student.Exams);
            return View();
        }

        // GET: Student/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var newStudent = new StudentHelper();
            Student student = newStudent.GetStudent(id);
            StudentExam studentExam = new StudentExam();
            if (student == null)
            {
                return HttpNotFound();
            }
            studentExam.Id = student.Id;
            studentExam.DepartmentId = (int)student.DepartmentId;
            studentExam.Sem = (int)student.Sem;
            studentExam.Name = student.Name;

            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", student.DepartmentId);
            ViewBag.ExamId = new MultiSelectList(db.Exams, "Code", "Name", student.Exams);
            return View(studentExam);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentExam studentExam)
        {
            //Student student = new Student();
            Student student = db.Students.Find(studentExam.Id);
            student.Exams.Clear();
            if (ModelState.IsValid)
            {
                if (studentExam.SelectedExams != null)
                {
                    foreach (var code in studentExam.SelectedExams)
                    {
                        Exam exam = db.Exams.Find(code);
                        student.Exams.Add(exam);
                    }
                }                             
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            } 
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", student.DepartmentId);
            ViewBag.ExamId = new MultiSelectList(db.Exams, "Code", "Name", student.Exams);
            return View(studentExam);
        }

        // GET: Student/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
