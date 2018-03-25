using ExamRoomAllocation.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamRoomAllocation.Helpers
{
    public class BestFitRoomAllotment
    {

        /// <summary>
        /// This method find the best room that the exam can be held in.
        /// The algorithm will look for the room that can fit the maximum number of students
        /// </summary>
        /// <param name="exam">The exam for which a room should be found</param>
        /// <param name="roomsAvailable">The rooms that are available for allotment</param>
        /// <param name="allotments">The previouly made allotments</param>
        /// <returns>The allotment that fits best</returns>
        private Allotment FindBestFitForExam(Exam exam, List<Room> roomsAvailable, List<Allotment> allotments)
        {
            int numberOfStudentsAlreadyAllotedARoom = allotments
                .Where(x => x.Exam.Id == exam.Id && x.Exam.SessionId == exam.SessionId)
                .Sum(x => x.NumberOfStudents);
            int studentsWritingExam = exam.Students.Count - numberOfStudentsAlreadyAllotedARoom;

            List<Allotment> allPossibleAllotmentsForExam = new List<Allotment>();
            foreach (var room in roomsAvailable)
            {
                int numberOfExamsInRoom = room.Exams
                    .Where(x => x.SessionId == exam.SessionId)
                    .Count();

                int studentsInRoom = allotments
                    .Where(x => x.Room.Id == room.Id)
                    .Sum(x => x.NumberOfStudents);

                int roomCapacity = (numberOfExamsInRoom == 0) ? (room.Capacity.Value / 2) : (room.Capacity.Value - studentsInRoom);

                if (roomCapacity > 0)
                {
                    if (!room.Exams.Contains(exam))
                    {
                        allPossibleAllotmentsForExam.Add(new Allotment
                        {
                            NumberOfStudents = (studentsWritingExam > roomCapacity) ? (roomCapacity) : (studentsWritingExam),
                            ExamIndexInRoom = room.Exams.Where(x => x.SessionId == exam.SessionId).Count() + 1,
                            Exam = exam,
                            Room = room
                        });
                    }
                }
            }

            return allPossibleAllotmentsForExam
                .OrderByDescending(x => x.NumberOfStudents)
                .ThenByDescending(x => x.ExamIndexInRoom)
                .First();
        }

        /// <summary>
        /// Method to fetch the previously made allotments from the database
        /// </summary>
        /// <param name="rooms">The rooms from which the allotments have to be fetched</param>
        /// <returns>The allotments made</returns>
        private List<Allotment> GetPreviousAllotments(List<Room> rooms)
        {
            List<Allotment> allotments = new List<Allotment>();

            foreach(var room in rooms)
            {
                int index = 1;
                foreach(var exam in room.Exams.ToList())
                {
                    allotments.Add(new Allotment
                    {
                        Exam = exam,
                        Room = room,
                        ExamIndexInRoom = index++,
                        NumberOfStudents = room.RoomStudents.Where(x => x.exam_id == exam.Id && x.Session_Id == exam.SessionId).Count()
                    });
                }
            }

            return allotments;
        }

        /// <summary>
        /// This method will find the best fit rooms for all the exams in a session
        /// </summary>
        /// <param name="session">The session for which rooms have to be alloted</param>
        /// <param name="roomsAvailable">The rooms available for allotment</param>
        /// <param name="db">The backing database context</param>
        /// <param name="isPartialAllotment">true, if we are resuming a previously started allotment</param>
        private void AllotStudentsForSession(Session session, List<Room> roomsAvailable, ExamRoomAllocationEntities db, bool isPartialAllotment)
        {
            List<Allotment> allotments = isPartialAllotment ? GetPreviousAllotments(roomsAvailable) : new List<Allotment>();
            int studentsToAllot = session.Exams.Sum(x => x.Students.Count());

            do
            {
                foreach (var exam in session.Exams.ToList())
                {
                    Allotment allotment = FindBestFitForExam(exam, roomsAvailable, allotments);
                    allotment.Save(db);
                    allotments.Add(allotment);

                    studentsToAllot -= allotment.NumberOfStudents;
                }
            } while (studentsToAllot > 0);

            allotments.Clear();
        }

        /// <summary>
        /// This is the starting point for the algorithm
        /// </summary>
        /// <param name="sessions">The sessions for which the allotment have to be performed</param>
        /// <param name="rooms">The list of available rooms</param>
        /// <param name="db">The backing database</param>
        /// <param name="isPartialAllotment">true, if we are resuming a previously started allotment</param>
        private void AllotStudentsToRooms(List<Session> sessions, List<Room> rooms, ExamRoomAllocationEntities db, bool isPartialAllotment = false)
        {
            foreach (var session in sessions)
            {
                AllotStudentsForSession(session, rooms, db, isPartialAllotment);
            }
        }

        /// <summary>
        /// This is the async starting point for the algorithm
        /// </summary>
        /// <param name="sessions">The sessions for which the allotment have to be performed</param>
        /// <param name="rooms">The list of available rooms</param>
        /// <param name="db">The backing database</param>
        /// <param name="isPartialAllotment">true, if we are resuming a previously started allotment</param>
        /// <returns>The task that corresponds to the algorithm </returns>
        public Task AllotStudentsToRoomsAsync(List<Session> sessions, List<Room> rooms, ExamRoomAllocationEntities db, bool isPartialAllotment = false)
        {
            return Task.Run(() => AllotStudentsToRooms(sessions, rooms, db, isPartialAllotment));
        }
    }
}