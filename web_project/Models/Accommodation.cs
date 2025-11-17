using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace web_project.Models
{
    public enum AccommodationType
    {
        Hotel,
        Motel,
        Villa
    }
    public class Accommodation
    {
        public int Id { get; set; }
        public AccommodationType AccommodationType { get; set; }
        public string Name { get; set; }
        public int NumberOfStars { get; set; }
        public bool PoolExistence { get; set; }
        public bool SpaCentreExistence { get; set; }
        public bool PeopleWDisabilities { get; set; }
        public bool WiFi { get; set; }
        public List<int> AccommodationUnits { get; set; } //accommodationUnit ids
        public bool LogicDelete { get; set; } //logicko brisanje true - logicki obrisan

        public Accommodation()
        {
            Id = 0;
            AccommodationType = AccommodationType.Hotel;
            Name = "";
            NumberOfStars = 0;
            PoolExistence = false;
            SpaCentreExistence = false;
            PeopleWDisabilities = false;
            WiFi = false;
            AccommodationUnits = new List<int>();
            LogicDelete = false;
        }

        public Accommodation(int id, AccommodationType accommodationType, string name, int numberOfStars, bool poolExistence, bool spaCentreExistence, bool peopleWDisabilities, bool wiFi, List<int> accommodationUnits)
        {
            Id = id;
            AccommodationType = accommodationType;
            Name = name;
            NumberOfStars = numberOfStars;
            PoolExistence = poolExistence;
            SpaCentreExistence= spaCentreExistence;
            PeopleWDisabilities = peopleWDisabilities;
            WiFi = wiFi;
            AccommodationUnits = accommodationUnits;
            LogicDelete = false;
        }
    }
}