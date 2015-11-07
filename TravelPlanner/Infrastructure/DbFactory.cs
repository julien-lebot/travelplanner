using TravelPlanner.Data;

namespace TravelPlanner.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        private TravelPlannerDbContext _ctx;

        public TravelPlannerDbContext Init()
        {
            return _ctx ?? (_ctx = new TravelPlannerDbContext());
        }

        protected override void DisposeCore()
        {
            if (_ctx != null)
            {
                _ctx.Dispose();
            }
        }
    }
}