using System;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Essentials;

namespace upendo.Helpers
{
    public static class PasswordForLocalDBFactory
    {
        public static string GetLocalDBPassword()
        {
            string pragmaKey = Preferences.Get(Constans.LOCAL_DB_PASSWORD, null);

            if (!string.IsNullOrEmpty(pragmaKey))
                return pragmaKey;

            return CreateLocalDBPassword();
        }

        private static string CreateLocalDBPassword()
        {
            Random random = new(Guid.NewGuid().GetHashCode());

            string pragmaKey = CreateMD5(
                string.Concat(
                    DateTime.UtcNow.Ticks.ToString(),
                    random.Next(0, int.MaxValue).ToString()
                )
            );

            Preferences.Set(Constans.LOCAL_DB_PASSWORD, pragmaKey);

            return pragmaKey;
        }

        private static string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    stringBuilder.Append(hashBytes[i].ToString("X2"));
                }

                return stringBuilder.ToString();
            }
        }
    }
}
