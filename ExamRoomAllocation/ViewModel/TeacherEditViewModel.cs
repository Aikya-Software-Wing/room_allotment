using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamRoomAllocation.ViewModel
{
    public partial class TeacherEditViewModel
    {
        public List<string> SelectedSessions { get; set; }
        public List<int> RoomNumber { get; set; }
    }
}