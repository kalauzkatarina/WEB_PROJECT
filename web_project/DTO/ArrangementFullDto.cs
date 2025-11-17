using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using web_project.Models;

namespace web_project.DTO
{
    public class ArrangementFullDto
    {
        //podaci za arrangement
        public string Name { get; set; }
        public string ArrangementType { get; set; }
        public string Transport { get; set; }
        public string Location { get; set; }
        public string TravelStartDate { get; set; }
        public string TravelEndDate { get; set; }
        public int MaxPassengers {  get; set; }
        public string ArrangementDescription { get; set; }
        public string TravelProgram {  set; get; }

        //podaci za accommodation
        public string AccommodationType { get; set; }
        public string AccommodationName { get; set; }
        public int NumberOfStars { get; set; }
        public bool PoolExistence { get; set; }
        public bool SpaCentreExistence { get; set; }
        public bool PeopleWDisabilities { get; set; }
        public bool WiFi { get; set; }

        //podaci za accommodationUnit
        public int MaxGuests { get; set; }
        public bool PetsAllowed { get; set; }
        public int Price { get; set; }
    }
}