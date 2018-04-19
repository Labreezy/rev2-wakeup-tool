using System.Collections.Generic;

namespace GGXrdWakeupDPUtil.Library
{
    public class SlotInput
    {
        public SlotInput(string input, IEnumerable<short> inputShorts, int wakeupFrameIndex)
        {
            Input = input;
            InputList = inputShorts;
            WakeupFrameIndex = wakeupFrameIndex;
        }

        public string Input { get; set; }
        public IEnumerable<short> InputList { get; set; }
        public int WakeupFrameIndex { get; set; }

    }
}
