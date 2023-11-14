using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Utils
{
    public class PasswordGenerator
    {
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

        public static string GenerateRandomPassword(int length)
        {
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string numberChars = "0123456789";
            const string specialChars = "!@#$%^&*";

            // Create a list to hold random characters
            List<char> randomChars = new List<char>();
            Random random = new Random();

            // Ensure at least one character from each category is included
            randomChars.Add(uppercaseChars[random.Next(uppercaseChars.Length)]);
            randomChars.Add(lowercaseChars[random.Next(lowercaseChars.Length)]);
            randomChars.Add(numberChars[random.Next(numberChars.Length)]);
            randomChars.Add(specialChars[random.Next(specialChars.Length)]);

            // Fill the rest of the password with random characters
            for (int i = 4; i < length; i++)
            {
                string pool = uppercaseChars + lowercaseChars + numberChars + specialChars;
                randomChars.Add(pool[random.Next(pool.Length)]);
            }

            // Shuffle the list of characters
            randomChars = randomChars.OrderBy(x => Guid.NewGuid()).ToList();

            // Convert the list of characters to a random password string
            string randomPassword = new string(randomChars.ToArray());

            return randomPassword;
        }
    }
}
