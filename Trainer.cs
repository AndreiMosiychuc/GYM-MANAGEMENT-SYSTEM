using Gym.Core;
using System;

namespace Gym.Models
{
    public class Trainer : Person
    {
        public string Specialization { get; set; }
        public int Experience { get; set; }

        public Trainer(int id, string fn, string ln, string ph, string spec, int exp)
            : base(id, fn, ln, ph)
        {
            Specialization = spec;
            Experience = exp;
        }

        public override string GetDetails() => $"Тренер: {LastName}, Спец: {Specialization}";
    }
}
