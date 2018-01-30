using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExamRoomAllocation.Models
{
    [MetadataType(typeof(ExamMetadata))]
    public partial class Exam
    {
    }

    [MetadataType(typeof(DepartmentMetadata))]
    public partial class Department
    {
    }

    [MetadataType(typeof(StreamMetadata))]
    public partial class Stream
    {
    }

    [MetadataType(typeof(DesignationMetadata))]
    public partial class Designation
    {
    }

    [MetadataType(typeof(RoomMetadata))]
    public partial class Room
    {
    }

    [MetadataType(typeof(SessionMetadata))]
    public partial class Session
    {
    }

    [MetadataType(typeof(StudentMetadata))]
    public partial class Student
    {
    }

    [MetadataType(typeof(TeacherMetadata))]
    public partial class Teacher
    {
    }
}