using ExamRoomAllocation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamRoomAllocation.ViewModel
{
    public class StudentExam
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Sem { get; set; }
        public int DepartmentId { get; set; }
        public virtual ICollection<Exam> Exam { get; set; }
    }
}