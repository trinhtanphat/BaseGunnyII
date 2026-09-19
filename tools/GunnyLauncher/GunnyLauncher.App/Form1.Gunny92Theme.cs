namespace GunnyLauncher.App;

public partial class Form1
{
    private static readonly Color GunnyPanel = Color.FromArgb(48, 35, 92);
    private static readonly Color GunnyPanelAlt = Color.FromArgb(66, 48, 122);
    private static readonly Color GunnyGold = Color.FromArgb(255, 204, 0);
    private static readonly Color GunnyInput = Color.FromArgb(255, 218, 104);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    private void BuildGunny92Layout()
    {
        SuspendLayout();
        Text = "Gunny Launcher";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1070, 676);
        MinimumSize = new Size(1070, 676);
        MaximumSize = new Size(1070, 676);
        FormBorderStyle = FormBorderStyle.None;
        MaximizeBox = false;
        ShowIcon = false;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(34, 27, 78);
        DoubleBuffered = true;

        var backgroundPath = Path.Combine(AppContext.BaseDirectory, "Assets", "launcher-bg.png");
        if (File.Exists(backgroundPath))
        {
            using var source = Image.FromFile(backgroundPath);
            BackgroundImage = new Bitmap(source);
            BackgroundImageLayout = ImageLayout.Stretch;
        }

        _loginTab.Controls.Clear();
        _registerTab.Controls.Clear();
        _authTabs.TabPages.Clear();

        _loginTab.Padding = new Padding(12);
        _registerTab.Padding = new Padding(12);
        _loginTab.BackColor = GunnyPanel;
        _registerTab.BackColor = GunnyPanel;
        _loginTab.ForeColor = Color.White;
        _registerTab.ForeColor = Color.White;
        _loginTab.Controls.Add(BuildLoginLayout());
        _registerTab.Controls.Add(BuildRegisterLayout());
        _authTabs.TabPages.Add(_loginTab);
        _authTabs.TabPages.Add(_registerTab);

        _authTabs.Appearance = TabAppearance.Buttons;
        _authTabs.DrawMode = TabDrawMode.OwnerDrawFixed;
        _authTabs.SizeMode = TabSizeMode.Fixed;
        _authTabs.ItemSize = new Size(164, 34);
        _authTabs.Padding = new Point(6, 4);
        _authTabs.DrawItem += DrawGunnyTab;

        var shell = new TableLayoutPanel
        {
            Location = new Point(628, 184),
            Size = new Size(365, 410),
            ColumnCount = 1,
            RowCount = 3,
            BackColor = GunnyPanel,
            Padding = new Padding(12, 10, 12, 8),
        };
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        var serverRow = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            BackColor = GunnyPanel,
            Margin = new Padding(0, 0, 0, 6),
        };
        serverRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78));
        serverRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var serverLabel = new Label
        {
            Text = "Máy chủ",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            ForeColor = GunnyGold,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
        };
        serverRow.Controls.Add(serverLabel, 0, 0);
        serverRow.Controls.Add(_server, 1, 0);

        _authTabs.Dock = DockStyle.Fill;
        _authTabs.Margin = new Padding(0);
        shell.Controls.Add(serverRow, 0, 0);
        shell.Controls.Add(_authTabs, 0, 1);

        _status.Dock = DockStyle.Fill;
        _status.AutoEllipsis = true;
        _status.TextAlign = ContentAlignment.MiddleLeft;
        _status.ForeColor = Color.WhiteSmoke;
        _status.Font = new Font("Segoe UI", 8.5F);
        shell.Controls.Add(_status, 0, 2);

        StyleGunnyTree(shell);
        StylePrimaryButton(_play);
        StylePrimaryButton(_registerButton);
        StyleSecondaryButton(_refreshCaptcha);

        var buttonSkinPath = Path.Combine(AppContext.BaseDirectory, "Assets", "formButtonBackground.png");
        if (File.Exists(buttonSkinPath))
        {
            _play.BackgroundImage = new Bitmap(buttonSkinPath);
            _play.BackgroundImageLayout = ImageLayout.Stretch;
            _registerButton.BackgroundImage = new Bitmap(buttonSkinPath);
            _registerButton.BackgroundImageLayout = ImageLayout.Stretch;
        }

        var minimize = CreateWindowButton("—", new Point(996, 105));
        minimize.Click += (_, _) => WindowState = FormWindowState.Minimized;

        var close = CreateWindowButton("×", new Point(1030, 105));
        close.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        close.Click += (_, _) => Close();

        Controls.Add(shell);
        Controls.Add(minimize);
        Controls.Add(close);
        minimize.BringToFront();
        close.BringToFront();

        MouseDown += BeginGunnyWindowDrag;
        ResumeLayout(false);
    }

    private static Button CreateWindowButton(string text, Point location)
    {
        var button = new Button
        {
            Text = text,
            Location = location,
            Size = new Size(30, 24),
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(80, 66, 140),
            TabStop = false,
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(174, 169, 244);
        return button;
    }

    private void BeginGunnyWindowDrag(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        ReleaseCapture();
        SendMessage(Handle, 0xA1, new IntPtr(0x2), IntPtr.Zero);
    }

    private void DrawGunnyTab(object? sender, DrawItemEventArgs e)
    {
        var selected = e.Index == _authTabs.SelectedIndex;
        var bounds = e.Bounds;
        using var fill = new SolidBrush(selected ? GunnyPanelAlt : Color.FromArgb(90, 67, 180));
        using var border = new Pen(selected ? GunnyGold : Color.FromArgb(120, 106, 200));
        e.Graphics.FillRectangle(fill, bounds);
        e.Graphics.DrawRectangle(border, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
        var text = _authTabs.TabPages[e.Index].Text;
        TextRenderer.DrawText(
            e.Graphics,
            text,
            new Font("Segoe UI", 10F, FontStyle.Bold),
            bounds,
            selected ? GunnyGold : Color.White,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    private static void StyleGunnyTree(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            switch (control)
            {
                case TableLayoutPanel table:
                    table.BackColor = GunnyPanel;
                    break;
                case FlowLayoutPanel flow:
                    flow.BackColor = GunnyPanel;
                    break;
                case Label label:
                    label.ForeColor = label.Name == "statusLabel" ? Color.WhiteSmoke : GunnyGold;
                    break;
                case TextBox box:
                    box.BackColor = GunnyInput;
                    box.ForeColor = Color.FromArgb(45, 34, 74);
                    box.BorderStyle = BorderStyle.FixedSingle;
                    break;
                case ComboBox combo:
                    combo.BackColor = GunnyInput;
                    combo.ForeColor = Color.FromArgb(45, 34, 74);
                    combo.FlatStyle = FlatStyle.Flat;
                    break;
                case CheckBox check:
                    check.ForeColor = Color.White;
                    check.BackColor = GunnyPanel;
                    break;
                case PictureBox picture:
                    picture.BackColor = Color.White;
                    break;
            }

            if (control.HasChildren)
                StyleGunnyTree(control);
        }
    }

    private static void StylePrimaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 183, 38);
        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(225, 128, 25);
        button.BackColor = Color.FromArgb(247, 166, 33);
        button.ForeColor = Color.White;
        button.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
        button.Height = 40;
    }

    private static void StyleSecondaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderColor = Color.FromArgb(153, 132, 225);
        button.BackColor = GunnyPanelAlt;
        button.ForeColor = Color.White;
        button.Cursor = Cursors.Hand;
    }
}
