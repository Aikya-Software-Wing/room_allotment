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
    public class TeacherController : Controller
    {
        private ExamRoomAllocationDb db = new ExamRoomAllocationDb();

        // GET: Teacher
        public ActionResult Index()
        {
            return View(db.Teachers.ToList());
        }

        // GET: Teacher/Details/5
        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if(teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // GET: Teacher/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teacher/Create
        [HttpPost]
        public ActionResult Create([Bind(Include ="Name, Experience, Department_Id, Designation_Id")]Teacher teacher)
        {
            try
            {
                // TODO: Add insert logic here
                if(ModelState.IsValid)
                {
                    db.Teachers.Add(teacher);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch(DataException)
            {
                ModelState.AddModelError("", "Creation not possible, contact admin");
            }
            return View(teacher);
        }

        // GET: Teacher/Edit/5
        public ActionResult Edit(int? id)
        {
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if(teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // POST: Teacher/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Name,Experience,Designation_Id,Department_Id")]Teacher teacher)
        {
            try
            {
                // TODO: Add update logic here
                if(ModelState.IsValid)
                {
                    db.Entry(teacher).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "unable to update, contact admin");
            }
            return View();
        }

        // GET: Teacher/Delete/5
        public ActionResult Delete(int? id, bool? SaveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(SaveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete Failed, try again or call admin.";
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // POST: Teacher/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // TODO: Add delete logic here
                Teacher teacher = db.Teachers.Find(id);
                db.Teachers.Remove(teacher);
                db.SaveChanges();
                
            }
            catch
            {
                return RedirectToAction("Delete", new { id = id, SaveChangesError = true });
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
