using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamRoomAllocation.Models
{
    public class ExamMetadata
    {
        [Required]
        [Display(Name = "Exam Code")]
        public string Code;

        [Required]
        [Display(Name = "Exam Start Time")]
        public Nullable<System.DateTime> ExamTime;

        [Required]
        [Display(Name = "Exam Name")]
        public string Name;

        [Required]
        [Display(Name = "Exam Date")]
        public Nullable<System.DateTime> Date;

        [Required]
        [Display(Name = "Department To be Held in")]
        public int Id;

        [Display(Name = "Session Code")]
        public int SessionId;
    }

    public class DepartmentMetadata
    {
        public int Id;

        [Required]
        [Display(Name = "Department Name")]
        public string Name;

        [Required]
        [Display(Name = "Stream Name")]
        public Nullable<int> StreamId;
    }
}