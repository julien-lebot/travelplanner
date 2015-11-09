using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.OData;
using Microsoft.AspNet.Identity;
using TravelPlanner.Data;
using TravelPlanner.Filters;
using TravelPlanner.Filters.BasicAuthentication;
using TravelPlanner.Infrastructure;
using TravelPlanner.Models;
using Trip = TravelPlanner.Models.Trip;

namespace TravelPlanner.Controllers
{
    /// <summary>
    /// Controller to CRUD trips.
    /// Any user can access this.
    /// </summary>
    [Authorize]
    [IdentityBasicAuthentication]
    public class TripsController : ApiController
    {
        private readonly ITripManager _tripManager;

        public TripsController(ITripManager tripManager)
        {
            _tripManager = tripManager;
        }

        /// <summary>
        /// Retrieves all the trips for the current user.
        /// </summary>
        /// <returns>All of the trips belonging to the current user.</returns>
        [EnableQuery]
        public IQueryable<TripWithId> Get()
        {
            var userId = User.Identity.GetUserId();
            return _tripManager.GetTripsByUserId(userId).AsQueryable().Select(t => new TripWithId
            {
                Id = t.Id,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Destination = t.Destination,
                Comment = t.Comment
            });
        }

        /// <summary>
        /// Retrieves all trips, from all users.
        /// Only Admin can access this method.
        /// </summary>
        /// <returns>All the trips.</returns>
        [EnableQuery]
        [Route("api/Trips/All")]
        [AuthorizedRoles(Roles.Admin)]
        public IQueryable<TripWithId> GetAll()
        {
            return _tripManager.Trips.Select(t => new TripWithId
            {
                Id = t.Id,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Destination = t.Destination,
                Comment = t.Comment
            });
        }

        /// <summary>
        /// Creates a new trip for the current user.
        /// </summary>
        /// <param name="trip">The trip to create.</param>
        /// <returns>The result of the operation: Ok (200) or BadRequest (400).</returns>
        [ResponseType(typeof(TripWithId))]
        public async Task<IHttpActionResult> Post(Trip trip)
        {
            var tripEntity = new Data.Trip
            {
                UserId = User.Identity.GetUserId(),
                Comment = trip.Comment,
                Destination = trip.Destination,
                EndDate = trip.EndDate,
                StartDate = trip.StartDate,
            };
            await _tripManager.CreateTrip(tripEntity);
            return Created(new Uri(string.Format("/trips/{0}", tripEntity.Id), UriKind.Relative), new TripWithId
            {
                Id = tripEntity.Id,
                StartDate = tripEntity.StartDate,
                EndDate = tripEntity.EndDate,
                Destination = tripEntity.Destination,
                Comment = tripEntity.Comment
            });
        }

        [ResponseType(typeof(TripWithId))]
        public async Task<IHttpActionResult> Patch(string id, Delta<PatchTrip> userTrip)
        {
            var trip = GetTripForRole(id);
            if (trip == null)
            {
                return NotFound();
            }
            var patchTrip = new PatchTrip(trip);
            userTrip.Patch(patchTrip);
            await _tripManager.UpdateTrip(trip);

            return Ok(new TripWithId
            {
                Id = trip.Id,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Destination = trip.Destination,
                Comment = trip.Comment
            });
        }

        private Data.Trip GetTripForRole(string tripId)
        {
            if (User.IsInRole(Roles.Admin))
            {
                return _tripManager.GetTripById(tripId);
            }
            var userId = User.Identity.GetUserId();
            return _tripManager.GetTripByIdForUser(tripId, userId);
        }

        /// <summary>
        /// Deletes a trip by id.
        /// If the user is an admin, any trip from any user can be deleted.
        /// Otherwise, the user can only delete trips they have created.
        /// </summary>
        /// <param name="id">The id of the trip to delete.</param>
        /// <returns>The result of the request: Ok (200) or NotFound (404)</returns>
        public async Task<IHttpActionResult> Delete(string id)
        {
            var trip = GetTripForRole(id);
            if (trip == null)
            {
                return NotFound();
            }
            await _tripManager.DeleteTrip(trip);
            return Ok();
        }
    }
}
