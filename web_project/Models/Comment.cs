using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_project.Models
{
    public enum CommentStatus
    {
        Pending,
        Approved,
        Rejected
    }
    public class Comment
    {
        public int Id { get; set; }
        public int TouristId { get; set; } //tourist id
        public int AccommodationId { get; set; } //accommodation id
        public string CommentText { get; set; }
        public int Rating { get; set; } // od 1 do 5
        public CommentStatus Status { get; set; }

        public Comment()
        {
            Id = 0;
            TouristId = 0;
            AccommodationId = 0;
            CommentText = "";
            Rating = 0;
            Status = CommentStatus.Pending; 
        }
    }
}