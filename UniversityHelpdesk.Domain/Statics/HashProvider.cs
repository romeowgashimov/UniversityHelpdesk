using System.Security.Cryptography;
using System.Text;

namespace Universityhelpdesk.Domain.Statics;

public static class HashProvider
{
    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    public static bool Equals(string passwordHash, string passwordDto)
    {
        return passwordHash == HashPassword(passwordDto);
    }
}