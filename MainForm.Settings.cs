using System;
using System.Drawing;
using System.Windows.Forms;

namespace RD_Tools
{
    public partial class MainForm
    {
        private void ApplyTopMost(bool enable)
        {
            TopMost = enable;
            _settings.AlwaysOnTop = enable;
            if (IsHandleCreated)
            {
                NativeMethods.SetWindowTopMost(Handle, enable);
            }
            UpdateTopMostButtonUI();
        }

        private void UpdateTopMostButtonUI()
        {
            // 실제 OS 윈도우 스타일(WS_EX_TOPMOST)을 직접 확인하여 표기
            bool isActuallyTop = IsHandleCreated ? NativeMethods.IsWindowTopMost(Handle) : (_settings?.AlwaysOnTop ?? TopMost);

            if (IsHandleCreated && TopMost != isActuallyTop)
            {
                TopMost = isActuallyTop;
            }

            if (isActuallyTop)
            {
                btnTopMost.Text = "📌 창 항상 위 [ON] (클릭 시 OFF)";
                btnTopMost.BackColor = _currentTheme?.Accent ?? Color.FromArgb(197, 128, 32);
                btnTopMost.ForeColor = Color.White;
            }
            else
            {
                btnTopMost.Text = "📌 창 항상 위 [OFF] (클릭 시 ON)";
                btnTopMost.BackColor = _currentTheme?.IsDark == true ? Color.FromArgb(55, 58, 65) : Color.FromArgb(236, 232, 224);
                btnTopMost.ForeColor = _currentTheme?.TextPrimary ?? Color.FromArgb(85, 75, 65);
            }
        }

        private string FormatRemainingTime(long remainMs, string unit)
        {
            if (unit == "시간")
            {
                return $"{remainMs / 3600000.0:F2} 시간";
            }
            else if (unit == "분")
            {
                return $"{remainMs / 60000.0:F1} 분";
            }
            else
            {
                return $"{remainMs / 1000.0:F1} 초";
            }
        }

        private void RefreshDashboardInitialValues()
        {
            if (_isRunning) return;

            string durUnit = cboDurationUnit?.SelectedItem?.ToString() ?? "초";
            string durValueStr = durUnit == "시간" ? $"{numDuration.Value:F2}" : $"{numDuration.Value:F1}";
            lblCompletedCount.Text = "0 회";
            lblRemainCount.Text = chkEnableCount.Checked ? $"{numCount.Value} 회" : "무제한";
            lblRemainTime.Text = chkEnableDuration.Checked ? $"{durValueStr} {durUnit}" : "무제한";
        }

        private void UpdateModeHint()
        {
            string durUnit = cboDurationUnit?.SelectedItem?.ToString() ?? "초";
            string durValueStr = durUnit == "시간" ? $"{numDuration.Value:F2}" : $"{numDuration.Value:F1}";
            string intUnit = cboIntervalUnit?.SelectedItem?.ToString() ?? "초";

            if (chkEnableDuration.Checked && chkEnableCount.Checked)
            {
                lblModeHint.Text = $"💡 설정: {durValueStr}{durUnit} 경과 또는 {numCount.Value}회 입력 중 먼저 도달 시 자동 종료됩니다. (간격: {numInterval.Value}{intUnit})";
                lblModeHint.ForeColor = _currentTheme?.IsDark == true ? Color.FromArgb(245, 180, 80) : Color.FromArgb(160, 95, 25);
            }
            else if (chkEnableDuration.Checked)
            {
                lblModeHint.Text = $"💡 설정: {durValueStr}{durUnit} 동안 반복 실행 후 자동 종료됩니다. (간격: {numInterval.Value}{intUnit})";
                lblModeHint.ForeColor = _currentTheme?.IsDark == true ? Color.FromArgb(245, 180, 80) : Color.FromArgb(160, 95, 25);
            }
            else if (chkEnableCount.Checked)
            {
                lblModeHint.Text = $"💡 설정: {numCount.Value}회 입력 후 자동 종료됩니다. (간격: {numInterval.Value}{intUnit})";
                lblModeHint.ForeColor = _currentTheme?.IsDark == true ? Color.FromArgb(245, 180, 80) : Color.FromArgb(160, 95, 25);
            }
            else
            {
                lblModeHint.Text = _hotkeysEnabled
                    ? $"💡 설정: [무제한 반복] 모드 - {_settings.HotkeyStart}/{_settings.HotkeyStop}/{_settings.HotkeyEmergency} 키 또는 버튼으로 조작합니다. (간격: {numInterval.Value}{intUnit})"
                    : $"💡 설정: [무제한 반복] 모드 - 정지 버튼 또는 긴급탈출({_settings.HotkeyEmergency})로 중지합니다. (간격: {numInterval.Value}{intUnit})";
                lblModeHint.ForeColor = _currentTheme?.IsDark == true ? Color.FromArgb(245, 100, 90) : Color.FromArgb(195, 65, 45);
            }
        }

