using System;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RD_Tools
{
    public partial class MainForm
    {
        private void PickCoordinate(NumericUpDown targetX, NumericUpDown targetY, string pointName)
        {
            using (var picker = new CoordinatePickerForm())
            {
                if (picker.ShowDialog(this) == DialogResult.OK)
                {
                    targetX.Value = picker.SelectedPoint.X;
                    targetY.Value = picker.SelectedPoint.Y;
                    SystemSounds.Beep.Play();
                    lblStatus.Text = $"{pointName} 좌표 설정 완료: X={picker.SelectedPoint.X}, Y={picker.SelectedPoint.Y}";
                }
            }
            if (_settings.AlwaysOnTop && IsHandleCreated)
            {
                ApplyTopMost(true);
            }
        }

        private void CaptureCurrentMouse(NumericUpDown targetX, NumericUpDown targetY, string pointName)
        {
            Point p = Cursor.Position;
            targetX.Value = p.X;
            targetY.Value = p.Y;
            SystemSounds.Beep.Play();
            lblStatus.Text = $"{pointName} 현재 마우스 위치 등록됨: X={p.X}, Y={p.Y}";
        }

        private void ApplyStoppedState(int currentIteration, bool isEmergency = false)
        {
            energyBar.IsActive = false;
            energyBar.IsStopped = true;
            energyBar.Value = 0.0;
            energyBar.StatusText = isEmergency
                ? $"🚨 [{_settings.HotkeyEmergency}] 비상탈출 긴급 중단됨"
                : (_hotkeysEnabled ? $"⏹ [{_settings.HotkeyStop}] 키 또는 정지로 중단됨" : "⏹ 정지 버튼으로 중단됨");

            lblEnergyTitle.Text = "⚡ 실시간 기동 에너지 바: ⏹ 중단됨";

            // Loop reset: Return to initial values instead of keeping leftovers
            RefreshDashboardInitialValues();

            lblStatus.Text = currentIteration > 0
                ? $"⏹ 작업이 중단되었습니다. (총 {currentIteration}회 실행 후 루프 초기화됨)"
                : "⏹ 작업이 시작 전 중단되었습니다.";
        }

        private async void StartAutoInput()
        {
            if (_isRunning) return;

            string text1 = txtInput.Text;
            if (string.IsNullOrEmpty(text1))
            {
                lblStatus.Text = "⚠️ 입력할 문구를 먼저 작성해 주세요.";
                return;
            }

            bool enablePoint2 = chkEnablePoint2.Checked;
            string text2 = (enablePoint2 && chkSeparateText2.Checked && !string.IsNullOrEmpty(txtInput2.Text))
                ? txtInput2.Text
                : text1;

            int x1 = (int)numX.Value;
            int y1 = (int)numY.Value;
            int x2 = (int)numX2.Value;
            int y2 = (int)numY2.Value;
            int pointIntervalMs = (int)(numPointInterval.Value * 1000);

            string intUnit = cboIntervalUnit.SelectedItem?.ToString() ?? "초";
            int intervalMs = intUnit switch
            {
                "분" => (int)(numInterval.Value * 60 * 1000),
                "시간" => (int)(numInterval.Value * 3600 * 1000),
                _ => (int)(numInterval.Value * 1000)
            };
            if (intervalMs < 10) intervalMs = 10;

            int clickDelayMs = (int)numClickDelay.Value;
            bool sendEnter = chkEnter.Checked;
            bool useClipboard = chkClipboard.Checked;

            bool enableDuration = chkEnableDuration.Checked;
            string durUnit = cboDurationUnit.SelectedItem?.ToString() ?? "초";
            long durationMs = durUnit switch
            {
                "시간" => (long)(numDuration.Value * 3600 * 1000),
                "분" => (long)(numDuration.Value * 60 * 1000),
                _ => (long)(numDuration.Value * 1000)
            };

            bool enableCount = chkEnableCount.Checked;
            int targetCount = (int)numCount.Value;

            bool immediateStart = rbStartImmediate.Checked;
            decimal startDelaySec = numStartDelay.Value;

            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            _isRunning = true;
            _isEmergencyStop = false;
            UpdateUIState(true);

            SaveCurrentSettings();

            // Initial dashboard setup
            energyBar.IsActive = false;
            energyBar.IsStopped = false;
            energyBar.Value = 0.0;
            energyBar.StatusText = "⏳ 준비 중...";
            lblEnergyTitle.Text = "⚡ 실시간 기동 에너지 바 (다음 입력 충전 게이지):";

            int currentIteration = 0;

            try
            {
                // Start Delay / Countdown Option
                if (!immediateStart && startDelaySec > 0)
                {
                    int totalDelayMs = (int)(startDelaySec * 1000);
                    var delaySw = Stopwatch.StartNew();

                    while (delaySw.ElapsedMilliseconds < totalDelayMs)
                    {
                        token.ThrowIfCancellationRequested();

                        double remainSec = Math.Max(0, (totalDelayMs - delaySw.ElapsedMilliseconds) / 1000.0);
                        string cancelGuide = _hotkeysEnabled ? $"[{_settings.HotkeyStop} 누르면 취소]" : "[정지 누르면 취소]";
                        lblStatus.Text = $"⏳ {remainSec:F1}초 후 입력이 시작됩니다... {cancelGuide}";
                        energyBar.StatusText = $"⏳ {remainSec:F1}초 후 시작... {cancelGuide}";

                        int step = Math.Min(100, (int)(totalDelayMs - delaySw.ElapsedMilliseconds));
                        if (step <= 0) break;
                        await Task.Delay(step, token);
                    }
                }

                token.ThrowIfCancellationRequested();

                energyBar.IsActive = true;
                lblEnergyTitle.Text = "⚡ 실시간 기동 에너지 바 (다음 입력 충전 게이지):";

                var stopwatch = Stopwatch.StartNew();

                while (!token.IsCancellationRequested)
                {
                    currentIteration++;

                    // 1. Move & Click Point 1
                    energyBar.Value = 1.0;
                    energyBar.StatusText = enablePoint2
                        ? $"💥 [포인트 1] 입력 실행 중... (제 {currentIteration}회차)"
                        : $"💥 입력 실행 중... (제 {currentIteration}회차)";
                    InputSimulator.ClickAt(x1, y1);

                    // Wait for focus
                    await Task.Delay(clickDelayMs, token);

                    // Type text at Point 1
                    if (useClipboard)
                    {
                        InputSimulator.SendClipboardPaste(text1);
                    }
                    else
                    {
                        InputSimulator.SendUnicodeText(text1);
                    }

                    // Enter key
                    if (sendEnter)
                    {
                        await Task.Delay(30, token);
                        InputSimulator.SendEnter();
                    }

                    // 2. Point 2 Sequential Input (if enabled)
                    if (enablePoint2)
                    {
                        token.ThrowIfCancellationRequested();

                        energyBar.StatusText = $"⏳ 포인트 2 대기 중... ({numPointInterval.Value:F1}초)";
                        var ptSw = Stopwatch.StartNew();
                        while (ptSw.ElapsedMilliseconds < pointIntervalMs)
                        {
                            token.ThrowIfCancellationRequested();
                            int ptStep = Math.Min(25, (int)(pointIntervalMs - ptSw.ElapsedMilliseconds));
                            if (ptStep <= 0) break;
                            await Task.Delay(ptStep, token);
                        }

                        token.ThrowIfCancellationRequested();

                        // Move & Click Point 2
                        energyBar.StatusText = $"💥 [포인트 2] 입력 실행 중... (제 {currentIteration}회차)";
                        InputSimulator.ClickAt(x2, y2);

                        // Wait for focus
                        await Task.Delay(clickDelayMs, token);

                        // Type text at Point 2
                        if (useClipboard)
                        {
                            InputSimulator.SendClipboardPaste(text2);
                        }
                        else
                        {
                            InputSimulator.SendUnicodeText(text2);
                        }

                        // Enter key
                        if (sendEnter)
                        {
                            await Task.Delay(30, token);
                            InputSimulator.SendEnter();
                        }
                    }

                    // 3. Update Stats & Check completion
                    long elapsedMs = stopwatch.ElapsedMilliseconds;
                    int remainCount = enableCount ? Math.Max(0, targetCount - currentIteration) : -1;
                    long remainDurationMs = enableDuration ? Math.Max(0, durationMs - elapsedMs) : -1;

                    // Update Stat Cards
                    lblCompletedCount.Text = enablePoint2 ? $"{currentIteration} 회 (P1+P2)" : $"{currentIteration} 회";
                    lblRemainCount.Text = enableCount ? $"{remainCount} 회" : "무제한";
                    lblRemainTime.Text = enableDuration
                        ? FormatRemainingTime(remainDurationMs, durUnit)
                        : "무제한";

                    if (!enableDuration && !enableCount)
                    {
                        // Unlimited mode
                        string stopGuide = _hotkeysEnabled ? $"[{_settings.HotkeyStop}로 정지]" : "[정지 버튼 클릭]";
                        lblStatus.Text = $"▶ 기동 중... (입력: {currentIteration}회, 경과: {elapsedMs / 1000.0:F1}초) {stopGuide}";
                    }
                    else if (enableDuration && enableCount)
                    {
                        lblStatus.Text = $"▶ 기동 중... [남은 시간: {FormatRemainingTime(remainDurationMs, durUnit)} | 남은 횟수: {remainCount}회]";

                        if (elapsedMs >= durationMs || currentIteration >= targetCount)
                        {
                            break;
                        }
                    }
                    else if (enableDuration)
                    {
                        lblStatus.Text = $"▶ 기동 중... [남은 시간: {FormatRemainingTime(remainDurationMs, durUnit)}] ({currentIteration}회 입력 완료)";

                        if (elapsedMs >= durationMs)
                        {
                            break;
                        }
                    }
                    else // enableCount
                    {
                        lblStatus.Text = $"▶ 기동 중... [남은 횟수: {remainCount}회] ({currentIteration}/{targetCount}회 완료)";

                        if (currentIteration >= targetCount)
                        {
                            break;
                        }
                    }

                    // 4. Energy Bar Charging Animation during repeat interval
                    var intervalSw = Stopwatch.StartNew();
                    while (intervalSw.ElapsedMilliseconds < intervalMs && !token.IsCancellationRequested)
                    {
                        long intElapsed = intervalSw.ElapsedMilliseconds;
                        double progress = Math.Clamp((double)intElapsed / intervalMs, 0.0, 1.0);
                        double remainingIntervalSec = Math.Max(0.0, (intervalMs - intElapsed) / 1000.0);

                        energyBar.Value = progress;
                        string timeRemainingStr = remainingIntervalSec switch
                        {
                            >= 3600 => $"{remainingIntervalSec / 3600.0:F1}시간",
                            >= 60 => $"{remainingIntervalSec / 60.0:F1}분",
                            _ => $"{remainingIntervalSec:F1}초"
                        };
                        energyBar.StatusText = $"⚡ 기동 중: 다음 입력까지 충전 {(int)(progress * 100)}% ({timeRemainingStr} 남음)";

                        // Live remaining duration countdown in Card
                        long liveElapsed = stopwatch.ElapsedMilliseconds;
                        if (enableDuration)
                        {
                            long liveRemainMs = Math.Max(0, durationMs - liveElapsed);
                            lblRemainTime.Text = FormatRemainingTime(liveRemainMs, durUnit);
                            if (liveRemainMs <= 0) break;
                        }

                        int step = Math.Min(25, (int)(intervalMs - intElapsed));
                        if (step <= 0) break;
                        await Task.Delay(step, token);
                    }
                }

                if (token.IsCancellationRequested)
                {
                    ApplyStoppedState(currentIteration, _isEmergencyStop);
                }
                else
                {
                    energyBar.IsActive = false;
                    energyBar.IsStopped = false;
                    energyBar.Value = 1.0;
                    energyBar.StatusText = "✔ 작업 완료";
                    lblEnergyTitle.Text = "⚡ 실시간 기동 에너지 바: ✔ 작업 완료";

                    // Loop reset: Return to initial values so next run starts from beginning!
                    RefreshDashboardInitialValues();
                    lblStatus.Text = $"✔ 작업이 완료되었습니다. (총 {currentIteration}회 입력 완료 후 루프 초기화됨)";
                }
            }
            catch (OperationCanceledException)
            {
                ApplyStoppedState(currentIteration, _isEmergencyStop);
            }
            catch (Exception ex)
            {
                energyBar.IsActive = false;
                energyBar.IsStopped = true;
                energyBar.Value = 0.0;
                energyBar.StatusText = "⚠️ 오류 발생으로 중단됨";
                lblEnergyTitle.Text = "⚡ 실시간 기동 에너지 바: ⚠️ 중단됨";
                lblStatus.Text = $"오류 발생: {ex.Message}";
                RefreshDashboardInitialValues();
            }
            finally
            {
                InputSimulator.ReleaseStuckKeys();
                _isRunning = false;
                energyBar.IsActive = false;
                UpdateUIState(false);
            }
        }

        private void StopAutoInput()
        {
            if (_isRunning && _cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
            else if (!_isRunning)
            {
                RefreshDashboardInitialValues();
                energyBar.IsActive = false;
                energyBar.IsStopped = false;
                energyBar.Value = 0.0;
                energyBar.StatusText = "⚪ 대기 중 (준비 완료)";
                lblEnergyTitle.Text = "⚡ 실시간 기동 에너지 바 (다음 입력 충전 게이지):";
                lblStatus.Text = "상태: 준비 완료 (루프 초기화됨)";
            }
        }

        private void UpdateUIState(bool running)
        {
            btnStart.Enabled = !running;
            btnStop.Enabled = running;

            cboTheme.Enabled = !running;

            // Point 1
            btnPickCoord.Enabled = !running;
            btnGetCurCoord.Enabled = !running;
            numX.Enabled = !running;
            numY.Enabled = !running;

            // Point 2
            chkEnablePoint2.Enabled = !running;
            btnPickCoord2.Enabled = !running && chkEnablePoint2.Checked;
            btnGetCurCoord2.Enabled = !running && chkEnablePoint2.Checked;
            numX2.Enabled = !running && chkEnablePoint2.Checked;
            numY2.Enabled = !running && chkEnablePoint2.Checked;
            numPointInterval.Enabled = !running && chkEnablePoint2.Checked;

            // Text
            txtInput.Enabled = !running;
            chkSeparateText2.Enabled = !running && chkEnablePoint2.Checked;
            txtInput2.Enabled = !running && chkEnablePoint2.Checked && chkSeparateText2.Checked;

            // Start Delay
            rbStartImmediate.Enabled = !running;
            rbStartDelay.Enabled = !running;
            numStartDelay.Enabled = !running && rbStartDelay.Checked;

            // Repeat & Timing
            numInterval.Enabled = !running;
            cboIntervalUnit.Enabled = !running;
            chkEnableDuration.Enabled = !running;
            numDuration.Enabled = !running && chkEnableDuration.Checked;
            cboDurationUnit.Enabled = !running && chkEnableDuration.Checked;
            chkEnableCount.Enabled = !running;
            numCount.Enabled = !running && chkEnableCount.Checked;
            chkEnter.Enabled = !running;
            chkClipboard.Enabled = !running;
            numClickDelay.Enabled = !running;

            // Custom Hotkey Controls
            cboStartMod.Enabled = !running;
            cboStartKey.Enabled = !running;

            cboStopMod.Enabled = !running;
            cboStopKey.Enabled = !running;

            cboEmergencyMod.Enabled = !running;
            cboEmergencyKey.Enabled = !running;

            btnResetHotkeys.Enabled = !running;

            if (running)
            {
                btnStart.BackColor = _currentTheme?.IsDark == true ? Color.FromArgb(70, 75, 80) : Color.FromArgb(205, 198, 190);
                btnStop.BackColor = Color.FromArgb(205, 65, 55);
            }
            else
            {
                btnStart.BackColor = Color.FromArgb(46, 139, 87);
                btnStop.BackColor = _currentTheme?.IsDark == true ? Color.FromArgb(70, 75, 80) : Color.FromArgb(205, 198, 190);
            }
        }
    }
}