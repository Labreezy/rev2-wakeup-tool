namespace GGXrdReversalTool.Library.Input;

public struct KeyboardInput
{
    public ushort wVk;
    public ushort wScan;
    public uint dwFlags;
    public readonly uint time;
    public IntPtr dwExtraInfo;
}