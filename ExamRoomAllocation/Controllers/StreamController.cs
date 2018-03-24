using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExamRoomAllocation.Models;

namespace ExamRoomAllocation.Controllers
{
    public class StreamController : Controller
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        // GET: Stream
        public ActionResult Index()
        {
            return View(db.Streams.ToList());
        }

        // GET: Stream/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stream stream = db.Streams.Find(id);
            if (stream == null)
            {
                return HttpNotFound();
            }
            return View(stream);
        }

        // GET: Stream/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stream/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Stream stream)
        {
            try
            {
                int id = db.Database.SqlQuery<int>("SELECT MAX(ID) from Department").FirstOrDefault<int>();
                stream.Id = id + 1;
            }
            catch (InvalidOperationException)

            {
                stream.Id = 1;
            }
            if (ModelState.IsValid)
            {
                db.Streams.Add(stream);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stream);
        }

        // GET: Stream/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stream stream = db.Streams.Find(id);
            if (stream == null)
            {
                return HttpNotFound();
            }
            return View(stream);
        }

        // POST: Stream/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Stream stream)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stream).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stream);
        }

        // GET: Stream/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stream stream = db.Streams.Find(id);
            if (stream == null)
            {
                return HttpNotFound();
            }
            return View(stream);
        }

        // POST: Stream/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stream stream = db.Streams.Find(id);
            db.Streams.Remove(stream);
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
