namespace ExamRoomAllocation.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Teacher")]
    public partial class Teacher
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Teacher()
        {
            Exams = new HashSet<Exam>();
        }

        [StringLength(255)]
        public string Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? Experience { get; set; }

        public int? Designation_Id { get; set; }

        public int? Department_Id { get; set; }

        public virtual Department Department { get; set; }

        public virtual Designation Designation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Exam> Exams { get; set; }
    }
}
