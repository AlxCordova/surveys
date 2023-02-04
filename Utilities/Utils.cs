using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Survey.API.Utilities;

public static class Utils
{
    internal static readonly char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    public static byte[] GenerateSalt()
    {
        var bytes = new byte[128 / 8];
        var rng = new RNGCryptoServiceProvider();
        rng.GetBytes(bytes);
        return bytes;
    }

    public static string CreateRandomSurveyUrl(int linkLength = 11)
    {
        string url = "";
        for (int i = 0; i < linkLength; i++)
        {
            Random rand = new Random();
            int random = rand.Next(0, 3);
            if (random == 1)
            {
                random = rand.Next(0, chars.Length);
                url += chars[random].ToString();
            }
            else
            {
                random = rand.Next(0, chars.Length);
                url += chars[random].ToString();
            }
        }
        return url;
    }

    public static string ComputeHash(string str, byte[] salt, int iterations = 10000)
    {
        var byteResult = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(str), salt, iterations);
        var itstr = Convert.ToBase64String(Encoding.UTF8.GetBytes(iterations + ""));
        var saltstr = Convert.ToBase64String(salt);
        var keystr = Convert.ToBase64String(byteResult.GetBytes(50));

        return $"{itstr}.{saltstr}.{keystr}";
    }

    public static bool CompareHash(string hashed, string plaintext)
    {
        try
        {
            string[] parts = hashed.Split('.');
            if (parts.Length != 3) return false;

            int it = Convert.ToInt32(Encoding.UTF8.GetString(Convert.FromBase64String(parts[0])));
            byte[] salt = Convert.FromBase64String(parts[1]);

            string hashedText = ComputeHash(plaintext, salt, it);

            return hashed.Equals(hashedText);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        return false;
    }
}
