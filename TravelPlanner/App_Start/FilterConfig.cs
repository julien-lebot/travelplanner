﻿using System.Web;
using System.Web.Mvc;
using TravelPlanner.Controllers;

namespace TravelPlanner
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
