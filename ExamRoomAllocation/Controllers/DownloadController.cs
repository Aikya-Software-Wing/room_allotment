using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExamRoomAllocation.Models;
using ClosedXML.Excel;
using System.Data;
using System.IO;

namespace ExamRoomAllocation.Controllers
{
    public class DownloadController : Controller
    {
        // GET: Download
        public ActionResult Index()
        {
            ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();
            return View(db.TeacherRooms);
        }
        [HttpPost]
        public FileResult Export()
        {
            ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[4] { new DataColumn("Teacher Name"),
                                            new DataColumn("Room No."),
                                            new DataColumn("Block"),
                                            new DataColumn("Session") });
            var TeacherRooms = db.TeacherRooms.ToList().OrderBy(t=>t.Teacher_Id);
            foreach (var tr in TeacherRooms)
            {
                Teacher teacher = db.Teachers.Where(t => t.Id == tr.Teacher_Id).First();
                Room room = db.Rooms.Where(r => r.Id == tr.Room_Id).First();
                Session session = db.Sessions.Where(s => s.Id == tr.Session_Id).First();
                dt.Rows.Add(teacher.Name, room.No,room.Block, session.Name);
            }
            int i = 0;
            List<DataTable> dt1 = new List<DataTable>();
            foreach (var session in db.Sessions.ToList())
            {
                DataTable f1 = new DataTable("Session" + i);
                f1.Columns.AddRange(new DataColumn[2] { new DataColumn("StudentUSN"),
                                            new DataColumn("Room No."),
                                             });
                var RoomStud = db.RoomStudents.ToList().OrderBy(t => t.Room_Id).Where(t => t.Session_Id == session.Id);

                foreach (var rs in RoomStud)
                {
                    f1.Rows.Add(rs.Student_Id, rs.Room_Id);
                }
                dt1.Add(f1);
                i++;
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                foreach (var temp in dt1)
                {
                    wb.Worksheets.Add(temp);
                    
                    
                }
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }

        }
    }
}