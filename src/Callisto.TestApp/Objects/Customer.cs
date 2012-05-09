using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Callisto.Data.SQLite;

namespace Callisto.TestApp
{
    public class Customer : Entity
    {
        [AutoIncrement, PrimaryKey]
        public int CustomerId { get; set; }

        [MaxLength(32)]
        public string FirstName { get; set; }

        [MaxLength(32)]
        public string LastName { get; set; }

        [MaxLength(48)]
        public string Email { get; set; }

        public Customer()
        {
        }

        public Customer(string firstName, string lastName, string email)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
        }
    }
}
