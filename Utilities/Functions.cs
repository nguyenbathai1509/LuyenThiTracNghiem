using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LuyenThiTracNghiem.Utilities
{
    public class Functions
    {
        private const int Iterations = 100000;

        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                stringBuilder.Append(result[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        public static string MD5Password(string text)
        {
            string str = MD5Hash(text);
            for (int i = 0; i <= 5; i++)
            {
                str = MD5Hash(str + str);
            }
            return str;
        }

        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            var salt = RandomNumberGenerator.GetBytes(16);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);

            return $"PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string storedHash, string providedPassword, out bool needsUpgrade)
        {
            needsUpgrade = false;
            if (string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrEmpty(providedPassword))
            {
                return false;
            }

            if (storedHash.StartsWith("PBKDF2$", StringComparison.OrdinalIgnoreCase))
            {
                var parts = storedHash.Split('$');
                if (parts.Length != 4) return false;

                if (!int.TryParse(parts[1], out var iterations)) return false;
                var salt = Convert.FromBase64String(parts[2]);
                var storedBytes = Convert.FromBase64String(parts[3]);

                var computed = Rfc2898DeriveBytes.Pbkdf2(providedPassword, salt, iterations, HashAlgorithmName.SHA256, storedBytes.Length);
                var matched = CryptographicOperations.FixedTimeEquals(storedBytes, computed);
                needsUpgrade = matched && iterations < Iterations;
                return matched;
            }

            // Legacy MD5 check for existing users
            var legacyHash = MD5Password(providedPassword);
            var legacyMatch = string.Equals(storedHash, legacyHash, StringComparison.OrdinalIgnoreCase);
            needsUpgrade = legacyMatch;
            return legacyMatch;
        }

        public static string TitleSlugGeneration(string type, string? title, long id)
        {
            return type + "/" + SlugGenerator.SlugGenerator.GenerateSlug(title) + "-" + id.ToString() + ".html";
        }
    }
}
