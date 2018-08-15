using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ExamRoomAllocation.Helpers;
using ExamRoomAllocation.Models;

namespace ExamRoomAllocation.Controllers
{
    [Authorize]
    public class ExamController : Controller
    {        
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        // GET: Exam
        public ActionResult Index()
        {
            var exams = db.Exams.Include(e => e.Department).Include(e => e.Session);
            return View(exams.ToList());
        }

        // GET: Exam/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exams.Find(id);
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
                string sessionNew = SessionHelper.CreateSession(exam);
                var session = db.Sessions.Where(s => s.Name == sessionNew).FirstOrDefault<Session>();
                if (session == null)
                {
                    var createSession = new SessionHelper();
                    createSession.AddSession(sessionNew);
                    var newSession = db.Sessions.Where(s => s.Name == sessionNew).FirstOrDefault<Session>();
                    exam.SessionId = newSession.Id;
                }
                else
                {
                    exam.SessionId = session.Id;

                }
                db.Exams.Add(exam);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.Departments, "Id", "Name", exam.Id);
            return View(exam);
        }
        // GET: Exam/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exams.Find(id);
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
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exams.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        // POST: Exam/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            Exam exam = db.Exams.Find(id);
            db.Exams.Remove(exam);
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
