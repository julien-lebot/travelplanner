using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using TravelPlanner.Data;
using TravelPlanner.Infrastructure;

namespace TravelPlanner.UnitTests
{
    public abstract class TripTestsBase
    {
        private ITripManager _tripManager;
        private IUnitOfWork _uow;
        private ITripRepository _tripRepository;
        private Dictionary<string, Trip> _trips;

        public ITripManager TripManager
        {
            get
            {
                return _tripManager;
            }
        }

        public IUnitOfWork Uow
        {
            get
            {
                return _uow;
            }
        }

        public ITripRepository Repository
        {
            get
            {
                return _tripRepository;
            }
        }

        public Dictionary<string, Trip> Trips
        {
            get
            {
                return _trips;
            }
        }

        private string GenerateRandomString(int minLen, int maxLen)
        {
            var rand = new Random(4242);
            var sb = new StringBuilder();
            var len = rand.Next(minLen, maxLen);
            for (int i = 0; i < len; ++i)
            {
                sb.Append((char)rand.Next('a', 'z'));
            }
            return sb.ToString();
        }

        private Trip GenerateTrip()
        {
            return new Trip
            {
                Id = GenerateRandomString(16, 16),
                Comment = GenerateRandomString(8, 140),
                Destination = GenerateRandomString(1, 64),
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now.AddDays(1)
            };
        }

        private void GenerateTrips()
        {
            _trips = new Dictionary<string, Trip>();
            for (int i = 0; i < 5; ++i)
            {
                var trip = GenerateTrip();
                Trips[trip.Id] = trip;
            }
        }

        public virtual void Setup()
        {
            GenerateTrips();
            _uow = Substitute.For<IUnitOfWork>();
            _tripRepository = Substitute.For<ITripRepository>();
            Repository.Entities.Returns(Trips.Values.AsQueryable());
            Repository.When(repo => repo.Add(Arg.Any<Trip>()))
                .Do(
                    _ =>
                    {
                        var trip = (Trip)_[0];
                        Trips.Add(trip.Id, trip);
                    });
            Repository.When(repo => repo.Update(Arg.Any<Trip>()))
                .Do(
                    _ =>
                    {
                        var trip = (Trip)_[0];
                        Trips[trip.Id] = trip;
                    });
            Repository.When(repo => repo.Delete(Arg.Any<Trip>()))
                .Do(
                    _ =>
                    {
                        var trip = (Trip)_[0];
                        Trips.Remove(trip.Id);
                    });
            Repository.GetById(Arg.Any<string>()).Returns(_ => Trips[(string)_[0]]);
            _tripManager = new TripManager(Uow, Repository);
        }
    }
}