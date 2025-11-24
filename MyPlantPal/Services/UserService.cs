using MyPlantPal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography; // Required for hashing
using System.Text;

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
        // - Hashing Helper Method -
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }




        // --- Business Logic ---

        public bool Register(string username, string password)
        {
            if (_users.Any(u => u.Username.ToLower() == username.ToLower()))
            {
                return false; // User already exists
            }
            if (!IsPasswordValid(password))
            {
                return false; // Password validation failed
            }
            //  Hash the password before creating the user
            var passwordHash = HashPassword(password);

            var newUser = new User(username, passwordHash);
            _users.Add(newUser);

            SaveUsers(); // Save changes to JSON

            return true;
        }
        private bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 6)    // Minimum length: 6 characters
                return false;

            if (!password.Any(char.IsDigit))    // Must contain at least ONE digit
                return false;

            if (!password.Any(char.IsUpper))    // Must contain at least ONE uppercase letter
                return false;

            return true;
        }

        public User Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());

            if (user != null)
            {
                //  Hash the input password to compare
                var inputHash = HashPassword(password);

                if (user.Password == inputHash)
                {
                    return user;
                }

            }

            return null;
        }
    }
}