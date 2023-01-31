using GGXrdReversalTool.Library.Input;
using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Models;

namespace GGXrdReversalTool.Library.Scenarios.Action.Implementations;

public class PlayReversalAction: IScenarioAction
{
    public void Execute()
    {
        Console.WriteLine("Execute!");
        InputManager.SendKey(DirectXKeyStrokes.DIK_N, false, InputType.Keyboard);
        Thread.Sleep(10);
        InputManager.SendKey(DirectXKeyStrokes.DIK_N, true, InputType.Keyboard);
    }

    public IMemoryReader MemoryReader { get; set; }
    public SlotInput Input { get; set; } = new SlotInput("5HD");
    // public SlotInput Input { get; set; } = new SlotInput("6,2,!3H");
    // public SlotInput Input { get; set; } = new SlotInput("!6H");
    public void Init()
    {
        MemoryReader.WriteInputInSlot(1, Input);
    }
}