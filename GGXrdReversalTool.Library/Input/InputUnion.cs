using System.Runtime.InteropServices;

namespace GGXrdReversalTool.Library.Input;

[StructLayout(LayoutKind.Explicit)]
public struct InputUnion
{
    [FieldOffset(0)]
    public readonly MouseInput mi;
    [FieldOffset(0)]
    public KeyboardInput ki;
    [FieldOffset(0)]
    public readonly HardwareInput hi;
}