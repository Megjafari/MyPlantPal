using System;
using System.Text.Json.Serialization;

namespace MyPlantPal.Models
{
    // Data model for a user
    public class User
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("date_time_created")]
        public DateTime DateTimeCreated { get; set; }

        // Default constructor (required for JSON deserialization)
        public User() { }

        // Constructor for creating a new user
        public User(string username, string password)
        {
            Username = username;
            Password = password;
            DateTimeCreated = DateTime.UtcNow;
        }
    }
}