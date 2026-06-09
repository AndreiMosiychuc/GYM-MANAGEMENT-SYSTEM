using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym.Core;

namespace Gym.Models
{
    public class Client : Person
    {
        private int _age;
        public int Age
        {
            get => _age;
            set
            {
                if (value < 14 || value > 100)
                    throw new ArgumentException("Вік має бути від 14 до 100 років!");
                _age = value;
            }
        }

        public DateTime SubscriptionEndDate { get; set; }

        public Client(int id, string fn, string ln, string ph, int age, DateTime subEnd)
            : base(id, fn, ln, ph)
        {
            Age = age;
            SubscriptionEndDate = subEnd;
        }

        public override string GetDetails()
        {
            return $"Клієнт: {LastName} {FirstName}, Вік: {Age}, До: {SubscriptionEndDate.ToShortDateString()}";
        }
    }
}
