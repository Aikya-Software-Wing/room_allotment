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
    public class SessionController : Controller
    {
        private ExamRoomAllocationDb db = new ExamRoomAllocationDb();
        // GET: Session
        public ActionResult Index()
        {
            return View(db.Sessions.ToList());
        }

        // GET: Session/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session session = db.Sessions.Find(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // GET: Session/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Session/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,Type")] Session session)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Sessions.Add(session);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes, contact admin.");
            }
            return View(session);
        }

        // GET: Session/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session session = db.Sessions.Find(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);

        }

        // POST: Session/Edit/5
        [HttpPost,ActionName("Edit")]
        public ActionResult Edit([Bind(Include ="Id,Type")]Session session)
        {
            if (ModelState.IsValid)
            {
                db.Entry(session).State = EntityState.Modified;
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
            return View(session);
        }

        // GET: Session/Delete/5
        [HttpGet]
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
            Session session = db.Sessions.Find(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // POST: Session/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // TODO: Add delete logic here
                Session session = db.Sessions.Find(id);
                db.Sessions.Remove(session);
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
