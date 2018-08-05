using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ExamRoomAllocation.Models;
using LinqToExcel;
using System.Data.SqlClient;

namespace ExcelImport.Controllers
{
    public class UploadController : Controller
    {
        private ExamRoomAllocationEntities db= new ExamRoomAllocationEntities();
        // GET: User  
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>  
        /// This function is used to download excel format.  
        /// </summary>  
        /// <param name="Path"></param>  
        /// <returns>file</returns>  
        public FileResult DownloadExcel()
        {
            string path = "/Doc/Details.xlsx";
            return File(path, "application/vnd.ms-excel", "Details.xlsx");
        }

        [HttpPost]
        public JsonResult UploadExcel(Student students, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Doc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Student$]", connectionString);
                    var adapter1 = new OleDbDataAdapter("SELECT * FROM [Room$]", connectionString);
                    var adapter2 = new OleDbDataAdapter("SELECT * FROM [Teacher$]", connectionString);
                    var ds = new DataSet();
                    var ds1 = new DataSet();
                    var ds2 = new DataSet();

                    adapter.Fill(ds, "ExcelTable");
                    adapter1.Fill(ds1, "ExcelTable");
                    adapter2.Fill(ds2, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];
                    DataTable dtable1 = ds1.Tables["ExcelTable"];
                    DataTable dtable2 = ds2.Tables["ExcelTable"];


                    string sheetName = "Student";
                    string sheetName1 = "Room";
                    string sheetName2 = "Teacher";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var stud = from a in excelFile.Worksheet<Student>(sheetName) select a;

                    foreach (var a in stud)
                    {
                        try
                        {
                            if (a.Name != "" && a.Id != "" && a.Sem != null && a.DepartmentId != null)
                            {
                                Student TU = new Student();
                                TU.Name = a.Name;
                                TU.Id = a.Id;
                                TU.Sem = a.Sem;
                                TU.DepartmentId = a.DepartmentId;
                                db.Students.Add(TU);
                                db.SaveChanges();
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                if (a.Id == "" || a.Id == null) data.Add("<li> Id is required</li>");
                                if (a.Sem == null) data.Add("<li>Sem is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    var rooms = from b in excelFile.Worksheet<Room>(sheetName1) select b;

                    foreach (var b in rooms)
                    {
                        try
                        {
                            if ( b.No != null && b.Block != "" && b.Department_Id != null  && b.Capacity != null)
                            {
                                Room TU = new Room();
                                TU.Id = b.Id;
                                TU.No = b.No;
                                TU.Department_Id = b.Department_Id;
                                TU.Capacity = b.Capacity;
                                TU.Block = b.Block;
                                TU.RoomStatus = b.RoomStatus;
                                db.Rooms.Add(TU);
                                db.SaveChanges();
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (b.No == null ) data.Add("<li> Number is required</li>");
                                if (b.Department_Id == null ) data.Add("<li> deptId is required</li>");
                                if (b.Capacity == null) data.Add("<li>capacity is required</li>");
                                if (b.Block == "" || b.Block == null) data.Add("<li>Block is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }

                    var teachers = from c in excelFile.Worksheet<Teacher>(sheetName2) select c;

                    foreach (var c in teachers)
                    {
                        try
                        {
                            if (c.Id != null && c.Name != "" && c.Duties != null && c.Designation_Id != null )
                            {
                                Teacher TU = new Teacher();
                                TU.Id = c.Id;
                                TU.Name = c.Name;
                                TU.Department_Id = c.Department_Id;
                                TU.Designation_Id = c.Designation_Id;
                                TU.Duties = c.Duties;
                                TU.TeacherPriority = c.TeacherPriority;
                                db.Teachers.Add(TU);
                                db.SaveChanges();
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (c.Id == null) data.Add("<li> ID is required</li>");
                                if (c.Name == null||c.Name=="") data.Add("<li> name is required</li>");
                                if (c.Designation_Id == null) data.Add("<li>designationid  is required</li>");
                                if (c.Duties == null ) data.Add("<li>Experience is required</li>");
                               

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
    }
}