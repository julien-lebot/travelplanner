using System;

namespace TravelPlanner.Models
{
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
}