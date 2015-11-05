namespace TravelPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using TravelPlanner.Data;

    internal sealed class Configuration : DbMigrationsConfiguration<Data.TravelPlannerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Data.TravelPlannerDbContext context)
        {
            TravelPlannerDbContext.Seed(context);
        }
    }
}
