using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamRoomAllocation.ViewModel
{
    public class TeacherViewModel
    {
        public List<string> Names { get; set; }
        public List<string> Sessions { get; set; }
        public List<string> Rooms { get; set; }
        public System.DateTime Date { get; set; }
    }
}