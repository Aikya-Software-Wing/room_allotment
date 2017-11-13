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
        private ExamRoomAllocationDb db = new ExamRoomAllocationDb();
        // GET: Exam
        public ActionResult Index()
        {
            return View(db.Exams.ToList());
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
            return View();
        }

        // POST: Exam/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "Code,Name,Date,Type")] Exam exam)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    db.Exams.Add(exam);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes, contact admin.");
            }
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
            return View(exam);
        }

        // POST: Exam/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Code,Name,Date,Type")] Exam exam)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exam).State = EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "unable to update, contact admin");
            }
            return View(exam);
        }

        // GET: Exam/Delete/5
        [HttpGet]
        public ActionResult Delete(string Code, bool? SaveChangesError = false)
        {
            if (Code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (SaveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete Failed, try again or call admin.";
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
            try
            {
                // TODO: Add delete logic here
                Exam exam = db.Exams.Find(Code);
                db.Exams.Remove(exam);
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("Delete", new { Code = Code, SaveChangesError = true });
            }
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
