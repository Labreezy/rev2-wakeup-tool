using GGXrdReversalTool.Library.Memory;

namespace GGXrdReversalTool.Library.Scenarios.Event.Implementations;

public class ComboEvent : IScenarioEvent
{
    public int ComboCount
    {
        get => _comboCount;
        set => _comboCount = value;
    }

    public IMemoryReader MemoryReader { get; set; }

    private int _oldComboCount;
    private int _comboCount = 5;

    public ScenarioEventTypes CheckEvent()
    {
        var comboCount = MemoryReader.GetComboCount(1);

        //TODO Implement parameter for combo values
        if (comboCount == ComboCount && _oldComboCount != comboCount)
        {
            _oldComboCount = comboCount;
            return ScenarioEventTypes.Combo;
        }
        
        _oldComboCount = comboCount;

        return ScenarioEventTypes.None;
    }
}