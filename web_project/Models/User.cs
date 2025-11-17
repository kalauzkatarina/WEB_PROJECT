using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace web_project.Models
{
    public enum Gender
    {
        Women,
        Men
    }

    public enum UserRole
    {
        Administrator,
        Manager,
        Tourist
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } //unique
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName {get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
        public string Date {  get; set; } //dd/MM/yyyy
        public UserRole Role { get; set; }
        public List<int> Reservations { get; set; } //lista id-jeva
        public List<int> Arrangements { get; set; } //lista id-jeva

        public bool LoggedIn { get; set; }

        public User()
        {
            Id = 0;
            Username = "";
            Password = "";
            FirstName = "";
            LastName = "";
            Gender = Gender.Women;
            Email = "";
            Date = "";
            Role = UserRole.Tourist;
            Reservations = new List<int>();
            Arrangements = new List<int>();
            LoggedIn = false;
        }

        public User(int id, string username, string password, string firstName, string lastName, Gender gender, string email, string date, UserRole role, List<int> reservations, List<int> arrangements)
        {
            Id = id;
            Username = username;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            Email = email;
            Date = date;
            Role = role;
            Reservations = reservations;
            Arrangements = arrangements;
            LoggedIn = false; ;
        }
    }
}