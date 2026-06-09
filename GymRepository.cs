using Gym.Core;
using Gym.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gym.Data
{
    public class GymRepository
    {
        private DatabaseHelper _db = new DatabaseHelper();
        private Dictionary<DateTime, List<string>> _schedule = new Dictionary<DateTime, List<string>>();

        public void AddClient(Client client)
        {
            _db.AddClient(client);
        }

        public List<Client> GetAllClients()
        {
            return _db.GetAllClients();
        }

        public Client this[int id]
        {
            get { return GetAllClients().FirstOrDefault(c => c.Id == id); }
        }

        public void AddToSchedule(DateTime date, string activity)
        {
            if (!_schedule.ContainsKey(date)) _schedule[date] = new List<string>();
            _schedule[date].Add(activity);
        }

        public void UpdateClient(Client client)
        {
            _db.UpdateClient(client);
        }

        public void DeleteClient(int id)
        {
            _db.DeleteClient(id);
        }

        public List<Trainer> GetAllTrainers()
        {
            return _db.GetAllTrainers();
        }

        public List<TrainingSession> GetAllTrainings()
        {
            return _db.GetAllTrainings();
        }

        public void AddTraining(TrainingSession session)
        {
            _db.AddTraining(session);
        }
        public void AddTrainer(Trainer trainer)
        {
            _db.AddTrainer(trainer);
        }
        public object GetFullSchedule()
        {
            var clients = GetAllClients();
            var trainers = GetAllTrainers();
            var trainings = GetAllTrainings();
            var fullSchedule = trainings.Select(t => new
            {
                Id = t.Id,
                ClientName = clients.FirstOrDefault(c => c.Id == t.ClientId)?.LastName ?? "Клієнт видалений",
                TrainerName = trainers.FirstOrDefault(tr => tr.Id == t.TrainerId)?.LastName ?? "Тренер видалений",
                Date = t.Date,
                Activity = t.Activity
            }).ToList();

            return fullSchedule;
        }
        public void DeleteTrainer(int id)
        {
            _db.DeleteTrainer(id);
        }
        public void DeleteTraining(int id)
        {
            _db.DeleteTraining(id);
        }
    }
}