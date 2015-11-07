using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using NSubstitute;
using NUnit.Framework;
using TravelPlanner.Controllers;
using TravelPlanner.Models;

namespace TravelPlanner.UnitTests
{
    [TestFixture]
    public class TripControllerTests : TripTestsBase
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }

        [Test]
        public void ShouldReturnAllTrips()
        {
            var ctrlr = new TripsController(TripManager);

            var result = ctrlr.GetAll().ToList();

            Assert.That(result, Is.EqualTo(Trips.Values.Select(t => new TripWithId
            {
                Id = t.Id,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Destination = t.Destination,
                Comment = t.Comment
            }).ToList()));
        }

        [Test]
        public async Task ShouldCreateTrip()
        {
            var ctrlr = new TripsController(TripManager);

            var result = await ctrlr.Post(
                new Trip
                {
                    StartDate = DateTimeOffset.Now,
                    EndDate = DateTimeOffset.Now.AddDays(2),
                    Destination = "Romania",
                    Comment = string.Empty
                }) as CreatedNegotiatedContentResult<TripWithId>;

            Assert.IsNotNull(result);
            Assert.That(result.Content.Destination, Is.EqualTo("Romania"));
            Assert.That(result.Content.Comment, Is.Empty);
            Assert.That(result.Location.ToString(), Is.StringStarting("/trips/"));
        }
    }
}
