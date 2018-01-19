using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExamRoomAllocation.Models;
using System.Data;
using System.Net;
using System.Data.Entity;

namespace ExamRoomAllocation.Controllers
{
    public class DesignationController : Controller
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();
        // GET: Designation
        public ActionResult Index()
        {
            return View(db.Designations.ToList());
        }

        // GET: Designation/Details/5
        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);                
            }
            Designation designation = db.Designations.Find(id);
            if(designation == null)
            {
                return HttpNotFound();
            }
            return View(designation);
        }

        // GET: Designation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Designation/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "id, Name")] Designation designation)
        {
            try
            {
                // TODO: Add insert logic here
                if(ModelState.IsValid)
                {
                    db.Designations.Add(designation);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch(DataException)
            {
                ModelState.AddModelError("", "Unable to Create, contact sys admin");
            }
            return View(designation);
        }

        // GET: Designation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Designation designation = db.Designations.Find(id);
            if (designation == null)
            {
                return HttpNotFound();
            }
            return View(designation);
        }

        // POST: Designation/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id, Name")] Designation designation)
        {
            try
            {
                // TODO: Add update logic here
                if(ModelState.IsValid)
                {
                    db.Entry(designation).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch(DataException)
            {
                ModelState.AddModelError("", "Updation Failed, contact admin");
            }
            return View(designation);
        }

        // GET: Designation/Delete/5
        public ActionResult Delete(int? id, bool? SaveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (SaveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete Failed, try again or call admin.";
            }
            Designation designation = db.Designations.Find(id);
            if (designation == null)
            {
                return HttpNotFound();
            }
            return View(designation);
        }

        // POST: Designation/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // TODO: Add delete logic here
                Designation designation = db.Designations.Find(id);
                db.Designations.Remove(designation);
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
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
