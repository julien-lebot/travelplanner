namespace TravelPlanner.Models
{
    public class TripWithId : Trip
    {
        public string Id
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
            return Equals((TripWithId)obj);
        }

        protected bool Equals(TripWithId other)
        {
            return base.Equals(other) && string.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (Id != null ? Id.GetHashCode() : 0);
            }
        }

        public static bool operator ==(TripWithId left, TripWithId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TripWithId left, TripWithId right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("[Trip {0}][{1}][{2}:{3}][{4}]", Id, Destination, StartDate, EndDate, Comment);
        }
    }
}