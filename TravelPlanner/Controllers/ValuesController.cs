using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.OData;
using Microsoft.AspNet.Identity;
using TravelPlanner.Controllers.Dto;
using TravelPlanner.Data;
using TravelPlanner.Filters;

namespace TravelPlanner.Controllers
{
    public interface ITrip
    {
        DateTimeOffset StartDate
        {
            get;
            set;
        }

        DateTimeOffset EndDate
        {
            get;
            set;
        }

        string Destination
        {
            get;
            set;
        }

        string Comment
        {
            get;
            set;
        }
    }

    namespace Dto
    {
        public class Trip : ITrip
        {
            public DateTimeOffset StartDate
            {
                get;
                set;
            }

            public DateTimeOffset EndDate
            {
                get;
                set;
            }

            public string Destination
            {
                get;
                set;
            }

            public string Comment
            {
                get;
                set;
            }
        }

        public class PatchTrip
        {
            private readonly Data.Trip _entity;

            public PatchTrip(Data.Trip entity)
            {
                _entity = entity;
            }

            public PatchTrip()
            {
                _entity = new Data.Trip();
            }

            public DateTimeOffset? StartDate
            {
                get
                {
                    return _entity.StartDate;
                }
                set
                {
                    _entity.StartDate = value.Value;
                }
            }

            public DateTimeOffset? EndDate
            {
                get
                {
                    return _entity.EndDate;
                }
                set
                {
                    _entity.EndDate = value.Value;
                }
            }

            public string Destination
            {
                get
                {
                    return _entity.Destination;
                }
                set
                {
                    _entity.Destination = value;
                }
            }

            public string Comment
            {
                get
                {
                    return _entity.Comment;
                }
                set
                {
                    _entity.Comment = value;
                }
            }
        }

        public class TripWithId : Trip
        {
            public string Id
            {
                get;
                set;
            }

        }
    }

    [Authorize]
    [IdentityBasicAuthentication]
    public class TripsController : ApiController
    {
        [EnableQuery]
        public IQueryable<TripWithId> Get()
        {
            var ctx = new TravelPlannerDbContext();
            var userId = User.Identity.GetUserId();
            return ctx.Trips.Where(t => t.UserId == userId).Select(t => new TripWithId
            {
                Id = t.Id,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Destination = t.Destination,
                Comment = t.Comment
            });
        }

        public async Task<IHttpActionResult> Post(TripWithId tripWithId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tripEntity = new Data.Trip
            {
                UserId = User.Identity.GetUserId(),
                Comment = tripWithId.Comment,
                Destination = tripWithId.Destination,
                EndDate = tripWithId.EndDate,
                StartDate = tripWithId.StartDate,
            };
            var ctx = new TravelPlannerDbContext();
            ctx.Trips.Add(tripEntity);
            await ctx.SaveChangesAsync();
            return Created(new Uri(string.Format("/trips/{0}", tripWithId.Id), UriKind.Relative), tripWithId);
        }

        public async Task<IHttpActionResult> Patch(string id, Delta<PatchTrip> userTrip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ctx = new TravelPlannerDbContext();
            var entity = await ctx.Trips.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
            {
                return NotFound();
            }
            var trip = new PatchTrip(entity);
            userTrip.Patch(trip);
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
            return Ok(new TripWithId
            {
                Id = entity.Id,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Destination = entity.Destination,
                Comment = entity.Comment
            });
        }
        

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
