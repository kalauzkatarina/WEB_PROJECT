using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using web_project.Models;
using web_project.Services;
using static System.Net.WebRequestMethods;

namespace web_project
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            UserService userService = new UserService();
            List<User> users = userService.GetAllUsers();
            HttpContext.Current.Application["users"] = users;

            ArrangementService arrangementService = new ArrangementService();
            List<Arrangement> arrangements = arrangementService.GetAllArrangements();
            HttpContext.Current.Application["arrangements"] = arrangements;

            AccommodationService accommodationService = new AccommodationService();
            List<Accommodation> accommodations = accommodationService.GetAllAccommodations();
            HttpContext.Current.Application["accommodations"] = accommodations;

            AccommodationUnitService accommodationUnitService = new AccommodationUnitService();
            List<AccommodationUnit> accommodationUnits = accommodationUnitService.GetAllAccommodationUnits();
            HttpContext.Current.Application["accommodationUnits"] = accommodationUnits;

            ReservationService reservationService = new ReservationService();
            List<Reservation> reservations = reservationService.GetAllReservations();
            HttpContext.Current.Application["reservations"] = reservations;

            CommentService commentService = new CommentService();
            List<Comment> comments = commentService.GetAllComments();
            HttpContext.Current.Application["comments"] = comments;
        }
    }
}
