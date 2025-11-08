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

        public static string TitleSlugGeneration(string? title, long id)
        {
            return SlugGenerator.SlugGenerator.GenerateSlug(title) + "-" + id.ToString() + ".html";
        }
    }
}