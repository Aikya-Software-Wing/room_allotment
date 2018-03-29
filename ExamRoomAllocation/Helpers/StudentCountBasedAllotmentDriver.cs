using ExamRoomAllocation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExamRoomAllocation.Models;
using System.Threading.Tasks;

namespace ExamRoomAllocation.Helpers
{
    public class StudentCountBasedAllotmentDriver : IAllotmentDriver
    {
        /// <summary>
        /// Method to drive the allotment
        /// </summary>
        /// <param name="db">The backing database</param>
        /// <param name="roomAllotment">The room allotment algorithm</param>
        /// <param name="resultOptimizer">The result optimizer algorithm</param>
        /// <returns>The task that represents the pipeline</returns>
        public Task DriveAllotmentAsync(ExamRoomAllocationEntities db, IRoomAllotment roomAllotment, IResultOptimizer resultOptimizer)
        {
            return Task.Run(async () =>
            {
                await RunAlgorithm(db, roomAllotment, resultOptimizer);
            });
        }

        private static async Task RunAlgorithm(ExamRoomAllocationEntities db, IRoomAllotment roomAllotment, IResultOptimizer resultOptimizer)
        {
            // for each session
            // TODO parallelize this loop for maximum efficiency 
            foreach (var session in db.Sessions.ToList())
            {
                // premute and combine the exams in the session 

                // construct the session object based on the permutation and combination

                // run the allotment algorithm
                List<Allotment> allotments = await roomAllotment.AllotAsync(session, db.Rooms.ToList(), new List<Allotment>());

                // optimize the results
                List<Allotment> optimizedAllotments = await resultOptimizer.Optimize(allotments);

                // save the results to the database
                foreach (var allotment in optimizedAllotments)
                {
                    await allotment.SaveAsync(db);
                }
            }
        }
    }
}