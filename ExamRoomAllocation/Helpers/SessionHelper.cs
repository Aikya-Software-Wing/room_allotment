using ExamRoomAllocation.Models;
using System.Linq;
using System.Text;

namespace ExamRoomAllocation.Helpers
{
    public class SessionHelper
    {
        private ExamRoomAllocationEntities db = new ExamRoomAllocationEntities();
        private Session session = new Session();
        public static string CreateSession()
        {
            StringBuilder s = new StringBuilder();
            Exam exam = new Exam();
            int examHour, examDay;
            string examMonth;

            examHour = exam.ExamTime.Value.Hour;
            examDay = exam.Date.Value.Day;
            examMonth = exam.Date.Value.Month.ToString("MMM");

            s.Append(examDay.ToString());
            s.Append(examMonth.ToString());
            s.Append(examMonth);
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
    }
}