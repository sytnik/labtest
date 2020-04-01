using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LabTest.Models
{
    public static class UserMock
    {
        public static List<User> Users { get; set; }

        static UserMock() => Users = new List<User> {new User(1, "root", "admin", "Admin", "Administrator")};
    }

    public class User
    {
        [Key] public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }

        public User(int id, string login, string password, string role, string name)
        {
            Id = id;
            Login = login;
            Password = password;
            Role = role;
            Name = name;
        }
    }
}