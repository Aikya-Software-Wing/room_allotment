using ExamRoomAllocation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamRoomAllocation.ViewModel
{
    public class RoomViewModel
    {
        public List<Teacher> Teachers { get; set; }
        //public List<string> TeacherDepartment { get; set; }      
        public Nullable<System.DateTime> Date  { get; set; }
        public Nullable<System.DateTime> SessionTime { get; set; }
        public List<string> ExamCode { get; set; }
        public List<Student> Students { get; set; }
        public string BlockName { get; set; }
        public Nullable<int> RoomNumber { get; set; }
        public int RoomId { get; set; }
        public List<string> Departments { get; set; }
    }
}