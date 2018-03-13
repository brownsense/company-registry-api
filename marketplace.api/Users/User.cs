using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marketplace.api.Users
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserState State { get; set; }
    }

    public enum UserState
    {
        None = 0,
        Active,
        Deleted
    }
}
