using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using GunnyLauncher.Core;

namespace GunnyMobile.Android;

[Activity(Label = "Gunny Mobile", MainLauncher = true, Exported = true)]
public sealed class MainActivity : Activity
{
    private EditText _server = null!;
    private TextView _status = null!;
    private EditText _loginUsername = null!;
    private EditText _loginPassword = null!;
    private LinearLayout _loginPanel = null!;
    private LinearLayout _registerPanel = null!;
    private EditText _registerUsername = null!;
    private EditText _registerPassword = null!;
    private EditText _registerConfirmation = null!;
    private EditText _registerEmail = null!;
    private EditText _captchaCode = null!;
    private ImageView _captchaImage = null!;
    private Bitmap? _captchaBitmap;
    private GunnyRegistrationClient? _registrationClient;
    private System.Uri? _registrationBase;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Title = "Gunny Mobile";
        _server = new EditText(this) { Text = "http://103.9.156.181/Gunny/", Hint = "Máy chủ" };
        _status = new TextView(this) { Text = "Sẵn sàng." };
        _loginUsername = new EditText(this) { Hint = "Tài khoản" };
        _loginPassword = PasswordBox("Mật khẩu");
        _loginPanel = Vertical();
        _registerPanel = Vertical();
        _registerUsername = new EditText(this) { Hint = "Tài khoản" };
        _registerPassword = PasswordBox("Mật khẩu");
        _registerConfirmation = PasswordBox("Nhập lại mật khẩu");
        _registerEmail = new EditText(this) { Hint = "Email" };
        _captchaCode = new EditText(this) { Hint = "Mã CAPTCHA" };
        _captchaImage = new ImageView(this);

        var root = Vertical();
        root.SetPadding(28, 24, 28, 24);
        root.AddView(new TextView(this)
        {
            Text = "GUNNY",
            TextSize = 28,
            Gravity = GravityFlags.CenterHorizontal
        });
        root.AddView(new TextView(this) { Text = "Máy chủ" });
        root.AddView(_server, FullWidth());

        var tabs = new LinearLayout(this) { Orientation = Orientation.Horizontal };
        var loginTab = new Button(this) { Text = "Đăng nhập" };
        var registerTab = new Button(this) { Text = "Đăng ký" };
        tabs.AddView(loginTab, Weighted());
        tabs.AddView(registerTab, Weighted());
        root.AddView(tabs, FullWidth());

        BuildLoginPanel();
        BuildRegisterPanel();
        root.AddView(_loginPanel, FullWidth());
        root.AddView(_registerPanel, FullWidth());
        root.AddView(_status, FullWidth());

        _registerPanel.Visibility = ViewStates.Gone;
        loginTab.Click += (_, _) => ShowLogin();
        registerTab.Click += async (_, _) =>
        {
            ShowRegister();
            if (_captchaImage.Drawable is null)
                await RefreshCaptchaAsync();
        };

