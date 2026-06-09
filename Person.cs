using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core
{
    public abstract class Person : IEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public Person(int id, string fn, string ln, string ph)
        {
            Id = id; FirstName = fn; LastName = ln; Phone = ph;
        }
        public Person() { }

        public abstract string GetDetails();
    }
}
