using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamRoomAllocation.Models
{
    public class ExamMetadata
    {
        [MetadataType(typeof(ExamMetadata))]
        public partial class Exam
        {
        }

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
        [RegularExpression(@"^([a-zA-sz\s]{4,100})$", ErrorMessage ="No numbers, at least 4 characters")]
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

    [MetadataType(typeof(DepartmentMetadata))]
    public partial class Department
    {
    }

    public class DepartmentMetadata
    {
        [Required]
        [Display(Name = "Department Name")]
        [RegularExpression(@"^([a-zA-sz\s]{4,100})$", ErrorMessage = "No numbers, at least 4 characters")]
        public string Name;

        [Display(Name = "Stream Name")]
        public Nullable<int> StreamId;
    }

    [MetadataType(typeof(StreamMetadata))]
    public partial class Stream
    {
    }

    public class StreamMetadata
    {
        [Required]
        [Display(Name ="Stream Name")]
        [RegularExpression(@"^([a-zA-sz\s]{4,100})$", ErrorMessage = "No numbers, at least 4 characters")]
        public string Name;
    }

    [MetadataType(typeof(DesignationMetadata))]
    public partial class Designation
    {
    }

    public class DesignationMetadata
    {
        [Required]
        [Display(Name = "Designation Name")]
        [RegularExpression(@"^([a-zA-sz\s]{4,100})$", ErrorMessage = "No numbers, at least 4 characters")]
        public string Name;
    }

    [MetadataType(typeof(RoomMetadata))]
    public partial class Room
    {
    }

    public class RoomMetadata
    {

        [Required]
        [Display(Name = "Room Number")]
        [RegularExpression("([0-9][0-9][0-9]*)", ErrorMessage = "Must be a natural number")]
        public Nullable<int> No { get; set; }

        [Required]
        [Display(Name = "Block Name")]
        [RegularExpression(@"^([a-zA-sz\s]{4,100})$", ErrorMessage = "No numbers, at least 4 characters")]
        public string Block;

        [Required]
        [Display(Name ="Capacity")]
        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "Capacity must be a natural number")]
        public Nullable<int> Capacity;

        public int RoomStatus;
    }

    [MetadataType(typeof(SessionMetadata))]
    public partial class Session
    {
    }

    public class SessionMetadata
    {
        [Required]
        [Display(Name ="Session Name")]
        public string Name;
    }

    [MetadataType(typeof(TeacherMetadata))]
    public partial class Teacher
    {
    }

    public class TeacherMetadata
    {
        [Required]
        [Display(Name = "Teacher Id")]
        public string Id;

        [Required]
        [Display(Name = "Teacher Name")]
        //[RegularExpression(@"^([a-zA-sz\s]{4,100})$", ErrorMessage = "No numbers, at least 4 characters")]
        public string Name;

        [Required]
        [Display(Name ="Experience")]
        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "Must be in between 0-100")]
        public Nullable<int> Experience;        

        [Required]
        [Display(Name ="Priority Level")]
        public int TeacherPriority;
    }
}

namespace ExamRoomAllocation.ViewModel
{
    public class StudentExamMetadata
    {
        [MetadataType(typeof(StudentExamMetadata))]
        public partial class StudentExam
        {
        }

        [Required]
        [Display(Name = "USN")]
        public string Id;

        [Required]
        [Display(Name = "Name")]
        [RegularExpression(@"([a-zA-sz\s]{4,100})$", ErrorMessage = "No numbers, at least 4 characters")]
        public string Name;

        [Required]
        [Display(Name = "Semester")]
        [RegularExpression("([1-8]*)", ErrorMessage = "Must be in between 1-8")]
        public Nullable<int> Sem;
    }
}

