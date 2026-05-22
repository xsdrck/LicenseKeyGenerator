using System.Security.Cryptography;
using System.Text;

namespace LicenseKeyGenerator.Maui.Services;

public static class LicenseService
{
    public static (string key, string raw, string hash, int days) GenerateLicenseKeyDebug(string motherboardSerial, string salt, bool isPermanent, int? validDays)
    {
        if (string.IsNullOrWhiteSpace(motherboardSerial))
            throw new ArgumentException("Motherboard serial cannot be empty", nameof(motherboardSerial));
        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Salt cannot be empty", nameof(salt));

        bool hasValidDays = !isPermanent && validDays.HasValue && validDays.Value > 0;
        int daysValue = hasValidDays ? validDays!.Value : 0;
        var raw = $"{motherboardSerial.Trim()}|{salt.Trim().ToLowerInvariant()}|{(hasValidDays ? $"DAYS:{daysValue}" : "PERMANENT")}";

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw));
        var hashBase64 = Convert.ToBase64String(hashBytes)
            .Replace('+', '_')
            .Replace('/', '.')
            .TrimEnd('=');

        char[] chars = hashBase64.ToCharArray();
        string seg1 = new string(chars, 0, 8);
        string seg2 = new string(chars, 8, 8);
        string seg3 = new string(chars, 16, 8);
        string seg4 = new string(chars, 24, 8);

        var key = $"WM-{seg1.ToUpperInvariant()}-{seg2.ToUpperInvariant()}-{seg3.ToUpperInvariant()}-{seg4.ToUpperInvariant()}";

        if (hasValidDays)
        {
            key += $"-{daysValue:X4}";
        }

        return (key, raw, hashBase64, daysValue);
    }

    public static string GenerateLicenseKey(string motherboardSerial, string salt, bool isPermanent, int? validDays)
    {
        var (key, _, _, _) = GenerateLicenseKeyDebug(motherboardSerial, salt, isPermanent, validDays);
        return key;
    }

    public static string GenerateRandomSalt(int length = 12)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[16];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)[..12];
    }

    public static string GenerateSaltWithSuffix(string baseSalt, int suffixLength = 4)
    {
        using var rng = RandomNumberGenerator.Create();
        var suffixBytes = new byte[suffixLength];
        rng.GetBytes(suffixBytes);
        var suffix = Convert.ToBase64String(suffixBytes)[..suffixLength];
        return $"{baseSalt}_{suffix}";
    }
}
