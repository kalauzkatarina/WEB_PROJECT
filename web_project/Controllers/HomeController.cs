using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using web_project.Models;
using web_project.Services;

namespace web_project.Controllers
{
    //komentar za testiranje gitlab
    public class HomeController : Controller
    {
        //Na početnoj strani može da gleda, pretražuje i sortira sve predstojeće aranžmane
        //(sortiranje od najskorijih prema najdaljim) koje pruža turistička agencija
        private List<Arrangement> arrangements;
       
        //GET: Home
        public ActionResult Index(string travelMinStartDate, string travelMaxStartDate, string travelMinEndDate, string travelMaxEndDate, string transportType, string arrangementType, string name, string sortBy, string sortOrder)
        {
            arrangements = (List<Arrangement>)HttpContext.Application["arrangements"];

            arrangements = arrangements.Where(a => a.LogicDelete == false).ToList();

            if (string.IsNullOrEmpty(travelMinStartDate)) travelMinStartDate = "";
            if (string.IsNullOrEmpty(travelMaxStartDate)) travelMaxStartDate = "";
            if (string.IsNullOrEmpty(travelMinEndDate)) travelMinEndDate = "";
            if (string.IsNullOrEmpty(travelMaxEndDate)) travelMaxEndDate = "";
            if (string.IsNullOrEmpty(transportType)) transportType = "";
            if (string.IsNullOrEmpty(arrangementType)) arrangementType = "";
            if (string.IsNullOrEmpty(name)) name = "";
            if (string.IsNullOrEmpty(sortBy)) sortBy = "";
            //asc rastuce, desc opadajuce
            if (string.IsNullOrEmpty(sortOrder)) sortOrder = "asc";

            if (!string.IsNullOrEmpty(travelMinStartDate))
            {
                arrangements = arrangements.Where(a => DateTime.ParseExact(a.TravelStartDate, "dd/MM/yyyy", null) >= DateTime.ParseExact(travelMinStartDate, "yyyy-MM-dd", null)).ToList();
            }

            if (!string.IsNullOrEmpty(travelMaxStartDate))
            {
                arrangements = arrangements.Where(a => DateTime.ParseExact(a.TravelStartDate, "dd/MM/yyyy", null) <= DateTime.ParseExact(travelMaxStartDate, "yyyy-MM-dd", null)).ToList();
            }

            if (!string.IsNullOrEmpty(travelMinEndDate))
            {
                arrangements = arrangements.Where(a => DateTime.ParseExact(a.TravelEndDate, "dd/MM/yyyy", null) >= DateTime.ParseExact(travelMinEndDate, "yyyy-MM-dd", null)).ToList();
            }

            if (!string.IsNullOrEmpty(travelMaxEndDate))
            {
                arrangements = arrangements.Where(a => DateTime.ParseExact(a.TravelEndDate, "dd/MM/yyyy", null) <= DateTime.ParseExact(travelMaxEndDate, "yyyy-MM-dd", null)).ToList();
            }

            if(!string.IsNullOrEmpty(transportType))
            {
                arrangements = arrangements.Where(a => a.Transport.ToString().Equals(transportType)).ToList();
            }

            if (!string.IsNullOrEmpty(arrangementType))
            {
                arrangements = arrangements.Where(a => a.ArrangementType.ToString().Equals(arrangementType)).ToList();
            }

            if (!string.IsNullOrEmpty(name))
            {
                arrangements = arrangements.Where(a => a.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0
                 || a.Location.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0
                 || a.ArrangementDescription.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                bool order = sortOrder.ToLower().Equals("asc");

                switch(sortBy.ToLower())
                {
                    case "name":
                        {
                            if (order) //asc
                            {
                                arrangements = arrangements.OrderBy(a =>  a.Name).ToList();
                            } else //desc
                            {
                                arrangements = arrangements.OrderByDescending(a => a.Name).ToList();
                            }
                                break;
                        }

                    case "travelstartdate":
                        {
                            if (order) //asc
                            {
                                arrangements = arrangements.OrderBy(a => a.TravelStartDate).ToList();
                            }
                            else //desc
                            {
                                 arrangements = arrangements.OrderByDescending(a => a.TravelStartDate).ToList();
                            }
                                break;
                        }

                    case "travelenddate":
                        {
                            if(order) //asc
                            {
                                arrangements = arrangements.OrderBy(a => a.TravelEndDate).ToList();
                            } else //desc
                            {
                                arrangements = arrangements.OrderByDescending(a => a.TravelEndDate).ToList();
                            }

                                break;
                        }
                }
            }

            ViewBag.TravelMinStartDate = travelMinStartDate;
            ViewBag.TravelMaxStartDate = travelMaxStartDate;
            ViewBag.TravelMinEndDate = travelMinEndDate;
            ViewBag.TravelMaxEndDate = travelMaxEndDate;
            ViewBag.TransportType = transportType;
            ViewBag.ArrangementType = arrangementType;
            ViewBag.Name = name;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;

            User loggedUser = (User)Session["user"];
            ViewBag.LoggedUser = loggedUser;

            return View(arrangements);
        }

        //GET: Home/Details/:id
        public ActionResult Details(int id, int? minGuests, int? maxGuests, bool? petAllowed, int? minPrice, int? maxPrice, string sortByAccommodationUnit, string sortOrderAccommodationUnit)
        {
            //ako korisnik ne salje vrednost npr za minGuests onda ce on uzeti podrazumevanu vrednost 0

            ArrangementService arrangementService = new ArrangementService();
            Arrangement arrangement = arrangementService.GetByArrangementId(id);

            //smestaj
            AccommodationService accommodationService = new AccommodationService();
            Accommodation accommodation = accommodationService.GetByAccommodationById(arrangement.AccommodationId);

            if(accommodation.LogicDelete != true)
            {
                ViewBag.Accomodation = accommodation;
            } else
            {
                ViewBag.Accomodation = null;
            }


            //smestajna jedinica
            AccommodationUnitService accommodationUnitService = new AccommodationUnitService();
            List<AccommodationUnit> accommodationUnits = accommodationUnitService.GetAllAccommodationUnits().Where(c => accommodation.AccommodationUnits.Contains(c.Id)).ToList();

            accommodationUnits = accommodationUnits.Where(ac => ac.LogicDelete == false).ToList();

            if (string.IsNullOrEmpty(sortByAccommodationUnit)) sortByAccommodationUnit = "";
            if (string.IsNullOrEmpty(sortOrderAccommodationUnit)) sortOrderAccommodationUnit = "asc";

            //filteri
            if (minGuests.HasValue)
                accommodationUnits = accommodationUnits.Where(u => u.MaxGuests >= minGuests.Value).ToList();

            if (maxGuests.HasValue)
                accommodationUnits = accommodationUnits.Where(u => u.MaxGuests <= maxGuests.Value).ToList();

            if (petAllowed.HasValue)
                accommodationUnits = accommodationUnits.Where(u => u.PetsAllowed == petAllowed.Value).ToList();

            if (minPrice.HasValue)
                accommodationUnits = accommodationUnits.Where(u => u.Price >= minPrice.Value).ToList();

            if (maxPrice.HasValue)
                accommodationUnits = accommodationUnits.Where(u => u.Price <= maxPrice.Value).ToList();

            //sortiranje
            bool order = sortOrderAccommodationUnit.ToLower().Equals("asc");

            switch (sortByAccommodationUnit.ToLower())
            {
                case "guests":
                    {
                        if (order) //asc
                        {
                            accommodationUnits = accommodationUnits.OrderBy(a => a.MaxGuests).ToList();
                        }
                        else //desc
                        {
                            accommodationUnits = accommodationUnits.OrderByDescending(a => a.MaxGuests).ToList();
                        }
                        break;
                    }
                case "price":
                    {
                        if (order) //asc
                        {
                            accommodationUnits = accommodationUnits.OrderBy(a => a.Price).ToList();
                        }
                        else //desc
                        {
                            accommodationUnits = accommodationUnits.OrderByDescending(a => a.Price).ToList();
                        }
                        break;
                    }
            }

            ViewBag.AccommodationUnits = accommodationUnits;

            //komentari za smestaj
            CommentService commentService = new CommentService();
            List<Comment> comments = commentService.GetAllComments().Where(c => c.AccommodationId == accommodation.Id).ToList();

            List<User> users = (List<User>)HttpContext.Application["users"];

            List<User> commentUsers = comments.Select(c => users.FirstOrDefault(u => u.Id == c.TouristId)).ToList();

            ViewBag.Comments = comments;
            ViewBag.CommentUsers = commentUsers;

            ReservationService reservationService = new ReservationService();
            List<Reservation> reservations = reservationService.GetAllReservations().Where(r => r.ArrangementId == arrangement.Id && r.Status == Status.Active).ToList();

            ViewBag.Reservations = reservations;

            ViewBag.MinGuests = minGuests;
            ViewBag.MaxGuests = maxGuests;
            ViewBag.PetAllowed = petAllowed;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortByAccommodationUnit = sortByAccommodationUnit;
            ViewBag.SortOrderAccommodationUnit = sortOrderAccommodationUnit;

            bool isPast = arrangement.TravelEndDate.AsDateTime() < DateTime.Now;
            ViewBag.CanAddUnits = !isPast;

            return View(arrangement);

        }
    }
}