using LicenseKeyGenerator.Maui.Models;
using LicenseKeyGenerator.Maui.Services;

namespace LicenseKeyGenerator.Maui;

public partial class MainPage : ContentPage
{
    private readonly List<SaltItem> _saltList;

    public MainPage()
    {
        InitializeComponent();
        _saltList = ConfigurationService.LoadSaltList();
        SaltCollectionView.ItemsSource = _saltList;
        
        if (_saltList.Count > 0)
        {
            SaltCollectionView.SelectedItem = _saltList[0];
        }
    }

    private void SaveSaltList()
    {
        ConfigurationService.SaveSaltList(_saltList);
        SaltCollectionView.ItemsSource = null;
        SaltCollectionView.ItemsSource = _saltList;
    }

    private async void AddSalt_Click(object sender, EventArgs e)
    {
        string salt = await DisplayPromptAsync("添加盐值", "请输入盐值：", "确定", "取消", "盐值");
        if (string.IsNullOrWhiteSpace(salt))
            return;

        string remark = await DisplayPromptAsync("添加备注", "请输入备注（可选）：", "确定", "跳过", "备注");

        var existingSalt = _saltList.Find(s => s.Salt == salt.Trim());
        if (existingSalt != null)
        {
            await DisplayAlert("提示", "该盐值已存在。", "确定");
            return;
        }

        _saltList.Add(new SaltItem(salt.Trim(), remark?.Trim() ?? ""));
        SaveSaltList();
    }

    private async void EditSalt_Click(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is SaltItem item)
        {
            string newSalt = await DisplayPromptAsync("编辑盐值", "请输入盐值：", "确定", "取消", "盐值", initialValue: item.Salt);
            if (string.IsNullOrWhiteSpace(newSalt))
                return;

            string newRemark = await DisplayPromptAsync("编辑备注", "请输入备注（可选）：", "确定", "跳过", "备注", initialValue: item.Remark);

            var existingSalt = _saltList.Find(s => s.Salt == newSalt.Trim() && s != item);
            if (existingSalt != null)
            {
                await DisplayAlert("提示", "该盐值已存在。", "确定");
                return;
            }

            item.Salt = newSalt.Trim();
            item.Remark = newRemark?.Trim() ?? "";
            SaveSaltList();
        }
    }

    private async void DeleteSalt_Click(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is SaltItem item)
        {
            bool confirmed = await DisplayAlert("确认", $"确定要删除盐值 '{item.Salt}' 吗？", "确定", "取消");
            if (confirmed)
            {
                var index = _saltList.IndexOf(item);
                _saltList.Remove(item);
                
                if (_saltList.Count > 0 && index >= 0)
                {
                    SaltCollectionView.SelectedItem = _saltList[Math.Min(index, _saltList.Count - 1)];
                }
                
                SaveSaltList();
            }
        }
    }

    private void PermanentRadio_Checked(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            DaysEntry.IsEnabled = false;
            DaysRadio.IsChecked = false;
        }
    }

    private void DaysRadio_Checked(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            DaysEntry.IsEnabled = true;
            PermanentRadio.IsChecked = false;
        }
    }

    private async void GenerateKey_Click(object sender, EventArgs e)
    {
        var serial = SerialEntry.Text?.Trim();
        if (string.IsNullOrEmpty(serial))
        {
            await DisplayAlert("提示", "请输入序列号。", "确定");
            return;
        }

        var selectedSaltItem = SaltCollectionView.SelectedItem as SaltItem;
        if (selectedSaltItem == null)
        {
            await DisplayAlert("提示", "请选择一个盐值。", "确定");
            return;
        }
        var salt = selectedSaltItem.Salt;

        bool isPermanent = PermanentRadio.IsChecked == true;
        int? validDays = null;

        if (!isPermanent)
        {
            if (!int.TryParse(DaysEntry.Text, out var days) || days < 1)
            {
                await DisplayAlert("提示", "请输入有效的天数（最少1天）。", "确定");
                return;
            }
            validDays = days;
        }

        try
        {
            var (key, raw, hash, daysVal) = LicenseService.GenerateLicenseKeyDebug(serial, salt, isPermanent, validDays);
            KeyEntry.Text = key;

            string debugInfo = $"调试信息：\n\n原始字符串：{raw}\n\n哈希结果：{hash}\n\n天数：{daysVal}\n\n密钥：{key}";
            await DisplayAlert("密钥生成调试", debugInfo, "确定");
        }
        catch (Exception ex)
        {
            await DisplayAlert("错误", $"生成密钥失败：{ex.Message}", "确定");
        }
    }

    private async void CopyKey_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(KeyEntry.Text))
        {
            await Clipboard.SetTextAsync(KeyEntry.Text);
            await DisplayAlert("提示", "密钥已复制到剪贴板。", "确定");
        }
    }
}
