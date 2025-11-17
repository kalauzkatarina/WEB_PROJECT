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
    public class UserController : Controller
    {
        private UserService userService = new UserService();

        //GET: /User/AllUsers
        public ActionResult AllUsers (string searchFirstName, string searchLastName, UserRole? role)
        {
            User currentUser = (User)Session["user"];
            if(currentUser == null || currentUser.Role != UserRole.Administrator)
            {
                return RedirectToAction("Index", "Home");
            }

            List<User> users = userService.GetAllUsers();

            if (!string.IsNullOrEmpty(searchFirstName))
            {
                users = users.Where(u => u.FirstName.ToLower().Contains(searchFirstName.ToLower())).ToList();
            }

            if(!string.IsNullOrEmpty(searchLastName))
            {
                users = users.Where(u => u.LastName.ToLower().Contains(searchLastName.ToLower())).ToList();
            }

            if(role.HasValue)
            {
                users = users.Where(u => u.Role == role.Value).ToList();
            }

            ViewBag.SearchFirstName = searchFirstName;
            ViewBag.SearchLastName = searchLastName;
            ViewBag.Role = role;

            return View(users);
        }

        //GET: /User/AddManager
        public ActionResult AddManager()
        {
            User currentUser = (User)Session["user"];

            if(currentUser == null || currentUser.Role != UserRole.Administrator)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //POST: /User/AddManager
        [HttpPost]
        public ActionResult AddManager(string username, string password, string firstName, string lastName, string gender, string email, string date)
        {
            List<User> users = (List<User>)HttpContext.Application["users"];

            // provera da li postoji isti username
            if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                ViewBag.DoubleUsernameErrorMessage = $"User with username {username} already exists!";
                return View();
            }

            int newId = users.Any() ? users.Max(u => u.Id) + 1 : 1;

            Gender parseGender = gender == "female" ? Gender.Women : Gender.Men;

            DateTime birthDate = DateTime.Parse(date);
            string formattedBirthDate = birthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

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
                Role = UserRole.Manager, 
                Reservations = new List<int>(),
                Arrangements = new List<int>(),
                LoggedIn = false 
            };

            userService.AddUser(newUser);

            HttpContext.Application["users"] = users;

            TempData["SuccessMessage"] = $"Manager '{newUser.Username}' added successfully!";
            return RedirectToAction("AllUsers");
        }

        //GET: /User/Details/:id
        public ActionResult Details(int id)
        {
            User user = userService.GetUserById(id);
            return View(user);
        }

        //GET: /User/AddUser
        public ActionResult AddUser()
        {
            return View(new User());
        }
        //POST: /User/AddUser
        [HttpPost]
        public ActionResult AddUser(User user)
        {
            userService.AddUser(user);
            HttpContext.Application["users"] = userService.GetAllUsers();
            return RedirectToAction("Index");
        }

        //GET: /User/EditUser/:id
        public ActionResult EditUser(int id)
        {
            User user = userService.GetUserById(id);
            return View(user);
        }

        //POST: /User/EditUser
        [HttpPost]
        public ActionResult EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(user.Date))
                {
                    DateTime birthDate = DateTime.Parse(user.Date);
                    string formattedBirthDate = birthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    user.Date = formattedBirthDate;
                }

                userService.UpdateUser(user);
                HttpContext.Application["users"] = userService.GetAllUsers();
                return RedirectToAction("Details", new { id = user.Id }); // vrati korisnika na profil
            }

            return View(user); // ako nešto nije u redu, prikaži formu ponovo
        }
    }
}