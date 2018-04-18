using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGXrdWakeupDPUtil.Library
{
    public class SlotInput
    {
        public SlotInput(string input, IEnumerable<short> inputShorts, int wakeupFrameIndex)
        {
            this.Input = input;
            this.InputList = inputShorts;
            this.WakeupFrameIndex = wakeupFrameIndex;
        }

        public string Input { get; set; }
        public IEnumerable<short> InputList { get; set; }
        public int WakeupFrameIndex { get; set; }

    }
}
