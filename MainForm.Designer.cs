using System.Media;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RD_Tools
{
    public partial class MainForm
    {
        // Top Bar
        private Panel topBar;
        private Label lblAppTitle;
        private Label lblTheme;
        private ComboBox cboTheme;

        // Section 1: Text
        private GroupBox grpText;
        private TextBox txtInput;
        private CheckBox chkSeparateText2;
        private TextBox txtInput2;
        private Panel pnlText2Row;
        private CheckBox chkEnter;
        private CheckBox chkClipboard;

        // Section 2: Coordinates
        private GroupBox grpCoord;
        private CheckBox chkEnablePoint2;
        private TableLayoutPanel pnlPoint1;
        private Label lblP1Title;
        private Label lblX1;
        private NumericUpDown numX;
        private Label lblY1;
        private NumericUpDown numY;
        private Button btnPickCoord;
        private Button btnGetCurCoord;

        private TableLayoutPanel pnlPoint2Controls;
        private Label lblP2Title;
        private Label lblX2;
        private NumericUpDown numX2;
        private Label lblY2;
        private NumericUpDown numY2;
        private Button btnPickCoord2;
        private Button btnGetCurCoord2;
        private Label lblPointInterval;
        private NumericUpDown numPointInterval;
        private Label lblPointIntervalUnit;

        private Label lblDelay;
        private NumericUpDown numClickDelay;
        private Label lblDelayUnit;

        // Section 3: Repeat & Duration
        private GroupBox grpRepeat;
        private Label lblStartDelay;
        private RadioButton rbStartImmediate;
        private RadioButton rbStartDelay;
        private NumericUpDown numStartDelay;
        private Label lblStartDelayUnit;

        private Label lblInterval;
        private NumericUpDown numInterval;
        private ComboBox cboIntervalUnit;
        private Label lblIntervalUnit;

        private CheckBox chkEnableDuration;
        private NumericUpDown numDuration;
        private ComboBox cboDurationUnit;
        private Label lblDurationUnit;

        private CheckBox chkEnableCount;
        private NumericUpDown numCount;
        private Label lblCountUnit;
        private Label lblModeHint;

        // Section 4: Action & Shortcuts
        private GroupBox grpControl;
        private Button btnStart;
        private Button btnStop;
        private Label lblGuide;

        // Custom Hotkey Controls
        private TableLayoutPanel pnlHotkeys;
        private Label lblStartHk;
        private ComboBox cboStartMod;
        private Label lblPlus1;
        private ComboBox cboStartKey;

        private Label lblStopHk;
        private ComboBox cboStopMod;
        private Label lblPlus2;
        private ComboBox cboStopKey;

        private Label lblEmergencyHk;
        private ComboBox cboEmergencyMod;
        private Label lblPlus3;
        private ComboBox cboEmergencyKey;

        private Label lblHkTip;
        private Button btnResetHotkeys;


        // Section 5: Dashboard / Status
        private GroupBox grpStatus;
        private Panel cardRemainCount;
        private Label lblCardTitle1;
        private Label lblRemainCount;
        private Panel cardRemainTime;
        private Label lblCardTitle2;
        private Label lblRemainTime;
        private Panel cardCompleted;
        private Label lblCardTitle3;
        private Label lblCompletedCount;

        private Label lblEnergyTitle;
        private EnergyBar energyBar;
        private Label lblStatus;

        // Bottom Bar
        private Panel bottomBar;
        private Button btnTopMost;
        private Button btnToggleHotkeys;


        private void InitializeComponent()
        {
            Text = "RD 오토 입력 툴 (Auto Input Tool)";
            AutoScaleMode = AutoScaleMode.Dpi;
            Size = new Size(620, 1160);
            MinimumSize = new Size(560, 860);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Malgun Gothic", 9f, FontStyle.Regular);

            // ==========================================
            // Top Bar: Title & Theme Selector
            // ==========================================
            topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(12, 6, 12, 6)
            };

            lblAppTitle = new Label
            {
                Text = "⚡ RD 오토 입력 툴 (Auto Input Tool)",
                Dock = DockStyle.Left,
                AutoSize = true,
                Font = new Font("Malgun Gothic", 10.5f, FontStyle.Bold),
                Padding = new Padding(0, 5, 0, 0)
            };

            var themeFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0)
            };

            lblTheme = new Label
            {
                Text = "🎨 테마 :",
                AutoSize = true,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                Padding = new Padding(0, 6, 4, 0)
            };

            cboTheme = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 115,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular),
                Margin = new Padding(0, 2, 0, 0),
                Cursor = Cursors.Hand
            };
            cboTheme.Items.AddRange(new object[] { "아이보리 웜", "모던 다크", "쿨 블루", "세이지 그린" });
            cboTheme.SelectedIndex = 0;
            cboTheme.SelectedIndexChanged += (s, e) =>
            {
                string selected = cboTheme.SelectedItem?.ToString() ?? "아이보리 웜";
                _settings.SelectedTheme = selected;
                _settings.Save();
                ApplyTheme(selected);
            };

            themeFlow.Controls.Add(lblTheme);
            themeFlow.Controls.Add(cboTheme);

            topBar.Controls.Add(lblAppTitle);
            topBar.Controls.Add(themeFlow);

            // ==========================================
            // Bottom Panel: Side-by-side TopMost & Hotkeys Toggle Buttons
            // ==========================================
            bottomBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 48,
                Padding = new Padding(10, 6, 10, 8)
            };

            var bottomTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0)
            };
            bottomTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            bottomTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            btnTopMost = new Button
            {
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 4, 0)
            };
            btnTopMost.FlatAppearance.BorderSize = 0;
            btnTopMost.Click += (s, e) =>
            {
                bool currentActual = IsHandleCreated ? NativeMethods.IsWindowTopMost(Handle) : TopMost;
                bool newTop = !currentActual;
                ApplyTopMost(newTop);
                _settings.AlwaysOnTop = newTop;
                _settings.Save();
                SystemSounds.Beep.Play();
                lblStatus.Text = newTop
                    ? "📌 창 항상 위: [ON] 활성화됨 (다른 창 위에 고정)"
                    : "📌 창 항상 위: [OFF] 비활성화됨 (일반 창 모드)";
            };

            btnToggleHotkeys = new Button
            {
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Malgun Gothic", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(4, 0, 0, 0)
            };
            btnToggleHotkeys.FlatAppearance.BorderSize = 0;
            btnToggleHotkeys.Click += (s, e) =>
            {
                _hotkeysEnabled = !_hotkeysEnabled;
                _settings.EnableHotkeys = _hotkeysEnabled;
                _settings.Save();
                UpdateHotkeyButtonUI();
                SystemSounds.Beep.Play();
                lblStatus.Text = _hotkeysEnabled
                    ? $"단축키 활성화됨 ({_settings.HotkeyStart}: 시작, {_settings.HotkeyStop}: 정지, {_settings.HotkeyEmergency}: 긴급탈출)"
                    : $"단축키 비활성 중 (긴급탈출 단축키 {_settings.HotkeyEmergency}는 항상 동작)";
            };

            bottomTable.Controls.Add(btnTopMost, 0, 0);
            bottomTable.Controls.Add(btnToggleHotkeys, 1, 0);
            bottomBar.Controls.Add(bottomTable);

            // ==========================================
            // Root Scrollable Panel
            // ==========================================
            var rootPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(12, 10, 12, 8)
            };

            // Main Vertical Stack Layout
            var contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(0)
            };
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // ==========================================
            // 1. 원하는 문구 지정 그룹 (Text Input)
            // ==========================================
            grpText = new GroupBox
            {
                Text = " 1. 원하는 문구 지정 ",
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10, 6, 10, 8),
                Margin = new Padding(0, 0, 0, 8),
                Font = new Font("Malgun Gothic", 9.5f, FontStyle.Bold)
            };

            var textInner = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular)
            };
            textInner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            txtInput = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Height = 52,
                Dock = DockStyle.Top,
                Font = new Font("Malgun Gothic", 9.5f),
                Margin = new Padding(0, 2, 0, 4)
            };

            var optLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Margin = new Padding(0, 2, 0, 2)
            };

            chkEnter = new CheckBox
            {
                Text = "입력 후 Enter 전송",
                AutoSize = true,
                Checked = true,
                Margin = new Padding(0, 0, 14, 0),
                Cursor = Cursors.Hand
            };

            chkClipboard = new CheckBox
            {
                Text = "클립보드 붙여넣기 (한글/특수문자 완벽 호환)",
                AutoSize = true,
                Checked = true,
                Margin = new Padding(0),
                Cursor = Cursors.Hand
            };

            optLayout.Controls.Add(chkEnter);
            optLayout.Controls.Add(chkClipboard);

            // Separate Text 2 Row (visible when 2-point mode is enabled)
            pnlText2Row = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Visible = false,
                Margin = new Padding(0, 4, 0, 2)
            };
            chkSeparateText2 = new CheckBox
            {
                Text = "포인트 2에 별도 문구 사용 (미체크 시 위 문구와 동일하게 입력)",
                Dock = DockStyle.Top,
                AutoSize = true,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 0, 2)
            };
            txtInput2 = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Height = 44,
                Dock = DockStyle.Top,
                Font = new Font("Malgun Gothic", 9.5f),
                Enabled = false,
                Margin = new Padding(0, 2, 0, 4)
            };
            chkSeparateText2.CheckedChanged += (s, e) =>
            {
                txtInput2.Enabled = chkSeparateText2.Checked;
                txtInput2.BackColor = chkSeparateText2.Checked
                    ? (_currentTheme?.InputBg ?? Color.White)
                    : (_currentTheme?.IsDark == true ? Color.FromArgb(40, 42, 46) : Color.FromArgb(245, 242, 237));
            };
            pnlText2Row.Controls.Add(txtInput2);
            pnlText2Row.Controls.Add(chkSeparateText2);

            textInner.Controls.Add(txtInput);
            textInner.Controls.Add(optLayout);
            textInner.Controls.Add(pnlText2Row);
            grpText.Controls.Add(textInner);

            // ==========================================
            // 2. 좌표 지정 그룹 (Coordinate)
            // ==========================================
            grpCoord = new GroupBox
            {
                Text = " 2. 입력 포인트 (좌표) 지정 ",
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10, 6, 10, 8),
                Margin = new Padding(0, 0, 0, 8),
                Font = new Font("Malgun Gothic", 9.5f, FontStyle.Bold)
            };

            var coordInner = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 4,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular)
            };
            coordInner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // 0) Toggle Option: 2 Points Mode
            chkEnablePoint2 = new CheckBox
            {
                Text = "입력 포인트 2개로 확장 (1초 간격 순차 입력)",
                Dock = DockStyle.Top,
                AutoSize = true,
                Checked = false,
                Font = new Font("Malgun Gothic", 9.5f, FontStyle.Bold),
                Margin = new Padding(0, 2, 0, 8),
                Cursor = Cursors.Hand
            };

            // 1) Point 1 Panel (TableLayoutPanel with proper spacing)
            pnlPoint1 = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10, 8, 10, 10),
                Margin = new Padding(0, 0, 0, 8)
            };
            pnlPoint1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            lblP1Title = new Label
            {
                Text = "📍 포인트 1 (P1)",
                Dock = DockStyle.Top,
                AutoSize = true,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 6)
            };

            var coord1BtnRow = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 32,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 10)
            };
            coord1BtnRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            coord1BtnRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            btnPickCoord = new Button
            {
                Text = "🎯 P1 좌표 지정",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 3, 0)
            };
            btnPickCoord.FlatAppearance.BorderSize = 0;
            btnPickCoord.Click += (s, e) => PickCoordinate(numX, numY, "포인트 1");

            btnGetCurCoord = new Button
            {
                Text = "📍 P1 현재 위치 등록",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(3, 0, 0, 0)
            };
            btnGetCurCoord.FlatAppearance.BorderSize = 0;
            btnGetCurCoord.Click += (s, e) => CaptureCurrentMouse(numX, numY, "포인트 1");

            coord1BtnRow.Controls.Add(btnPickCoord, 0, 0);
            coord1BtnRow.Controls.Add(btnGetCurCoord, 1, 0);

            var coord1Inputs = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 2),
                Padding = new Padding(0, 2, 0, 0)
            };
            lblX1 = new Label { Text = "X:", AutoSize = true, Padding = new Padding(0, 4, 0, 0), Font = new Font("Malgun Gothic", 9f, FontStyle.Bold) };
            numX = new NumericUpDown { Minimum = -10000, Maximum = 10000, Value = 500, Width = 75, Margin = new Padding(2, 0, 14, 0) };
            lblY1 = new Label { Text = "Y:", AutoSize = true, Padding = new Padding(0, 4, 0, 0), Font = new Font("Malgun Gothic", 9f, FontStyle.Bold) };
            numY = new NumericUpDown { Minimum = -10000, Maximum = 10000, Value = 500, Width = 75, Margin = new Padding(2, 0, 0, 0) };
            coord1Inputs.Controls.AddRange(new Control[] { lblX1, numX, lblY1, numY });

            pnlPoint1.Controls.Add(lblP1Title, 0, 0);
            pnlPoint1.Controls.Add(coord1BtnRow, 0, 1);
            pnlPoint1.Controls.Add(coord1Inputs, 0, 2);

            // 2) Point 2 Panel (Collapsible TableLayoutPanel with proper spacing)
            pnlPoint2Controls = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10, 8, 10, 10),
                Margin = new Padding(0, 0, 0, 8),
                Visible = false
            };
            pnlPoint2Controls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            lblP2Title = new Label
            {
                Text = "📍 포인트 2 (P2) - 1초 간격 순차 입력",
                Dock = DockStyle.Top,
                AutoSize = true,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 6)
            };

            var coord2BtnRow = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 32,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 10)
            };
            coord2BtnRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            coord2BtnRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            btnPickCoord2 = new Button
            {
                Text = "🎯 P2 좌표 지정",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 3, 0)
            };
            btnPickCoord2.FlatAppearance.BorderSize = 0;
            btnPickCoord2.Click += (s, e) => PickCoordinate(numX2, numY2, "포인트 2");

            btnGetCurCoord2 = new Button
            {
                Text = "📍 P2 현재 위치 등록",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(3, 0, 0, 0)
            };
            btnGetCurCoord2.FlatAppearance.BorderSize = 0;
            btnGetCurCoord2.Click += (s, e) => CaptureCurrentMouse(numX2, numY2, "포인트 2");

            coord2BtnRow.Controls.Add(btnPickCoord2, 0, 0);
            coord2BtnRow.Controls.Add(btnGetCurCoord2, 1, 0);

            var coord2Inputs = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 2),
                Padding = new Padding(0, 2, 0, 0)
            };
            lblX2 = new Label { Text = "X:", AutoSize = true, Padding = new Padding(0, 4, 0, 0), Font = new Font("Malgun Gothic", 9f, FontStyle.Bold) };
            numX2 = new NumericUpDown { Minimum = -10000, Maximum = 10000, Value = 600, Width = 75, Margin = new Padding(2, 0, 14, 0) };
            lblY2 = new Label { Text = "Y:", AutoSize = true, Padding = new Padding(0, 4, 0, 0), Font = new Font("Malgun Gothic", 9f, FontStyle.Bold) };
            numY2 = new NumericUpDown { Minimum = -10000, Maximum = 10000, Value = 600, Width = 75, Margin = new Padding(2, 0, 0, 0) };
            coord2Inputs.Controls.AddRange(new Control[] { lblX2, numX2, lblY2, numY2 });

            pnlPoint2Controls.Controls.Add(lblP2Title, 0, 0);
            pnlPoint2Controls.Controls.Add(coord2BtnRow, 0, 1);
            pnlPoint2Controls.Controls.Add(coord2Inputs, 0, 2);

            // 3) Common Timing Options Row
            var timingRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 2, 0, 2)
            };

            lblPointInterval = new Label { Text = "P1→P2 간격 :", AutoSize = true, Padding = new Padding(0, 3, 0, 0), Font = new Font("Malgun Gothic", 9f, FontStyle.Bold), Visible = false };
            numPointInterval = new NumericUpDown { DecimalPlaces = 1, Increment = 0.1m, Minimum = 0.1m, Maximum = 3600m, Value = 1.0m, Width = 65, Margin = new Padding(2, 0, 4, 0), Visible = false };
            lblPointIntervalUnit = new Label { Text = "초", AutoSize = true, Padding = new Padding(0, 3, 14, 0), Visible = false };

            lblDelay = new Label { Text = "클릭 후 대기 :", AutoSize = true, Padding = new Padding(0, 3, 0, 0), Font = new Font("Malgun Gothic", 9f, FontStyle.Regular) };
            numClickDelay = new NumericUpDown { Minimum = 10, Maximum = 5000, Value = 100, Width = 65, Margin = new Padding(2, 0, 4, 0) };
            lblDelayUnit = new Label { Text = "ms", AutoSize = true, Padding = new Padding(0, 3, 0, 0) };

            timingRow.Controls.AddRange(new Control[] { lblPointInterval, numPointInterval, lblPointIntervalUnit, lblDelay, numClickDelay, lblDelayUnit });

            chkEnablePoint2.CheckedChanged += (s, e) =>
            {
                pnlPoint2Controls.Visible = chkEnablePoint2.Checked;
                pnlText2Row.Visible = chkEnablePoint2.Checked;
                lblPointInterval.Visible = chkEnablePoint2.Checked;
                numPointInterval.Visible = chkEnablePoint2.Checked;
                lblPointIntervalUnit.Visible = chkEnablePoint2.Checked;
            };

            coordInner.Controls.Add(chkEnablePoint2);
            coordInner.Controls.Add(pnlPoint1);
            coordInner.Controls.Add(pnlPoint2Controls);
            coordInner.Controls.Add(timingRow);
            grpCoord.Controls.Add(coordInner);

            // ==========================================
            // 3. 반복 간격 및 지속 시간 그룹 (Repeat & Duration)
            // ==========================================
            grpRepeat = new GroupBox
            {
                Text = " 3. 반복 간격 및 지속 시간 지정 ",
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10, 6, 10, 8),
                Margin = new Padding(0, 0, 0, 8),
                Font = new Font("Malgun Gothic", 9.5f, FontStyle.Bold)
            };

            var repeatInner = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 5,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular)
            };
            repeatInner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // 0) Start Delay Row
            var startDelayRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 1, 0, 4)
            };
            lblStartDelay = new Label
            {
                Text = "시작 대기 :",
                AutoSize = true,
                Padding = new Padding(0, 3, 0, 0),
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold)
            };
            rbStartImmediate = new RadioButton
            {
                Text = "바로 시작 (0초)",
                AutoSize = true,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular),
                Margin = new Padding(4, 1, 8, 0),
                Cursor = Cursors.Hand
            };
            rbStartDelay = new RadioButton
            {
                Text = "대기 후 시작 :",
                AutoSize = true,
                Checked = true,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular),
                Margin = new Padding(0, 1, 2, 0),
                Cursor = Cursors.Hand
            };
            numStartDelay = new NumericUpDown
            {
                DecimalPlaces = 1,
                Increment = 1m,
                Minimum = 0.5m,
                Maximum = 60m,
                Value = 2.0m,
                Width = 60,
                Margin = new Padding(2, 0, 4, 0)
            };
            lblStartDelayUnit = new Label
            {
                Text = "초 후 시작",
                AutoSize = true,
                Padding = new Padding(0, 3, 0, 0)
            };

            rbStartImmediate.CheckedChanged += (s, e) =>
            {
                numStartDelay.Enabled = !rbStartImmediate.Checked;
                lblStartDelayUnit.ForeColor = !rbStartImmediate.Checked
                    ? (_currentTheme?.IsDark == true ? Color.FromArgb(245, 180, 80) : Color.FromArgb(180, 100, 25))
                    : (_currentTheme?.TextMuted ?? Color.FromArgb(160, 150, 140));
            };
            rbStartDelay.CheckedChanged += (s, e) =>
            {
                numStartDelay.Enabled = rbStartDelay.Checked;
                lblStartDelayUnit.ForeColor = rbStartDelay.Checked
                    ? (_currentTheme?.IsDark == true ? Color.FromArgb(245, 180, 80) : Color.FromArgb(180, 100, 25))
                    : (_currentTheme?.TextMuted ?? Color.FromArgb(160, 150, 140));
            };
            startDelayRow.Controls.AddRange(new Control[] { lblStartDelay, rbStartImmediate, rbStartDelay, numStartDelay, lblStartDelayUnit });

            // 1) Interval Row (with Seconds / Minutes / Hours ComboBox)
            var intervalRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 1, 0, 3)
            };
            lblInterval = new Label { Text = "반복 간격 :", AutoSize = true, Padding = new Padding(0, 3, 0, 0), Font = new Font("Malgun Gothic", 9f, FontStyle.Bold) };
            numInterval = new NumericUpDown
            {
                DecimalPlaces = 2,
                Increment = 0.1m,
                Minimum = 0.05m,
                Maximum = 86400m,
                Value = 1.0m,
                Width = 72,
                Margin = new Padding(4, 0, 4, 0)
            };

            cboIntervalUnit = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 65,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular),
                Margin = new Padding(0, 0, 6, 0),
                Cursor = Cursors.Hand
            };
            cboIntervalUnit.Items.AddRange(new object[] { "초", "분", "시간" });
            cboIntervalUnit.SelectedIndex = 0;
            cboIntervalUnit.SelectedIndexChanged += (s, e) =>
            {
                string unit = cboIntervalUnit.SelectedItem?.ToString() ?? "초";
                if (unit == "분")
                {
                    numInterval.Minimum = 0.01m;
                    numInterval.Maximum = 1440m;
                    numInterval.Increment = 0.1m;
                }
                else if (unit == "시간")
                {
                    numInterval.Minimum = 0.01m;
                    numInterval.Maximum = 24m;
                    numInterval.Increment = 0.1m;
                }
                else
                {
                    numInterval.Minimum = 0.05m;
                    numInterval.Maximum = 86400m;
                    numInterval.Increment = 0.5m;
                }
                numInterval.Value = Math.Clamp(numInterval.Value, numInterval.Minimum, numInterval.Maximum);
                _settings.IntervalUnit = unit;
                UpdateModeHint();
            };

            lblIntervalUnit = new Label { Text = "마다 입력", AutoSize = true, Padding = new Padding(0, 3, 0, 0) };
            intervalRow.Controls.AddRange(new Control[] { lblInterval, numInterval, cboIntervalUnit, lblIntervalUnit });

            // 2) Duration Toggle Row (with Seconds / Minutes / Hours ComboBox)
            var durationRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 1, 0, 3)
            };
            chkEnableDuration = new CheckBox
            {
                Text = "지속 시간 지정 (ON/OFF) :",
                AutoSize = true,
                Checked = true,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                Margin = new Padding(0, 2, 4, 0),
                Cursor = Cursors.Hand
            };
            numDuration = new NumericUpDown
            {
                DecimalPlaces = 1,
                Increment = 1m,
                Minimum = 0.5m,
                Maximum = 86400m,
                Value = 10.0m,
                Width = 72,
                Margin = new Padding(2, 0, 4, 0)
            };

            cboDurationUnit = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 65,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular),
                Margin = new Padding(0, 0, 6, 0),
                Cursor = Cursors.Hand
            };
            cboDurationUnit.Items.AddRange(new object[] { "초", "분", "시간" });
            cboDurationUnit.SelectedIndex = 0;
            cboDurationUnit.SelectedIndexChanged += (s, e) =>
            {
                string unit = cboDurationUnit.SelectedItem?.ToString() ?? "초";
                if (unit == "시간")
                {
                    numDuration.Minimum = 0.01m;
                    numDuration.Maximum = 720m;
                    numDuration.Increment = 0.1m;
                    numDuration.DecimalPlaces = 2;
                }
                else if (unit == "분")
                {
                    numDuration.Minimum = 0.1m;
                    numDuration.Maximum = 1440m;
                    numDuration.Increment = 0.5m;
                    numDuration.DecimalPlaces = 1;
                }
                else
                {
                    numDuration.Minimum = 0.5m;
                    numDuration.Maximum = 86400m;
                    numDuration.Increment = 1m;
                    numDuration.DecimalPlaces = 1;
                }
                numDuration.Value = Math.Clamp(numDuration.Value, numDuration.Minimum, numDuration.Maximum);
                lblDurationUnit.Text = chkEnableDuration.Checked ? $"{unit} 동안 반복 후 자동 종료" : "(꺼짐: 시간 제한 없이 계속)";
                _settings.DurationUnit = unit;
                UpdateModeHint();
                RefreshDashboardInitialValues();
            };

            lblDurationUnit = new Label
            {
                Text = "초 동안 반복 후 자동 종료",
                AutoSize = true,
                Padding = new Padding(0, 3, 0, 0)
            };

            chkEnableDuration.CheckedChanged += (s, e) =>
            {
                string unit = cboDurationUnit.SelectedItem?.ToString() ?? "초";
                numDuration.Enabled = chkEnableDuration.Checked;
                cboDurationUnit.Enabled = chkEnableDuration.Checked;
                lblDurationUnit.Text = chkEnableDuration.Checked ? $"{unit} 동안 반복 후 자동 종료" : "(꺼짐: 시간 제한 없이 계속)";
                lblDurationUnit.ForeColor = chkEnableDuration.Checked
                    ? (_currentTheme?.IsDark == true ? Color.FromArgb(245, 180, 80) : Color.FromArgb(180, 100, 25))
                    : (_currentTheme?.TextMuted ?? Color.FromArgb(160, 150, 140));
                UpdateModeHint();
                RefreshDashboardInitialValues();
            };
            durationRow.Controls.AddRange(new Control[] { chkEnableDuration, numDuration, cboDurationUnit, lblDurationUnit });

            // 3) Count Toggle Row
            var countRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 1, 0, 3)
            };
            chkEnableCount = new CheckBox
            {
                Text = "반복 횟수 지정 (ON/OFF) :",
                AutoSize = true,
                Checked = false,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                Margin = new Padding(0, 2, 4, 0),
                Cursor = Cursors.Hand
            };
            numCount = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 1000000,
                Value = 10,
                Width = 75,
                Enabled = false,
                Margin = new Padding(2, 0, 6, 0)
            };
            lblCountUnit = new Label
            {
                Text = "(꺼짐: 횟수 제한 없음)",
                AutoSize = true,
                Padding = new Padding(0, 3, 0, 0)
            };

            chkEnableCount.CheckedChanged += (s, e) =>
            {
                numCount.Enabled = chkEnableCount.Checked;
                lblCountUnit.Text = chkEnableCount.Checked ? "회 입력 후 자동 종료" : "(꺼짐: 횟수 제한 없음)";
                lblCountUnit.ForeColor = chkEnableCount.Checked
                    ? (_currentTheme?.IsDark == true ? Color.FromArgb(245, 180, 80) : Color.FromArgb(180, 100, 25))
                    : (_currentTheme?.TextMuted ?? Color.FromArgb(160, 150, 140));
                UpdateModeHint();
                RefreshDashboardInitialValues();
            };
            countRow.Controls.AddRange(new Control[] { chkEnableCount, numCount, lblCountUnit });

            // 4) Live Mode Hint
            lblModeHint = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(2, 1, 2, 2),
                Font = new Font("Malgun Gothic", 8.5f, FontStyle.Bold)
            };

            repeatInner.Controls.Add(startDelayRow);
            repeatInner.Controls.Add(intervalRow);
            repeatInner.Controls.Add(durationRow);
            repeatInner.Controls.Add(countRow);
            repeatInner.Controls.Add(lblModeHint);
            grpRepeat.Controls.Add(repeatInner);

            // ==========================================
            // 4. 실행 및 단축키 설정 그룹 (Controls)
            // ==========================================
            grpControl = new GroupBox
            {
                Text = " 4. 실행 및 단축키 설정 ",
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10, 6, 10, 8),
                Margin = new Padding(0, 0, 0, 8),
                Font = new Font("Malgun Gothic", 9.5f, FontStyle.Bold)
            };

            var ctrlInner = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular)
            };
            ctrlInner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // Start / Stop Buttons Table (Side-by-Side 50% / 50%)
            var btnTable = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 42,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 1, 0, 6)
            };
            btnTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            btnTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            btnStart = new Button
            {
                Text = "▶ 시작",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(46, 139, 87),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Malgun Gothic", 11f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 4, 0)
            };
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Click += (s, e) => StartAutoInput();

            btnStop = new Button
            {
                Text = "⏹ 정지",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(205, 65, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Malgun Gothic", 11f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false,
                Margin = new Padding(4, 0, 0, 0)
            };
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.Click += (s, e) => StopAutoInput();

            btnTable.Controls.Add(btnStart, 0, 0);
            btnTable.Controls.Add(btnStop, 1, 0);

            // Warning / Guide Banner for Shortcuts
            lblGuide = new Label
            {
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(0, 0, 0, 6)
            };

            // Hotkey Customization Panel (Dropdowns only)
            pnlHotkeys = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 5,
                RowCount = 4,
                Margin = new Padding(0, 2, 0, 2),
                Padding = new Padding(2, 2, 2, 2)
            };
            pnlHotkeys.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135f)); // Title
            pnlHotkeys.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120f)); // Modifier Dropdown
            pnlHotkeys.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 24f));  // +
            pnlHotkeys.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120f)); // Key Dropdown
            pnlHotkeys.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));  // Reset button / spacing

            // Row 0: Start Hotkey
            lblStartHk = new Label { Text = "▶ 시작 단축키 :", AutoSize = true, Anchor = AnchorStyles.Left, Font = new Font("Malgun Gothic", 9f, FontStyle.Bold) };
            cboStartMod = CreateModCombo();
            lblPlus1 = new Label { Text = "+", AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Font = new Font("Malgun Gothic", 9f, FontStyle.Bold) };
            cboStartKey = CreateKeyCombo();

            pnlHotkeys.Controls.Add(lblStartHk, 0, 0);
            pnlHotkeys.Controls.Add(cboStartMod, 1, 0);
            pnlHotkeys.Controls.Add(lblPlus1, 2, 0);
            pnlHotkeys.Controls.Add(cboStartKey, 3, 0);

            // Row 1: Stop Hotkey
            lblStopHk = new Label { Text = "⏹ 정지 단축키 :", AutoSize = true, Anchor = AnchorStyles.Left, Font = new Font("Malgun Gothic", 9f, FontStyle.Bold) };
            cboStopMod = CreateModCombo();
            lblPlus2 = new Label { Text = "+", AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Font = new Font("Malgun Gothic", 9f, FontStyle.Bold) };
            cboStopKey = CreateKeyCombo();

            pnlHotkeys.Controls.Add(lblStopHk, 0, 1);
            pnlHotkeys.Controls.Add(cboStopMod, 1, 1);
            pnlHotkeys.Controls.Add(lblPlus2, 2, 1);
            pnlHotkeys.Controls.Add(cboStopKey, 3, 1);

            // Row 2: Emergency Hotkey (Default: Alt + F4)
            lblEmergencyHk = new Label { Text = "🚨 긴급탈출 단축키 :", AutoSize = true, Anchor = AnchorStyles.Left, Font = new Font("Malgun Gothic", 9f, FontStyle.Bold) };
            cboEmergencyMod = CreateModCombo();
            lblPlus3 = new Label { Text = "+", AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Font = new Font("Malgun Gothic", 9f, FontStyle.Bold) };
            cboEmergencyKey = CreateKeyCombo();

            pnlHotkeys.Controls.Add(lblEmergencyHk, 0, 2);
            pnlHotkeys.Controls.Add(cboEmergencyMod, 1, 2);
            pnlHotkeys.Controls.Add(lblPlus3, 2, 2);
            pnlHotkeys.Controls.Add(cboEmergencyKey, 3, 2);

            // Row 3: Bottom info & Reset button
            lblHkTip = new Label
            {
                Text = "※ 드롭다운 선택 시 즉시 저장 및 반영됩니다. (긴급탈출은 단축키 OFF 시에도 항상 동작)",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Malgun Gothic", 8.25f, FontStyle.Regular),
                Margin = new Padding(0, 4, 0, 0)
            };
            btnResetHotkeys = new Button
            {
                Text = "🔄 기본값",
                Dock = DockStyle.Fill,
                Height = 26,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Malgun Gothic", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(4, 2, 0, 0)
            };
            btnResetHotkeys.FlatAppearance.BorderSize = 0;
            btnResetHotkeys.Click += (s, e) => ResetHotkeysToDefault();

            pnlHotkeys.Controls.Add(lblHkTip, 0, 3);
            pnlHotkeys.SetColumnSpan(lblHkTip, 4);
            pnlHotkeys.Controls.Add(btnResetHotkeys, 4, 3);

            ctrlInner.Controls.Add(btnTable);
            ctrlInner.Controls.Add(lblGuide);
            ctrlInner.Controls.Add(pnlHotkeys);
            grpControl.Controls.Add(ctrlInner);

            // ==========================================
            // 5. 실시간 기동 상태 대시보드 (Dashboard)
            // ==========================================
            grpStatus = new GroupBox
            {
                Text = " 5. 실시간 기동 상태 대시보드 ",
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(12, 10, 12, 12),
                Margin = new Padding(0, 0, 0, 8),
                Font = new Font("Malgun Gothic", 9.5f, FontStyle.Bold)
            };

            var statusInner = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 4,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular)
            };
            statusInner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // Stat Cards Row: Height 86px
            var statsGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 86,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 8)
            };
            statsGrid.ColumnStyles.Clear();
            statsGrid.RowStyles.Clear();
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));
            statsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            cardRemainCount = CreateStatCard("🎯 남은 횟수", out lblRemainCount, out lblCardTitle1, "무제한", Color.FromArgb(180, 100, 25));
            cardRemainCount.Margin = new Padding(0, 0, 4, 0);

            cardRemainTime = CreateStatCard("⏳ 남은 시간", out lblRemainTime, out lblCardTitle2, "10.0초", Color.FromArgb(140, 70, 130));
            cardRemainTime.Margin = new Padding(2, 0, 2, 0);

            cardCompleted = CreateStatCard("📊 입력 완료", out lblCompletedCount, out lblCardTitle3, "0회", Color.FromArgb(46, 139, 87));
            cardCompleted.Margin = new Padding(4, 0, 0, 0);

            statsGrid.Controls.Add(cardRemainCount, 0, 0);
            statsGrid.Controls.Add(cardRemainTime, 1, 0);
            statsGrid.Controls.Add(cardCompleted, 2, 0);

            // Energy Bar Header
            lblEnergyTitle = new Label
            {
                Text = "⚡ 실시간 기동 에너지 바 (다음 입력 충전 게이지):",
                Dock = DockStyle.Top,
                AutoSize = true,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                Margin = new Padding(0, 2, 0, 4)
            };

            // Energy Bar Control
            energyBar = new EnergyBar
            {
                Dock = DockStyle.Top,
                Height = 34,
                Margin = new Padding(0, 0, 0, 8)
            };

            // Status Description Text
            lblStatus = new Label
            {
                Text = "상태: 준비 완료 (시작을 누르면 동작합니다)",
                Dock = DockStyle.Top,
                Height = 26,
                Font = new Font("Malgun Gothic", 9.5f, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(2, 0, 0, 0)
            };

            statusInner.Controls.Add(statsGrid);
            statusInner.Controls.Add(lblEnergyTitle);
            statusInner.Controls.Add(energyBar);
            statusInner.Controls.Add(lblStatus);
            grpStatus.Controls.Add(statusInner);

            // Add all groupboxes to content layout
            contentLayout.Controls.Add(grpText, 0, 0);
            contentLayout.Controls.Add(grpCoord, 0, 1);
            contentLayout.Controls.Add(grpRepeat, 0, 2);
            contentLayout.Controls.Add(grpControl, 0, 3);
            contentLayout.Controls.Add(grpStatus, 0, 4);

            rootPanel.Controls.Add(contentLayout);

            // Form Controls docking: rootPanel fills the middle area
            Controls.Add(rootPanel);
            Controls.Add(topBar);
            Controls.Add(bottomBar);
            rootPanel.BringToFront();

            FormClosing += MainForm_FormClosing;
        }

        private Panel CreateStatCard(string title, out Label valueLabel, out Label titleLabel, string initialValue, Color valueColor)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(1)
            };

            titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 26,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            valueLabel = new Label
            {
                Text = initialValue,
                Dock = DockStyle.Fill,
                Font = new Font("Malgun Gothic", 14f, FontStyle.Bold),
                ForeColor = valueColor,
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);
            titleLabel.BringToFront();
            return card;
        }

        private ComboBox CreateModCombo()
        {
            var cbo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 0, 2)
            };
            cbo.Items.AddRange(ModifierOptions);
            cbo.SelectedIndex = 0;
            cbo.SelectedIndexChanged += (s, e) => OnHotkeyControlChanged();
            return cbo;
        }

        private ComboBox CreateKeyCombo()
        {
            var cbo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill,
                Font = new Font("Malgun Gothic", 9f, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 0, 2)
            };
            cbo.Items.AddRange(KeyOptions);
            cbo.SelectedIndex = 0;
            cbo.SelectedIndexChanged += (s, e) => OnHotkeyControlChanged();
            return cbo;
        }
    }
}