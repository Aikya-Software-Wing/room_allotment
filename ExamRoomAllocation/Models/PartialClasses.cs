using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExamRoomAllocation.Models
{
    [MetadataType(typeof(Exam))]
    public partial class Exam
    {
    }

    [MetadataType(typeof(Department))]
    public partial class Department
    {
    }
}