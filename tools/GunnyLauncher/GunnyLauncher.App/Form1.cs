using System.Diagnostics;
using GunnyLauncher.Core;

namespace GunnyLauncher.App;

public partial class Form1 : Form
{
    private readonly TextBox _server = new() { Text = "http://103.9.156.182/Gunny/", Dock = DockStyle.Fill };
    private readonly TextBox _username = new() { Dock = DockStyle.Fill };
    private readonly TextBox _password = new() { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
    private readonly Button _play = new() { Text = "Chơi ngay", Dock = DockStyle.Fill, Height = 38 };
    private readonly Label _status = new() { Text = "Sẵn sàng.", AutoSize = true, Dock = DockStyle.Fill };

    public Form1()
    {
        Text = "Gunny Desktop Launcher";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(520, 245);
        MinimumSize = new Size(520, 245);
        BuildLayout();
        _play.Click += PlayAsync;
        AcceptButton = _play;
    }

    private void BuildLayout()
    {
        var table = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), ColumnCount = 2, RowCount = 5 };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        table.Controls.Add(new Label { Text = "Server", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        table.Controls.Add(_server, 1, 0);
        table.Controls.Add(new Label { Text = "Tài khoản", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        table.Controls.Add(_username, 1, 1);
        table.Controls.Add(new Label { Text = "Mật khẩu", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
        table.Controls.Add(_password, 1, 2);
        table.Controls.Add(_play, 1, 3);
        table.Controls.Add(_status, 0, 4);
        table.SetColumnSpan(_status, 2);
        Controls.Add(table);
    }

    private async void PlayAsync(object? sender, EventArgs e)
    {
        _play.Enabled = false;
        _status.Text = "Đang đăng nhập...";
        try
        {
            if (!Uri.TryCreate(_server.Text.Trim(), UriKind.Absolute, out var gameBase))
                throw new InvalidOperationException("Địa chỉ server không hợp lệ.");

            var runtimeRoot = AppContext.BaseDirectory;
            var ruffleExe = Path.Combine(runtimeRoot, "runtime", "ruffle.exe");
            if (!File.Exists(ruffleExe))
                throw new FileNotFoundException("Không tìm thấy runtime\\ruffle.exe.", ruffleExe);
            var service = new GunnyLauncherService(gameBase, runtimeRoot);
            var startInfo = await service.BuildStartInfoAsync(
                _username.Text.Trim(),
                _password.Text,
                CancellationToken.None);

            var process = Process.Start(startInfo);
            if (process is null)
                throw new InvalidOperationException("Không thể khởi chạy Ruffle.");

            _password.Clear();
            _status.Text = "Đã mở game. Có thể giữ launcher này để đăng nhập lại.";
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
}
