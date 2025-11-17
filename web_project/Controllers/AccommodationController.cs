using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using web_project.Models;
using web_project.Services;

namespace web_project.Controllers
{
    public class AccommodationController : Controller
    {
        private AccommodationService accommodationService = new AccommodationService();
        private ArrangementService arrangementService = new ArrangementService();


        //GET: /Accommodation
        public ActionResult Index()
        {
            List<Accommodation> accommodations = accommodationService.GetAllAccommodations();
            HttpContext.Application["accommodations"] = accommodations;
            return View(accommodations);
        }

        //GET: /Accommodation/Details/:id
        public ActionResult Details(int id)
        {
            Accommodation accommodation = accommodationService.GetByAccommodationById(id);
            return View(accommodation);
        }

        //GET: /Accommodation/AddAccommodation/:id
        public ActionResult AddAccommodation(int arrangementId)
        {
            var user = Session["user"] as User;
            if (user == null || user.Role != UserRole.Manager)
                return RedirectToAction("Index", "Authentication");

            var arrangement = arrangementService.GetByArrangementId(arrangementId);
            if (arrangement == null || arrangement.ManagerId != user.Id)
                return RedirectToAction("Index", "Home");

            ViewBag.ArrangementId = arrangementId;
            return View();
        }

        //POST: /Accommodation/AddAccommodation
        [HttpPost]
        public ActionResult AddAccommodation(Accommodation accommodation, int arrangementId)
        {
            var accommodations = accommodationService.GetAllAccommodations();
            int newId = accommodations.Any() ? accommodations.Max(a => a.Id) + 1 : 1;

            accommodation.Id = newId;
            accommodation.LogicDelete = false;

            accommodationService.AddAccommodation(accommodation);

            // poveži sa aranžmanom
            Arrangement arrangement = arrangementService.GetByArrangementId(arrangementId);
            arrangement.AccommodationId = accommodation.Id;
            arrangementService.UpdateArrangement(arrangement);

            HttpContext.Application["accommodations"] = accommodationService.GetAllAccommodations();
            HttpContext.Application["arrangements"] = arrangementService.GetAllArrangements();

            return RedirectToAction("Details", "Home", new { id = arrangementId });
        }

        //GET: /Accommodation/EditAccommodation/:id
        public ActionResult EditAccommodation(int accommodationId, int arrangementId)
        {
            Accommodation accommodation = accommodationService.GetByAccommodationById(accommodationId);

            ViewBag.ArrangementId = arrangementId;

            return View(accommodation);
        }

        //POST: /Accommodation/EditAccommodation
        [HttpPost]
        public ActionResult EditAccommodation(Accommodation updatedAccommodation, int arrangementId)
        {
            Accommodation originAccommodation = accommodationService.GetByAccommodationById(updatedAccommodation.Id);

            originAccommodation.Name = updatedAccommodation.Name;
            originAccommodation.NumberOfStars = updatedAccommodation.NumberOfStars;
            originAccommodation.AccommodationType = updatedAccommodation.AccommodationType;
            originAccommodation.PoolExistence = updatedAccommodation.PoolExistence;
            originAccommodation.SpaCentreExistence = updatedAccommodation.SpaCentreExistence;
            originAccommodation.PeopleWDisabilities = updatedAccommodation.PeopleWDisabilities;
            originAccommodation.WiFi = updatedAccommodation.WiFi;

            accommodationService.UpdateAccommodation(originAccommodation);
            HttpContext.Application["accommodations"] = accommodationService.GetAllAccommodations();
            return RedirectToAction("Details", "Home", new { id = arrangementId});
        }

        //POST: /Accommodation/DeleteAccommodation/:id
        [HttpPost]
        public ActionResult DeleteAccommodation(int accommodationId, int arrangementId)
        {
            Accommodation accommodation = accommodationService.GetByAccommodationById(accommodationId);

            List<Arrangement> arrangementsWithThisAccommodation = arrangementService.GetAllArrangements().Where(a => a.AccommodationId == accommodation.Id &&
            a.TravelStartDate.AsDateTime() > DateTime.Now && a.Id != arrangementId).ToList();

            if (arrangementsWithThisAccommodation.Count() > 0)
            {
                TempData["DeleteError"] = "You cannot edit this accommodation because it is used in future arrangements.";
                return RedirectToAction("Details", "Home", new { id = arrangementId });
            }

            accommodation.LogicDelete = true;
            accommodationService.UpdateAccommodation(accommodation);
            List<Accommodation> accommodations = accommodationService.GetAllAccommodations();
            HttpContext.Application["accommodations"] = accommodations;
            return RedirectToAction("Details", "Home", new { id = arrangementId });
        }
    }
}