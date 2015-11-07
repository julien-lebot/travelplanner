using System;
using TravelPlanner.Data;

namespace TravelPlanner.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        TravelPlannerDbContext Init();
    }
}