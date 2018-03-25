using ExamRoomAllocation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamRoomAllocation.Interfaces
{
    public interface IRoomAllotment
    {
        Task<List<Allotment>> AllotAsync(Session session, List<Room> rooms, List<Allotment> allotments);
    }
}
