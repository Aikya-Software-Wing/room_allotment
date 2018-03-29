using ExamRoomAllocation.Models;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace ExamRoomAllocation.Helpers
{
    public class SessionHelper
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();

        /// <summary>
        /// This method returns the session name
        /// </summary>
        /// <param name="exam"></param>
        /// <returns> the session name string . ex : 15Mar1 (1st session on 15th mar) </returns>
        public static string CreateSession(Exam exam)
        {
            var examInDateTime = new DateTime(exam.Date.Value.Year, exam.Date.Value.Month, exam.Date.Value.Day);
            var examString = examInDateTime.ToString("ddMMM");
            int examHour = exam.ExamTime.Value.Hour;
            var examStringBuilder = new StringBuilder();
            examStringBuilder.Append(examString);

            if (examHour < 12)
            {
                examStringBuilder.Append("1");
            }
            else
            {
                examStringBuilder.Append("2");
            }
            return examStringBuilder.ToString();
        }

        /// <summary>
        /// This method returns the session name
        /// </summary>
        /// <param name="Date on which the exam is held"></param>
        /// <returns> the session name string . ex : 15Mar </returns>
        public static string TimeHelper(DateTime? CurrentDate)
        {
            StringBuilder s = new StringBuilder();
            var examInDateTime = new DateTime(CurrentDate.Value.Year, CurrentDate.Value.Month, CurrentDate.Value.Day);
            var examString = examInDateTime.ToString("ddMMM");
            return examString;
        }


        /// <summary>
        /// This method creates a new session and saves it in the database
        /// </summary>
        /// <param name="Name"></param>
        /// <returns> the session that is created and saved </returns>
        [HttpPost]
        public Session AddSession(string Name)
        {
            Session session = new Session();
            try
            {
                int id = db.Sessions.Max(x => x.Id);
                session.Id = id + 1;
            }
            catch (InvalidOperationException)
            {
                session.Id = 0;
            }
            session.Name = Name;
            db.Sessions.Add(session);
            db.SaveChanges();
            return session;
        }
    }
}