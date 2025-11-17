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
    public class AccommodationUnitController : Controller
    {
        private AccommodationUnitService accommodationUnitService = new AccommodationUnitService();
        private ReservationService reservationService = new ReservationService();
        private ArrangementService arrangementService = new ArrangementService();
        private AccommodationService accommodationService = new AccommodationService();

        //GET: /AccommodationUnit
        public ActionResult Index()
        {
            List<AccommodationUnit> accommodationUnits = accommodationUnitService.GetAllAccommodationUnits();
            HttpContext.Application["accommodationUnits"] = accommodationUnits;
            return View(accommodationUnits);
        }

        //GET: /AccommodationUnit/Details/:id
        public ActionResult Details(int id)
        {
            AccommodationUnit accommodationUnit = accommodationUnitService.GetAccommodationUnitById(id);
            return View(accommodationUnit);
        }

        //GET: /AccommodationUnit/AddAccommodationUnit
        public ActionResult AddAccommodationUnit(int arrangementId)
        {
            ViewBag.ArrangementId = arrangementId;
            return View();
        }

        //POST: /AccommodationUnit/AddAccommodationUnit
        [HttpPost]
        public ActionResult AddAccommodationUnit(AccommodationUnit accommodationUnit, int arrangementId)
        {
            List<AccommodationUnit> accommodationUnits = accommodationUnitService.GetAllAccommodationUnits();
            int newUnitId = accommodationUnits.Any() ? accommodationUnits.Max(u => u.Id) + 1 : 1;

            accommodationUnit.Id = newUnitId;
            accommodationUnit.LogicDelete = false;

            accommodationUnitService.AddAccommodationUnit(accommodationUnit);
            HttpContext.Application["accommodationUnits"] = accommodationUnitService.GetAllAccommodationUnits();

            Arrangement arrangement = arrangementService.GetByArrangementId(arrangementId);
            Accommodation accommodation = accommodationService.GetByAccommodationById(arrangement.AccommodationId);
            accommodation.AccommodationUnits.Add(newUnitId);
            accommodationService.UpdateAccommodation(accommodation);
            HttpContext.Application["accommodations"] = accommodationService.GetAllAccommodations();

            return RedirectToAction("Details", "Home", new { id = arrangementId});
        }

        //GET: /AccommodationUnit/EditAccommodationUnit/:id
        public ActionResult EditAccommodationUnit(int accommodationUnitId, int arrangementId)
        {
            AccommodationUnit accommodationUnit = accommodationUnitService.GetAccommodationUnitById(accommodationUnitId);

            //da li postoji neka buduca rezervacija za ovaj unit
            bool activeReservationsWithThisUnit = reservationService.GetAllReservations().Any(r => r.SelectedUnitId == accommodationUnitId && r.Status == Status.Active &&
                            arrangementService.GetAllArrangements().Any(a => a.Id == r.ArrangementId && a.TravelStartDate.AsDateTime() > DateTime.Now));

            ViewBag.CanEditNumberOfBeds = !activeReservationsWithThisUnit; //ako nema buduca rezervacija
            ViewBag.ArrangementId = arrangementId;

            return View(accommodationUnit);
        }

        //POST: /AccommodationUnit/EditAccommodationUnit
        [HttpPost]
        public ActionResult EditAccommodationUnit(AccommodationUnit accommodationUnit, int arrangementId)
        {
            AccommodationUnit originAccommodationUnit = accommodationUnitService.GetAccommodationUnitById(accommodationUnit.Id);

            //da li postoji neka buduca rezervacija za ovaj unit
            bool activeReservationsWithThisUnit = reservationService.GetAllReservations().Any(r => r.SelectedUnitId == accommodationUnit.Id && r.Status == Status.Active &&
                            arrangementService.GetAllArrangements().Any(a => a.Id == r.ArrangementId && a.TravelStartDate.AsDateTime() > DateTime.Now));

            if(activeReservationsWithThisUnit == false)
            {
                originAccommodationUnit.MaxGuests = accommodationUnit.MaxGuests;
            } else
            {
                TempData["EditError"] = "Cannot change the number of beds because there are active future reservations.";
            }

            originAccommodationUnit.PetsAllowed = accommodationUnit.PetsAllowed;
            originAccommodationUnit.Price = accommodationUnit.Price;

            accommodationUnitService.UpdateAccommodationUnit(originAccommodationUnit);
            HttpContext.Application["accommodationUnits"] = accommodationUnitService.GetAllAccommodationUnits();
            return RedirectToAction("Details", "Home", new { id = arrangementId });
        }

        //POST: /AccommodationUnit/DeleteAccommodationUnit/:id
        [HttpPost]
        public ActionResult DeleteAccommodationUnit(int accommodationUnitId, int arrangementId)
        {
            AccommodationUnit accommodationUnit = accommodationUnitService.GetAccommodationUnitById(accommodationUnitId);

            //da li postoji neka buduca rezervacija za ovaj unit
            bool activeReservationsWithThisUnit = reservationService.GetAllReservations().Any(r => r.SelectedUnitId == accommodationUnitId && r.Status == Status.Active &&
                            arrangementService.GetAllArrangements().Any(a => a.Id == r.ArrangementId && a.TravelStartDate.AsDateTime() > DateTime.Now));

            if (activeReservationsWithThisUnit)
            {
                TempData["DeleteError"] = "Cannot delete this unit because it has active future reservations.";
                return RedirectToAction("Details", "Home", new { id = arrangementId });
            }

            accommodationUnit.LogicDelete = true;
            accommodationUnitService.UpdateAccommodationUnit(accommodationUnit);
            List<AccommodationUnit> accommodationUnits = accommodationUnitService.GetAllAccommodationUnits();
            HttpContext.Application["accommodationUnits"] = accommodationUnits;

            //treba izbrisati iz liste accommodationUnit id-jeva kod accommodation
            List<Accommodation> accommodationsHasThisUnit = accommodationService.GetAllAccommodations().Where(a => a.AccommodationUnits.Contains(accommodationUnitId)).ToList();

            if(accommodationsHasThisUnit.Count() > 0)
            {
                foreach(var acc in accommodationsHasThisUnit)
                {
                    acc.AccommodationUnits.Remove(accommodationUnitId);
                    accommodationService.UpdateAccommodation(acc);
                }
            }

            return RedirectToAction("Details", "Home", new { id = arrangementId} );
        }
    }
}