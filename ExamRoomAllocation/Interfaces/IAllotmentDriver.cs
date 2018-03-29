using ExamRoomAllocation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamRoomAllocation.Interfaces
{
    interface IAllotmentDriver
    {
        Task DriveAllotmentAsync(ExamRoomAllocationEntities db, IRoomAllotment roomAllotment, IResultOptimizer resultOptimizer);
    }
}
