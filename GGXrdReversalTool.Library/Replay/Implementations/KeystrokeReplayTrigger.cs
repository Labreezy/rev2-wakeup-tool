using System.Diagnostics;
using System.Runtime.InteropServices;
using GGXrdReversalTool.Library.Input;

namespace GGXrdReversalTool.Library.Replay.Implementations;

public class KeystrokeReplayTrigger : IReplayTrigger
{
    private readonly DirectXKeyStrokes _replayKeyStroke;
    private readonly Process? _process;
    
    #region Dll Imports
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
    #endregion

    public KeystrokeReplayTrigger(DirectXKeyStrokes replayKeyStroke, Process process)
    {
        _replayKeyStroke = replayKeyStroke;
        _process = process;
    }

    public void Trigger()
    {
        BringWindowToFront();
        InputManager.SendKey(_replayKeyStroke, false, InputType.Keyboard);
        Thread.Sleep(50);
        InputManager.SendKey(_replayKeyStroke, true, InputType.Keyboard);
    }
    
    private void BringWindowToFront()
    {
        IntPtr handle = GetForegroundWindow();
        if (_process != null && _process.MainWindowHandle != handle)
        {
            SetForegroundWindow(_process.MainWindowHandle);
        }
    }
}