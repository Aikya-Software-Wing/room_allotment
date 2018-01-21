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
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH-mm}")]
        [Display(Name = "Session start time")]
        public DateTime ExamTime;

        [Required]
        [Display(Name = "Exam Name")]
        public string Name;

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM}")]
        [Display(Name = "Exam Date")]
        public DateTime Date;

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

        [Display(Name = "Stream Name")]
        public Nullable<int> StreamId;
    }

    public class StreamMetadata
    {
        public int Id;

        [Required]
        [Display(Name ="Stream Name")]
        public string Name;
    }
}