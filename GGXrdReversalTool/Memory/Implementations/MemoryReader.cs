using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GGXrdReversalTool.Memory.Implementations;

//TODO Lint
public class MemoryReader : IMemoryReader
{
    private readonly Process _process;

    public MemoryReader(Process process)
    {
        _process = process;
    }


    private readonly MemoryPointer _p1AnimStringPtr = new MemoryPointer("P1AnimStringPtr");
    private readonly MemoryPointer _p2AnimStringPtr = new MemoryPointer("P2AnimStringPtr");

    public string ReadAnimationString(int player)
    {
        const int length = 32;

        switch (player)
        {
            case 1:
                return ReadString(_p1AnimStringPtr, length);
            case 2:
                return ReadString(_p2AnimStringPtr, length);
            default:
                return string.Empty;
        }
    }















    #region DLL Imports

    [DllImport("kernel32.dll")]
    private static extern bool ReadProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        ref int lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory
    (IntPtr hProcess,
        IntPtr lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        ref int lpNumberOfBytesRead);

    #endregion

    private string ReadString(MemoryPointer memoryPointer, int length)
    {
        return memoryPointer.Offsets.Any()
            ? this.ReadStringWithOffsets(memoryPointer.Pointer, length, memoryPointer.Offsets.ToArray())
            : this.ReadString(memoryPointer.Pointer, length);
    }

    private string ReadStringWithOffsets(IntPtr baseAddress, int length, params int[] offsets)
    {
        IntPtr newAddress = IntPtr.Add(this._process.MainModule.BaseAddress, (int)baseAddress);
        IntPtr value = this.Read<IntPtr>(newAddress);
        string result = string.Empty;

        for (int i = 0; i < offsets.Length; i++)
        {
            newAddress = IntPtr.Add(value, offsets[i]);

            if (i + 1 == offsets.Length)
            {
                result = this.ReadString(newAddress, length);
            }
            else
            {
                value = this.Read<IntPtr>(newAddress);
            }
        }

        return result;
    }

    private T Read<T>(IntPtr address)
    {
        Type outputType = typeof(T).IsEnum ? Enum.GetUnderlyingType(typeof(T)) : typeof(T);

        int length = Marshal.SizeOf(outputType);

        var value = this.ReadBytes(address, length);


        T result;

        GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);

        try
        {
            result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), outputType);
        }
        finally
        {
            handle.Free();
        }

        return result;
    }

    private string ReadString(IntPtr address, int length)
    {
        var value = ReadBytes(address, length);
        var result = Encoding.Default.GetString(value);
        return result.Replace("\0", "");
    }

    private byte[] ReadBytes(IntPtr address, int length)
    {
        IntPtr handle = this._process.Handle;
        byte[] bytes = new byte[length];
        int lpNumberOfBytesRead = 0;
        ReadProcessMemory(handle, address, bytes, bytes.Length, ref lpNumberOfBytesRead);

        return bytes;
    }
}