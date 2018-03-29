using ExamRoomAllocation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRoomAllocation.Interfaces
{
    public interface IResultOptimizer
    {
        Task<List<Allotment>> Optimize(List<Allotment> allotments);
    }
}
