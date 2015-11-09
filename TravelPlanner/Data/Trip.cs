using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TravelPlanner.Data
{
    public class Trip
    {
        [Key]
        public string Id
        {
            get;
            set;
        }

        public Trip()
        {
            Id = Guid.NewGuid().ToString();
        }

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

        public virtual ApplicationUser User
        {
            get;
            set;
        }

        public string UserId
        {
            get;
            set;
        }
    }
}