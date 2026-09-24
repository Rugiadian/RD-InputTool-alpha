using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace RD_Tools
{
    public partial class MainForm : Form
    {
        private GlobalKeyboardHook _keyboardHook;
        private CancellationTokenSource _cts;
        private bool _isRunning = false;
        private bool _isEmergencyStop = false;
        private bool _hotkeysEnabled = true;
        private readonly AppSettings _settings;

        public MainForm()
        {
            _isUpdatingHotkeyUI = true;
            _settings = AppSettings.Load();
            InitializeComponent();
            ApplySettingsToUI();
            SetupGlobalKeyboardHook();
            _isUpdatingHotkeyUI = false;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // 최초 실행 시 창이 화면에 올라온 뒤 OS 레벨에서 TopMost를 확실하게 강제 적용 및 상태 검증
            if (_settings.AlwaysOnTop)
            {
                ApplyTopMost(true);
            }
            else
            {
                ApplyTopMost(false);
            }

            UpdateTopMostButtonUI();

            bool isActuallyTop = NativeMethods.IsWindowTopMost(Handle);
            if (isActuallyTop)
            {
                lblStatus.Text = "상태: 준비 완료 (창 항상 위: ON)";
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // 창이 최소화되었다가 복원될 때 OS에서 HWND_TOPMOST가 풀리는 현상 방지 및 상태 동기화
            if (WindowState != FormWindowState.Minimized && _settings.AlwaysOnTop && IsHandleCreated)
            {
                if (!NativeMethods.IsWindowTopMost(Handle))
                {
                    NativeMethods.SetWindowTopMost(Handle, true);
                    TopMost = true;
                }
                UpdateTopMostButtonUI();
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            // 창 활성화 시 항상 위 상태가 해제되어 있다면 다시 복구 및 UI 갱신
            if (_settings.AlwaysOnTop && IsHandleCreated && !NativeMethods.IsWindowTopMost(Handle))
            {
                NativeMethods.SetWindowTopMost(Handle, true);
                TopMost = true;
                UpdateTopMostButtonUI();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAutoInput();
            SaveCurrentSettings();
            _keyboardHook?.Dispose();
        }
    }
}