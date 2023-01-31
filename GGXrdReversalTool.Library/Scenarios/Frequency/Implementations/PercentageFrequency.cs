using GGXrdReversalTool.Library.Memory;

namespace GGXrdReversalTool.Library.Scenarios.Frequency.Implementations;

public class PercentageFrequency : IScenarioFrequency
{
    private readonly Random _random = new();
    private readonly int _percentage;

    public PercentageFrequency(int percentage)
    {
        _percentage = Math.Max(Math.Min(percentage, 100), 0);
    }

    public bool ShouldHappen()
    {
        return _random.Next(0, 101) <= _percentage;
    }

    public IMemoryReader MemoryReader { get; set; }
}