        var scroll = new ScrollView(this);
        scroll.AddView(root);
        SetContentView(scroll);
    }

    private void BuildLoginPanel()
    {
        _loginPanel.AddView(_loginUsername, FullWidth());
        _loginPanel.AddView(_loginPassword, FullWidth());
        var play = new Button(this) { Text = "CHƠI NGAY" };
        play.Click += async (_, _) => await LoginAndLaunchAsync(play);
        _loginPanel.AddView(play, FullWidth());
    }

    private void BuildRegisterPanel()
    {
        _registerPanel.AddView(_registerUsername, FullWidth());
        _registerPanel.AddView(_registerPassword, FullWidth());
        _registerPanel.AddView(_registerConfirmation, FullWidth());
        _registerPanel.AddView(_registerEmail, FullWidth());
        _captchaImage.SetMinimumHeight(120);
        _captchaImage.SetAdjustViewBounds(true);
        _registerPanel.AddView(_captchaImage, FullWidth());
        var refresh = new Button(this) { Text = "Làm mới CAPTCHA" };
        refresh.Click += async (_, _) => await RefreshCaptchaAsync();
        _registerPanel.AddView(refresh, FullWidth());
        _registerPanel.AddView(_captchaCode, FullWidth());
        var submit = new Button(this) { Text = "TẠO TÀI KHOẢN" };
        submit.Click += async (_, _) => await RegisterAsync(submit);
        _registerPanel.AddView(submit, FullWidth());
    }

    private async Task LoginAndLaunchAsync(Button play)
    {
        if (!TryGetServer(out var gameBase)) return;
        if (!RuffleIntentLauncher.IsRuntimeInstalled(this))
        {
            _status.Text = "Chưa cài Ruffle Android runtime đã được pin.";
            return;
        }

        play.Enabled = false;
        _status.Text = "Đang đăng nhập...";
        try
        {
            var client = new GunnyLoginClient(gameBase);
            var launch = await client.AuthenticateAsync(
                _loginUsername.Text?.Trim() ?? string.Empty,
                _loginPassword.Text ?? string.Empty,
                CancellationToken.None);
            _loginPassword.Text = string.Empty;
            RuffleIntentLauncher.Launch(this, launch, gameBase);
            _status.Text = "Đã chuyển sang Ruffle Android.";
        }
        catch (Exception ex)
        {
            _status.Text = Sanitize(ex.Message);
        }
        finally
        {
            play.Enabled = true;
        }
    }

    private async Task RegisterAsync(Button submit)
    {
        if (!TryGetServer(out var gameBase)) return;
        submit.Enabled = false;
        _status.Text = "Đang tạo tài khoản...";
        try
        {
            var client = GetRegistrationClient(gameBase);
            var result = await client.RegisterAsync(
                _registerUsername.Text?.Trim() ?? string.Empty,
                _registerPassword.Text ?? string.Empty,
                _registerConfirmation.Text ?? string.Empty,
                _registerEmail.Text?.Trim() ?? string.Empty,
                "1",
                _captchaCode.Text?.Trim() ?? string.Empty,
                CancellationToken.None);

            _registerPassword.Text = string.Empty;
            _registerConfirmation.Text = string.Empty;
            _captchaCode.Text = string.Empty;
            _status.Text = result.Success ? "Tạo tài khoản thành công." : Sanitize(result.Message);
            await RefreshCaptchaAsync();
        }
        catch (Exception ex)
        {
            _status.Text = Sanitize(ex.Message);
            await TryRefreshCaptchaAsync();
        }
        finally
        {
            submit.Enabled = true;
        }
    }

    private async Task RefreshCaptchaAsync()
    {
        if (!TryGetServer(out var gameBase)) return;
        var captcha = await GetRegistrationClient(gameBase).GetCaptchaAsync(CancellationToken.None);
        var next = BitmapFactory.DecodeByteArray(captcha.ImageBytes, 0, captcha.ImageBytes.Length);
        var previous = _captchaBitmap;
        _captchaBitmap = next;
        _captchaImage.SetImageBitmap(next);
        previous?.Dispose();
        _status.Text = "CAPTCHA đã được làm mới.";
    }

    private async Task TryRefreshCaptchaAsync()
    {
        try { await RefreshCaptchaAsync(); }
        catch { }
    }

    private GunnyRegistrationClient GetRegistrationClient(System.Uri gameBase)
    {
        if (_registrationClient is not null && _registrationBase == gameBase)
            return _registrationClient;
        _registrationClient = new GunnyRegistrationClient(gameBase);
        _registrationBase = gameBase;
        return _registrationClient;
    }

    private bool TryGetServer(out System.Uri gameBase)
    {
        if (System.Uri.TryCreate(_server.Text?.Trim(), UriKind.Absolute, out gameBase!))
            return true;
        _status.Text = "Địa chỉ máy chủ không hợp lệ.";
        return false;
    }

    private void ShowLogin()
    {
        _loginPanel.Visibility = ViewStates.Visible;
        _registerPanel.Visibility = ViewStates.Gone;
    }

    private void ShowRegister()
    {
        _loginPanel.Visibility = ViewStates.Gone;
        _registerPanel.Visibility = ViewStates.Visible;
    }

    protected override void OnDestroy()
    {
        _captchaBitmap?.Dispose();
        base.OnDestroy();
    }

    private static string Sanitize(string message) =>
        string.IsNullOrWhiteSpace(message) ? "Có lỗi xảy ra." : message.Trim();

    private EditText PasswordBox(string hint) => new(this)
    {
        Hint = hint,
        InputType = InputTypes.ClassText | InputTypes.TextVariationPassword
    };

    private LinearLayout Vertical() => new(this) { Orientation = Orientation.Vertical };

    private static LinearLayout.LayoutParams FullWidth() =>
        new(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

    private static LinearLayout.LayoutParams Weighted() =>
        new(0, ViewGroup.LayoutParams.WrapContent, 1f);
}
