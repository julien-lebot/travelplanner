using System.Collections.Generic;
using System.Linq;
using TravelPlanner.Data;

namespace TravelPlanner.Infrastructure
{
    public class TripRepository : RepositoryBase<Trip, string>, ITripRepository
    {
        public TripRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }

        public IEnumerable<Trip> GetTripsByUserId(string userId)
        {
            return GetMany(trip => trip.UserId == userId);
        }

        public Trip GetTripByIdForUser(string id, string userId)
        {
            return GetMany(trip => trip.UserId == userId && trip.Id == id).FirstOrDefault();
        }
    }
}