//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ExamRoomAllocation.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RoomStudent
    {
        public string Student_Id { get; set; }
        public int Room_Id { get; set; }
        public int Session_Id { get; set; }
    
        public virtual Room Room { get; set; }
        public virtual Session Session { get; set; }
        public virtual Student Student { get; set; }
    }
}
