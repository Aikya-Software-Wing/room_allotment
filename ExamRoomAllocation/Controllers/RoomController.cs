using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExamRoomAllocation.Models;
using ExamRoomAllocation.Helpers;

namespace ExamRoomAllocation.Controllers
{
    public class RoomController : Controller
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        // GET: Room
        public ActionResult Index()
        {
            var rooms = db.Rooms.Include(r => r.Department);
            return View(rooms.ToList());
        }

        // GET: Room/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // GET: Room/Create
        public ActionResult Create()
        {
            ViewBag.Department_Id = new SelectList(db.Departments, "Id", "Name");
            return View();
        }

        // POST: Room/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "No,Block,Capacity,Department_Id,RoomStatus")] Room room)
        {          
            try
            {
                int id = db.Database.SqlQuery<int>("SELECT MAX(ID) from Room").FirstOrDefault<int>();
                room.Id = id + 1;
            }
            catch (InvalidOperationException)
            {
                room.Id = 1;
            }
            if (ModelState.IsValid)
            {
                room.RoomStatus = 0;
                db.Rooms.Add(room);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Department_Id = new SelectList(db.Departments, "Id", "Name", room.Department_Id);
            return View(room);
        }

        // GET: Room/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            ViewBag.Department_Id = new SelectList(db.Departments, "Id", "Name", room.Department_Id);
            return View(room);
        }

        // POST: Room/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "No,Block,Capacity,Department_Id,RoomStatus")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Entry(room).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Department_Id = new SelectList(db.Departments, "Id", "Name", room.Department_Id);
            return View(room);
        }

        // GET: Room/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Room room = db.Rooms.Find(id);
            db.Rooms.Remove(room);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // room/Assign
        public ActionResult Assign()
        {
            StudentHelpher Stud = new StudentHelpher();
            Stud.Index();
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
