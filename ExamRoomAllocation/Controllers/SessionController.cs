using ExamRoomAllocation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ExamRoomAllocation.Controllers
{
    public class SessionController : Controller
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();
        // GET: Session
        public ActionResult Index()
        {
            return View(db.Sessions.ToList());
        }

        // GET: Session/Details/5
        public ActionResult Details(int? id)
        {
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session session = db.Sessions.Find(id);
            if(session==null)
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

        // POST: Session/Create/SessionName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Name)
        {
            Session session = new Session();
            session.Name = Name;
            if (ModelState.IsValid)
            {
                db.Sessions.Add(session);
                db.SaveChanges();
                return RedirectToAction("Index");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include ="SessionName")]Session session)
        {
            if(ModelState.IsValid)
            {
                db.Entry(session).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(session);
        }

        // GET: Session/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Session/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            Session session = db.Sessions.Find(id);
            db.Sessions.Remove(session);
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
