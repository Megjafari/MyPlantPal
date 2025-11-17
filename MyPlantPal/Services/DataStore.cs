using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MyPlantPal.Services
{
    // Generic service for saving and loading data to JSON files
    public class DataStore
    {
        private readonly string _baseDataPath;

        public DataStore()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(exePath, @"..\..\..\")); //that put json info to user.json
            _baseDataPath = Path.Combine(projectRoot, "Data");


            // Create the 'Data/' folder if it doesn't exist
            if (!Directory.Exists(_baseDataPath))
            {
                Directory.CreateDirectory(_baseDataPath);
            }
        }

        private string GetFilePath(string fileName)
        {
            return Path.Combine(_baseDataPath, fileName);
        }

        // Load a list of objects of type T from a JSON file
        public List<T> LoadFromFile<T>(string fileName) where T : new()
        {
            string filePath = GetFilePath(fileName);
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var items = JsonSerializer.Deserialize<List<T>>(jsonString, options);

                return items ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data from {fileName}: {ex.Message}");
                return new List<T>();
            }
        }

        // Save a list of objects of type T to a JSON file
        public void SaveToFile<T>(string fileName, List<T> data)
        {
            string filePath = GetFilePath(fileName);

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(data, options);

                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data to {fileName}: {ex.Message}");
            }
        }
    }
}