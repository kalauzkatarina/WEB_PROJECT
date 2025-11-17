using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_project.Models
{
    public class AccommodationUnit
    {
        public int Id { get; set; }
        public int MaxGuests { get; set; }
        public bool PetsAllowed { get; set; }
        public int Price { get; set; }
        public bool LogicDelete { get; set; } //logicko brisanje true - logicki obrisan

        public AccommodationUnit()
        {
            Id = 0;
            MaxGuests = 0;
            PetsAllowed = false;
            Price = 0;
            LogicDelete = false;
        }

        public AccommodationUnit(int id, int maxGuests, bool petsAllowed, int price)
        {
            Id = id;
            MaxGuests = maxGuests;
            PetsAllowed = petsAllowed;
            Price = price;
            LogicDelete = false;
        }
    }
}