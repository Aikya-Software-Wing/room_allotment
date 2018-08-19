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
            dt.Columns.AddRange(new DataColumn[3] { new DataColumn("TeacherId"),
                                            new DataColumn("RoomId"),
                                            new DataColumn("SessionId") });
            var TeacherRooms = db.TeacherRooms.ToList().OrderBy(t=>t.Teacher_Id);
            foreach (var tr in TeacherRooms)
            {
                Teacher teacher = db.Teachers.Where(t => t.Id == tr.Teacher_Id).First(); 
                dt.Rows.Add(teacher.Name, tr.Room_Id, tr.Session_Id);
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }

        }
    }
}