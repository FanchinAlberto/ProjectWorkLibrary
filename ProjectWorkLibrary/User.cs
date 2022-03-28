using System;
using System.Collections.Generic;

namespace ProjectWorkLibrary
{
    public partial class ManchesterLibrary
    {
        class User
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string City { get; set; }
            public string CF { get; set; }
            public string Password { get; set; }
            public string BirthDate { get; set; }

            public int ActiveLoans = 3;

            public List<Borrow> borrows { get; set; }

            public User()
            {
                borrows = new List<Borrow>();
            }
        }
    }
}
