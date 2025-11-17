using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using web_project.DTO;
using web_project.Models;
using web_project.Services;

namespace web_project.Controllers
{
    public class ArrangementController : Controller
    {
        private ArrangementService arrangementService = new ArrangementService();
        private AccommodationService accommodationService = new AccommodationService();
        private AccommodationUnitService accommodationUnitService = new AccommodationUnitService();
        private UserService userService = new UserService();
        private ReservationService reservationService = new ReservationService();

        //GET: /Arrangement/AddFullArrangement
        public ActionResult AddFullArrangement()
        {
            return View();
        }

        //POST: /Arrangement/AddFullArrangement
        [HttpPost]
        public ActionResult AddFullArrangement(ArrangementFullDto model, HttpPostedFileBase ArrangementPicturePath)
        {
            User loggedUser = (User)Session["user"];
            if (loggedUser == null || loggedUser.Role != UserRole.Manager)
            {
                return RedirectToAction("Index", "Authentication");
            }

            //ARRANGEMENT
            List<Arrangement> arrangements = arrangementService.GetAllArrangements();
            int newArrangementId = arrangements.Any() ? arrangements.Max(u => u.Id) + 1 : 1; //sledeci index

            // Datum parsiranje
            string startDateFormatted = "";
            string endDateFormatted = "";
            if (!string.IsNullOrEmpty(model.TravelStartDate))
            {
                DateTime startDate = DateTime.Parse(model.TravelStartDate);
                startDateFormatted = startDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(model.TravelEndDate))
            {
                DateTime endDate = DateTime.Parse(model.TravelEndDate);
                endDateFormatted = endDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            //ACCOMMODATION

            AccommodationService accommodationService = new AccommodationService();
            List<Accommodation> accommodations = accommodationService.GetAllAccommodations();
            int newAccommodationId = accommodations.Any() ? accommodations.Max(a => a.Id) + 1 : 1;

            Accommodation accommodation = new Accommodation
            {
                Id = newAccommodationId,
                Name = model.AccommodationName,
                NumberOfStars = model.NumberOfStars,
                AccommodationType = (AccommodationType)Enum.Parse(typeof(AccommodationType), model.AccommodationType, true),
                PoolExistence = model.PoolExistence,
                SpaCentreExistence = model.SpaCentreExistence,
                PeopleWDisabilities = model.PeopleWDisabilities,
                WiFi = model.WiFi,
                AccommodationUnits = new List<int>(),
                LogicDelete = false
            };

            accommodationService.AddAccommodation(accommodation);

            //ACCOMMODATION UNIT
            List<AccommodationUnit> accommodationUnits = accommodationUnitService.GetAllAccommodationUnits();
            int newUnitId = accommodationUnits.Any() ? accommodationUnits.Max(u => u.Id) + 1 : 1;

            AccommodationUnit accommodationUnit = new AccommodationUnit
            {
                Id = newUnitId,
                MaxGuests = model.MaxGuests,
                PetsAllowed = model.PetsAllowed,
                Price = model.Price,
                LogicDelete = false
            };

            accommodationUnitService.AddAccommodationUnit(accommodationUnit);

            //upisujemo id u listu accommodation units id
            accommodation.AccommodationUnits.Add(accommodationUnit.Id);
            accommodationService.UpdateAccommodation(accommodation);

            string picturePath = "";
            if (ArrangementPicturePath != null && ArrangementPicturePath.ContentLength > 0)
            {
                string fileName = Path.GetFileName(ArrangementPicturePath.FileName);
                string path = Path.Combine(Server.MapPath("~/Content/Images/Arrangements"), fileName);
                ArrangementPicturePath.SaveAs(path);
                picturePath = "~/Content/Images/Arrangements/" + fileName;
            }

            Arrangement arrangement = new Arrangement
            {
                Id = newArrangementId,
                Name = model.Name,
                ArrangementType = (ArrangementType)Enum.Parse(typeof(ArrangementType), model.ArrangementType, true),
                Transport = (TransportType)Enum.Parse(typeof(TransportType), model.Transport, true),
                Location = model.Location,
                TravelStartDate = startDateFormatted,
                TravelEndDate = endDateFormatted,
                MaxPassengers = model.MaxPassengers,
                ArrangementDescription = model.ArrangementDescription,
                TravelProgram = model.TravelProgram,
                ArrangementPicturePath = picturePath,
                AccommodationId = accommodation.Id,
                ManagerId = loggedUser.Id,
                LogicDelete = false
            };

            arrangementService.AddArrangement(arrangement);

            loggedUser.Arrangements.Add(newArrangementId);
            userService.UpdateUser(loggedUser);

            HttpContext.Application["arrangements"] = arrangementService.GetAllArrangements();
            HttpContext.Application["accommodations"] = accommodationService.GetAllAccommodations();
            HttpContext.Application["accommodationUnits"] = accommodationUnitService.GetAllAccommodationUnits();

            return RedirectToAction("Index", "Home");
        }
        
        //GET: /Arrangement/EditArrangement/:id
        public ActionResult EditArrangement(int id)
        {
            User loggedUser = (User)Session["user"];
            if(loggedUser == null || loggedUser.Role != UserRole.Manager)
            {
                return RedirectToAction("Index", "Authentication");
            }
            
            Arrangement arrangement = arrangementService.GetByArrangementId(id);

            return View(arrangement);
        }

        //POST: /Arrangement/EditArrangement
        [HttpPost]
        public ActionResult EditArrangement(Arrangement arrangement, HttpPostedFileBase ArrangementPicturePath) 
        {
            User loggedUser = (User)Session["user"];
            if (loggedUser == null || loggedUser.Role != UserRole.Manager)
            {
                return RedirectToAction("Index", "Authentication");
            }

            if (arrangement.ManagerId != loggedUser.Id)
            {
                ViewBag.Error = "You are not allowed to edit this arrangement.";
                return View("Details", arrangement);
            }

            if (!string.IsNullOrEmpty(arrangement.TravelStartDate))
            {
                DateTime startDate = DateTime.Parse(arrangement.TravelStartDate);
                string formattedStartDate = startDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                arrangement.TravelStartDate = formattedStartDate;
            }

            if (!string.IsNullOrEmpty(arrangement.TravelEndDate))
            {
                DateTime endDate = DateTime.Parse(arrangement.TravelEndDate);
                string formattedEndDate = endDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                arrangement.TravelEndDate = formattedEndDate;
            }

            if (ArrangementPicturePath != null && ArrangementPicturePath.ContentLength > 0)
            {
                string fileName = Path.GetFileName(ArrangementPicturePath.FileName);
                string path = Path.Combine(Server.MapPath("~/Content/Images/Arrangements"), fileName);
                ArrangementPicturePath.SaveAs(path);
                arrangement.ArrangementPicturePath = "~/Content/Images/Arrangements/" + fileName;
            }

            arrangementService.UpdateArrangement(arrangement);
            HttpContext.Application["arrangements"] = arrangementService.GetAllArrangements();

            return RedirectToAction("Details", "Home", new { id = arrangement.Id });
        }

        //POST: /Arrangement/DeleteArrangement/:id
        [HttpPost]
        public ActionResult DeleteArrangement(int id)
        {
            User loggedUser = (User)Session["user"];
            if (loggedUser == null || loggedUser.Role != UserRole.Manager)
            {
                return RedirectToAction("Index", "Authentication");
            }

            Arrangement arrangement = arrangementService.GetByArrangementId(id);

            if (arrangement.ManagerId != loggedUser.Id)
            {
                ViewBag.Error = "You are not allowed to edit this arrangement.";
                return View("Details", "Home", new { id = id });
            }

            bool hasReservations = reservationService.GetAllReservations()
                                                      .Any(r => r.ArrangementId == id && r.Status == Status.Active);

            if (hasReservations)
            {
                TempData["DeleteError"] = "Cannot delete arrangement because it has active reservations.";
                return RedirectToAction("Details", "Home", new { id = id });
            }

            arrangement.LogicDelete = true;
            arrangementService.UpdateArrangement(arrangement);
            
            loggedUser.Arrangements.Remove(id);
            userService.UpdateUser(loggedUser);

            Accommodation accommodation = accommodationService.GetByAccommodationById(arrangement.AccommodationId);
            accommodation.LogicDelete = true;
            accommodationService.UpdateAccommodation(accommodation);

            foreach (int unitId in accommodation.AccommodationUnits.ToList())
            {
                AccommodationUnit unit = accommodationUnitService.GetAccommodationUnitById(unitId);
                if (unit != null)
                {
                    unit.LogicDelete = true;
                    accommodationUnitService.UpdateAccommodationUnit(unit);
                }
            }

            HttpContext.Application["arrangements"] = arrangementService.GetAllArrangements();
            HttpContext.Application["accommodations"] = accommodationService.GetAllAccommodations();
            HttpContext.Application["accommodationUnits"] = accommodationUnitService.GetAllAccommodationUnits();

            return RedirectToAction("Index", "Home");
        }

        //GET: /Arrangement/MyArrangements
        public ActionResult MyArrangements()
        {
            User user = (User)Session["user"];
            if (user == null || user.Role != UserRole.Manager)
                return RedirectToAction("Index", "Authentication");

            List<Arrangement> arrangements = arrangementService.GetAllArrangements()
                                                 .Where(a => a.ManagerId == user.Id && !a.LogicDelete)
                                                 .ToList();

            List<Reservation> reservations = reservationService.GetAllReservations()
                                         .Where(r => arrangements.Any(a => a.Id == r.ArrangementId))
                                         .ToList();

            List<User> allUsers = userService.GetAllUsers(); 
            List<User> touristsHaveReservations = allUsers
                                         .Where(u => reservations.Any(r => r.TouristId == u.Id))
                                         .ToList();

            ViewBag.Reservations = reservations;
            ViewBag.Tourists = touristsHaveReservations;

            return View(arrangements);
        }
    }
}