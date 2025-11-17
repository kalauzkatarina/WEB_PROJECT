using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;
using web_project.Models;

namespace web_project.Services
{
    public class UserService
    {
        private readonly string _filePath = "~/App_Data/users.json";

        //HostingEnvironment.MapPath() prevodi
        //virtualnu putanju (~/) u stvarnu fizičku putanju na serveru.
        public List<User> GetAllUsers()
        {
            var path = HostingEnvironment.MapPath(_filePath);

            if (!File.Exists(path))
            {
                return new List<User>();
            }

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<User>>(json);
        }

        public User GetUserById(int id)
        {
            List<User> users = GetAllUsers();
            foreach (var user in users)
            {
                if(user.Id == id)
                {
                    return user;
                }
            }
            return new User();
        }

        public void AddUser(User user)
        {
            List<User> users = GetAllUsers();
            users.Add(user);
            SaveUsers(users);
        }

        public void AddReservationId(int reservationId, int userId)
        {
            List<User> users = GetAllUsers();
            foreach(var user in users)
            {
                if(user.Id == userId)
                {
                    user.Reservations.Add(reservationId);
                    break;
                }
            }
            SaveUsers(users);
        }

        public void RemoveReservationId(int reservationId, int userId)
        {
            List<User> users = GetAllUsers();
            foreach (var user in users)
            {
                if (user.Id == userId && user.Reservations.Contains(reservationId))
                {
                    user.Reservations.Remove(reservationId);
                    break;
                }
            }
            SaveUsers(users);
        }

        public void UpdateUser(User updateUser)
        {
            List<User> users = GetAllUsers();
            int i;
            for (i = 0; i < users.Count; i++)
            {
                if (users[i].Id == updateUser.Id)
                {
                    users[i] = updateUser;
                    break;
                }
            }
            SaveUsers(users);
        }

        private void SaveUsers(List<User> users)
        {
            var path = HostingEnvironment.MapPath(_filePath);
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}