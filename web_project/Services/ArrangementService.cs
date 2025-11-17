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
    public class ArrangementService
    {
        private readonly string _filePath = "~/App_Data/arrangements.json";

        public List<Arrangement> GetAllArrangements()
        {
            var path = HostingEnvironment.MapPath(_filePath);

            if (!File.Exists(path))
            {
                return new List<Arrangement>();
            }

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<Arrangement>>(json);
        }

        public Arrangement GetByArrangementId(int id)
        {
            List<Arrangement> arrangements = GetAllArrangements();
            foreach (var arrangement in arrangements)
            {
                if (arrangement.Id == id)
                {
                    return arrangement;
                }
            }
            return new Arrangement();
        }

        public void AddArrangement(Arrangement arrangement)
        {
            List<Arrangement> arrangements = GetAllArrangements();
            arrangements.Add(arrangement);
            SaveArrangement(arrangements);
        }

        public void UpdateArrangement(Arrangement updateArrangement)
        {
            List<Arrangement> arrangements = GetAllArrangements();
            int i; 
            for (i = 0; i < arrangements.Count; i++)
            {
                if (arrangements[i].Id == updateArrangement.Id)
                {
                    arrangements[i] = updateArrangement;
                    break;
                }
            }
            SaveArrangement(arrangements);
        }

        public void DeleteArrangement(int id)
        {
            List<Arrangement> arrangements = GetAllArrangements();
            int i; //za poziciju gde se nalazi arrangement koji treba da se brise
            for(i = 0; i <  arrangements.Count; i++)
            {
                if(arrangements[i].Id == id)
                {
                    arrangements[i].LogicDelete = true;
                    break;
                }
            }
            SaveArrangement(arrangements);
        }

        private void SaveArrangement(List<Arrangement> arrangements)
        {
            var path = HostingEnvironment.MapPath(_filePath);
            var json = JsonConvert.SerializeObject(arrangements, Formatting.Indented);
            //Ova funkcija prepisuje ceo fajl sa sadržajem koji si prosledila (json).
            //Važno: ne dodaje, ne duplira, nego briše postojeći sadržaj i upisuje novi.
            File.WriteAllText(path, json);
        }
    }
}