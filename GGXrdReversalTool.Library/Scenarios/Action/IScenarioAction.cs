using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Models;

namespace GGXrdReversalTool.Library.Scenarios.Action;

public interface IScenarioAction
{
    void Execute();
    IMemoryReader MemoryReader { get; internal set; }
    
    SlotInput Input { get; set; }
    void Init();
}