        private void ApplySettingsToUI()
        {
            txtInput.Text = _settings.TextToInput;
            chkSeparateText2.Checked = _settings.UseSeparateText2;
            txtInput2.Text = _settings.TextToInput2;
            txtInput2.Enabled = _settings.UseSeparateText2;

            // Point 1
            numX.Value = Math.Clamp(_settings.TargetX, numX.Minimum, numX.Maximum);
            numY.Value = Math.Clamp(_settings.TargetY, numY.Minimum, numY.Maximum);

            // Point 2
            chkEnablePoint2.Checked = _settings.EnablePoint2;
            pnlPoint2Controls.Visible = _settings.EnablePoint2;
            pnlText2Row.Visible = _settings.EnablePoint2;
            lblPointInterval.Visible = _settings.EnablePoint2;
            numPointInterval.Visible = _settings.EnablePoint2;
            lblPointIntervalUnit.Visible = _settings.EnablePoint2;

            numX2.Value = Math.Clamp(_settings.TargetX2, numX2.Minimum, numX2.Maximum);
            numY2.Value = Math.Clamp(_settings.TargetY2, numY2.Minimum, numY2.Maximum);
            numPointInterval.Value = Math.Clamp(_settings.PointIntervalSeconds, numPointInterval.Minimum, numPointInterval.Maximum);

            // Start Delay
            rbStartImmediate.Checked = _settings.ImmediateStart;
            rbStartDelay.Checked = !_settings.ImmediateStart;
            numStartDelay.Value = Math.Clamp(_settings.StartDelaySeconds, numStartDelay.Minimum, numStartDelay.Maximum);
            numStartDelay.Enabled = !_settings.ImmediateStart;

            // Repeat Interval & Unit
            if (!string.IsNullOrEmpty(_settings.IntervalUnit) && cboIntervalUnit.Items.Contains(_settings.IntervalUnit))
            {
                cboIntervalUnit.SelectedItem = _settings.IntervalUnit;
            }
            numInterval.Value = Math.Clamp(_settings.IntervalSeconds, numInterval.Minimum, numInterval.Maximum);

            // Duration & Unit
            if (!string.IsNullOrEmpty(_settings.DurationUnit) && cboDurationUnit.Items.Contains(_settings.DurationUnit))
            {
                cboDurationUnit.SelectedItem = _settings.DurationUnit;
            }
            string curDurUnit = cboDurationUnit.SelectedItem?.ToString() ?? "초";
            if (curDurUnit == "시간")
            {
                numDuration.Minimum = 0.01m;
                numDuration.Maximum = 720m;
                numDuration.Increment = 0.1m;
                numDuration.DecimalPlaces = 2;
            }
            else if (curDurUnit == "분")
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

            chkEnableDuration.Checked = _settings.EnableDuration;
            numDuration.Value = Math.Clamp(_settings.DurationSeconds, numDuration.Minimum, numDuration.Maximum);
            numDuration.Enabled = _settings.EnableDuration;
            cboDurationUnit.Enabled = _settings.EnableDuration;
            lblDurationUnit.Text = _settings.EnableDuration
                ? $"{cboDurationUnit.SelectedItem} 동안 반복 후 자동 종료"
                : "(꺼짐: 시간 제한 없이 계속)";

            // Count
            chkEnableCount.Checked = _settings.EnableCount;
            numCount.Value = Math.Clamp(_settings.RepeatCount, numCount.Minimum, numCount.Maximum);
            numCount.Enabled = _settings.EnableCount;
            lblCountUnit.Text = _settings.EnableCount ? "회 입력 후 자동 종료" : "(꺼짐: 횟수 제한 없음)";

            chkEnter.Checked = _settings.SendEnterAfterInput;
            chkClipboard.Checked = _settings.UseClipboardPaste;
            numClickDelay.Value = Math.Clamp(_settings.ClickDelayMs, numClickDelay.Minimum, numClickDelay.Maximum);

            // TopMost & Hotkeys
            TopMost = _settings.AlwaysOnTop;
            _hotkeysEnabled = _settings.EnableHotkeys;

            // Theme
            string targetTheme = _settings.SelectedTheme ?? "아이보리 웜";
            if (cboTheme.Items.Contains(targetTheme))
            {
                cboTheme.SelectedItem = targetTheme;
            }
            ApplyTheme(targetTheme);

            ApplyHotkeysToUI();
            UpdateTopMostButtonUI();
            UpdateHotkeyButtonUI();
            UpdateModeHint();
            RefreshDashboardInitialValues();
        }

