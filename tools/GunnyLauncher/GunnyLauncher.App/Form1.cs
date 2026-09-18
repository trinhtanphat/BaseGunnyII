using System.Diagnostics;
using GunnyLauncher.Core;

namespace GunnyLauncher.App;

public partial class Form1 : Form
{
    private readonly LauncherSettingsStore _settingsStore = new();
    private readonly ComboBox _server = new() { Name = "serverSelector", Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDown };
    private readonly TextBox _username = new() { Name = "loginUsername", Dock = DockStyle.Fill };
    private readonly TextBox _password = new() { Name = "loginPassword", Dock = DockStyle.Fill, UseSystemPasswordChar = true };
    private readonly CheckBox _showPassword = new() { Text = "Hiện mật khẩu", AutoSize = true };
    private readonly CheckBox _rememberUsername = new() { Text = "Nhớ tài khoản", AutoSize = true };
    private readonly Button _play = new() { Name = "playButton", Text = "CHƠI NGAY", Dock = DockStyle.Fill, Height = 42 };
    private readonly Label _status = new() { Name = "statusLabel", Text = "Sẵn sàng.", AutoSize = true, Dock = DockStyle.Fill };
    private readonly TabControl _authTabs = new() { Name = "authTabs", Dock = DockStyle.Fill };
    private readonly TabPage _loginTab = new("Đăng nhập") { Name = "loginTab" };
    private readonly TabPage _registerTab = new("Đăng ký") { Name = "registerTab" };

