using ExamRoomAllocation.Helpers;
using ExamRoomAllocation.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ExamRoomAllocation.Controllers
{
    public class ExamController : Controller
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();        
        // GET: Exam
        public ActionResult Index()
        {
            var departments = db.Exams.Include(d => d.Department);
            return View(departments.ToList());
        }

        // GET: Exam/Details/5
        public ActionResult Details(string Code)
        {
            if (Code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exams.Find(Code);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        // GET: Exam/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name");
            return View();
        }

        // POST: Exam/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code,Name,Date,ExamTime,DepartmentId")] Exam exam)
        {
            if (ModelState.IsValid)
            {
                Session session = new Session();
                string sessionNew = SessionHelper.CreateSession(exam);
                string query = "SELECT * FROM session where Name = '@name'";
                session = db.Sessions.SqlQuery(query, sessionNew).SingleOrDefault();
                if ( session == null)
                {
                    var createSession = new SessionController();
                    createSession.Create(sessionNew);
                }
                else
                {
                    exam.SessionId = session.Id;
                }
                db.Exams.Add(exam);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", exam.Id);
            return View(exam);
        }

        // GET: Exam/Edit/5
        public ActionResult Edit(string Code)
        {
            if (Code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exams.Find(Code);
            if (exam == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", exam.Id);
            return View(exam);
        }

        // POST: Exam/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Code,Name,Date,ExamTime,DepartmentId")] Exam exam)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exam).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", exam.Id);
            return View(exam);
        }

        // GET: Exam/Delete/5
        [HttpGet]
        public ActionResult Delete(string Code)
        {
            if (Code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exams.Find(Code);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }
        // POST: Exam/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(string Code)
        {
            Exam exam = db.Exams.Find(Code);
            db.Exams.Remove(exam);
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
