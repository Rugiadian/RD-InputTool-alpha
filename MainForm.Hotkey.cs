using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace RD_Tools
{
    public partial class MainForm
    {
        private bool _isUpdatingHotkeyUI = false;

        private static readonly string[] ModifierOptions = new[]
        {
            "없음", "Alt", "Ctrl", "Shift", "Ctrl + Alt", "Ctrl + Shift", "Alt + Shift"
        };

        private static readonly string[] KeyOptions = new[]
        {
            "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12",
            "Pause", "Escape", "Space", "Tab", "Insert", "Delete", "Home", "End", "PageUp", "PageDown",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "NumPad0", "NumPad1", "NumPad2", "NumPad3", "NumPad4", "NumPad5", "NumPad6", "NumPad7", "NumPad8", "NumPad9"
        };

        private static string ModToDisplay(string mod)
        {
            if (string.IsNullOrEmpty(mod) || mod.Equals("None", StringComparison.OrdinalIgnoreCase) || mod == "없음")
                return "없음";
            if (mod.Equals("Ctrl+Alt", StringComparison.OrdinalIgnoreCase)) return "Ctrl + Alt";
            if (mod.Equals("Ctrl+Shift", StringComparison.OrdinalIgnoreCase)) return "Ctrl + Shift";
            if (mod.Equals("Alt+Shift", StringComparison.OrdinalIgnoreCase)) return "Alt + Shift";
            return mod;
        }

        private static string ModFromDisplay(string display)
        {
            if (string.IsNullOrEmpty(display) || display == "없음" || display.Equals("None", StringComparison.OrdinalIgnoreCase))
                return "None";
            if (display == "Ctrl + Alt") return "Ctrl+Alt";
            if (display == "Ctrl + Shift") return "Ctrl+Shift";
            if (display == "Alt + Shift") return "Alt+Shift";
            return display;
        }

        private void OnHotkeyControlChanged()
        {
            if (_isUpdatingHotkeyUI ||
                cboStartMod == null || cboStartKey == null ||
                cboStopMod == null || cboStopKey == null ||
                cboEmergencyMod == null || cboEmergencyKey == null)
            {
                return;
            }

            string startMod = ModFromDisplay(cboStartMod.SelectedItem?.ToString());
            string startKey = cboStartKey.SelectedItem?.ToString() ?? "F1";

            string stopMod = ModFromDisplay(cboStopMod.SelectedItem?.ToString());
            string stopKey = cboStopKey.SelectedItem?.ToString() ?? "F2";

            string emgMod = ModFromDisplay(cboEmergencyMod.SelectedItem?.ToString());
            string emgKey = cboEmergencyKey.SelectedItem?.ToString() ?? "F4";

            _settings.HotkeyStart = new HotkeyConfig(startMod, startKey);
            _settings.HotkeyStop = new HotkeyConfig(stopMod, stopKey);
            _settings.HotkeyEmergency = new HotkeyConfig(emgMod, emgKey);
            _settings.Save();

            UpdateHotkeyButtonUI();
            UpdateModeHint();
        }

        private void ResetHotkeysToDefault()
        {
            _isUpdatingHotkeyUI = true;
            cboStartMod.SelectedItem = "Alt";
            cboStartKey.SelectedItem = "F1";

            cboStopMod.SelectedItem = "없음";
            cboStopKey.SelectedItem = "F2";

            cboEmergencyMod.SelectedItem = "Alt";
            cboEmergencyKey.SelectedItem = "F4";
            _isUpdatingHotkeyUI = false;

            OnHotkeyControlChanged();
            SystemSounds.Beep.Play();
            lblStatus.Text = "단축키가 기본값으로 복원되었습니다. (시작: Alt+F1, 정지: F2, 긴급탈출: Alt+F4)";
        }

        private void UpdateHotkeyButtonUI()
        {
            if (_hotkeysEnabled)
            {
                btnToggleHotkeys.Text = $"⌨ 단축키 활성 [ON] ({_settings.HotkeyStart}/{_settings.HotkeyStop})";
                btnToggleHotkeys.BackColor = Color.FromArgb(46, 139, 87);
                btnToggleHotkeys.ForeColor = Color.White;
                btnStart.Text = $"▶ 시작 ({_settings.HotkeyStart})";
                btnStop.Text = $"⏹ 정지 ({_settings.HotkeyStop})";
            }
            else
            {
                btnToggleHotkeys.Text = $"단축키 비활성 중. 긴급탈출 {_settings.HotkeyEmergency}";
                btnToggleHotkeys.BackColor = _currentTheme?.IsDark == true ? Color.FromArgb(70, 72, 78) : Color.FromArgb(145, 138, 130);
                btnToggleHotkeys.ForeColor = Color.White;
                btnStart.Text = "▶ 시작";
                btnStop.Text = "⏹ 정지";
            }
            ApplyThemeToGuide();
        }

        private void SetupGlobalKeyboardHook()
        {
            try
            {
                _keyboardHook = new GlobalKeyboardHook();
                _keyboardHook.KeyDown += (sender, e) =>
                {
                    // 1. Emergency Escape: ALWAYS stops auto input when running, regardless of _hotkeysEnabled!
                    if (_isRunning && _settings.HotkeyEmergency != null && _settings.HotkeyEmergency.Matches(e.Key, e.Alt, e.Control, e.Shift))
                    {
                        e.Handled = true; // Block Alt+F4 from closing target application
                        _isEmergencyStop = true;
                        BeginInvoke(new Action(() =>
                        {
                            StopAutoInput();
                            SystemSounds.Hand.Play();
                        }));
                        return;
                    }

                    // 2. Regular hotkeys only active when _hotkeysEnabled is true
                    if (!_hotkeysEnabled) return;

                    if (_isRunning)
                    {
                        // Stop hotkey while running
                        if (_settings.HotkeyStop != null && _settings.HotkeyStop.Matches(e.Key, e.Alt, e.Control, e.Shift))
                        {
                            e.Handled = true;
                            BeginInvoke(new Action(() =>
                            {
                                StopAutoInput();
                                SystemSounds.Beep.Play();
                            }));
                            return;
                        }
                    }
                    else
                    {
                        // Start hotkey while stopped
                        if (_settings.HotkeyStart != null && _settings.HotkeyStart.Matches(e.Key, e.Alt, e.Control, e.Shift))
                        {
                            e.Handled = true;
                            BeginInvoke(new Action(() =>
                            {
                                StartAutoInput();
                                SystemSounds.Beep.Play();
                            }));
                            return;
                        }
                    }
                };
            }
            catch
            {
                // Non-intrusive fallback
            }
        }
    }
}