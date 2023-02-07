using System;
using GGXrdWakeupDPUtil.Library.Memory;

namespace GGXrdWakeupDPUtil.Library.Scenarios.Events.Implementations
{
    public class WakeupEvent : IScenarioEvent
    {
        private const string FaceDownAnimation = "CmnActFDown2Stand";
        private const string FaceUpAnimation = "CmnActBDown2Stand";
        private const string WallSplatAnimation = "CmnActWallHaritsukiGetUp";
        
        public IMemoryReader MemoryReader { get; }
        public event EventHandler Occured;


        public WakeupEvent(IMemoryReader memoryReader)
        {
            MemoryReader = memoryReader;
        }
        public void CheckEvent()
        {
            var animationString = MemoryReader.ReadAnimationString(2);

            if (animationString == FaceDownAnimation)
            {
                Occured?.Invoke(this, null);
            }
        }
    }
}