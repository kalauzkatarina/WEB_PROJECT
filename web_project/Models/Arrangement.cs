using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace web_project.Models
{
    public enum ArrangementType
    {
        BedAndBreakfast,
        HalfBoard,
        FullBoard,         
        AllInclusive,
        RentApartment
    }
    public enum TransportType
    {
        Bus,
        Airplane,
        BusAndAirplane,
        Individual,
        Other
    }
    public class Arrangement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ArrangementType ArrangementType { get; set; }
        public TransportType Transport { get; set; }
        public string Location { get; set; }
        public string TravelStartDate { get; set; } //dd/MM/yyyy
        public string TravelEndDate { get; set; } //dd/MM/yyyy
        public int MaxPassengers { get; set; }
        public string ArrangementDescription { get; set; }
        public string TravelProgram {  get; set; }
        public string ArrangementPicturePath { get; set; }
        public int AccommodationId {  get; set; } //accommodation id
        public int ManagerId { get; set; } //svaki menadzer moze da kreira neki aranzman
        public bool LogicDelete { get; set; } //logicko brisanje true - obrisan logicki

        public Arrangement()
        {
            Id = 0;
            Name = "";
            ArrangementType = ArrangementType.BedAndBreakfast;
            Transport = TransportType.Airplane;
            Location = "";
            TravelStartDate = "";
            TravelEndDate = "";
            MaxPassengers = 0;
            ArrangementDescription = "";
            ArrangementPicturePath = "";
            AccommodationId = 0;
            ManagerId = 0;
            LogicDelete = false;
        }

        public Arrangement(int  id, string name, ArrangementType arrangementType, TransportType transport, string location, string travelStartDate, string travelEndDate, int maxPassengers, string arrangementDescription, string travelProgram, string arrangementPicturePath, int accommodationId, int managerId)
        {
            Id = id;
            Name = name;
            ArrangementType = arrangementType;
            Transport = transport;
            Location = location;
            TravelStartDate = travelStartDate;
            TravelEndDate = travelEndDate;
            MaxPassengers = maxPassengers;
            ArrangementDescription = arrangementDescription;
            TravelProgram = travelProgram;
            ArrangementPicturePath = arrangementPicturePath;
            AccommodationId = accommodationId;
            ManagerId = managerId;
            LogicDelete = false;
        }
    }
}