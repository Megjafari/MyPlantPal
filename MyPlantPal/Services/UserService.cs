using MyPlantPal.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyPlantPal.Services
{
    // Service for managing users
    public class UserService
    {
        private const string UserDataFileName = "users.json";

        private readonly DataStore _dataStore;
        private List<User> _users; // List of users loaded into memory

        public UserService(DataStore dataStore)
        {
            _dataStore = dataStore;
            _users = LoadUsers(); // Load data upon service initialization
        }

        // Internal methods for working with the store
        private List<User> LoadUsers()
        {
            return _dataStore.LoadFromFile<User>(UserDataFileName);
        }

        private void SaveUsers()
        {
            _dataStore.SaveToFile(UserDataFileName, _users);
        }

        // --- Business Logic ---

        public bool RegisterUser(string username, string password)
        {
            if (_users.Any(u => u.Username.ToLower() == username.ToLower()))
            {
                return false; // User already exists
            }

            var newUser = new User(username, password);
            _users.Add(newUser);

            SaveUsers(); // Save changes to JSON

            return true;
        }

        public User Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());

            if (user != null && user.Password == password)
            {
                return user;
            }

            return null;
        }
    }
}