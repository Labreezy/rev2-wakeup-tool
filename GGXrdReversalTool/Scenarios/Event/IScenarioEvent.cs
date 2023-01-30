using System;
using GGXrdReversalTool.Memory;

namespace GGXrdReversalTool.Scenarios.Event;

public interface IScenarioEvent
{
    IMemoryReader MemoryReader { get; }
    //TODO Change signature?
    event EventHandler Occured;
    void CheckEvent();
}