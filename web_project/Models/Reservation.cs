using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_project.Models
{
    public enum Status
    {
        Active,
        Canceled
    }
    public class Reservation
    {
        public int Id { get; set; }
        public int TouristId { get; set; } //tourist id
        public Status Status { get; set; }
        public int ArrangementId { get; set; } //arrangement id
        public int SelectedUnitId { get; set; } //unit id

        public Reservation()
        {
            Id = 0;
            TouristId = 0;
            Status = Status.Canceled;
            ArrangementId = 0;
            SelectedUnitId = 0;
        }

        public Reservation(int id, int touristId, Status status, int arrangementId, int selectedUnitId)
        {
            Id = id;
            TouristId = touristId;
            Status = status;
            ArrangementId = arrangementId;
            SelectedUnitId = selectedUnitId;
        }
    }
}