using System;
using GGXrdWakeupDPUtil.Library.Memory;

namespace GGXrdWakeupDPUtil.Library.Scenarios.Events
{
    public interface IScenarioEvent
    {
        IMemoryReader MemoryReader { get; }
        event EventHandler Occured;
        void CheckEvent();
    }
}