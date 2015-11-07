using System;

namespace TravelPlanner.Models
{
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Trip)obj);
        }

        protected bool Equals(Trip other)
        {
            return StartDate.Equals(other.StartDate) &&
                   EndDate.Equals(other.EndDate) &&
                   string.Equals(Destination, other.Destination) &&
                   string.Equals(Comment, other.Comment);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StartDate.GetHashCode();
                hashCode = (hashCode * 397) ^ EndDate.GetHashCode();
                hashCode = (hashCode * 397) ^ (Destination != null ? Destination.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Comment != null ? Comment.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Trip left, Trip right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Trip left, Trip right)
        {
            return !Equals(left, right);
        }
    }
}