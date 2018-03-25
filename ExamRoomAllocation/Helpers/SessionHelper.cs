using ExamRoomAllocation.Models;
using System;
using System.Linq;
using System.Text;

namespace ExamRoomAllocation.Helpers
{
    public class SessionHelper
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();
        private Session session = new Session();

        public static string CreateSession(Exam exam)
        {
            StringBuilder s = new StringBuilder();
            int examHour, examDay, examMonth;

            examHour = exam.ExamTime.Value.Hour;
            examDay = exam.Date.Value.Day;
            examMonth = exam.Date.Value.Month;

            s.Append(examDay.ToString());
            switch (examMonth)
            {
                case 1: s.Append("JAN");
                    break;
                case 2:
                    s.Append("FEB");
                    break;
                case 3:
                    s.Append("MAR");
                    break;
                case 4:
                    s.Append("APR");
                    break;
                case 5:
                    s.Append("MAY");
                    break;
                case 6:
                    s.Append("JUN");
                    break;
                case 7:
                    s.Append("JUL");
                    break;
                case 8:
                    s.Append("AUG");
                    break;
                case 9:
                    s.Append("SEP");
                    break;
                case 10:
                    s.Append("OCT");
                    break;
                case 11:
                    s.Append("NOV");
                    break;
                case 12:
                    s.Append("DEC");
                    break;
            }

            if (examHour < 12)
            {
                s.Append("1");
            }
            else
            {
                s.Append("2");
            }
            return s.ToString();
        }

        public static string TimeHelper(DateTime? CurrentDate)
        {
            StringBuilder s = new StringBuilder();
            int  examDay, examMonth;
            examDay = CurrentDate.Value.Day;
            examMonth = CurrentDate.Value.Month;
            s.Append(examDay.ToString());
            switch (examMonth)
            {
                case 1:
                    s.Append("JAN");
                    break;
                case 2:
                    s.Append("FEB");
                    break;
                case 3:
                    s.Append("MAR");
                    break;
                case 4:
                    s.Append("APR");
                    break;
                case 5:
                    s.Append("MAY");
                    break;
                case 6:
                    s.Append("JUN");
                    break;
                case 7:
                    s.Append("JUL");
                    break;
                case 8:
                    s.Append("AUG");
                    break;
                case 9:
                    s.Append("SEP");
                    break;
                case 10:
                    s.Append("OCT");
                    break;
                case 11:
                    s.Append("NOV");
                    break;
                case 12:
                    s.Append("DEC");
                    break;
            }
            return s.ToString();
        }

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