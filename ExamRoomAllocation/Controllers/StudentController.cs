using ExamRoomAllocation.Models;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Data.Entity;

namespace ExamRoomAllocation.Controllers
{
    public class StudentController : Controller
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();
        // GET: Students

        public ActionResult Index()
        {
            return View(db.Students.ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details(string USN)
        {
            if (USN == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(USN);
            if(student==null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "USN,Name,Sem")] Student student)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch(DataException )
            {
                ModelState.AddModelError("", "Unable to save changes, contact admin.");
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(string USN)
        {
            if(USN == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(USN);
            if(student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost, ActionName("Edit")]
        public ActionResult Edit([Bind(Include = "USN,Name,Sem")] Student student)
        {
            if(ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
            }            
            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");                
            }
            catch(DataException)
            {
                ModelState.AddModelError("", "unable to update, contact admin");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        [HttpGet]
        public ActionResult Delete(string USN, bool? SaveChangesError = false)
        {
            if(USN == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(SaveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete Failed, try again or call admin.";
            }
            Student student = db.Students.Find(USN);
            if(student==null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(string USN)
        {
            try
            {
                // TODO: Add delete logic here
                Student student = db.Students.Find(USN);
                db.Students.Remove(student);
                db.SaveChanges();                
            }
            catch
            {
                return RedirectToAction("Delete", new { USN = USN, SaveChangesError = true});
            }
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }             
            base.Dispose(disposing);
        }
    }
}
