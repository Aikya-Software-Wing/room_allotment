using ExamRoomAllocation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamRoomAllocation.ViewModel
{
    public class RoomViewModel
    {
        public string TeacherName { get; set; }
        public string TeacherId { get; set; }      
        public string SessionName { get; set; }
        public List<string> ExamCode { get; set; }
        public List<string> Students { get; set; }
    }
}