using GGXrdReversalTool.Library.Memory;

namespace GGXrdReversalTool.Library.Scenarios.Event.Implementations;

public class ComboEvent : IScenarioEvent
{
    public int MinComboCount { get; set; } = 1;
    public int MaxComboCount { get; set; } = 5;

    public IMemoryReader MemoryReader { get; set; }

    private int _oldComboCount;

    public ScenarioEventTypes CheckEvent()
    {
        var comboCount = MemoryReader.GetComboCount(1);

        if (comboCount >= MinComboCount && comboCount<= MaxComboCount && _oldComboCount != comboCount)
        {
            _oldComboCount = comboCount;
            return ScenarioEventTypes.Combo;
        }

        _oldComboCount = comboCount;

        return ScenarioEventTypes.None;
    }

}