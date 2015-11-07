using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TravelPlanner.Data;

namespace TravelPlanner.UnitTests
{
    [TestFixture]
    public class TripManagerTests : TripTestsBase
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }

        [Test]
        public void ShouldReturnAllTrips()
        {
            var trips = TripManager.Trips.ToList();
            Assert.That(trips, Is.EqualTo(Trips.Values));
            var unused = Uow.DidNotReceive().CommitAsync();
        }

        [Test]
        public void ShouldGetTripByIdReturnRightTrip()
        {
            var sampleTrip = Trips.First();
            var trip = TripManager.GetTripById(sampleTrip.Key);
            Assert.That(trip, Is.EqualTo(sampleTrip.Value));
            var unused = Uow.DidNotReceive().CommitAsync();
        }

        [Test]
        public async Task ShouldAddNewTrip()
        {
            var trip = new Trip
            {
                StartDate = DateTimeOffset.Now.AddDays(10),
                EndDate = DateTimeOffset.Now.AddDays(20),
                Comment = "Test Add",
                Destination = "France"
            };

            int numTrips = Trips.Count;
            await TripManager.CreateTrip(trip);

            Assert.That(trip, Is.EqualTo(Trips[trip.Id]));
            Assert.That(Trips.Count, Is.EqualTo(numTrips + 1));
            var unused = Uow.Received(1).CommitAsync();
        }

        [Test]
        public async Task ShouldUpdateTrip()
        {
            var trip = Trips.First();
            var oldComment = trip.Value.Comment;
            trip.Value.Comment = "Test update";
            var end = DateTimeOffset.Now.AddDays(10);
            trip.Value.EndDate = end;
            await TripManager.UpdateTrip(trip.Value);

            Assert.That(Trips.First().Value.Comment, Is.Not.EqualTo(oldComment));
            Assert.That(Trips.First().Value.EndDate, Is.EqualTo(end));
            Assert.That(Trips.First().Value.Id, Is.EqualTo(trip.Value.Id));
            var unused = Uow.Received(1).CommitAsync();
        }

        [Test]
        public async Task ShouldDeleteTrip()
        {
            var trip = Trips.First();
            int numTrips = Trips.Count;
            await TripManager.DeleteTrip(trip.Value);

            Assert.That(!Trips.ContainsKey(trip.Key));
            Assert.That(Trips.Count, Is.EqualTo(numTrips - 1));
            var unused = Uow.Received(1).CommitAsync();
        }
    }
}
