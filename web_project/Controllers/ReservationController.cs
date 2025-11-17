using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web_project.Models;
using web_project.Services;

namespace web_project.Controllers
{
    public class ReservationController : Controller
    {
        private ReservationService reservationService = new ReservationService();
        private ArrangementService arrangementService = new ArrangementService();
        private AccommodationUnitService accommodationUnitService = new AccommodationUnitService();

        //GET: /Reservation
        public ActionResult Index(string search, string sortBy, string sortOrder)
        {
            var user = (User)Session["user"];
            if (user == null || user.Role != UserRole.Tourist)
            {
                return RedirectToAction("Index", "Home");
            }

            var allReservations = reservationService.GetAllReservations();
            var myReservations = allReservations.Where(r => r.TouristId == user.Id).ToList();
            ArrangementService arrangementService = new ArrangementService();
      
            var activeArrangements = arrangementService.GetAllArrangements()
                                    .Where(a => !a.LogicDelete)
                                    .ToList();

            myReservations = myReservations
                             .Where(r => activeArrangements.Any(a => a.Id == r.ArrangementId))
                             .ToList();

            var arrangements = arrangementService.GetAllArrangements()
                                          .Where(a => myReservations.Any(r => r.ArrangementId == a.Id && a.LogicDelete != true))
                                          .ToList();
            //?.Name → uzmi njegovo ime (ako je null, neće puknuti program nego preskočiti)
            if (!string.IsNullOrEmpty(search))
            {
                myReservations = myReservations.Where(r => r.Id.ToString().Contains(search) ||
                arrangements.FirstOrDefault(a => a.Id == r.ArrangementId)?.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                r.Status.ToString().IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            if (string.IsNullOrEmpty(sortOrder)) sortOrder = "asc";
            bool order = sortOrder.ToLower().Equals("asc");

            switch (sortBy?.ToLower())
            {
                case "arrangement":
                    {
                        if (order) //asc
                        {
                            myReservations = myReservations.OrderBy(r => arrangements.FirstOrDefault(a => a.Id == r.ArrangementId)?.Name).ToList();
                        }
                        else //desc
                        {
                            myReservations = myReservations.OrderByDescending(r => arrangements.FirstOrDefault(a => a.Id == r.ArrangementId)?.Name).ToList();
                        }
                        break;
                    }
                default:
                    break;
            }
            
            ViewBag.Search = search; 
            ViewBag.SortOrder = sortOrder;
            ViewBag.SortBy = sortBy;
            ViewBag.Arrangements = arrangements;

            return View(myReservations);
        }

        //GET: /Reservation/Details/:id
        public ActionResult Details(int id)
        {
            Reservation reservation = reservationService.GetReservationById(id);
            
            UserService userService = new UserService();
            User tourist = userService.GetUserById(reservation.TouristId);

            Arrangement arrangement = arrangementService.GetByArrangementId(reservation.ArrangementId);

            AccommodationService accommodationService = new AccommodationService();
            Accommodation accommodation = accommodationService.GetByAccommodationById(arrangement.AccommodationId);

            AccommodationUnit accommodationUnit = accommodationUnitService.GetAccommodationUnitById(reservation.SelectedUnitId);

            ViewBag.Tourist = tourist;
            ViewBag.Arrangement = arrangement;
            ViewBag.Accommodation = accommodation;
            ViewBag.Unit = accommodationUnit;

            return View(reservation);
        }

        //POST: /Reservation/ReserveTheUnit/:id
        [HttpPost]
        public ActionResult ReserveTheUnit(int arrangementId, int accommodationUnitId)
        {
            var user = (User)Session["user"];
            if (user == null || user.Role != UserRole.Tourist)
            {
                return RedirectToAction("Details", "Home", new { id = arrangementId });
            }

            ReservationService reservationService = new ReservationService();
            List<Reservation> reservations = reservationService.GetAllReservations();

            var existingReservation = reservations
                .FirstOrDefault(r => r.SelectedUnitId == accommodationUnitId && r.TouristId == user.Id);

            if (existingReservation != null)
            {
                if (existingReservation.Status == Status.Canceled)
                {
                    existingReservation.Status = Status.Active;
                    reservationService.UpdateReservation(existingReservation);
                }
            }
            else
            {
                int newId = reservations.Any() ? reservations.Max(r => r.Id) + 1 : 1;

                Reservation newReservation = new Reservation
                {
                    Id = newId,
                    TouristId = user.Id,
                    ArrangementId = arrangementId,
                    SelectedUnitId = accommodationUnitId,
                    Status = Status.Active
                };
                reservationService.AddReservation(newReservation);

                UserService userService = new UserService();
                userService.AddReservationId(newId, user.Id);
                HttpContext.Application["users"] = userService.GetAllUsers();
            }

            HttpContext.Application["reservations"] = reservationService.GetAllReservations();

            return RedirectToAction("Details", "Home", new { id = arrangementId });
        }


        //POST: /Reservation/CancelTheUnit/:id
        [HttpPost]
        public ActionResult CancelTheUnit(int reservationId)
        {
            var user = (User)Session["user"];
            if (user == null || user.Role != UserRole.Tourist)
            {
                return RedirectToAction("Index", "Home");
            }

            Reservation reservation = reservationService.GetReservationById(reservationId);
            if (reservation == null || reservation.TouristId != user.Id)
            {
                return RedirectToAction("Index", "Reservation");
            }

            ArrangementService arrangementService = new ArrangementService();
            Arrangement arrangement = arrangementService.GetByArrangementId(reservation.ArrangementId);

            DateTime TravelEndDate = DateTime.ParseExact(arrangement.TravelEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (TravelEndDate < DateTime.Now)
            {
                TempData["CancelMessage"] = "Cannot cancel, arrangement has already passed.";
                return RedirectToAction("Index", "Reservation");
            }

            reservation.Status = Status.Canceled;
            reservationService.UpdateReservation(reservation);

            HttpContext.Application["reservations"] = reservationService.GetAllReservations();

            return RedirectToAction("Index", "Reservation");
        }
    }
}