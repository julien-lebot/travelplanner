using System.Collections.Generic;
using TravelPlanner.Data;

namespace TravelPlanner.Infrastructure
{
    public interface ITripRepository : IRepository<Trip, string>
    {
        IEnumerable<Trip> GetTripsByUserId(string userId);
        Trip GetTripByIdForUser(string id, string userId);
    }
}