        private void ApplyHotkeysToUI()
        {
            _isUpdatingHotkeyUI = true;
            cboStartMod.SelectedItem = ModToDisplay(_settings.HotkeyStart?.Modifier);
            if (!cboStartKey.Items.Contains(_settings.HotkeyStart?.Key))
                cboStartKey.Items.Add(_settings.HotkeyStart?.Key);
            cboStartKey.SelectedItem = _settings.HotkeyStart?.Key;

            cboStopMod.SelectedItem = ModToDisplay(_settings.HotkeyStop?.Modifier);
            if (!cboStopKey.Items.Contains(_settings.HotkeyStop?.Key))
                cboStopKey.Items.Add(_settings.HotkeyStop?.Key);
            cboStopKey.SelectedItem = _settings.HotkeyStop?.Key;

            cboEmergencyMod.SelectedItem = ModToDisplay(_settings.HotkeyEmergency?.Modifier);
            if (!cboEmergencyKey.Items.Contains(_settings.HotkeyEmergency?.Key))
                cboEmergencyKey.Items.Add(_settings.HotkeyEmergency?.Key);
            cboEmergencyKey.SelectedItem = _settings.HotkeyEmergency?.Key;
            _isUpdatingHotkeyUI = false;
        }

        private void SaveCurrentSettings()
        {
            _settings.TextToInput = txtInput.Text;
            _settings.UseSeparateText2 = chkSeparateText2.Checked;
            _settings.TextToInput2 = txtInput2.Text;

            _settings.TargetX = (int)numX.Value;
            _settings.TargetY = (int)numY.Value;

            _settings.EnablePoint2 = chkEnablePoint2.Checked;
            _settings.TargetX2 = (int)numX2.Value;
            _settings.TargetY2 = (int)numY2.Value;
            _settings.PointIntervalSeconds = numPointInterval.Value;

            _settings.ImmediateStart = rbStartImmediate.Checked;
            _settings.StartDelaySeconds = numStartDelay.Value;

            _settings.IntervalSeconds = numInterval.Value;
            _settings.IntervalUnit = cboIntervalUnit.SelectedItem?.ToString() ?? "초";
            _settings.EnableDuration = chkEnableDuration.Checked;
            _settings.DurationSeconds = numDuration.Value;
            _settings.DurationUnit = cboDurationUnit.SelectedItem?.ToString() ?? "초";
            _settings.EnableCount = chkEnableCount.Checked;
            _settings.RepeatCount = (int)numCount.Value;
            _settings.SendEnterAfterInput = chkEnter.Checked;
            _settings.UseClipboardPaste = chkClipboard.Checked;
            _settings.ClickDelayMs = (int)numClickDelay.Value;
            _settings.AlwaysOnTop = IsHandleCreated ? NativeMethods.IsWindowTopMost(Handle) : _settings.AlwaysOnTop;
            _settings.EnableHotkeys = _hotkeysEnabled;
            _settings.SelectedTheme = cboTheme.SelectedItem?.ToString() ?? "아이보리 웜";

            _settings.HotkeyStart = new HotkeyConfig(ModFromDisplay(cboStartMod.SelectedItem?.ToString()), cboStartKey.SelectedItem?.ToString() ?? "F1");
            _settings.HotkeyStop = new HotkeyConfig(ModFromDisplay(cboStopMod.SelectedItem?.ToString()), cboStopKey.SelectedItem?.ToString() ?? "F2");
            _settings.HotkeyEmergency = new HotkeyConfig(ModFromDisplay(cboEmergencyMod.SelectedItem?.ToString()), cboEmergencyKey.SelectedItem?.ToString() ?? "F4");

            _settings.Save();
        }
    }
}