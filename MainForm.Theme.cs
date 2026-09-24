using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RD_Tools
{
    public partial class MainForm
    {
        // 1. Theme Configuration
        // ==========================================
        private class ThemeColors
        {
            public Color FormBg { get; set; }
            public Color BarBg { get; set; }
            public Color GroupBg { get; set; }
            public Color GroupBorder { get; set; }
            public Color TextPrimary { get; set; }
            public Color TextSecondary { get; set; }
            public Color TextMuted { get; set; }
            public Color Accent { get; set; }
            public Color InputBg { get; set; }
            public Color InputFg { get; set; }
            public Color PnlP1Bg { get; set; }
            public Color PnlP2Bg { get; set; }
            public Color CardHeaderBg { get; set; }
            public Color CardBg { get; set; }
            public Color StatusBadgeBg { get; set; }
            public Color StatusBadgeFg { get; set; }
            public Color BannerBg { get; set; }
            public Color BannerBorder { get; set; }
            public Color BannerFg { get; set; }
            public bool IsDark { get; set; }
        }

        private static readonly Dictionary<string, ThemeColors> Themes = new Dictionary<string, ThemeColors>
        {
            ["아이보리 웜"] = new ThemeColors
            {
                FormBg = Color.FromArgb(250, 248, 243),
                BarBg = Color.FromArgb(242, 238, 231),
                GroupBg = Color.FromArgb(255, 254, 250),
                GroupBorder = Color.FromArgb(228, 222, 212),
                TextPrimary = Color.FromArgb(52, 44, 38),
                TextSecondary = Color.FromArgb(85, 75, 65),
                TextMuted = Color.FromArgb(120, 110, 100),
                Accent = Color.FromArgb(197, 128, 32),
                InputBg = Color.White,
                InputFg = Color.FromArgb(40, 35, 30),
                PnlP1Bg = Color.FromArgb(248, 245, 238),
                PnlP2Bg = Color.FromArgb(242, 246, 250),
                CardHeaderBg = Color.FromArgb(244, 240, 232),
                CardBg = Color.FromArgb(255, 254, 250),
                StatusBadgeBg = Color.FromArgb(242, 238, 230),
                StatusBadgeFg = Color.FromArgb(110, 100, 90),
                BannerBg = Color.FromArgb(254, 250, 240),
                BannerBorder = Color.FromArgb(235, 210, 175),
                BannerFg = Color.FromArgb(165, 80, 15),
                IsDark = false
            },
            ["모던 다크"] = new ThemeColors
            {
                FormBg = Color.FromArgb(28, 30, 33),
                BarBg = Color.FromArgb(20, 22, 25),
                GroupBg = Color.FromArgb(36, 38, 42),
                GroupBorder = Color.FromArgb(55, 58, 64),
                TextPrimary = Color.FromArgb(235, 238, 242),
                TextSecondary = Color.FromArgb(195, 200, 208),
                TextMuted = Color.FromArgb(150, 155, 165),
                Accent = Color.FromArgb(245, 158, 11),
                InputBg = Color.FromArgb(48, 51, 57),
                InputFg = Color.FromArgb(245, 245, 245),
                PnlP1Bg = Color.FromArgb(32, 34, 38),
                PnlP2Bg = Color.FromArgb(26, 38, 52),
                CardHeaderBg = Color.FromArgb(44, 46, 52),
                CardBg = Color.FromArgb(34, 36, 40),
                StatusBadgeBg = Color.FromArgb(44, 47, 53),
                StatusBadgeFg = Color.FromArgb(180, 185, 195),
                BannerBg = Color.FromArgb(48, 40, 30),
                BannerBorder = Color.FromArgb(120, 85, 30),
                BannerFg = Color.FromArgb(245, 185, 80),
                IsDark = true
            },
            ["쿨 블루"] = new ThemeColors
            {
                FormBg = Color.FromArgb(240, 245, 252),
                BarBg = Color.FromArgb(226, 234, 246),
                GroupBg = Color.FromArgb(252, 254, 255),
                GroupBorder = Color.FromArgb(210, 224, 242),
                TextPrimary = Color.FromArgb(24, 43, 73),
                TextSecondary = Color.FromArgb(45, 70, 105),
                TextMuted = Color.FromArgb(100, 120, 150),
                Accent = Color.FromArgb(28, 105, 205),
                InputBg = Color.White,
                InputFg = Color.FromArgb(20, 35, 60),
                PnlP1Bg = Color.FromArgb(235, 242, 252),
                PnlP2Bg = Color.FromArgb(228, 238, 250),
                CardHeaderBg = Color.FromArgb(230, 238, 248),
                CardBg = Color.FromArgb(255, 255, 255),
                StatusBadgeBg = Color.FromArgb(232, 240, 250),
                StatusBadgeFg = Color.FromArgb(60, 90, 130),
                BannerBg = Color.FromArgb(238, 244, 255),
                BannerBorder = Color.FromArgb(180, 205, 240),
                BannerFg = Color.FromArgb(25, 85, 175),
                IsDark = false
            },
            ["세이지 그린"] = new ThemeColors
            {
                FormBg = Color.FromArgb(242, 247, 243),
                BarBg = Color.FromArgb(228, 237, 230),
                GroupBg = Color.FromArgb(252, 255, 253),
                GroupBorder = Color.FromArgb(212, 228, 216),
                TextPrimary = Color.FromArgb(30, 52, 36),
                TextSecondary = Color.FromArgb(50, 78, 58),
                TextMuted = Color.FromArgb(100, 130, 110),
                Accent = Color.FromArgb(42, 130, 75),
                InputBg = Color.White,
                InputFg = Color.FromArgb(25, 45, 30),
                PnlP1Bg = Color.FromArgb(236, 245, 238),
                PnlP2Bg = Color.FromArgb(230, 243, 241),
                CardHeaderBg = Color.FromArgb(232, 242, 234),
                CardBg = Color.FromArgb(255, 255, 255),
                StatusBadgeBg = Color.FromArgb(234, 243, 236),
                StatusBadgeFg = Color.FromArgb(65, 105, 75),
                BannerBg = Color.FromArgb(240, 248, 242),
                BannerBorder = Color.FromArgb(185, 220, 195),
                BannerFg = Color.FromArgb(35, 115, 65),
                IsDark = false
            }
        };

        private ThemeColors _currentTheme;

        private void ApplyTheme(string themeName)
        {
            if (string.IsNullOrEmpty(themeName) || !Themes.TryGetValue(themeName, out var theme))
            {
                theme = Themes["아이보리 웜"];
                themeName = "아이보리 웜";
            }
            _currentTheme = theme;

            BackColor = theme.FormBg;

            // TopBar
            topBar.BackColor = theme.BarBg;
            lblAppTitle.ForeColor = theme.TextPrimary;
            lblTheme.ForeColor = theme.TextSecondary;
            cboTheme.BackColor = theme.InputBg;
            cboTheme.ForeColor = theme.InputFg;

            // BottomBar
            bottomBar.BackColor = theme.BarBg;

            // GroupBoxes
            GroupBox[] groups = { grpText, grpCoord, grpRepeat, grpControl, grpStatus };
            foreach (var grp in groups)
            {
                grp.BackColor = theme.GroupBg;
                grp.ForeColor = theme.TextPrimary;
            }

            // Section 1 Controls
            txtInput.BackColor = theme.InputBg;
            txtInput.ForeColor = theme.InputFg;
            chkEnter.ForeColor = theme.TextPrimary;
            chkClipboard.ForeColor = theme.TextPrimary;
            chkSeparateText2.ForeColor = theme.TextSecondary;
            txtInput2.BackColor = chkSeparateText2.Checked
                ? theme.InputBg
                : (theme.IsDark ? Color.FromArgb(40, 42, 46) : Color.FromArgb(245, 242, 237));
            txtInput2.ForeColor = theme.InputFg;

            // Section 2 Controls
            chkEnablePoint2.ForeColor = theme.Accent;
            pnlPoint1.BackColor = theme.PnlP1Bg;
            lblP1Title.ForeColor = theme.TextSecondary;
            lblX1.ForeColor = theme.TextPrimary;
            lblY1.ForeColor = theme.TextPrimary;
            numX.BackColor = theme.InputBg;
            numX.ForeColor = theme.InputFg;
            numY.BackColor = theme.InputBg;
            numY.ForeColor = theme.InputFg;
            btnPickCoord.BackColor = theme.Accent;
            btnGetCurCoord.BackColor = theme.IsDark ? Color.FromArgb(80, 84, 92) : Color.FromArgb(112, 102, 92);

            pnlPoint2Controls.BackColor = theme.PnlP2Bg;
            lblP2Title.ForeColor = theme.IsDark ? Color.FromArgb(120, 180, 240) : Color.FromArgb(30, 80, 130);
            lblX2.ForeColor = theme.TextPrimary;
            lblY2.ForeColor = theme.TextPrimary;
            numX2.BackColor = theme.InputBg;
            numX2.ForeColor = theme.InputFg;
            numY2.BackColor = theme.InputBg;
            numY2.ForeColor = theme.InputFg;
            btnPickCoord2.BackColor = theme.IsDark ? Color.FromArgb(40, 110, 180) : Color.FromArgb(45, 110, 175);
            btnGetCurCoord2.BackColor = theme.IsDark ? Color.FromArgb(70, 95, 120) : Color.FromArgb(100, 120, 140);

            lblPointInterval.ForeColor = theme.TextPrimary;
            numPointInterval.BackColor = theme.InputBg;
            numPointInterval.ForeColor = theme.InputFg;
            lblPointIntervalUnit.ForeColor = theme.TextMuted;

            lblDelay.ForeColor = theme.TextPrimary;
            numClickDelay.BackColor = theme.InputBg;
            numClickDelay.ForeColor = theme.InputFg;
            lblDelayUnit.ForeColor = theme.TextMuted;

            // Section 3 Controls
            lblStartDelay.ForeColor = theme.TextPrimary;
            rbStartImmediate.ForeColor = theme.TextPrimary;
            rbStartDelay.ForeColor = theme.TextPrimary;
            numStartDelay.BackColor = theme.InputBg;
            numStartDelay.ForeColor = theme.InputFg;
            lblStartDelayUnit.ForeColor = !rbStartImmediate.Checked
                ? (theme.IsDark ? Color.FromArgb(245, 180, 80) : Color.FromArgb(180, 100, 25))
                : theme.TextMuted;

            lblInterval.ForeColor = theme.TextPrimary;
            numInterval.BackColor = theme.InputBg;
            numInterval.ForeColor = theme.InputFg;
            cboIntervalUnit.BackColor = theme.InputBg;
            cboIntervalUnit.ForeColor = theme.InputFg;
            lblIntervalUnit.ForeColor = theme.TextMuted;

            chkEnableDuration.ForeColor = theme.TextPrimary;
            numDuration.BackColor = theme.InputBg;
            numDuration.ForeColor = theme.InputFg;
            cboDurationUnit.BackColor = theme.InputBg;
            cboDurationUnit.ForeColor = theme.InputFg;
            lblDurationUnit.ForeColor = chkEnableDuration.Checked
                ? (theme.IsDark ? Color.FromArgb(245, 180, 80) : Color.FromArgb(180, 100, 25))
                : theme.TextMuted;

            chkEnableCount.ForeColor = theme.TextPrimary;
            numCount.BackColor = theme.InputBg;
            numCount.ForeColor = theme.InputFg;
            lblCountUnit.ForeColor = chkEnableCount.Checked
                ? (theme.IsDark ? Color.FromArgb(245, 180, 80) : Color.FromArgb(180, 100, 25))
                : theme.TextMuted;

            // Section 4 Controls
            ApplyThemeToGuide();
            if (pnlHotkeys != null)
            {
                lblStartHk.ForeColor = theme.TextPrimary;
                lblStopHk.ForeColor = theme.TextPrimary;
                lblEmergencyHk.ForeColor = theme.IsDark ? Color.FromArgb(245, 140, 140) : Color.FromArgb(195, 50, 50);
                lblPlus1.ForeColor = theme.TextMuted;
                lblPlus2.ForeColor = theme.TextMuted;
                lblPlus3.ForeColor = theme.TextMuted;
                lblHkTip.ForeColor = theme.TextMuted;

                cboStartMod.BackColor = theme.InputBg; cboStartMod.ForeColor = theme.InputFg;
                cboStartKey.BackColor = theme.InputBg; cboStartKey.ForeColor = theme.InputFg;
                cboStopMod.BackColor = theme.InputBg; cboStopMod.ForeColor = theme.InputFg;
                cboStopKey.BackColor = theme.InputBg; cboStopKey.ForeColor = theme.InputFg;
                cboEmergencyMod.BackColor = theme.InputBg; cboEmergencyMod.ForeColor = theme.InputFg;
                cboEmergencyKey.BackColor = theme.InputBg; cboEmergencyKey.ForeColor = theme.InputFg;

                btnResetHotkeys.BackColor = theme.IsDark ? Color.FromArgb(50, 54, 60) : Color.FromArgb(230, 226, 218);
                btnResetHotkeys.ForeColor = theme.TextSecondary;
            }

            // Section 5 Controls
            lblEnergyTitle.ForeColor = theme.TextPrimary;
            lblStatus.ForeColor = theme.TextSecondary;

            // Stat Cards
            ApplyCardTheme(cardRemainCount, lblCardTitle1, lblRemainCount, theme);
            ApplyCardTheme(cardRemainTime, lblCardTitle2, lblRemainTime, theme);
            ApplyCardTheme(cardCompleted, lblCardTitle3, lblCompletedCount, theme);

            UpdateTopMostButtonUI();
            UpdateHotkeyButtonUI();
            UpdateModeHint();
        }

        private void ApplyCardTheme(Panel card, Label titleLabel, Label valueLabel, ThemeColors theme)
        {
            card.BackColor = theme.CardBg;
            titleLabel.BackColor = theme.CardHeaderBg;
            titleLabel.ForeColor = theme.TextMuted;
            valueLabel.BackColor = theme.CardBg;
        }

        private void ApplyThemeToGuide()
        {
            if (_currentTheme == null) return;

            if (!_hotkeysEnabled)
            {
                lblGuide.Text = $"⚠️ 단축키 비활성 상태  |  [긴급탈출 정지: {_settings.HotkeyEmergency}]";
                lblGuide.BackColor = _currentTheme.BannerBg;
                lblGuide.ForeColor = _currentTheme.IsDark ? Color.FromArgb(245, 170, 70) : Color.FromArgb(190, 80, 20);
            }
            else
            {
                lblGuide.Text = $"🚨 [{_settings.HotkeyStart}] 시작  |  [{_settings.HotkeyStop}] 정지  (긴급탈출: {_settings.HotkeyEmergency})";
                lblGuide.BackColor = _currentTheme.BannerBg;
                lblGuide.ForeColor = _currentTheme.BannerFg;
            }
        }
    }
}