using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExamRoomAllocation.Helpers;
using ExamRoomAllocation.Models;

namespace ExamRoomAllocation.Controllers
{
    public class ExamController : Controller
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        // GET: Exam
        public ActionResult Index()
        {
            var exams = db.Exam.Include(e => e.Department).Include(e => e.Session);
            return View(exams.ToList());
        }

        // GET: Exam/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exam.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        // GET: Exam/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.Departments, "Id", "Name");
            return View();
        }

        // POST: Exam/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code,ExamTime,Name,Date,Id")] Exam exam)
        {
            if (ModelState.IsValid)
            {
                Session session = new Session();
                string sessionNew = SessionHelper.CreateSession(exam);
                using (var ctx = new ExamRoomAllocationEntities())
                {
                    var sessionInDb = ctx.Sessions
                                    .Where(s => s.Name == sessionNew)
                                    .FirstOrDefault<Session>();
                    if (sessionInDb == null)
                    {
                        var createSession = new SessionController();
                        createSession.Create(sessionNew);
                        var newSessionInDb = db.Sessions.Where(s => s.Name == sessionNew).FirstOrDefault<Session>();
                        exam.SessionId = newSessionInDb.Id;
                    }
                    else
                    {
                        exam.SessionId = sessionInDb.Id;

                    }

                    db.Exam.Add(exam);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Id = new SelectList(db.Departments, "Id", "Name", exam.Id);

            return View(exam);
        }
        // GET: Exam/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exam.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.Departments, "Id", "Name", exam.Id);
            ViewBag.SessionId = new SelectList(db.Sessions, "Id", "Name", exam.SessionId);
            return View(exam);
        }

        // POST: Exam/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Code,ExamTime,Name,Date,Id,SessionId")] Exam exam)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exam).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.Departments, "Id", "Name", exam.Id);
            ViewBag.SessionId = new SelectList(db.Sessions, "Id", "Name", exam.SessionId);
            return View(exam);
        }

        // GET: Exam/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exam.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        // POST: Exam/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Exam exam = db.Exam.Find(id);
            db.Exam.Remove(exam);
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
