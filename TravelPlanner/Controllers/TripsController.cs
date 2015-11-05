using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using System.Web.OData;
using FluentValidation;
using Microsoft.AspNet.Identity;
using TravelPlanner.Controllers.Dto;
using TravelPlanner.Data;
using TravelPlanner.Filters;

namespace TravelPlanner.Controllers
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CheckModelForNullAttribute : ActionFilterAttribute
    {
        private readonly Func<Dictionary<string, object>, bool> _validate;

        public CheckModelForNullAttribute()
            : this(arguments => arguments.ContainsValue(null))
        { }

        public CheckModelForNullAttribute(Func<Dictionary<string, object>, bool> checkCondition)
        {
            _validate = checkCondition;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (_validate(actionContext.ActionArguments))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The argument cannot be null");
            }
        }
    }

    namespace Dto
    {
        public class TripValidator : AbstractValidator<Trip>
        {
            public TripValidator()
            {
                RuleFor(x => x.StartDate).NotNull().LessThan(x => x.EndDate);
                RuleFor(x => x.EndDate).NotNull()/*.GreaterThan(x => x.StartDate)*/;
                RuleFor(x => x.Destination).NotNull();
                RuleFor(x => x.Comment).NotNull();
            }
        }

        [FluentValidation.Attributes.Validator(typeof(TripValidator))]
        public class Trip
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

            public string StartDate
            {
                get
                {
                    return _entity.StartDate.ToString();
                }
                set
                {
                    _entity.StartDate = DateTimeOffset.Parse(value);
                }
            }

            public string EndDate
            {
                get
                {
                    return _entity.EndDate.ToString();
                }
                set
                {
                    _entity.EndDate = DateTimeOffset.Parse(value);
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

    /// <summary>
    /// Controller to CRUD trips.
    /// Any user can access this.
    /// </summary>
    [Authorize]
    [IdentityBasicAuthentication]
    public class TripsController : ApiController
    {
        /// <summary>
        /// Retrieves all the trips for the current user.
        /// </summary>
        /// <returns>All of the trips belonging to the current user.</returns>
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

        /// <summary>
        /// Retrieves all trips, from all users.
        /// Only Admin can access this method.
        /// </summary>
        /// <returns>All the trips.</returns>
        [EnableQuery]
        [Route("api/Trips/All")]
        [AuthorizedRoles("Admin")]
        public IQueryable<TripWithId> GetAll()
        {
            var ctx = new TravelPlannerDbContext();
            return ctx.Trips.Select(t => new TripWithId
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
        public async Task<IHttpActionResult> Post(Dto.Trip trip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tripEntity = new Data.Trip
            {
                UserId = User.Identity.GetUserId(),
                Comment = trip.Comment,
                Destination = trip.Destination,
                EndDate = trip.EndDate,
                StartDate = trip.StartDate,
            };
            var ctx = new TravelPlannerDbContext();
            ctx.Trips.Add(tripEntity);
            await ctx.SaveChangesAsync();
            return Created(new Uri(string.Format("/trips/{0}", tripEntity.Id), UriKind.Relative), new TripWithId
            {
                Id = tripEntity.Id,
                StartDate = tripEntity.StartDate,
                EndDate = tripEntity.EndDate,
                Destination = tripEntity.Destination,
                Comment = tripEntity.Comment
            });
        }

        public async Task<IHttpActionResult> Patch(string id, Delta<PatchTrip> userTrip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ctx = new TravelPlannerDbContext();

            Data.Trip trip = null;
            if (User.IsInRole("Admin"))
            {
                trip = ctx.Trips.FirstOrDefault(t => t.Id == id);
            }
            else
            {
                var userId = User.Identity.GetUserId();
                trip = ctx.Trips.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            }

            if (trip == null)
            {
                return NotFound();
            }
            var patchTrip = new PatchTrip(trip);
            userTrip.Patch(patchTrip);
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
                Id = trip.Id,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Destination = trip.Destination,
                Comment = trip.Comment
            });
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
            var ctx = new TravelPlannerDbContext();
            Data.Trip trip = null;

            if (User.IsInRole("Admin"))
            {
                trip = ctx.Trips.FirstOrDefault(t => t.Id == id);
            }
            else
            {
                var userId = User.Identity.GetUserId();
                trip = ctx.Trips.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            }

            if (trip == null)
            {
                return NotFound();
            }
            ctx.Trips.Remove(trip);
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
            return Ok();
        }
    }
}
