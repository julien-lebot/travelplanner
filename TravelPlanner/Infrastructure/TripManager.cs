using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelPlanner.Data;

namespace TravelPlanner.Infrastructure
{
    /*
            try
            {
                await ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ctx.Trips.Any(t => t.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
     */
    public class TripManager : ITripManager
    {
        private readonly IUnitOfWork _uow;
        private readonly ITripRepository _repo;

        public TripManager(IUnitOfWork uow, ITripRepository repo)
        {
            _uow = uow;
            _repo = repo;
        }

        public IQueryable<Trip> Trips
        {
            get
            {
                return _repo.Entities;
            }
        }

        public Trip GetTripById(string id)
        {
            return _repo.GetById(id);
        }

        public Trip GetTripByIdForUser(string id, string userId)
        {
            return _repo.GetTripByIdForUser(id, userId);
        }

        public IEnumerable<Trip> GetTripsByUserId(string userId)
        {
            return _repo.GetTripsByUserId(userId);
        }

        public async Task CreateTrip(Trip trip)
        {
            _repo.Add(trip);
            await _uow.CommitAsync();
        }

        public async Task UpdateTrip(Trip trip)
        {
            _repo.Update(trip);
            await _uow.CommitAsync();
        }

        public async Task DeleteTrip(Trip trip)
        {
            _repo.Delete(trip);
            await _uow.CommitAsync();
        }
    }
}