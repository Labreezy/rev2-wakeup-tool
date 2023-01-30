using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using GGXrdReversalTool.Configuration;

namespace GGXrdReversalTool.Memory;

//TODO Lint
public class MemoryPointer
{
    private const char SeparatorChar = '|';
    public IntPtr Pointer { get; }

    public IEnumerable<int> Offsets { get; }
    public string Name { get; }


    public MemoryPointer(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Memory pointer should have a name", nameof(name));
        }

        Name = name;

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Memory pointer {name} value should not be null", nameof(value));
        }

        var values = value.Split(SeparatorChar);

        if (!values.Any())
        {
            throw new ArgumentException($"Memory pointer {name} value should not be null", nameof(value));
        }

        int pointerValue;

        try
        {
            pointerValue = Convert.ToInt32(values[0], 16);
        }
        catch (Exception)
        {
            throw new ArgumentException($"Pointer {name} value is invalid", nameof(value));
        }

        Pointer = new IntPtr(pointerValue);

        Offsets = values.Skip(1).Select(offset =>
        {
            int offsetValue;
            try
            {
                offsetValue = Convert.ToInt32(offset, 16);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Pointer {name} value is invalid", nameof(value));
            }

            return offsetValue;
        });

    }

    public MemoryPointer(string name)
        : this(name, ReversalToolConfiguration.Get(name))
    {
    }
}