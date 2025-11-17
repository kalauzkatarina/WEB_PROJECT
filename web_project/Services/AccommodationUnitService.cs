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
    public class AccommodationUnitService
    {
        private readonly string _filePath = "~/App_Data/accommodationUnits.json";

        public List<AccommodationUnit> GetAllAccommodationUnits()
        {
            var path = HostingEnvironment.MapPath(_filePath);
            
            if (!File.Exists(path))
            {
                return new List<AccommodationUnit>();
            }

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<AccommodationUnit>>(json);
        }

        public AccommodationUnit GetAccommodationUnitById(int id)
        {
            List<AccommodationUnit> accommodationUnits = GetAllAccommodationUnits();
            foreach (var accommodationUnit in accommodationUnits)
            {
                if (accommodationUnit.Id == id)
                {
                    return accommodationUnit;
                }
            }
            return new AccommodationUnit();
        }

        public void AddAccommodationUnit(AccommodationUnit accommodationUnit)
        {
            List<AccommodationUnit> accommodationUnits = GetAllAccommodationUnits();
            accommodationUnits.Add(accommodationUnit);
            SaveAccommodationUnits(accommodationUnits);
        }

        public void UpdateAccommodationUnit(AccommodationUnit updateAccommodationUnit)
        {
            List<AccommodationUnit> accommodationUnits = GetAllAccommodationUnits();
            int i;
            for (i = 0; i < accommodationUnits.Count; i++)
            {
                if (accommodationUnits[i].Id == updateAccommodationUnit.Id)
                {
                    accommodationUnits[i] = updateAccommodationUnit;
                    break;
                }
            }
            SaveAccommodationUnits(accommodationUnits);
        }

        public void DeleteAccommodationUnit(int id)
        {
            List<AccommodationUnit> accommodationUnits = GetAllAccommodationUnits();
            int i;
            for (i = 0; i < accommodationUnits.Count; i++)
            {
                if (accommodationUnits[i].Id == id)
                {
                    accommodationUnits[i].LogicDelete = true;
                    break;
                }
            }
            SaveAccommodationUnits(accommodationUnits);
        }

        private void SaveAccommodationUnits(List<AccommodationUnit> accommodationUnits)
        {
            var path = HostingEnvironment.MapPath(_filePath);
            var json = JsonConvert.SerializeObject(accommodationUnits, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}