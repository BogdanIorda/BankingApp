using Newtonsoft.Json; // The tool we just installed
using System.Collections.Generic;
using System.IO; // Needed to talk to the Hard Drive

namespace MyFirstProject
{
    // We make this 'static' because it's just a utility tool helper.
    // We don't need to create 'new DataService()', we just use its tools.
    public static class DataService
    {
        // The name of the file where we will save data
        private static string filePath = "bank_data_v1.json";

        // METHOD: SAVE (The "Freeze" Ray)
        public static void SaveAccounts(List<BankAccount> accounts)
        {
            // Because 'BankAccount' is Abstract, we must tell JSON to remember
            // exactly which Child Class (GiftCard, Credit, etc.) each object is.
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented // Makes the file readable for humans
            };

            // 1. Serialize: Turn the C# Objects into a Text String
            string json = JsonConvert.SerializeObject(accounts, settings);

            // 2. Write: Save that text to the hard drive
            File.WriteAllText(filePath, json);
        }

        // METHOD: LOAD (The "Thaw" Ray)
        public static List<BankAccount> LoadAccounts()
        {
            // 1. Check if the file exists (Can't read what isn't there!)
            if (!File.Exists(filePath))
            {
                return new List<BankAccount>();
            }

            // 2. Read the text from the file
            string json = File.ReadAllText(filePath);

            // 3. The Settings (Must match the Save settings!)
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            // 4. Deserialize: Turn Text back into C# Objects
            // It recreates the specific Child classes.
            var accounts = JsonConvert.DeserializeObject<List<BankAccount>>(json, settings);

            return accounts;
        }
    }
}