    private readonly TextBox _registerUsername = new() { Name = "registerUsername", Dock = DockStyle.Fill };
    private readonly TextBox _registerPassword = new() { Name = "registerPassword", Dock = DockStyle.Fill, UseSystemPasswordChar = true };
    private readonly TextBox _registerConfirmation = new() { Name = "registerConfirmation", Dock = DockStyle.Fill, UseSystemPasswordChar = true };
    private readonly TextBox _registerEmail = new() { Name = "registerEmail", Dock = DockStyle.Fill };
    private readonly ComboBox _registerSex = new() { Name = "registerSex", Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _captchaCode = new() { Name = "captchaCode", Dock = DockStyle.Fill };
    private readonly PictureBox _captchaImage = new() { Name = "captchaImage", Dock = DockStyle.Fill, Height = 48, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
    private readonly Button _refreshCaptcha = new() { Name = "refreshCaptcha", Text = "Làm mới CAPTCHA", Dock = DockStyle.Fill };
    private readonly Button _registerButton = new() { Name = "registerButton", Text = "TẠO TÀI KHOẢN", Dock = DockStyle.Fill, Height = 42 };
    private readonly CheckBox _showRegisterPassword = new() { Text = "Hiện mật khẩu", AutoSize = true };

    private GunnyRegistrationClient? _registrationClient;
    private Uri? _registrationBase;

    public Form1()
    {
        Text = "Gunny Launcher";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(760, 620);
        MinimumSize = new Size(680, 560);
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(245, 247, 251);
        BuildLayout();
        LoadSettings();
        WireEvents();
        AcceptButton = _play;
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), RowCount = 4, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

        var header = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(38, 74, 140) };
        header.Controls.Add(new Label
        {
            Text = "GUNNY",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 24F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(18, 10)
        });
        header.Controls.Add(new Label
        {
            Text = "Desktop & Mobile Account Launcher",
            ForeColor = Color.WhiteSmoke,
            AutoSize = true,
            Location = new Point(21, 53)
        });
        root.Controls.Add(header, 0, 0);

        var serverRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(0, 10, 0, 4) };
        serverRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        serverRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        serverRow.Controls.Add(new Label { Text = "Máy chủ", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        serverRow.Controls.Add(_server, 1, 0);
        root.Controls.Add(serverRow, 0, 1);

        _loginTab.Padding = new Padding(14);
        _registerTab.Padding = new Padding(14);
        _loginTab.Controls.Add(BuildLoginLayout());
        _registerTab.Controls.Add(BuildRegisterLayout());
        _authTabs.TabPages.Add(_loginTab);
        _authTabs.TabPages.Add(_registerTab);
        root.Controls.Add(_authTabs, 0, 2);

        var statusPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4, 12, 4, 0) };
        statusPanel.Controls.Add(_status);
        root.Controls.Add(statusPanel, 0, 3);
        Controls.Add(root);
    }

    private Control BuildLoginLayout()
    {
        var table = CreateFormTable(5);
        AddRow(table, 0, "Tài khoản", _username);
        AddRow(table, 1, "Mật khẩu", _password);
        var options = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        options.Controls.Add(_rememberUsername);
        options.Controls.Add(_showPassword);
        table.Controls.Add(options, 1, 2);
        table.Controls.Add(_play, 1, 3);
        return table;
    }

    private Control BuildRegisterLayout()
    {
        _registerSex.Items.AddRange(new object[] { "Nam", "Nữ" });
        _registerSex.SelectedIndex = 0;
        var table = CreateFormTable(9);
        AddRow(table, 0, "Tài khoản", _registerUsername);
        AddRow(table, 1, "Mật khẩu", _registerPassword);
        AddRow(table, 2, "Nhập lại", _registerConfirmation);
        AddRow(table, 3, "Email", _registerEmail);
        AddRow(table, 4, "Giới tính", _registerSex);

        var passwordOptions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        passwordOptions.Controls.Add(_showRegisterPassword);
        table.Controls.Add(passwordOptions, 1, 5);

        var captchaPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Height = 86 };
        captchaPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        captchaPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        captchaPanel.Controls.Add(_captchaImage, 0, 0);
        captchaPanel.Controls.Add(_refreshCaptcha, 1, 0);
        captchaPanel.Controls.Add(_captchaCode, 0, 1);
        captchaPanel.SetColumnSpan(_captchaCode, 2);
        table.Controls.Add(new Label { Text = "CAPTCHA", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 6);
        table.Controls.Add(captchaPanel, 1, 6);
        table.Controls.Add(_registerButton, 1, 7);
        return table;
    }

    private static TableLayoutPanel CreateFormTable(int rows)
    {
        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = rows, AutoScroll = true };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (var i = 0; i < rows; i++) table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        return table;
    }

    private static void AddRow(TableLayoutPanel table, int row, string label, Control control)
    {
        table.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
        table.Controls.Add(control, 1, row);
    }

    private void LoadSettings()
    {
        var settings = _settingsStore.Load();
        _server.Text = settings.ServerUrl;
        _username.Text = settings.Username;
        _rememberUsername.Checked = !string.IsNullOrWhiteSpace(settings.Username);
    }

    private void WireEvents()
    {
        _play.Click += PlayAsync;
        _registerButton.Click += RegisterAsync;
        _refreshCaptcha.Click += RefreshCaptchaAsync;
        _showPassword.CheckedChanged += (_, _) => _password.UseSystemPasswordChar = !_showPassword.Checked;
        _showRegisterPassword.CheckedChanged += (_, _) =>
        {
            var hidden = !_showRegisterPassword.Checked;
            _registerPassword.UseSystemPasswordChar = hidden;
            _registerConfirmation.UseSystemPasswordChar = hidden;
        };
        _authTabs.SelectedIndexChanged += async (_, _) =>
        {
            if (_authTabs.SelectedTab == _registerTab && _captchaImage.Image is null)
                await RefreshCaptchaCoreAsync();
        };
    }

    private bool TryGetServer(out Uri gameBase)
    {
        if (!Uri.TryCreate(_server.Text.Trim(), UriKind.Absolute, out gameBase!))
        {
            _status.Text = "Địa chỉ máy chủ không hợp lệ.";
            return false;
        }

        if (!_settingsStore.IsServerCompatible(gameBase.AbsoluteUri))
        {
            _status.Text = "Máy chủ không đúng profile launcher hiện tại.";
            return false;
        }

        return true;
    }

    private async void PlayAsync(object? sender, EventArgs e)
    {
        if (!TryGetServer(out var gameBase)) return;
        _play.Enabled = false;
        _status.Text = "Đang đăng nhập...";
        try
        {
            var runtimeRoot = AppContext.BaseDirectory;
            var ruffleExe = Path.Combine(runtimeRoot, "runtime", "ruffle.exe");
            if (!File.Exists(ruffleExe))
                throw new FileNotFoundException("Không tìm thấy runtime\\ruffle.exe.", ruffleExe);

            var service = new GunnyLauncherService(gameBase, runtimeRoot);
            var startInfo = await service.BuildStartInfoAsync(
                _username.Text.Trim(), _password.Text, CancellationToken.None);
            var process = Process.Start(startInfo);
            if (process is null) throw new InvalidOperationException("Không thể khởi chạy Ruffle.");

            var savedUsername = _rememberUsername.Checked ? _username.Text.Trim() : string.Empty;
            _settingsStore.Save(new LauncherSettings(gameBase.ToString(), savedUsername));
            _password.Clear();
            _status.Text = "Đã mở game. Có thể giữ launcher để đăng nhập lại.";
        }
        catch (Exception ex)
        {
            _status.Text = ex.Message;
        }
        finally
        {
            _play.Enabled = true;
        }
    }

    private async void RegisterAsync(object? sender, EventArgs e)
    {
        if (!TryGetServer(out var gameBase)) return;
        _registerButton.Enabled = false;
        _status.Text = "Đang tạo tài khoản...";
        try
        {
            EnsureRegistrationClient(gameBase);
            var sex = _registerSex.SelectedIndex == 0 ? "1" : "0";
            var result = await _registrationClient!.RegisterAsync(
                _registerUsername.Text.Trim(),
                _registerPassword.Text,
                _registerConfirmation.Text,
                _registerEmail.Text.Trim(),
                sex,
                _captchaCode.Text.Trim(),
                CancellationToken.None);            if (!result.Success)
            {
                _status.Text = string.IsNullOrWhiteSpace(result.Message)
                    ? "Máy chủ từ chối đăng ký."
                    : result.Message;
                return;
            }

            _registerPassword.Clear();
            _registerConfirmation.Clear();
            _captchaCode.Clear();
            _status.Text = "Tạo tài khoản thành công. Bạn có thể đăng nhập ngay.";
            _username.Text = _registerUsername.Text.Trim();
            _authTabs.SelectedTab = _loginTab;
        }
        catch (Exception ex)
        {
            _status.Text = ex.Message;
        }
        finally
        {
            _registerButton.Enabled = true;
            try
            {
                await RefreshCaptchaCoreAsync();
            }
            catch
            {
                // Preserve the registration result message when CAPTCHA refresh fails.
            }
        }
    }
    private async void RefreshCaptchaAsync(object? sender, EventArgs e)
    {
        _refreshCaptcha.Enabled = false;
        try
        {
            await RefreshCaptchaCoreAsync();
        }
        catch (Exception ex)
        {
            _status.Text = ex.Message;
        }
        finally
        {
            _refreshCaptcha.Enabled = true;
        }
    }

    private async Task RefreshCaptchaCoreAsync()
    {
        if (!TryGetServer(out var gameBase)) return;
        EnsureRegistrationClient(gameBase);
        _status.Text = "Đang tải CAPTCHA...";
        var captcha = await _registrationClient!.GetCaptchaAsync(CancellationToken.None);
        using var stream = new MemoryStream(captcha.ImageBytes, writable: false);
        using var image = Image.FromStream(stream);
        var replacement = new Bitmap(image);
        var old = _captchaImage.Image;
        _captchaImage.Image = replacement;
        old?.Dispose();
        _status.Text = "CAPTCHA đã sẵn sàng.";
    }
    private void EnsureRegistrationClient(Uri gameBase)
    {
        if (_registrationClient is not null && _registrationBase == gameBase) return;
        _registrationClient = new GunnyRegistrationClient(gameBase);
        _registrationBase = gameBase;
    }
}