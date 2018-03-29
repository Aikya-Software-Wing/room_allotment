using ExamRoomAllocation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExamRoomAllocation.Models;
using System.Threading.Tasks;

namespace ExamRoomAllocation.Helpers
{
    public class GreedyResultOptimizer : IResultOptimizer
    {
        /// <summary>
        /// Scaffold
        /// </summary>
        /// <param name="allotments">The allotments</param>
        /// <returns>The optimized allotments</returns>
        public Task<List<Allotment>> Optimize(List<Allotment> allotments)
        {
            return Task.Run(() =>
            {
                return allotments;
            });
        }
    }
}