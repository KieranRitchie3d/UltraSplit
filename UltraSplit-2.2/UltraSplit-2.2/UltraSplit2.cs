using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Win32;

namespace UltraSplit2
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Native.EnableBestDpiAwareness();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    [Serializable]
    public sealed class SplitZone
    {
        public string Name { get; set; }
        public int Width { get; set; }

        public SplitZone()
        {
            Name = "Zone";
            Width = 1280;
        }

        public SplitZone(string name, int width)
        {
            Name = name;
            Width = width;
        }

        public SplitZone Clone()
        {
            return new SplitZone(Name, Width);
        }
    }

    [Serializable]
    public sealed class LayoutProfile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MonitorWidth { get; set; }
        public int MonitorHeight { get; set; }
        public int GameZoneIndex { get; set; }
        public int GameWidth { get; set; }
        public int GameHeight { get; set; }
        public bool CentreGameVertically { get; set; }
        public List<SplitZone> Zones { get; set; }

        public LayoutProfile()
        {
            Id = Guid.NewGuid().ToString("N");
            Name = "Custom Profile";
            Description = "";
            MonitorWidth = 5120;
            MonitorHeight = 1440;
            GameZoneIndex = 1;
            GameWidth = 3440;
            GameHeight = 1440;
            CentreGameVertically = true;
            Zones = new List<SplitZone>();
        }

        public LayoutProfile Clone()
        {
            return new LayoutProfile
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = Name + " Copy",
                Description = Description,
                MonitorWidth = MonitorWidth,
                MonitorHeight = MonitorHeight,
                GameZoneIndex = GameZoneIndex,
                GameWidth = GameWidth,
                GameHeight = GameHeight,
                CentreGameVertically = CentreGameVertically,
                Zones = Zones.Select(delegate(SplitZone z) { return z.Clone(); }).ToList()
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public sealed class AppSettings
    {
        public string SelectedProfileId { get; set; }
        public string MonitorDeviceName { get; set; }
        public bool DetectRememberedGames { get; set; }
        public bool DetectUnknownFullscreen { get; set; }
        public bool RemoveWindowFrame { get; set; }
        public bool EnforcePosition { get; set; }
        public bool ManageTaskbar { get; set; }
        public bool Notifications { get; set; }
        public bool RunOnWindowsStartup { get; set; }
        public int DetectionDelayMs { get; set; }
        public List<string> GameProcesses { get; set; }
        public List<LayoutProfile> Profiles { get; set; }

        public AppSettings()
        {
            SelectedProfileId = "5120-3440";
            MonitorDeviceName = "";
            DetectRememberedGames = true;
            DetectUnknownFullscreen = true;
            RemoveWindowFrame = true;
            EnforcePosition = true;
            ManageTaskbar = true;
            Notifications = true;
            RunOnWindowsStartup = false;
            DetectionDelayMs = 400;
            GameProcesses = new List<string>();
            Profiles = BuiltInProfiles();
        }

        public static List<LayoutProfile> BuiltInProfiles()
        {
            List<LayoutProfile> p = new List<LayoutProfile>();

            p.Add(Profile(
                "5120-native", "5120×1440 Native 32:9",
                "Uses the complete super-ultrawide panel.",
                5120, 1440, 0, 5120, 1440,
                new SplitZone("Game", 5120)));

            p.Add(Profile(
                "5120-3440", "3440×1440 Ultrawide Centre",
                "Exact 21:9 game resolution with 840 px desktop space on both sides.",
                5120, 1440, 1, 3440, 1440,
                new SplitZone("Left apps", 840),
                new SplitZone("Game", 3440),
                new SplitZone("Right apps", 840)));

            p.Add(Profile(
                "5120-2560", "2560×1440 QHD Centre",
                "Exact 16:9 QHD game resolution with 1280 px side workspaces.",
                5120, 1440, 1, 2560, 1440,
                new SplitZone("Left apps", 1280),
                new SplitZone("Game", 2560),
                new SplitZone("Right apps", 1280)));

            p.Add(Profile(
                "5120-1920", "1920×1080 Full HD Centre",
                "Exact 16:9 Full HD game rectangle, centred horizontally and vertically.",
                5120, 1440, 1, 1920, 1080,
                new SplitZone("Left apps", 1600),
                new SplitZone("Game column", 1920),
                new SplitZone("Right apps", 1600)));

            p.Add(Profile(
                "5120-2560x1080", "2560×1080 Ultrawide Centre",
                "Exact 21:9 1080p game rectangle with large side workspaces.",
                5120, 1440, 1, 2560, 1080,
                new SplitZone("Left apps", 1280),
                new SplitZone("Game column", 2560),
                new SplitZone("Right apps", 1280)));

            p.Add(Profile(
                "5120-dual", "Dual 2560×1440",
                "Two exact QHD workspaces. Select either side as the game zone.",
                5120, 1440, 0, 2560, 1440,
                new SplitZone("Left", 2560),
                new SplitZone("Right", 2560)));

            p.Add(Profile(
                "3440-native", "3440×1440 Native 21:9",
                "Uses the complete standard ultrawide panel.",
                3440, 1440, 0, 3440, 1440,
                new SplitZone("Game", 3440)));

            p.Add(Profile(
                "3440-2560", "2560×1440 QHD Centre",
                "Exact 16:9 QHD game resolution with 440 px side spaces.",
                3440, 1440, 1, 2560, 1440,
                new SplitZone("Left apps", 440),
                new SplitZone("Game", 2560),
                new SplitZone("Right apps", 440)));

            p.Add(Profile(
                "3440-1920", "1920×1080 Full HD Centre",
                "Exact 16:9 Full HD game rectangle centred inside a 3440×1440 panel.",
                3440, 1440, 1, 1920, 1080,
                new SplitZone("Left apps", 760),
                new SplitZone("Game column", 1920),
                new SplitZone("Right apps", 760)));

            p.Add(Profile(
                "3440-2560x1080", "2560×1080 Ultrawide Centre",
                "Exact 21:9 1080p game rectangle with 440 px side spaces.",
                3440, 1440, 1, 2560, 1080,
                new SplitZone("Left apps", 440),
                new SplitZone("Game column", 2560),
                new SplitZone("Right apps", 440)));

            p.Add(Profile(
                "3440-dual", "Dual 1720×1440 Desktop",
                "Two equal desktop zones. Intended for productivity rather than games.",
                3440, 1440, 0, 1720, 1440,
                new SplitZone("Left", 1720),
                new SplitZone("Right", 1720)));

            return p;
        }

        private static LayoutProfile Profile(
            string id, string name, string description,
            int monitorWidth, int monitorHeight,
            int gameZoneIndex, int gameWidth, int gameHeight,
            params SplitZone[] zones)
        {
            return new LayoutProfile
            {
                Id = id,
                Name = name,
                Description = description,
                MonitorWidth = monitorWidth,
                MonitorHeight = monitorHeight,
                GameZoneIndex = gameZoneIndex,
                GameWidth = gameWidth,
                GameHeight = gameHeight,
                CentreGameVertically = true,
                Zones = zones.ToList()
            };
        }
    }

    internal sealed class WindowInfo
    {
        public IntPtr Handle;
        public string ProcessName;
        public string Title;

        public override string ToString()
        {
            return ProcessName + " — " + Title;
        }
    }

    internal sealed class CapturedWindow
    {
        public IntPtr Handle;
        public long OriginalStyle;
        public long OriginalExStyle;
        public Native.RECT OriginalRect;
        public Native.WINDOWPLACEMENT OriginalPlacement;
        public IntPtr OriginalMenu;
        public int OriginalDwmNcPolicy;
        public bool OriginalDwmPolicyKnown;
        public bool WasMaximized;
        public string ProcessName;
        public string WindowTitle;
        public Rectangle TargetRect;
    }

    internal static class SettingsStore
    {
        public static string Folder
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "UltraSplit 2");
            }
        }

        public static string FilePath
        {
            get { return Path.Combine(Folder, "settings.json"); }
        }

        public static string NormalizeProcessName(string name)
        {
            string value = (name ?? "").Trim();
            if (value.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                value = value.Substring(0, value.Length - 4);
            return value;
        }

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return new AppSettings();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                AppSettings s = serializer.Deserialize<AppSettings>(File.ReadAllText(FilePath));
                if (s == null)
                    return new AppSettings();

                if (s.Profiles == null || s.Profiles.Count == 0)
                    s.Profiles = AppSettings.BuiltInProfiles();

                if (s.GameProcesses == null)
                    s.GameProcesses = new List<string>();

                return s;
            }
            catch
            {
                return new AppSettings();
            }
        }

        public static void Save(AppSettings settings)
        {
            Directory.CreateDirectory(Folder);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            File.WriteAllText(FilePath, serializer.Serialize(settings));
        }
    }

    internal sealed class ToastForm : Form
    {
        private readonly Timer _timer;

        public ToastForm(Icon icon, string heading, string message, bool warning)
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            Width = 560;
            Height = 108;
            BackColor = Color.FromArgb(24, 28, 34);
            Opacity = 0.98;
            Icon = icon;

            Panel stripe = new Panel();
            stripe.Dock = DockStyle.Left;
            stripe.Width = 7;
            stripe.BackColor = warning
                ? Color.FromArgb(217, 166, 59)
                : Color.FromArgb(0, 189, 135);
            Controls.Add(stripe);

            Label h = new Label();
            h.Text = heading;
            h.Left = 26;
            h.Top = 14;
            h.AutoSize = true;
            h.ForeColor = stripe.BackColor;
            h.Font = new Font("Segoe UI Semibold", 10.5f, FontStyle.Bold);
            Controls.Add(h);

            Label body = new Label();
            body.Text = message;
            body.Left = 26;
            body.Top = 43;
            body.Width = 500;
            body.Height = 55;
            body.ForeColor = Color.White;
            body.Font = new Font("Segoe UI", 9.25f);
            body.AutoEllipsis = true;
            Controls.Add(body);

            Screen target = Screen.FromPoint(Cursor.Position);
            Rectangle work = target.WorkingArea;
            Location = new Point(
                work.Left + (work.Width - Width) / 2,
                work.Top + 24);

            _timer = new Timer();
            _timer.Interval = warning ? 4800 : 3000;
            _timer.Tick += delegate
            {
                _timer.Stop();
                Close();
            };
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _timer.Start();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= Native.WS_EX_NOACTIVATE | Native.WS_EX_TOOLWINDOW;
                return cp;
            }
        }
    }

    internal sealed class SplitEditorPanel : Panel
    {
        public LayoutProfile Profile { get; set; }
        public int SelectedDivider { get; private set; }
        private bool _dragging;

        public SplitEditorPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(12, 15, 19);
            SelectedDivider = -1;
            Cursor = Cursors.Default;
        }

        public event Action LayoutChanged;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle area = ClientRectangle;
            area.Inflate(-8, -8);

            if (Profile == null || Profile.Zones == null || Profile.Zones.Count == 0)
                return;

            int total = Math.Max(1, Profile.Zones.Sum(delegate(SplitZone z) { return z.Width; }));
            int x = area.Left;

            for (int i = 0; i < Profile.Zones.Count; i++)
            {
                SplitZone zone = Profile.Zones[i];
                int width = i == Profile.Zones.Count - 1
                    ? area.Right - x
                    : (int)Math.Round(area.Width * zone.Width / (double)total);

                Rectangle rect = new Rectangle(x, area.Top, Math.Max(1, width), area.Height);
                bool game = i == Profile.GameZoneIndex;

                using (SolidBrush brush = new SolidBrush(
                    game ? Color.FromArgb(0, 137, 97) : Color.FromArgb(37, 48, 56)))
                    e.Graphics.FillRectangle(brush, rect);

                using (Pen border = new Pen(Color.FromArgb(9, 14, 16), 2))
                    e.Graphics.DrawRectangle(border, rect);

                string label = zone.Name + "\n" + zone.Width + " px";
                using (Font font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                using (StringFormat fmt = new StringFormat())
                {
                    fmt.Alignment = StringAlignment.Center;
                    fmt.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(label, font, textBrush, rect, fmt);
                }

                x = rect.Right;
            }

            if (SelectedDivider >= 0 && SelectedDivider < Profile.Zones.Count - 1)
            {
                int dividerX = area.Left;
                for (int i = 0; i <= SelectedDivider; i++)
                    dividerX += (int)Math.Round(area.Width * Profile.Zones[i].Width / (double)total);

                using (Pen pen = new Pen(Color.FromArgb(0, 225, 160), 4))
                    e.Graphics.DrawLine(pen, dividerX, area.Top, dividerX, area.Bottom);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Profile == null || Profile.Zones.Count < 2)
                return;

            Rectangle area = ClientRectangle;
            area.Inflate(-8, -8);
            int total = Profile.Zones.Sum(delegate(SplitZone z) { return z.Width; });

            if (_dragging && SelectedDivider >= 0)
            {
                int desiredLeftTotal = (int)Math.Round(
                    Math.Max(0, Math.Min(area.Width, e.X - area.Left)) *
                    total / (double)Math.Max(1, area.Width));

                int fixedBefore = 0;
                for (int i = 0; i < SelectedDivider; i++)
                    fixedBefore += Profile.Zones[i].Width;

                int pairTotal =
                    Profile.Zones[SelectedDivider].Width +
                    Profile.Zones[SelectedDivider + 1].Width;

                int newLeft = desiredLeftTotal - fixedBefore;
                newLeft = Math.Max(200, Math.Min(pairTotal - 200, newLeft));

                Profile.Zones[SelectedDivider].Width = newLeft;
                Profile.Zones[SelectedDivider + 1].Width = pairTotal - newLeft;

                Invalidate();
                if (LayoutChanged != null)
                    LayoutChanged();
                return;
            }

            int x = area.Left;
            SelectedDivider = -1;
            for (int i = 0; i < Profile.Zones.Count - 1; i++)
            {
                x += (int)Math.Round(area.Width * Profile.Zones[i].Width / (double)total);
                if (Math.Abs(e.X - x) <= 7)
                {
                    SelectedDivider = i;
                    break;
                }
            }

            Cursor = SelectedDivider >= 0 ? Cursors.VSplit : Cursors.Default;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (SelectedDivider >= 0 && e.Button == MouseButtons.Left)
                _dragging = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragging = false;
        }
    }

    internal sealed class MainForm : Form
    {
        private const int HK_FORCE = 0x8201;
        private const int HK_RESTORE = 0x8202;

        private AppSettings _settings;
        private readonly Dictionary<IntPtr, CapturedWindow> _captured;
        private readonly Dictionary<IntPtr, DateTime> _candidates;
        private readonly HashSet<IntPtr> _suppressed;
        private Timer _timer;
        private NotifyIcon _tray;
        private Icon _appIcon;
        private bool _reallyExit;
        private bool _loading;
        private uint _originalTaskbarState;
        private bool _taskbarOverlayApplied;
        private IntPtr _lastTaskbarForeground = IntPtr.Zero;
        private bool? _lastTaskbarVisible = null;

        private TabControl _tabs;
        private ComboBox _monitorCombo;
        private ComboBox _profileCombo;
        private Label _profileDescription;
        private SplitEditorPanel _preview;
        private DataGridView _zoneGrid;
        private NumericUpDown _gameWidth;
        private NumericUpDown _gameHeight;
        private ComboBox _gameZoneCombo;
        private TextBox _profileName;
        private TextBox _profileDescriptionEdit;
        private Label _validationLabel;
        private ComboBox _windowCombo;
        private ListBox _gamesList;
        private CheckBox _rememberedGames;
        private CheckBox _unknownFullscreen;
        private CheckBox _removeFrame;
        private CheckBox _enforce;
        private CheckBox _taskbar;
        private CheckBox _notifications;
        private CheckBox _startup;
        private NumericUpDown _delay;
        private Label _status;

        public MainForm()
        {
            _settings = SettingsStore.Load();
            _captured = new Dictionary<IntPtr, CapturedWindow>();
            _candidates = new Dictionary<IntPtr, DateTime>();
            _suppressed = new HashSet<IntPtr>();
            _originalTaskbarState = Native.GetTaskbarState();

            _appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application;
            Icon = _appIcon;
            Text = "UltraSplit 2.2";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1180, 820);
            MinimumSize = new Size(1020, 720);
            BackColor = Color.FromArgb(15, 18, 22);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9f);
            AutoScaleMode = AutoScaleMode.Dpi;

            BuildUi();
            BuildTray();
            LoadUi();
            RegisterHotkeys();

            _timer = new Timer();
            _timer.Interval = 120;
            _timer.Tick += TimerTick;
            _timer.Start();

            FormClosing += MainFormClosing;
            Resize += delegate
            {
                if (WindowState == FormWindowState.Minimized)
                    Hide();
            };
        }

        private Color Accent
        {
            get { return Color.FromArgb(0, 189, 135); }
        }

        private void BuildUi()
        {
            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 84;
            header.BackColor = Color.FromArgb(9, 12, 15);
            Controls.Add(header);

            PictureBox logo = new PictureBox();
            logo.Image = _appIcon.ToBitmap();
            logo.SizeMode = PictureBoxSizeMode.Zoom;
            logo.SetBounds(28, 18, 48, 48);
            header.Controls.Add(logo);

            Label title = new Label();
            title.Text = "UltraSplit 2.2";
            title.Font = new Font("Segoe UI Semibold", 20f, FontStyle.Bold);
            title.ForeColor = Accent;
            title.AutoSize = true;
            title.Left = 90;
            title.Top = 13;
            header.Controls.Add(title);

            Label subtitle = new Label();
            subtitle.Text = "Resolution-safe game layouts for ultrawide and super-ultrawide displays";
            subtitle.ForeColor = Color.FromArgb(167, 178, 190);
            subtitle.AutoSize = true;
            subtitle.Left = 92;
            subtitle.Top = 50;
            header.Controls.Add(subtitle);

            _status = new Label();
            _status.Text = "ADMIN MODE";
            _status.TextAlign = ContentAlignment.MiddleCenter;
            _status.BackColor = Color.FromArgb(24, 76, 60);
            _status.ForeColor = Color.FromArgb(93, 241, 188);
            _status.Width = 150;
            _status.Height = 32;
            _status.Left = ClientSize.Width - 180;
            _status.Top = 25;
            _status.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            header.Controls.Add(_status);

            _tabs = new TabControl();
            _tabs.Dock = DockStyle.Fill;
            _tabs.Alignment = TabAlignment.Top;
            _tabs.Padding = new Point(24, 8);
            Controls.Add(_tabs);
            _tabs.BringToFront();

            _tabs.TabPages.Add(BuildHomePage());
            _tabs.TabPages.Add(BuildProfilesPage());
            _tabs.TabPages.Add(BuildDetectionPage());
            _tabs.TabPages.Add(BuildSettingsPage());
        }

        private TabPage NewPage(string title)
        {
            TabPage page = new TabPage(title);
            page.BackColor = Color.FromArgb(24, 29, 35);
            page.ForeColor = Color.White;
            page.Padding = new Padding(30);
            return page;
        }

        private TabPage BuildHomePage()
        {
            TabPage page = NewPage("Home");
            Panel centre = NewCenteredPanel(page, 980, 600);

            Label h = Heading("Current layout", 0, 0, 980);
            centre.Controls.Add(h);

            AddLabel(centre, "Target monitor", 100, 60, 200);
            _monitorCombo = NewCombo(100, 84, 780);
            _monitorCombo.SelectedIndexChanged += delegate
            {
                if (!_loading)
                {
                    _settings.MonitorDeviceName = CurrentScreen.DeviceName;
                    FilterProfilesForMonitor();
                    SaveSettings();
                }
            };
            centre.Controls.Add(_monitorCombo);

            AddLabel(centre, "Active profile", 100, 140, 200);
            _profileCombo = NewCombo(100, 164, 780);
            _profileCombo.SelectedIndexChanged += delegate
            {
                if (!_loading)
                    SelectProfileFromCombo();
            };
            centre.Controls.Add(_profileCombo);

            _profileDescription = new Label();
            _profileDescription.SetBounds(100, 212, 780, 55);
            _profileDescription.ForeColor = Color.FromArgb(170, 180, 192);
            _profileDescription.TextAlign = ContentAlignment.TopCenter;
            centre.Controls.Add(_profileDescription);

            _preview = new SplitEditorPanel();
            _preview.SetBounds(100, 280, 780, 150);
            _preview.Enabled = false;
            centre.Controls.Add(_preview);

            Button force = AccentButton("Apply profile to selected app", 190, 470, 280);
            force.Click += delegate { ForceSelectedWindow(); };
            centre.Controls.Add(force);

            Button restore = Button("Restore everything", 490, 470, 220);
            restore.Click += delegate
            {
                RestoreAll(true);
                Popup("Restore complete", "All modified windows and the original taskbar state were restored.", false);
            };
            centre.Controls.Add(restore);

            Label help = new Label();
            help.Text =
                "Choose a profile, explicitly select a running application under Game Detection, then apply it. " +
                "UltraSplit confirms the exact window and profile before changing anything.";
            help.SetBounds(120, 530, 740, 48);
            help.ForeColor = Color.FromArgb(170, 180, 192);
            help.TextAlign = ContentAlignment.MiddleCenter;
            centre.Controls.Add(help);

            return page;
        }

        private TabPage BuildProfilesPage()
        {
            TabPage page = NewPage("Profiles & Layouts");
            Panel centre = NewCenteredPanel(page, 1080, 660);

            centre.Controls.Add(Heading("Profiles & custom layout editor", 0, 0, 1080));

            // -----------------------------------------------------------------
            // Profile library
            // -----------------------------------------------------------------
            GroupBox library = NewGroup("Profile library", 10, 48, 270, 595);
            centre.Controls.Add(library);

            ListBox profiles = new ListBox();
            profiles.Name = "profilesList";
            profiles.SetBounds(15, 30, 240, 395);
            profiles.BackColor = Color.FromArgb(14, 18, 22);
            profiles.ForeColor = Color.White;
            library.Controls.Add(profiles);

            Label profileState = new Label();
            profileState.Name = "profileEditorState";
            profileState.SetBounds(15, 432, 240, 28);
            profileState.TextAlign = ContentAlignment.MiddleCenter;
            profileState.Font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold);
            library.Controls.Add(profileState);

            Button use = AccentButton("Use selected profile", 15, 470, 240);
            use.Name = "useProfileButton";
            use.Click += delegate
            {
                LayoutProfile p = profiles.SelectedItem as LayoutProfile;
                if (p == null)
                {
                    Popup("No profile selected", "Select a profile first.", true);
                    return;
                }

                _settings.SelectedProfileId = p.Id;
                SaveSettings();
                LoadProfileCombos();
                PopulateProfilesList(profiles);
                Popup("Profile activated", p.Name + " is now the active profile.", false);
            };
            library.Controls.Add(use);

            Button duplicate = Button("Duplicate to edit", 15, 520, 115);
            duplicate.Name = "duplicateProfileButton";
            duplicate.Click += delegate
            {
                LayoutProfile p = profiles.SelectedItem as LayoutProfile;
                if (p == null)
                {
                    Popup("No profile selected", "Select a profile first.", true);
                    return;
                }

                LayoutProfile copy = p.Clone();
                _settings.Profiles.Add(copy);
                _settings.SelectedProfileId = copy.Id;
                SaveSettings();

                PopulateProfilesList(profiles);
                profiles.SelectedItem = copy;
                LoadProfileIntoEditor(copy);

                Popup(
                    "Editable profile created",
                    copy.Name + " is a custom copy. Its splits and game rectangle can now be edited.",
                    false);
            };
            library.Controls.Add(duplicate);

            Button delete = Button("Delete", 140, 520, 115);
            delete.Name = "deleteProfileButton";
            delete.Click += delegate
            {
                LayoutProfile p = profiles.SelectedItem as LayoutProfile;
                if (p == null)
                {
                    Popup("No profile selected", "Select a profile first.", true);
                    return;
                }

                if (IsBuiltInProfile(p))
                {
                    Popup(
                        "Built-in profile",
                        "Built-in profiles cannot be deleted. Duplicate it to create an editable custom profile.",
                        true);
                    return;
                }

                DialogResult answer = MessageBox.Show(
                    this,
                    "Delete custom profile \"" + p.Name + "\"?",
                    "Confirm profile deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (answer != DialogResult.Yes)
                {
                    Popup("Deletion cancelled", "The profile was not changed.", false);
                    return;
                }

                _settings.Profiles.Remove(p);

                if (_settings.SelectedProfileId == p.Id)
                {
                    LayoutProfile fallback = _settings.Profiles.FirstOrDefault(delegate(LayoutProfile x)
                    {
                        return x.MonitorWidth == CurrentScreen.Bounds.Width &&
                               x.MonitorHeight == CurrentScreen.Bounds.Height;
                    }) ?? _settings.Profiles.First();

                    _settings.SelectedProfileId = fallback.Id;
                }

                SaveSettings();
                PopulateProfilesList(profiles);
                LoadProfileCombos();
                Popup("Profile deleted", p.Name + " was deleted.", false);
            };
            library.Controls.Add(delete);

            // -----------------------------------------------------------------
            // Editor
            // -----------------------------------------------------------------
            GroupBox editorGroup = NewGroup("Selected profile", 300, 48, 770, 595);
            centre.Controls.Add(editorGroup);

            AddLabel(editorGroup, "Profile name", 18, 29, 140);
            _profileName = NewText(18, 53, 350);
            editorGroup.Controls.Add(_profileName);

            AddLabel(editorGroup, "Description", 390, 29, 140);
            _profileDescriptionEdit = NewText(390, 53, 360);
            editorGroup.Controls.Add(_profileDescriptionEdit);

            SplitEditorPanel editor = new SplitEditorPanel();
            editor.Name = "layoutEditor";
            editor.SetBounds(18, 105, 732, 125);
            editor.LayoutChanged += delegate
            {
                RefreshZoneGrid();
                ValidateEditor();
            };
            editorGroup.Controls.Add(editor);

            _zoneGrid = new DataGridView();
            _zoneGrid.SetBounds(18, 252, 445, 205);
            _zoneGrid.AllowUserToAddRows = false;
            _zoneGrid.AllowUserToDeleteRows = false;
            _zoneGrid.RowHeadersVisible = false;
            _zoneGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _zoneGrid.BackgroundColor = Color.FromArgb(14, 18, 22);
            _zoneGrid.ForeColor = Color.Black;
            _zoneGrid.Columns.Add("Name", "Zone name");
            _zoneGrid.Columns.Add("Width", "Width (px)");
            _zoneGrid.CellEndEdit += delegate
            {
                ApplyGridToEditor();
            };
            editorGroup.Controls.Add(_zoneGrid);

            GroupBox game = NewGroup("Game rectangle", 482, 252, 268, 205);
            editorGroup.Controls.Add(game);

            AddLabel(game, "Game zone", 17, 29, 110);
            _gameZoneCombo = NewCombo(17, 53, 234);
            _gameZoneCombo.SelectedIndexChanged += delegate
            {
                if (!_loading && CurrentEditorProfile != null)
                {
                    CurrentEditorProfile.GameZoneIndex =
                        Math.Max(0, _gameZoneCombo.SelectedIndex);
                    editor.Invalidate();
                    ValidateEditor();
                }
            };
            game.Controls.Add(_gameZoneCombo);

            AddLabel(game, "Game width", 17, 105, 100);
            _gameWidth = NewNumber(17, 129, 108, 320, 10000);
            _gameWidth.ValueChanged += delegate
            {
                if (!_loading && CurrentEditorProfile != null)
                {
                    CurrentEditorProfile.GameWidth = (int)_gameWidth.Value;
                    ValidateEditor();
                }
            };
            game.Controls.Add(_gameWidth);

            AddLabel(game, "Game height", 143, 105, 100);
            _gameHeight = NewNumber(143, 129, 108, 320, 10000);
            _gameHeight.ValueChanged += delegate
            {
                if (!_loading && CurrentEditorProfile != null)
                {
                    CurrentEditorProfile.GameHeight = (int)_gameHeight.Value;
                    ValidateEditor();
                }
            };
            game.Controls.Add(_gameHeight);

            Button add = Button("Add split", 18, 478, 125);
            add.Name = "addSplitButton";
            add.Click += delegate
            {
                LayoutProfile p = CurrentEditorProfile;
                if (p == null)
                {
                    Popup("No profile loaded", "Select or duplicate a profile first.", true);
                    return;
                }

                if (IsBuiltInProfile(p))
                {
                    Popup("Built-in profile", "Duplicate this profile before editing its splits.", true);
                    return;
                }

                if (p.Zones.Count >= 6)
                {
                    Popup("Split limit", "Profiles support up to six horizontal zones.", true);
                    return;
                }

                SplitZone largest = p.Zones
                    .OrderByDescending(delegate(SplitZone z) { return z.Width; })
                    .First();

                if (largest.Width < 600)
                {
                    Popup(
                        "No room for another split",
                        "The widest existing zone must be at least 600 px before another split can be added.",
                        true);
                    return;
                }

                int newWidth = Math.Max(200, Math.Min(400, largest.Width / 2));
                largest.Width -= newWidth;
                p.Zones.Add(new SplitZone("New zone", newWidth));

                RefreshZoneGrid();
                editor.Invalidate();
                ValidateEditor();
                Popup(
                    "Split added",
                    "A " + newWidth + " px split was added without changing the total monitor width.",
                    false);
            };
            editorGroup.Controls.Add(add);

            Button remove = Button("Remove split", 153, 478, 125);
            remove.Name = "removeSplitButton";
            remove.Click += delegate
            {
                LayoutProfile p = CurrentEditorProfile;
                if (p == null)
                {
                    Popup("No profile loaded", "Select or duplicate a profile first.", true);
                    return;
                }

                if (IsBuiltInProfile(p))
                {
                    Popup("Built-in profile", "Duplicate this profile before editing its splits.", true);
                    return;
                }

                if (p.Zones.Count <= 1)
                {
                    Popup("Cannot remove split", "A profile must contain at least one zone.", true);
                    return;
                }

                int index = _zoneGrid.CurrentCell == null
                    ? p.Zones.Count - 1
                    : _zoneGrid.CurrentCell.RowIndex;

                index = Math.Max(0, Math.Min(index, p.Zones.Count - 1));

                int removedWidth = p.Zones[index].Width;
                p.Zones.RemoveAt(index);

                int recipient = Math.Max(0, Math.Min(index - 1, p.Zones.Count - 1));
                p.Zones[recipient].Width += removedWidth;
                p.GameZoneIndex = Math.Min(p.GameZoneIndex, p.Zones.Count - 1);

                RefreshZoneGrid();
                editor.Invalidate();
                ValidateEditor();
                Popup(
                    "Split removed",
                    "The removed " + removedWidth + " px was transferred to the adjacent zone.",
                    false);
            };
            editorGroup.Controls.Add(remove);

            Button balance = Button("Balance", 288, 478, 125);
            balance.Name = "balanceSplitButton";
            balance.Click += delegate
            {
                LayoutProfile p = CurrentEditorProfile;
                if (p == null)
                {
                    Popup("No profile loaded", "Select or duplicate a profile first.", true);
                    return;
                }

                if (IsBuiltInProfile(p))
                {
                    Popup("Built-in profile", "Duplicate this profile before editing its splits.", true);
                    return;
                }

                int each = p.MonitorWidth / p.Zones.Count;
                for (int i = 0; i < p.Zones.Count; i++)
                    p.Zones[i].Width = each;

                p.Zones[p.Zones.Count - 1].Width +=
                    p.MonitorWidth - (each * p.Zones.Count);

                RefreshZoneGrid();
                editor.Invalidate();
                ValidateEditor();
                Popup(
                    "Splits balanced",
                    "All splits were balanced and still total exactly " + p.MonitorWidth + " px.",
                    false);
            };
            editorGroup.Controls.Add(balance);

            Button reset = Button("Reload", 423, 478, 110);
            reset.Name = "reloadProfileButton";
            reset.Click += delegate
            {
                LayoutProfile selectedProfile = profiles.SelectedItem as LayoutProfile;
                if (selectedProfile == null)
                {
                    Popup("No profile selected", "Select a profile first.", true);
                    return;
                }

                AppSettings diskSettings = SettingsStore.Load();
                LayoutProfile stored = diskSettings.Profiles.FirstOrDefault(delegate(LayoutProfile x)
                {
                    return x.Id == selectedProfile.Id;
                });

                if (stored != null)
                {
                    int index = _settings.Profiles.IndexOf(selectedProfile);
                    _settings.Profiles[index] = stored;
                    profiles.Items[profiles.SelectedIndex] = stored;
                    profiles.SelectedItem = stored;
                    LoadProfileIntoEditor(stored);
                    Popup("Profile reloaded", "Unsaved editor changes were discarded.", false);
                }
                else
                {
                    LoadProfileIntoEditor(selectedProfile);
                    Popup("Profile reloaded", "The profile was reloaded from memory.", false);
                }
            };
            editorGroup.Controls.Add(reset);

            Button save = AccentButton("Save custom profile", 550, 478, 200);
            save.Name = "saveProfileButton";
            save.Click += delegate { SaveEditorProfile(); };
            editorGroup.Controls.Add(save);

            _validationLabel = new Label();
            _validationLabel.SetBounds(18, 535, 732, 42);
            _validationLabel.TextAlign = ContentAlignment.MiddleCenter;
            editorGroup.Controls.Add(_validationLabel);

            profiles.SelectedIndexChanged += delegate
            {
                LayoutProfile p = profiles.SelectedItem as LayoutProfile;
                if (p == null)
                {
                    ClearProfileEditor();
                    return;
                }

                LoadProfileIntoEditor(p);
                SetEditorReadOnly(IsBuiltInProfile(p));

                bool active = p.Id == _settings.SelectedProfileId;
                bool builtIn = IsBuiltInProfile(p);

                profileState.Text =
                    active
                        ? (builtIn ? "ACTIVE • BUILT-IN" : "ACTIVE • CUSTOM")
                        : (builtIn ? "BUILT-IN • READ ONLY" : "CUSTOM • EDITABLE");

                profileState.ForeColor =
                    active
                        ? Color.FromArgb(91, 241, 187)
                        : (builtIn
                            ? Color.FromArgb(215, 166, 59)
                            : Color.FromArgb(181, 192, 204));
            };

            page.Tag = profiles;
            return page;
        }

        private TabPage BuildDetectionPage()
        {
            TabPage page = NewPage("Game Detection");
            Panel centre = NewCenteredPanel(page, 960, 610);

            centre.Controls.Add(Heading("Game detection and manual control", 0, 0, 960));

            AddLabel(centre, "Open application/window", 80, 65, 260);
            _windowCombo = NewCombo(80, 89, 620);
            centre.Controls.Add(_windowCombo);

            Button refresh = Button("Refresh", 720, 86, 150);
            refresh.Click += delegate
            {
                PopulateWindows();
                Popup("Window list refreshed", "Open applications were scanned again.", false);
            };
            centre.Controls.Add(refresh);

            Button remember = AccentButton("Remember selected as game", 80, 145, 300);
            remember.Click += delegate { RememberSelectedGame(); };
            centre.Controls.Add(remember);

            Button force = AccentButton("Force active profile now", 400, 145, 300);
            force.Click += delegate { ForceSelectedWindow(); };
            centre.Controls.Add(force);

            Button forget = Button("Forget selected game", 720, 145, 150);
            forget.Click += delegate { ForgetSelectedGame(); };
            centre.Controls.Add(forget);

            AddLabel(centre, "Remembered game processes", 80, 215, 300);
            _gamesList = new ListBox();
            _gamesList.SetBounds(80, 245, 790, 135);
            _gamesList.BackColor = Color.FromArgb(14, 18, 22);
            _gamesList.ForeColor = Color.White;
            centre.Controls.Add(_gamesList);

            GroupBox auto = NewGroup("Automatic detection", 80, 410, 790, 145);
            centre.Controls.Add(auto);

            _rememberedGames = NewCheck(
                "Detect remembered games in Windowed or Borderless mode",
                30, 35, 360);
            auto.Controls.Add(_rememberedGames);

            _unknownFullscreen = NewCheck(
                "Detect unknown applications only when genuinely fullscreen",
                410, 35, 350);
            auto.Controls.Add(_unknownFullscreen);

            AddLabel(auto, "Detection delay", 30, 85, 130);
            _delay = NewNumber(165, 81, 110, 100, 5000);
            auto.Controls.Add(_delay);
            AddLabel(auto, "milliseconds", 285, 85, 100);

            Label note = new Label();
            note.Text =
                "Remembering a process is the reliable way to distinguish a windowed game from ordinary desktop software.";
            note.SetBounds(80, 565, 790, 35);
            note.ForeColor = Color.FromArgb(170, 180, 192);
            note.TextAlign = ContentAlignment.MiddleCenter;
            centre.Controls.Add(note);

            return page;
        }

        private TabPage BuildSettingsPage()
        {
            TabPage page = NewPage("Settings");
            Panel centre = NewCenteredPanel(page, 850, 560);

            centre.Controls.Add(Heading("Application settings", 0, 0, 850));

            GroupBox behavior = NewGroup("Window and taskbar behaviour", 100, 70, 650, 190);
            centre.Controls.Add(behavior);

            _removeFrame = NewCheck(
                "Remove Windows title bars, resize frames and DWM non-client rendering",
                30, 38, 580);
            behavior.Controls.Add(_removeFrame);

            _enforce = NewCheck(
                "Apply the selected profile once when the game is detected",
                30, 78, 580);
            behavior.Controls.Add(_enforce);

            _taskbar = NewCheck(
                "Use overlay auto-hide taskbar while a captured game has focus",
                30, 118, 580);
            behavior.Controls.Add(_taskbar);

            _notifications = NewCheck(
                "Show a confirmation or error pop-up whenever an action is performed",
                30, 158, 590);
            behavior.Controls.Add(_notifications);

            GroupBox startup = NewGroup("Windows startup", 100, 290, 650, 105);
            centre.Controls.Add(startup);

            _startup = NewCheck(
                "Run UltraSplit automatically when I sign in to Windows",
                30, 40, 560);
            startup.Controls.Add(_startup);

            Label admin = new Label();
            admin.Text =
                "UltraSplit 2.1 always requests administrator permission when it opens. " +
                "This is required to control games that run elevated.";
            admin.SetBounds(120, 420, 610, 55);
            admin.ForeColor = Color.FromArgb(180, 188, 199);
            admin.TextAlign = ContentAlignment.MiddleCenter;
            centre.Controls.Add(admin);

            Button save = AccentButton("Save all settings", 260, 495, 330);
            save.Click += delegate
            {
                ReadSettingsFromUi();
                SaveSettings();
                Popup("Settings saved", "All detection, taskbar and startup settings were saved.", false);
            };
            centre.Controls.Add(save);

            return page;
        }

        private Panel NewCenteredPanel(Control parent, int width, int height)
        {
            Panel panel = new Panel();
            panel.Width = width;
            panel.Height = height;
            panel.BackColor = Color.Transparent;

            Action centre = delegate
            {
                panel.Left = Math.Max(0, (parent.ClientSize.Width - panel.Width) / 2);
                panel.Top = Math.Max(0, (parent.ClientSize.Height - panel.Height) / 2);
            };

            parent.Controls.Add(panel);
            parent.Resize += delegate { centre(); };
            centre();
            return panel;
        }

        private Label Heading(string text, int x, int y, int width)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("Segoe UI Semibold", 18f, FontStyle.Bold);
            label.ForeColor = Accent;
            label.SetBounds(x, y, width, 44);
            label.TextAlign = ContentAlignment.MiddleCenter;
            return label;
        }

        private Label AddLabel(Control parent, string text, int x, int y, int width)
        {
            Label label = new Label();
            label.Text = text;
            label.SetBounds(x, y, width, 22);
            label.ForeColor = Color.FromArgb(207, 214, 222);
            parent.Controls.Add(label);
            return label;
        }

        private GroupBox NewGroup(string text, int x, int y, int width, int height)
        {
            GroupBox group = new GroupBox();
            group.Text = text;
            group.SetBounds(x, y, width, height);
            group.ForeColor = Color.White;
            return group;
        }

        private ComboBox NewCombo(int x, int y, int width)
        {
            ComboBox combo = new ComboBox();
            combo.SetBounds(x, y, width, 34);
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            return combo;
        }

        private TextBox NewText(int x, int y, int width)
        {
            TextBox box = new TextBox();
            box.SetBounds(x, y, width, 34);
            return box;
        }

        private NumericUpDown NewNumber(int x, int y, int width, int min, int max)
        {
            NumericUpDown number = new NumericUpDown();
            number.SetBounds(x, y, width, 34);
            number.Minimum = min;
            number.Maximum = max;
            number.ThousandsSeparator = true;
            return number;
        }

        private CheckBox NewCheck(string text, int x, int y, int width)
        {
            CheckBox box = new CheckBox();
            box.Text = text;
            box.SetBounds(x, y, width, 27);
            return box;
        }

        private Button Button(string text, int x, int y, int width)
        {
            Button button = new Button();
            button.Text = text;
            button.SetBounds(x, y, width, 40);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = Color.FromArgb(67, 77, 89);
            button.BackColor = Color.FromArgb(43, 49, 58);
            button.ForeColor = Color.White;
            return button;
        }

        private Button AccentButton(string text, int x, int y, int width)
        {
            Button button = Button(text, x, y, width);
            button.BackColor = Accent;
            button.ForeColor = Color.FromArgb(3, 18, 13);
            button.FlatAppearance.BorderColor = Accent;
            return button;
        }

        private void BuildTray()
        {
            _tray = new NotifyIcon();
            _tray.Visible = true;
            _tray.Icon = _appIcon;
            _tray.Text = "UltraSplit 2.2";
            _tray.DoubleClick += delegate { ShowMainWindow(); };

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Open UltraSplit", null, delegate { ShowMainWindow(); });
            menu.Items.Add("Force selected game", null, delegate { ForceSelectedWindow(); });
            menu.Items.Add("Restore everything", null, delegate { RestoreAll(true); });
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Exit", null, delegate
            {
                _reallyExit = true;
                Close();
            });
            _tray.ContextMenuStrip = menu;
        }

        private void LoadUi()
        {
            _loading = true;
            PopulateMonitors();
            LoadProfileCombos();
            PopulateWindows();
            RefreshGameList();

            _rememberedGames.Checked = _settings.DetectRememberedGames;
            _unknownFullscreen.Checked = _settings.DetectUnknownFullscreen;
            _removeFrame.Checked = _settings.RemoveWindowFrame;
            _enforce.Checked = _settings.EnforcePosition;
            _taskbar.Checked = _settings.ManageTaskbar;
            _notifications.Checked = _settings.Notifications;
            _startup.Checked = _settings.RunOnWindowsStartup;
            _delay.Value = Math.Max(_delay.Minimum, Math.Min(_delay.Maximum, _settings.DetectionDelayMs));

            ListBox profiles = _tabs.TabPages[1].Tag as ListBox;
            PopulateProfilesList(profiles);
            _loading = false;

            SelectProfileFromCombo();

            if (profiles != null && profiles.SelectedItem is LayoutProfile)
                LoadProfileIntoEditor((LayoutProfile)profiles.SelectedItem);
        }

        private Screen CurrentScreen
        {
            get
            {
                Screen[] screens = Screen.AllScreens;
                int index = _monitorCombo.SelectedIndex;
                if (index < 0 || index >= screens.Length)
                    return Screen.PrimaryScreen;
                return screens[index];
            }
        }

        private LayoutProfile ActiveProfile
        {
            get
            {
                return _settings.Profiles.FirstOrDefault(delegate(LayoutProfile p)
                {
                    return p.Id == _settings.SelectedProfileId;
                }) ?? _settings.Profiles.First();
            }
        }

        private LayoutProfile CurrentEditorProfile
        {
            get
            {
                SplitEditorPanel editor = FindControl<SplitEditorPanel>("layoutEditor");
                return editor == null ? null : editor.Profile;
            }
        }

        private T FindControl<T>(string name) where T : Control
        {
            foreach (Control control in GetAllControls(this))
            {
                T typed = control as T;
                if (typed != null && typed.Name == name)
                    return typed;
            }
            return null;
        }

        private IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                yield return child;
                foreach (Control nested in GetAllControls(child))
                    yield return nested;
            }
        }

        private void PopulateMonitors()
        {
            _monitorCombo.Items.Clear();
            Screen[] screens = Screen.AllScreens;
            int selected = 0;

            for (int i = 0; i < screens.Length; i++)
            {
                Screen s = screens[i];
                _monitorCombo.Items.Add(
                    s.DeviceName + " — " + s.Bounds.Width + "×" + s.Bounds.Height +
                    (s.Primary ? " (Primary)" : ""));

                if (s.DeviceName == _settings.MonitorDeviceName)
                    selected = i;
                else if (string.IsNullOrWhiteSpace(_settings.MonitorDeviceName) && s.Primary)
                    selected = i;
            }

            if (_monitorCombo.Items.Count > 0)
                _monitorCombo.SelectedIndex = selected;
        }

        private void FilterProfilesForMonitor()
        {
            int width = CurrentScreen.Bounds.Width;
            int height = CurrentScreen.Bounds.Height;
            List<LayoutProfile> compatible = _settings.Profiles
                .Where(delegate(LayoutProfile p)
                {
                    return p.MonitorWidth == width && p.MonitorHeight == height;
                })
                .ToList();

            _loading = true;
            _profileCombo.Items.Clear();
            foreach (LayoutProfile p in compatible)
                _profileCombo.Items.Add(p);

            LayoutProfile selected = compatible.FirstOrDefault(delegate(LayoutProfile p)
            {
                return p.Id == _settings.SelectedProfileId;
            }) ?? compatible.FirstOrDefault();

            if (selected != null)
            {
                _profileCombo.SelectedItem = selected;
                _settings.SelectedProfileId = selected.Id;
            }
            _loading = false;
            SelectProfileFromCombo();
        }

        private void LoadProfileCombos()
        {
            FilterProfilesForMonitor();
        }

        private void SelectProfileFromCombo()
        {
            LayoutProfile p = _profileCombo.SelectedItem as LayoutProfile;
            if (p == null)
                return;

            _settings.SelectedProfileId = p.Id;
            _profileDescription.Text = p.Description;
            _preview.Profile = p;
            _preview.Invalidate();
            SaveSettings();

            if (_tabs != null && _tabs.TabPages.Count > 1)
            {
                ListBox profiles = _tabs.TabPages[1].Tag as ListBox;
                if (profiles != null)
                {
                    LayoutProfile matching = profiles.Items
                        .Cast<LayoutProfile>()
                        .FirstOrDefault(delegate(LayoutProfile x)
                        {
                            return x.Id == p.Id;
                        });

                    if (matching != null)
                        profiles.SelectedItem = matching;
                }
            }
        }

        private void PopulateProfilesList(ListBox list)
        {
            if (list == null)
                return;

            string previousId = null;
            LayoutProfile previous = list.SelectedItem as LayoutProfile;
            if (previous != null)
                previousId = previous.Id;

            list.BeginUpdate();
            list.Items.Clear();

            foreach (LayoutProfile p in _settings.Profiles
                .OrderBy(delegate(LayoutProfile x) { return x.MonitorWidth; })
                .ThenBy(delegate(LayoutProfile x) { return x.Name; }))
            {
                list.Items.Add(p);
            }

            list.EndUpdate();

            string targetId =
                previousId ??
                _settings.SelectedProfileId;

            LayoutProfile target = list.Items
                .Cast<LayoutProfile>()
                .FirstOrDefault(delegate(LayoutProfile p)
                {
                    return p.Id == targetId;
                });

            if (target != null)
                list.SelectedItem = target;
            else if (list.Items.Count > 0)
                list.SelectedIndex = 0;
        }

        private bool IsBuiltInProfile(LayoutProfile p)
        {
            return p.Id.StartsWith("5120-") || p.Id.StartsWith("3440-");
        }

        private void LoadProfileIntoEditor(LayoutProfile p)
        {
            if (p == null)
            {
                ClearProfileEditor();
                return;
            }

            SplitEditorPanel editor = FindControl<SplitEditorPanel>("layoutEditor");
            if (editor == null)
                return;

            _loading = true;

            editor.Profile = p;
            _profileName.Text = p.Name;
            _profileDescriptionEdit.Text = p.Description;

            _gameWidth.Value = Math.Max(
                _gameWidth.Minimum,
                Math.Min(_gameWidth.Maximum, p.GameWidth));

            _gameHeight.Value = Math.Max(
                _gameHeight.Minimum,
                Math.Min(_gameHeight.Maximum, p.GameHeight));

            RefreshZoneGrid();

            _loading = false;
            editor.Invalidate();
            ValidateEditor();
        }

        private void ClearProfileEditor()
        {
            SplitEditorPanel editor = FindControl<SplitEditorPanel>("layoutEditor");
            if (editor != null)
            {
                editor.Profile = null;
                editor.Invalidate();
            }

            if (_profileName != null)
                _profileName.Text = "";

            if (_profileDescriptionEdit != null)
                _profileDescriptionEdit.Text = "";

            if (_zoneGrid != null)
                _zoneGrid.Rows.Clear();

            if (_gameZoneCombo != null)
                _gameZoneCombo.Items.Clear();

            if (_validationLabel != null)
            {
                _validationLabel.Text = "Select a profile from the library.";
                _validationLabel.ForeColor = Color.FromArgb(181, 192, 204);
            }
        }

        private void SetEditorReadOnly(bool readOnly)
        {
            SplitEditorPanel editor = FindControl<SplitEditorPanel>("layoutEditor");
            if (editor != null)
                editor.Enabled = !readOnly;

            _profileName.ReadOnly = readOnly;
            _profileDescriptionEdit.ReadOnly = readOnly;
            _zoneGrid.ReadOnly = readOnly;
            _gameZoneCombo.Enabled = !readOnly;
            _gameWidth.Enabled = !readOnly;
            _gameHeight.Enabled = !readOnly;

            string[] editableButtons =
            {
                "addSplitButton",
                "removeSplitButton",
                "balanceSplitButton",
                "saveProfileButton"
            };

            foreach (string name in editableButtons)
            {
                Button button = FindControl<Button>(name);
                if (button != null)
                    button.Enabled = !readOnly;
            }

            Button reload = FindControl<Button>("reloadProfileButton");
            if (reload != null)
                reload.Enabled = true;

            if (readOnly && _validationLabel != null)
            {
                _validationLabel.Text =
                    "BUILT-IN PROFILE — preview is loaded. Use Duplicate to edit to create a custom copy.";
                _validationLabel.ForeColor = Color.FromArgb(215, 166, 59);
            }
            else
            {
                ValidateEditor();
            }
        }

        private void RefreshZoneGrid()
        {
            LayoutProfile p = CurrentEditorProfile;
            if (p == null)
                return;

            _loading = true;
            _zoneGrid.Rows.Clear();
            _gameZoneCombo.Items.Clear();

            for (int i = 0; i < p.Zones.Count; i++)
            {
                _zoneGrid.Rows.Add(p.Zones[i].Name, p.Zones[i].Width);
                _gameZoneCombo.Items.Add((i + 1) + " — " + p.Zones[i].Name);
            }

            if (_gameZoneCombo.Items.Count > 0)
                _gameZoneCombo.SelectedIndex = Math.Max(0, Math.Min(p.GameZoneIndex, p.Zones.Count - 1));
            _loading = false;
        }

        private void ApplyGridToEditor()
        {
            LayoutProfile p = CurrentEditorProfile;
            if (p == null)
                return;

            for (int i = 0; i < p.Zones.Count; i++)
            {
                object name = _zoneGrid.Rows[i].Cells[0].Value;
                object width = _zoneGrid.Rows[i].Cells[1].Value;

                p.Zones[i].Name = Convert.ToString(name);
                int parsed;
                if (int.TryParse(Convert.ToString(width), out parsed))
                    p.Zones[i].Width = Math.Max(200, parsed);
            }

            RefreshZoneGrid();
            FindControl<SplitEditorPanel>("layoutEditor").Invalidate();
            ValidateEditor();
        }

        private bool ValidateEditor()
        {
            LayoutProfile p = CurrentEditorProfile;
            if (p == null)
                return false;

            int total = p.Zones.Sum(delegate(SplitZone z) { return z.Width; });
            bool widthValid = total == p.MonitorWidth;
            bool gameZoneValid =
                p.GameZoneIndex >= 0 &&
                p.GameZoneIndex < p.Zones.Count &&
                (int)_gameWidth.Value <= p.Zones[p.GameZoneIndex].Width;
            bool heightValid = (int)_gameHeight.Value <= p.MonitorHeight;

            if (widthValid && gameZoneValid && heightValid)
            {
                _validationLabel.Text =
                    "VALID — zones total " + total + " px and the game rectangle fits.";
                _validationLabel.ForeColor = Color.FromArgb(91, 241, 187);
                return true;
            }

            List<string> issues = new List<string>();
            if (!widthValid)
                issues.Add("zone widths must total exactly " + p.MonitorWidth + " px");
            if (!gameZoneValid)
                issues.Add("game width exceeds the selected game zone");
            if (!heightValid)
                issues.Add("game height exceeds monitor height");

            _validationLabel.Text = "INVALID — " + string.Join("; ", issues.ToArray());
            _validationLabel.ForeColor = Color.FromArgb(255, 130, 140);
            return false;
        }

        private void SaveEditorProfile()
        {
            LayoutProfile p = CurrentEditorProfile;
            if (p == null)
            {
                Popup("No profile loaded", "Select a profile, then use Duplicate to edit if it is built-in.", true);
                return;
            }

            if (IsBuiltInProfile(p))
            {
                Popup(
                    "Built-in profile",
                    "Built-in profiles are read-only. Use Duplicate to edit first.",
                    true);
                return;
            }

            ApplyGridToEditor();

            p.Name = _profileName.Text.Trim();
            p.Description = _profileDescriptionEdit.Text.Trim();
            p.GameWidth = (int)_gameWidth.Value;
            p.GameHeight = (int)_gameHeight.Value;
            p.GameZoneIndex = Math.Max(0, _gameZoneCombo.SelectedIndex);

            if (string.IsNullOrWhiteSpace(p.Name))
            {
                Popup("Profile name required", "Enter a name before saving.", true);
                return;
            }

            if (!ValidateEditor())
            {
                Popup(
                    "Profile is not valid",
                    "The editor is locked to exact monitor width. Correct the highlighted issue before saving.",
                    true);
                return;
            }

            SaveSettings();
            LoadProfileCombos();

            ListBox profiles = _tabs.TabPages[1].Tag as ListBox;
            PopulateProfilesList(profiles);

            if (profiles != null)
            {
                LayoutProfile refreshed = profiles.Items
                    .Cast<LayoutProfile>()
                    .FirstOrDefault(delegate(LayoutProfile x)
                    {
                        return x.Id == p.Id;
                    });

                if (refreshed != null)
                    profiles.SelectedItem = refreshed;
            }

            Popup(
                "Profile saved",
                p.Name + " was saved and is available from Home and Profiles & Layouts.",
                false);
        }

        private void PopulateWindows()
        {
            IntPtr oldHandle = IntPtr.Zero;
            WindowInfo old = _windowCombo.SelectedItem as WindowInfo;
            if (old != null)
                oldHandle = old.Handle;

            List<WindowInfo> windows = Native.GetTopLevelWindows()
                .Where(delegate(WindowInfo w) { return w.Handle != Handle; })
                .OrderBy(delegate(WindowInfo w) { return w.ProcessName; })
                .ThenBy(delegate(WindowInfo w) { return w.Title; })
                .ToList();

            _windowCombo.Items.Clear();
            foreach (WindowInfo w in windows)
                _windowCombo.Items.Add(w);

            int selected = -1;
            for (int i = 0; i < _windowCombo.Items.Count; i++)
            {
                if (((WindowInfo)_windowCombo.Items[i]).Handle == oldHandle)
                {
                    selected = i;
                    break;
                }
            }

            if (selected >= 0)
                _windowCombo.SelectedIndex = selected;
            else
                _windowCombo.SelectedIndex = -1;
        }

        private void RefreshGameList()
        {
            _gamesList.Items.Clear();
            foreach (string game in _settings.GameProcesses.OrderBy(delegate(string x) { return x; }))
                _gamesList.Items.Add(game);
        }

        private void RememberSelectedGame()
        {
            WindowInfo w = _windowCombo.SelectedItem as WindowInfo;
            if (w == null)
            {
                Popup("No game selected", "Select an open game window first.", true);
                return;
            }

            string process = SettingsStore.NormalizeProcessName(w.ProcessName);
            if (!_settings.GameProcesses.Contains(process, StringComparer.OrdinalIgnoreCase))
                _settings.GameProcesses.Add(process);

            SaveSettings();
            RefreshGameList();
            Popup(
                "Game remembered",
                process + " will now be detected in Windowed or Borderless mode.",
                false);
        }

        private void ForgetSelectedGame()
        {
            string process = _gamesList.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(process))
            {
                Popup("No game selected", "Select a remembered game first.", true);
                return;
            }

            _settings.GameProcesses.RemoveAll(delegate(string x)
            {
                return string.Equals(x, process, StringComparison.OrdinalIgnoreCase);
            });
            SaveSettings();
            RefreshGameList();
            Popup("Game forgotten", process + " was removed from automatic detection.", false);
        }

        private bool IsRememberedGame(string processName)
        {
            string p = SettingsStore.NormalizeProcessName(processName);
            return _settings.GameProcesses.Any(delegate(string x)
            {
                return string.Equals(x, p, StringComparison.OrdinalIgnoreCase);
            });
        }

        private Rectangle GetGameRectangle(LayoutProfile profile)
        {
            Rectangle monitor = CurrentScreen.Bounds;
            int x = monitor.Left;

            for (int i = 0; i < profile.GameZoneIndex; i++)
                x += profile.Zones[i].Width;

            int zoneWidth = profile.Zones[profile.GameZoneIndex].Width;
            x += Math.Max(0, (zoneWidth - profile.GameWidth) / 2);

            int y = monitor.Top;
            if (profile.CentreGameVertically)
                y += Math.Max(0, (monitor.Height - profile.GameHeight) / 2);

            return new Rectangle(x, y, profile.GameWidth, profile.GameHeight);
        }

        private void ForceSelectedWindow()
        {
            ReadSettingsFromUi();

            WindowInfo selected = _windowCombo.SelectedItem as WindowInfo;
            if (selected == null)
            {
                _tabs.SelectedIndex = 2;
                Popup(
                    "No application selected",
                    "Open Game Detection, press Refresh, then explicitly select the exact game/application you want to modify.",
                    true);
                return;
            }

            WindowInfo live = ResolveBestWindow(selected);
            if (live == null)
            {
                PopulateWindows();
                Popup(
                    "Application window not found",
                    "The selected application recreated or closed its window. Refresh and select it again.",
                    true);
                return;
            }

            LayoutProfile profile = ActiveProfile;
            Rectangle target = GetGameRectangle(profile);

            string confirmation =
                "Application:\n" +
                live.ProcessName + " — " + live.Title + "\n\n" +
                "Profile:\n" +
                profile.Name + "\n\n" +
                "Target:\nX=" + target.X +
                ", Y=" + target.Y +
                ", " + target.Width + "×" + target.Height +
                "\n\nApply this profile once?";

            DialogResult answer = MessageBox.Show(
                this,
                confirmation,
                "Confirm UltraSplit action",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (answer != DialogResult.Yes)
            {
                Popup(
                    "Action cancelled",
                    "No changes were made to " + live.ProcessName + ".",
                    false);
                return;
            }

            CaptureAndApply(live.Handle, live.ProcessName, live.Title, true);
        }

        private WindowInfo ResolveBestWindow(WindowInfo selected)
        {
            List<WindowInfo> matches = Native.GetTopLevelWindows()
                .Where(delegate(WindowInfo w)
                {
                    return string.Equals(
                        SettingsStore.NormalizeProcessName(w.ProcessName),
                        SettingsStore.NormalizeProcessName(selected.ProcessName),
                        StringComparison.OrdinalIgnoreCase);
                })
                .OrderByDescending(delegate(WindowInfo w) { return Native.GetWindowArea(w.Handle); })
                .ToList();

            return matches.FirstOrDefault();
        }

        private void CaptureAndApply(IntPtr hwnd, string processName, string windowTitle, bool notify)
        {
            if (!Native.IsWindow(hwnd))
                return;

            CapturedWindow state;
            if (!_captured.TryGetValue(hwnd, out state))
            {
                Native.RECT original;
                if (!Native.GetWindowRect(hwnd, out original))
                {
                    Popup("Access denied", "Windows blocked access to the game window.", true);
                    return;
                }

                Native.WINDOWPLACEMENT placement = Native.GetPlacement(hwnd);
                int dwmPolicy;
                bool dwmKnown = Native.TryGetDwmNcPolicy(hwnd, out dwmPolicy);

                state = new CapturedWindow
                {
                    Handle = hwnd,
                    OriginalStyle = Native.GetStyle(hwnd),
                    OriginalExStyle = Native.GetExStyle(hwnd),
                    OriginalRect = original,
                    OriginalPlacement = placement,
                    OriginalMenu = Native.GetMenu(hwnd),
                    OriginalDwmNcPolicy = dwmPolicy,
                    OriginalDwmPolicyKnown = dwmKnown,
                    WasMaximized = Native.IsZoomed(hwnd),
                    ProcessName = processName,
                    WindowTitle = windowTitle
                };
                _captured[hwnd] = state;
            }

            state.TargetRect = GetGameRectangle(ActiveProfile);

            if (_settings.ManageTaskbar)
            {
                EnsureTaskbarOverlayMode();
                Native.SetTaskbarsVisible(false);
            }

            // One-shot application only. UltraSplit does not keep the game
            // pinned to the top or continuously rewrite its position afterward.
            ApplyWindow(state, true);

            Rectangle finalRect;
            bool exact = Native.TryGetWindowRectangle(hwnd, out finalRect) &&
                Math.Abs(finalRect.X - state.TargetRect.X) <= 2 &&
                Math.Abs(finalRect.Y - state.TargetRect.Y) <= 2 &&
                Math.Abs(finalRect.Width - state.TargetRect.Width) <= 2 &&
                Math.Abs(finalRect.Height - state.TargetRect.Height) <= 2;

            bool frameGone = !_settings.RemoveWindowFrame || !Native.HasVisibleWindowFrame(hwnd);

            if (notify)
            {
                Popup(
                    exact && frameGone ? "Profile applied" : "Profile partially blocked",
                    processName + " → " +
                    state.TargetRect.X + "," + state.TargetRect.Y + " " +
                    state.TargetRect.Width + "×" + state.TargetRect.Height +
                    ". Position: " + (exact ? "OK" : "blocked") +
                    "; window frame: " + (frameGone ? "removed" : "still present") + ".",
                    !(exact && frameGone));
            }
        }

        private void ApplyWindow(CapturedWindow state, bool activate)
        {
            Native.ShowWindow(state.Handle, Native.SW_RESTORE);

            if (_settings.RemoveWindowFrame)
                Native.MakeAggressivelyBorderless(state.Handle);

            Native.SetWindowPos(
                state.Handle,
                IntPtr.Zero,
                state.TargetRect.X,
                state.TargetRect.Y,
                state.TargetRect.Width,
                state.TargetRect.Height,
                Native.SWP_NOZORDER |
                Native.SWP_NOOWNERZORDER |
                Native.SWP_FRAMECHANGED |
                Native.SWP_SHOWWINDOW |
                Native.SWP_NOCOPYBITS |
                (activate ? 0u : Native.SWP_NOACTIVATE));

            Native.NotifyClientSize(
                state.Handle,
                state.TargetRect.Width,
                state.TargetRect.Height);

            if (activate)
            {
                Native.BringWindowToTop(state.Handle);
                Native.SetForegroundWindow(state.Handle);
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            CleanupClosedWindows();

            // Profiles are intentionally one-shot. Other applications may
            // overlap the game and UltraSplit does not continuously reapply size,
            // position or z-order.
            UpdateTaskbar();

            IntPtr hwnd = Native.GetForegroundWindow();
            if (hwnd == IntPtr.Zero || hwnd == Handle || _captured.ContainsKey(hwnd))
                return;

            WindowInfo info = Native.GetWindowInfo(hwnd);
            if (info == null)
                return;

            bool remembered =
                _settings.DetectRememberedGames &&
                IsRememberedGame(info.ProcessName);

            Screen targetScreen;
            bool fullscreen =
                _settings.DetectUnknownFullscreen &&
                Native.IsFullscreenLike(hwnd, out targetScreen);

            if (!remembered && !fullscreen)
            {
                _candidates.Remove(hwnd);
                _suppressed.Remove(hwnd);
                return;
            }

            DateTime first;
            if (!_candidates.TryGetValue(hwnd, out first))
            {
                _candidates[hwnd] = DateTime.UtcNow;
                return;
            }

            if ((DateTime.UtcNow - first).TotalMilliseconds < _settings.DetectionDelayMs)
                return;

            _candidates.Remove(hwnd);
            CaptureAndApply(hwnd, info.ProcessName, info.Title, true);
        }

        private void EnsureTaskbarOverlayMode()
        {
            if (_taskbarOverlayApplied)
                return;

            Native.SetTaskbarState(
                _originalTaskbarState |
                Native.ABS_AUTOHIDE |
                Native.ABS_ALWAYSONTOP);
            _taskbarOverlayApplied = true;
        }

        private void RestoreTaskbarMode()
        {
            if (_taskbarOverlayApplied)
            {
                Native.SetTaskbarState(_originalTaskbarState);
                _taskbarOverlayApplied = false;
            }
            Native.SetTaskbarsVisible(true);
        }

        private void UpdateTaskbar()
        {
            if (!_settings.ManageTaskbar || _captured.Count == 0)
            {
                if (_captured.Count == 0 && _taskbarOverlayApplied)
                    RestoreTaskbarMode();

                _lastTaskbarForeground = IntPtr.Zero;
                _lastTaskbarVisible = null;
                return;
            }

            EnsureTaskbarOverlayMode();

            IntPtr foreground = Native.GetForegroundWindow();
            bool visible = !_captured.ContainsKey(foreground);

            if (foreground != _lastTaskbarForeground ||
                !_lastTaskbarVisible.HasValue ||
                _lastTaskbarVisible.Value != visible)
            {
                Native.SetTaskbarsVisible(visible);
                _lastTaskbarForeground = foreground;
                _lastTaskbarVisible = visible;
            }
        }

        private void CleanupClosedWindows()
        {
            foreach (IntPtr hwnd in _captured.Keys.Where(delegate(IntPtr h) { return !Native.IsWindow(h); }).ToList())
                _captured.Remove(hwnd);

            if (_captured.Count == 0 && _taskbarOverlayApplied)
                RestoreTaskbarMode();
        }

        private void RestoreAll(bool notify)
        {
            List<string> restored = new List<string>();
            List<string> failed = new List<string>();

            foreach (CapturedWindow state in _captured.Values.ToList())
            {
                if (!Native.IsWindow(state.Handle))
                    continue;

                try
                {
                    // Restore menu and DWM non-client rendering before restoring
                    // styles so the Windows title bar can be rebuilt correctly.
                    Native.SetWindowMenu(state.Handle, state.OriginalMenu);

                    if (state.OriginalDwmPolicyKnown)
                        Native.SetDwmNcPolicy(state.Handle, state.OriginalDwmNcPolicy);
                    else
                        Native.ResetDwmNcPolicy(state.Handle);

                    Native.SetStyle(state.Handle, state.OriginalStyle);
                    Native.SetExStyle(state.Handle, state.OriginalExStyle);

                    Native.SetWindowPos(
                        state.Handle,
                        IntPtr.Zero,
                        state.OriginalRect.Left,
                        state.OriginalRect.Top,
                        state.OriginalRect.Right - state.OriginalRect.Left,
                        state.OriginalRect.Bottom - state.OriginalRect.Top,
                        Native.SWP_NOZORDER |
                        Native.SWP_NOOWNERZORDER |
                        Native.SWP_FRAMECHANGED |
                        Native.SWP_SHOWWINDOW);

                    Native.SetPlacement(state.Handle, state.OriginalPlacement);
                    Native.RefreshWindowFrame(state.Handle);

                    restored.Add(state.ProcessName + " — " + state.WindowTitle);
                }
                catch (Exception ex)
                {
                    failed.Add(state.ProcessName + ": " + ex.Message);
                }
            }

            _captured.Clear();
            RestoreTaskbarMode();
            _lastTaskbarForeground = IntPtr.Zero;
            _lastTaskbarVisible = null;

            if (notify)
            {
                if (failed.Count == 0)
                {
                    Popup(
                        "Restore complete",
                        restored.Count == 0
                            ? "No modified windows were being tracked."
                            : "Restored " + restored.Count +
                              " window(s), including title bars, menus, sizes, positions and taskbar state.",
                        false);
                }
                else
                {
                    Popup(
                        "Restore partially failed",
                        "Restored " + restored.Count +
                        " window(s). Failed: " + string.Join("; ", failed.ToArray()),
                        true);
                }
            }
        }

        private void ReadSettingsFromUi()
        {
            if (_rememberedGames != null)
                _settings.DetectRememberedGames = _rememberedGames.Checked;
            if (_unknownFullscreen != null)
                _settings.DetectUnknownFullscreen = _unknownFullscreen.Checked;
            if (_removeFrame != null)
                _settings.RemoveWindowFrame = _removeFrame.Checked;
            if (_enforce != null)
                _settings.EnforcePosition = false;
            if (_taskbar != null)
                _settings.ManageTaskbar = _taskbar.Checked;
            if (_notifications != null)
                _settings.Notifications = _notifications.Checked;
            if (_startup != null)
                _settings.RunOnWindowsStartup = _startup.Checked;
            if (_delay != null)
                _settings.DetectionDelayMs = (int)_delay.Value;
        }

        private void SaveSettings()
        {
            SettingsStore.Save(_settings);
            UpdateStartupRegistration();
        }

        private void UpdateStartupRegistration()
        {
            const string path = @"Software\Microsoft\Windows\CurrentVersion\Run";
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
                {
                    if (key == null)
                        return;

                    if (_settings.RunOnWindowsStartup)
                        key.SetValue("UltraSplit 2.2", "\"" + Application.ExecutablePath + "\"");
                    else
                        key.DeleteValue("UltraSplit 2.2", false);
                }
            }
            catch (Exception ex)
            {
                Popup("Startup setting failed", ex.Message, true);
            }
        }

        private void RegisterHotkeys()
        {
            Native.RegisterHotKey(
                Handle,
                HK_FORCE,
                Native.MOD_CONTROL | Native.MOD_ALT,
                (uint)Keys.Enter);

            Native.RegisterHotKey(
                Handle,
                HK_RESTORE,
                Native.MOD_CONTROL | Native.MOD_ALT | Native.MOD_SHIFT,
                (uint)Keys.R);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Native.WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == HK_FORCE)
                    ForceSelectedWindow();
                else if (id == HK_RESTORE)
                    RestoreAll(true);
            }

            base.WndProc(ref m);
        }

        private void Popup(string heading, string message, bool warning)
        {
            if (!_settings.Notifications && !warning)
                return;

            ToastForm toast = new ToastForm(_appIcon, heading, message, warning);
            toast.Show();
        }

        private void ShowMainWindow()
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
            BringToFront();
        }

        private void MainFormClosing(object sender, FormClosingEventArgs e)
        {
            // Closing the main window always exits UltraSplit and restores every
            // modified window. Minimise still sends the app to the tray.
            _reallyExit = true;
            _timer.Stop();
            RestoreAll(false);
            Native.UnregisterHotKey(Handle, HK_FORCE);
            Native.UnregisterHotKey(Handle, HK_RESTORE);
            _tray.Visible = false;
            _tray.Dispose();
        }
    }

    internal static class Native
    {
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        public const long WS_POPUP = unchecked((int)0x80000000);
        public const long WS_VISIBLE = 0x10000000L;
        public const long WS_CAPTION = 0x00C00000L;
        public const long WS_THICKFRAME = 0x00040000L;
        public const long WS_MINIMIZEBOX = 0x00020000L;
        public const long WS_MAXIMIZEBOX = 0x00010000L;
        public const long WS_SYSMENU = 0x00080000L;
        public const long WS_OVERLAPPEDWINDOW =
            WS_CAPTION | WS_THICKFRAME | WS_MINIMIZEBOX |
            WS_MAXIMIZEBOX | WS_SYSMENU;

        public const long WS_EX_DLGMODALFRAME = 0x00000001L;
        public const long WS_EX_WINDOWEDGE = 0x00000100L;
        public const long WS_EX_CLIENTEDGE = 0x00000200L;
        public const long WS_EX_STATICEDGE = 0x00020000L;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_NOACTIVATE = 0x08000000;

        public static readonly IntPtr HWND_TOP = IntPtr.Zero;

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_FRAMECHANGED = 0x0020;
        public const uint SWP_SHOWWINDOW = 0x0040;
        public const uint SWP_NOCOPYBITS = 0x0100;
        public const uint SWP_NOOWNERZORDER = 0x0200;

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public const int SW_MAXIMIZE = 3;
        public const int SW_RESTORE = 9;

        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const int WM_HOTKEY = 0x0312;

        public const uint ABM_GETSTATE = 0x00000004;
        public const uint ABM_SETSTATE = 0x0000000A;
        public const uint ABS_AUTOHIDE = 0x00000001;
        public const uint ABS_ALWAYSONTOP = 0x00000002;

        public const int WM_SIZE = 0x0005;
        public const int WM_EXITSIZEMOVE = 0x0232;
        public const uint RDW_INVALIDATE = 0x0001;
        public const uint RDW_UPDATENOW = 0x0100;
        public const uint RDW_FRAME = 0x0400;
        public const int SIZE_RESTORED = 0;
        public const int DWMWA_NCRENDERING_POLICY = 2;
        public const int DWMNCRP_USEWINDOWSTYLE = 0;
        public const int DWMNCRP_DISABLED = 1;
        public const int DWMNCRP_ENABLED = 2;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public int lParam;
        }

        private delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hwnd, StringBuilder text, int maxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint processId);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BringWindowToTop(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT rect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetClientRect(IntPtr hwnd, out RECT rect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsZoomed(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hwnd, int command);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(
            IntPtr hwnd, IntPtr insertAfter,
            int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(
            IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SetMenu(IntPtr hwnd, IntPtr menu);

        [DllImport("user32.dll")]
        public static extern IntPtr GetMenu(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(
            IntPtr hwnd,
            ref WINDOWPLACEMENT placement);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPlacement(
            IntPtr hwnd,
            ref WINDOWPLACEMENT placement);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DrawMenuBar(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RedrawWindow(
            IntPtr hwnd,
            IntPtr updateRect,
            IntPtr updateRegion,
            uint flags);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong32(IntPtr hwnd, int index);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hwnd, int index);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hwnd, int index, int value);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(
            IntPtr hwnd, int index, IntPtr value);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
            IntPtr hwnd, int id, uint modifiers, uint key);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hwnd, int id);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindowEx(
            IntPtr parent, IntPtr childAfter, string className, string windowName);

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        private static extern bool SetProcessDpiAwarenessContext(IntPtr value);

        [DllImport("shell32.dll")]
        private static extern uint SHAppBarMessage(uint message, ref APPBARDATA data);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(
            IntPtr hwnd, int attribute, ref int value, int valueSize);

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(
            IntPtr hwnd, int attribute, out int value, int valueSize);

        public static void EnableBestDpiAwareness()
        {
            try
            {
                if (SetProcessDpiAwarenessContext(new IntPtr(-4)))
                    return;
            }
            catch
            {
            }

            try { SetProcessDPIAware(); } catch { }
        }

        public static long GetStyle(IntPtr hwnd)
        {
            return GetWindowLongPtr(hwnd, GWL_STYLE).ToInt64();
        }

        public static long GetExStyle(IntPtr hwnd)
        {
            return GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
        }

        public static void SetStyle(IntPtr hwnd, long value)
        {
            SetWindowLongPtr(hwnd, GWL_STYLE, new IntPtr(value));
        }

        public static void SetExStyle(IntPtr hwnd, long value)
        {
            SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(value));
        }

        private static IntPtr GetWindowLongPtr(IntPtr hwnd, int index)
        {
            return IntPtr.Size == 8
                ? GetWindowLongPtr64(hwnd, index)
                : new IntPtr(GetWindowLong32(hwnd, index));
        }

        private static IntPtr SetWindowLongPtr(IntPtr hwnd, int index, IntPtr value)
        {
            return IntPtr.Size == 8
                ? SetWindowLongPtr64(hwnd, index, value)
                : new IntPtr(SetWindowLong32(hwnd, index, value.ToInt32()));
        }

        public static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            GetWindowPlacement(hwnd, ref placement);
            return placement;
        }

        public static void SetPlacement(IntPtr hwnd, WINDOWPLACEMENT placement)
        {
            placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            SetWindowPlacement(hwnd, ref placement);
        }

        public static bool TryGetDwmNcPolicy(IntPtr hwnd, out int policy)
        {
            policy = DWMNCRP_USEWINDOWSTYLE;
            try
            {
                return DwmGetWindowAttribute(
                    hwnd,
                    DWMWA_NCRENDERING_POLICY,
                    out policy,
                    Marshal.SizeOf(typeof(int))) == 0;
            }
            catch
            {
                return false;
            }
        }

        public static void SetDwmNcPolicy(IntPtr hwnd, int policy)
        {
            try
            {
                DwmSetWindowAttribute(
                    hwnd,
                    DWMWA_NCRENDERING_POLICY,
                    ref policy,
                    Marshal.SizeOf(typeof(int)));
            }
            catch { }
        }

        public static void ResetDwmNcPolicy(IntPtr hwnd)
        {
            int policy = DWMNCRP_USEWINDOWSTYLE;
            SetDwmNcPolicy(hwnd, policy);
        }

        public static void SetWindowMenu(IntPtr hwnd, IntPtr menu)
        {
            SetMenu(hwnd, menu);
            DrawMenuBar(hwnd);
        }

        public static void RefreshWindowFrame(IntPtr hwnd)
        {
            SetWindowPos(
                hwnd,
                IntPtr.Zero,
                0, 0, 0, 0,
                SWP_NOMOVE |
                SWP_NOSIZE |
                SWP_NOZORDER |
                SWP_NOOWNERZORDER |
                SWP_FRAMECHANGED |
                SWP_SHOWWINDOW);

            DrawMenuBar(hwnd);
            RedrawWindow(
                hwnd,
                IntPtr.Zero,
                IntPtr.Zero,
                RDW_INVALIDATE | RDW_UPDATENOW | RDW_FRAME);
        }

        public static void MakeAggressivelyBorderless(IntPtr hwnd)
        {
            ShowWindow(hwnd, SW_RESTORE);

            long style = GetStyle(hwnd);
            style &= ~WS_OVERLAPPEDWINDOW;
            style |= WS_POPUP | WS_VISIBLE;
            SetStyle(hwnd, style);

            long ex = GetExStyle(hwnd);
            ex &= ~(WS_EX_DLGMODALFRAME | WS_EX_WINDOWEDGE |
                    WS_EX_CLIENTEDGE | WS_EX_STATICEDGE);
            SetExStyle(hwnd, ex);

            SetMenu(hwnd, IntPtr.Zero);

            try
            {
                int disabled = DWMNCRP_DISABLED;
                SetDwmNcPolicy(hwnd, disabled);
            }
            catch { }

            SetWindowPos(
                hwnd, HWND_TOP, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE |
                SWP_NOOWNERZORDER | SWP_FRAMECHANGED | SWP_SHOWWINDOW);
        }

        public static bool HasVisibleWindowFrame(IntPtr hwnd)
        {
            long style = GetStyle(hwnd);
            long ex = GetExStyle(hwnd);
            return
                (style & WS_OVERLAPPEDWINDOW) != 0 ||
                (ex & (WS_EX_DLGMODALFRAME | WS_EX_WINDOWEDGE |
                       WS_EX_CLIENTEDGE | WS_EX_STATICEDGE)) != 0;
        }

        public static void NotifyClientSize(IntPtr hwnd, int width, int height)
        {
            int packed =
                ((Math.Min(65535, Math.Max(1, height))) << 16) |
                (Math.Min(65535, Math.Max(1, width)) & 0xFFFF);

            SendMessage(hwnd, WM_SIZE, new IntPtr(SIZE_RESTORED), new IntPtr(packed));
            SendMessage(hwnd, WM_EXITSIZEMOVE, IntPtr.Zero, IntPtr.Zero);
        }

        public static bool TryGetWindowRectangle(IntPtr hwnd, out Rectangle rectangle)
        {
            RECT rect;
            if (!GetWindowRect(hwnd, out rect))
            {
                rectangle = Rectangle.Empty;
                return false;
            }

            rectangle = Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
            return true;
        }

        public static long GetWindowArea(IntPtr hwnd)
        {
            Rectangle rect;
            return TryGetWindowRectangle(hwnd, out rect)
                ? Math.Max(0L, (long)rect.Width * rect.Height)
                : 0;
        }

        public static bool IsFullscreenLike(IntPtr hwnd, out Screen screen)
        {
            screen = Screen.FromHandle(hwnd);
            RECT rect;
            if (!GetWindowRect(hwnd, out rect))
                return false;

            Rectangle bounds = screen.Bounds;
            int tolerance = 12;
            bool covers =
                rect.Left <= bounds.Left + tolerance &&
                rect.Top <= bounds.Top + tolerance &&
                rect.Right >= bounds.Right - tolerance &&
                rect.Bottom >= bounds.Bottom - tolerance;

            long style = GetStyle(hwnd);
            return covers &&
                (((style & WS_CAPTION) == 0) || ((style & WS_POPUP) != 0));
        }

        public static List<WindowInfo> GetTopLevelWindows()
        {
            List<WindowInfo> result = new List<WindowInfo>();

            EnumWindows(delegate(IntPtr hwnd, IntPtr lParam)
            {
                if (!IsWindowVisible(hwnd))
                    return true;

                int length = GetWindowTextLength(hwnd);
                if (length <= 0)
                    return true;

                StringBuilder builder = new StringBuilder(length + 1);
                GetWindowText(hwnd, builder, builder.Capacity);
                string title = builder.ToString().Trim();
                if (title.Length == 0)
                    return true;

                uint pid;
                GetWindowThreadProcessId(hwnd, out pid);

                string process = "Unknown";
                try
                {
                    using (Process p = Process.GetProcessById((int)pid))
                        process = p.ProcessName;
                }
                catch { }

                result.Add(new WindowInfo
                {
                    Handle = hwnd,
                    ProcessName = process,
                    Title = title
                });
                return true;
            }, IntPtr.Zero);

            return result;
        }

        public static WindowInfo GetWindowInfo(IntPtr hwnd)
        {
            if (!IsWindow(hwnd))
                return null;

            int length = GetWindowTextLength(hwnd);
            StringBuilder builder = new StringBuilder(Math.Max(1, length + 1));
            GetWindowText(hwnd, builder, builder.Capacity);

            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);

            string process = "Unknown";
            try
            {
                using (Process p = Process.GetProcessById((int)pid))
                    process = p.ProcessName;
            }
            catch { }

            return new WindowInfo
            {
                Handle = hwnd,
                ProcessName = process,
                Title = builder.ToString()
            };
        }

        public static uint GetTaskbarState()
        {
            APPBARDATA data = new APPBARDATA();
            data.cbSize = Marshal.SizeOf(typeof(APPBARDATA));
            return SHAppBarMessage(ABM_GETSTATE, ref data);
        }

        public static void SetTaskbarState(uint state)
        {
            APPBARDATA data = new APPBARDATA();
            data.cbSize = Marshal.SizeOf(typeof(APPBARDATA));
            data.lParam = unchecked((int)state);
            SHAppBarMessage(ABM_SETSTATE, ref data);
        }

        public static void SetTaskbarsVisible(bool visible)
        {
            int command = visible ? SW_SHOW : SW_HIDE;

            IntPtr primary = FindWindow("Shell_TrayWnd", null);
            if (primary != IntPtr.Zero)
                ShowWindow(primary, command);

            IntPtr current = IntPtr.Zero;
            while (true)
            {
                current = FindWindowEx(
                    IntPtr.Zero, current,
                    "Shell_SecondaryTrayWnd", null);
                if (current == IntPtr.Zero)
                    break;
                ShowWindow(current, command);
            }
        }
    }
}
