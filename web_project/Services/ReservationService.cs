using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using web_project.Models;

namespace web_project.Services
{
    public class ReservationService
    {
        private readonly string _filePath = "~/App_data/reservations.json";

        public List<Reservation> GetAllReservations()
        {
            var path = HostingEnvironment.MapPath(_filePath);

            if (!File.Exists(path))
            {
                return new List<Reservation>();
            }

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<Reservation>>(json);
        }

        public Reservation GetReservationById(int id)
        {
            List<Reservation> reservations = GetAllReservations();
            foreach (var reservation in reservations)
            {
                if (reservation.Id == id)
                {
                    return reservation;
                }
            }
            return new Reservation();
        }

        public void AddReservation(Reservation reservation)
        {
            List<Reservation> reservations = GetAllReservations();
            reservations.Add(reservation);
            SaveReservations(reservations);
        }

        public void UpdateReservation(Reservation updateReservation)
        {
            List<Reservation> reservations = GetAllReservations();
            int i;
            for (i = 0; i < reservations.Count; i++)
            {
                if (reservations[i].Id == updateReservation.Id)
                {
                    reservations[i] = updateReservation;
                    break;
                }
            }
            SaveReservations(reservations);
        }

        private void SaveReservations(List<Reservation> reservations)
        {
            var path = HostingEnvironment.MapPath(_filePath);
            var json = JsonConvert.SerializeObject(reservations, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}