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
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id, Name")] Designation designation)
        {
            if (ModelState.IsValid)
            {
                db.Designations.Add(designation);
                db.SaveChanges();
                return RedirectToAction("Index");
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
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id, Name")] Designation designation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(designation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(designation);
        }

        // GET: Designation/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Designation/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            Designation designation = db.Designations.Find(id);
            db.Designations.Remove(designation);
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
