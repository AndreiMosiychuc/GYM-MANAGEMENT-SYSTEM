using System;

namespace Gym.Models
{
    public class TrainingSession
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int TrainerId { get; set; }
        public DateTime Date { get; set; }
        public string Activity { get; set; }
        public class TrainingView
        {
            public int Id { get; set; }
            public string ClientName { get; set; }
            public string TrainerName { get; set; }
            public string Date { get; set; }
            public string Activity { get; set; }
        }

        public TrainingSession(int id, int clientId, int trainerId, DateTime date, string activity)
        {
            Id = id;
            ClientId = clientId;
            TrainerId = trainerId;
            Date = date;
            Activity = activity;
        }
    }
}
