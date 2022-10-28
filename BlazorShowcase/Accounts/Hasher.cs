using System.Security.Cryptography;
using System.Text;

namespace BlazorShowcase.Accounts;

public sealed class Hasher
{
    public static string Hash(string s)
    {
        s ??= string.Empty;

        using var cryptor = SHA512.Create();
        var builder = new StringBuilder();

        var cryptBytes = cryptor.ComputeHash(Encoding.UTF8.GetBytes(s));
        for (int i = 0; i < cryptBytes.Length; i++)
        {
            builder.Append(cryptBytes[i].ToString("x2"));
        }

        return builder.ToString().ToUpper();
    }
}
