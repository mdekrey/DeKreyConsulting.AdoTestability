using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeKreyConsulting.AdoTestability.Postgres.Example
{
    public class Person
    {
        public Person(int id, string fullName, string email, bool optOut)
        {
            Id = id;
            FullName = fullName;
            Email = email;
            OptOut = optOut;
        }

        public string Email { get; }
        public string FullName { get; }
        public int Id { get; }
        public bool OptOut { get; }
    }
}
