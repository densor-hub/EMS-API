using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Services.PasswordService
{
    public interface IPasswordGenerator
    {
        string GeneratePassword(int lengthOfPassword, bool includeLowercase = true, bool includeUppercase = true,
        bool includeNumeric = true, bool includeSpecial = false, bool includeSpaces = false);


    }

    public class PasswordGenerator : IPasswordGenerator
    {
        public string GeneratePassword(
            int lengthOfPassword,
            bool includeLowercase = true,
            bool includeUppercase = true,
            bool includeNumeric = true,
            bool includeSpecial = false,
            bool includeSpaces = false)
        {
            const string LOWERCASE = "abcdefghijklmnopqrstuvwxyz";
            const string UPPERCASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NUMERIC = "0123456789";
            const string SPECIAL = @"!#$%&*@\";
            const string SPACE = " ";

            const int PASSWORD_LENGTH_MIN = 8;
            const int PASSWORD_LENGTH_MAX = 128;

            if (lengthOfPassword < PASSWORD_LENGTH_MIN || lengthOfPassword > PASSWORD_LENGTH_MAX)
                throw new ArgumentException("Password length must be between 8 and 128.");

            var random = new Random();

            var selectedSets = new List<string>();

            if (includeLowercase) selectedSets.Add(LOWERCASE);
            if (includeUppercase) selectedSets.Add(UPPERCASE);
            if (includeNumeric) selectedSets.Add(NUMERIC);
            if (includeSpecial) selectedSets.Add(SPECIAL);
            if (includeSpaces) selectedSets.Add(SPACE);

            // ❗ Ensure at least ONE option is selected
            if (!selectedSets.Any())
                throw new ArgumentException("At least one character set must be selected.");

            // ❗ Ensure password length can fit required characters
            if (lengthOfPassword < selectedSets.Count)
                throw new ArgumentException("Password length too short for selected constraints.");

            var passwordChars = new List<char>();

            // ✅ Step 1: Guarantee at least one from each selected set
            foreach (var set in selectedSets)
            {
                passwordChars.Add(set[random.Next(set.Length)]);
            }

            // ✅ Step 2: Fill the rest randomly
            var allChars = string.Concat(selectedSets);

            while (passwordChars.Count < lengthOfPassword)
            {
                passwordChars.Add(allChars[random.Next(allChars.Length)]);
            }

            // ✅ Step 3: Shuffle (so forced chars aren't predictable at start)
            passwordChars = passwordChars
                .OrderBy(_ => random.Next())
                .ToList();

            return new string(passwordChars.ToArray());
        }
    }
}