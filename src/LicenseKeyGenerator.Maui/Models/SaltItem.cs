namespace LicenseKeyGenerator.Maui.Models;

public class SaltItem
{
    public string Salt { get; set; }
    public string Remark { get; set; }

    public SaltItem(string salt, string remark = "")
    {
        Salt = salt;
        Remark = remark;
    }
}
