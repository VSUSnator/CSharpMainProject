using System;
using XYZ;

namespace MyApp
{
    internal class Programs
    {
        static void Main(string[] args)
        {
            string userName = ReadLineWithMessage("Введите ваше имя: ")
            string login = ReadLineWithMessage("Введите ваш никнейм: ")
            string password = ReadLineWithMessage("Придумайте пароль: ")

            User newUser = new User(userName, login, password);
          //  Database database1 = new Database();
           // database.Users.Add(newUser);
           // database.ShowAll();

            List<User> users = new Database();
            users.Add(newUser);

            Database database = new Database(users);
            database.ShowAll();
        }

        static string ReadLineWithMessage(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }

    }

    public class User
    {
        public string Name;
        public string Login;
        public string Password;
        public string Gold;

        public User(string name, string login, string password)
        {
            Name = name;
            Login = login;
            Password = password;
        }
        public void ShowInfo()
        {
            Console.WriteLine($"Login: {Login}, Name: {name}");
        }
    }

    public class Database
    {
        public List<User> Users;

        public Database() 
        {
            Users = new List<User>();
        }
    }
    public Datavase(List<User> users)
    {
        Users = users;
    }

    public void ShowAll()
    {
        foreach (User user in Users)
        {
            user.ShowInfo();
        }
    }
}
