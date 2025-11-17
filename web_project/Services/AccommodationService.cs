using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using web_project.Models;

namespace web_project.Services
{
    public class AccommodationService
    {
        private readonly string _filePath = "~/App_Data/accommodations.json";

        public List<Accommodation> GetAllAccommodations()
        {
            var path = HostingEnvironment.MapPath(_filePath);

            if (!File.Exists(path))
            {
                return new List<Accommodation>();
            }

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<Accommodation>>(json);
        }

        public Accommodation GetByAccommodationById(int id)
        {
            List<Accommodation> accommodations = GetAllAccommodations();
            foreach (var accommodation in accommodations)
            {
                if (accommodation.Id == id)
                {
                    return accommodation;
                }
            }
            return new Accommodation();
        }

        public void AddAccommodation(Accommodation accommodation)
        {
            List<Accommodation> accommodations = GetAllAccommodations();
            accommodations.Add(accommodation);
            SaveAccommodations(accommodations);
        }

        public void UpdateAccommodation(Accommodation updateAccommodation)
        {
            List<Accommodation> accommodations = GetAllAccommodations();
            int i;
            for (i = 0; i < accommodations.Count; i++)
            {
                if (accommodations[i].Id == updateAccommodation.Id)
                {
                    accommodations[i] = updateAccommodation;
                    break;
                }
            }
            SaveAccommodations(accommodations);
        }

        public void DeleteAccommodation(int id)
        {
            List<Accommodation> accommodations = GetAllAccommodations();
            int i;
            for (i = 0; i < accommodations.Count; i++)
            {
                if (accommodations[i].Id == id)
                {
                    accommodations[i].LogicDelete = true;
                    break;
                }
            }
            SaveAccommodations(accommodations);
        }

        private void SaveAccommodations(List<Accommodation> accommodations)
        {
            var path = HostingEnvironment.MapPath(_filePath);
            var json = JsonConvert.SerializeObject(accommodations, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}