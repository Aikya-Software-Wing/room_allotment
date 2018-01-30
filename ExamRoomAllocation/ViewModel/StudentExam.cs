using ExamRoomAllocation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExamRoomAllocation.ViewModel
{
    public partial class StudentExam
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1,8)]
        public int Sem { get; set; }

        public int DepartmentId { get; set; }

        public List<string> SelectedExams { get; set; }
    }
}