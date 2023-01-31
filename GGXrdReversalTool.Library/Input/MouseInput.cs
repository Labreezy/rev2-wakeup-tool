namespace GGXrdReversalTool.Library.Input;

public struct MouseInput
{
    public readonly int dx;
    public readonly int dy;
    public readonly uint mouseData;
    public readonly uint dwFlags;
    public readonly uint time;
    public readonly IntPtr dwExtraInfo;
}