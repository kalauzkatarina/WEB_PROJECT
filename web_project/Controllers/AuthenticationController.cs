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
    public class AuthenticationController : Controller
    {
        private UserService userService = new UserService();

        //GET: Authentication
        public ActionResult Index()
        {
            //login forma

            return View();
        }

        //GET: Authentication/Register
        public ActionResult Register()
        {
            //register forma

            return View(); 
        }
        //POST: Authentication/Register
        [HttpPost]
        public ActionResult Register(string username, string password, string firstName, string lastName, string gender, string email, string date)
        {
            List<User> users = (List<User>)HttpContext.Application["users"];

            if (users.Any(u => u.Username.Equals(username)))
            {
                ViewBag.DoubleUsernameErrorMessage = $"User with username {username} already exists!";
                return View();
            }

            int newId = users.Any() ? users.Max(u => u.Id) + 1 : 1; //garantuje da uzmemo uvek najveci postojeci ID i dodati +1

            Gender parseGender = gender == "female" ? Gender.Women : Gender.Men;

            DateTime birthDate = DateTime.Parse(date);
            string formattedBirthDate = birthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); //ovaj cultureInfo treba dodati i onda ce lepo parsirati da bude u formatu dd/MM/yyyy
            //bez cultureInfo je samo u formatu dd-MM-yyyy

            User newUser = new User
            {
                Id = newId,
                Username = username,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Gender = parseGender,
                Email = email,
                Date = formattedBirthDate,
                Role = UserRole.Tourist,
                Reservations = new List<int>(),
                Arrangements = new List<int>(),
                LoggedIn = true
            };

            userService.AddUser(newUser);
            
            HttpContext.Application["users"] = users;
            //postavljanje u session kao trenutno prijavljen korisnik
            Session["user"] = newUser;
            return RedirectToAction("Index", "Home");
        }

        //POST: Authentication/Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            List<User> users = (List<User>)HttpContext.Application["users"];

            User user = users.Find(u => u.Username.Equals(username) && u.Password.Equals(password));
            if (user == null)
            {
                ViewBag.Message = $"User with credentials does not exists!";
                return View("Index");
            }
            user.LoggedIn = true;
            UserService userService = new UserService();
            userService.UpdateUser(user);
            Session["user"] = user;
            return RedirectToAction("Index", "Home");
        }

        //GET: Authentication/Logout
        public ActionResult Logout(string username)
        {
            var user = Session["user"] as User;
            if (user != null)
            {
                user.LoggedIn = false;
                userService.UpdateUser(user);

                var users = (List<User>)HttpContext.Application["users"];
                var idx = users.FindIndex(u => u.Id == user.Id);
                if (idx >= 0)
                {
                    users[idx] = user;
                    HttpContext.Application["users"] = users;
                }
            }

            Session["user"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}