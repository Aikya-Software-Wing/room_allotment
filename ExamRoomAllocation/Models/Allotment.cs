using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamRoomAllocation.Models
{
    public class Allotment
    {
        public Exam Exam { get; set; }

        public Room Room { get; set; }

        public int NumberOfStudents { get; set; }

        public int ExamIndexInRoom { get; set; }

        /// <summary>
        /// Method to add the exam to the room
        /// </summary>
        /// <param name="db">The database</param>
        private void AllotExamToRoom(ExamRoomAllocationEntities db)
        {
            Room.Exams.Add(Exam);
            db.Entry(Exam).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// Method to allot students to a room in the current allotment
        /// </summary>
        /// <param name="db">The database</param>
        private void AllotStudentsToRoom(ExamRoomAllocationEntities db)
        {
            List<Student> students = db.Students
                .Where(x => x.Exams.Where(y => y.Id == Exam.Id).Count() != 0 &&
                    x.RoomStudents.Where(y => y.exam_id == Exam.Id).Count() == 0)
                .OrderBy(x => x.DepartmentId)
                .Take(NumberOfStudents)
                .ToList();

            foreach (var student in students)
            {
                db.RoomStudents.Add(new RoomStudent
                {
                    Room_Id = Room.Id,
                    Student_Id = student.Id,
                    Session_Id = Exam.SessionId,
                    exam_id = Exam.Id
                });
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Function to save the current allotment to the database
        /// </summary>
        /// <param name="db">the database to save to</param>
        /// <returns>Task that represents the save</returns>
        public Task SaveAsync(ExamRoomAllocationEntities db)
        {
            return Task.Run(() =>
            {
                AllotExamToRoom(db);
                AllotStudentsToRoom(db);
            });
        }
    }
}