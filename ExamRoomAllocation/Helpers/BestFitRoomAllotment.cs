using ExamRoomAllocation.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExamRoomAllocation.Interfaces;

namespace ExamRoomAllocation.Helpers
{
    public class BestFitRoomAllotment : IRoomAllotment
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
            if (allPossibleAllotmentsForExam.Count() != 0)
                return allPossibleAllotmentsForExam
                    .OrderByDescending(x => x.NumberOfStudents)
                    .ThenByDescending(x => x.ExamIndexInRoom)
                    .First();
            else
                return null;
        }

        /// <summary>
        /// This method will find the best fit rooms for all the exams in a session
        /// </summary>
        /// <param name="session">The session for which rooms have to be alloted</param>
        /// <param name="roomsAvailable">The rooms available for allotment</param>
        /// <param name="allotments">The previous allotments</param>
        private List<Allotment> AllotStudentsForSession(Session session, List<Room> roomsAvailable, List<Allotment> allotments)
        {
            int studentsToAllot = session.Exams.Sum(x => x.Students.Count());
            do
            {
                foreach (var exam in session.Exams.ToList())
                {
                    Allotment allotment = FindBestFitForExam(exam, roomsAvailable, allotments);

                    if (allotment != null)
                    {
                        allotment.Room.Exams.Add(allotment.Exam);
                        allotments.Add(allotment);
                        studentsToAllot -= allotment.NumberOfStudents;
                    }
                }
            } while (studentsToAllot > 0);

            return allotments;
        }

        /// <summary>
        /// This is the starting point for the algorithm
        /// </summary>
        /// <param name="sessions">The sessions for which the allotment have to be performed</param>
        /// <param name="rooms">The list of available rooms</param>
        /// <param name="allotments">The previous allotments</param>
        private List<Allotment> AllotStudentsToRooms(List<Session> sessions, List<Room> rooms, List<Allotment> allotments)
        {
            foreach (var session in sessions)
            {
                allotments.AddRange(AllotStudentsForSession(session, rooms, allotments));
            }
            return allotments;
        }

        /// <summary>
        /// This is the async starting point for the algorithm
        /// </summary>
        /// <param name="sessions">The sessions for which the allotment have to be performed</param>
        /// <param name="rooms">The list of available rooms</param>
        /// <param name="allotments">The previous allotments</param>
        /// <returns>The task that corresponds to the algorithm </returns>
        public Task<List<Allotment>> AllotAsync(Session session, List<Room> rooms, List<Allotment> allotments)
        {
            return Task.Run(() =>
            {
                return AllotStudentsForSession(session, rooms, allotments);
            });
        }
    }
}