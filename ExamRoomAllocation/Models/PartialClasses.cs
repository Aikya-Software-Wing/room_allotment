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
}