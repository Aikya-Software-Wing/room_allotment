using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamRoomAllocation.ViewModel
{
    public class TeacherViewModel
    {
        public string TeacherId { get; set; }
        public string TeacherName { get; set; }
        public List<string> SessionList { get; set; }
        public System.DateTime Date { get; set; }
    }
}