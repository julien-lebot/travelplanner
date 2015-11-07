using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelPlanner.Data;

namespace TravelPlanner.Infrastructure
{
    public interface ITripManager
    {
        IQueryable<Trip> Trips
        {
            get;
        }

        Trip GetTripById(string id);
        Trip GetTripByIdForUser(string id, string userId);
        IEnumerable<Trip> GetTripsByUserId(string userId);
        Task CreateTrip(Trip trip);
        Task UpdateTrip(Trip trip);
        Task DeleteTrip(Trip trip);
    }
}