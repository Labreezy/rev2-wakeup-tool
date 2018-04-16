using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGXrdWakeupDPUtil
{
    public class SlotInput
    {
        public string Input { get; set; }
        public List<short> InputList { get; set; }
        public int WakeupFrameIndex { get; set; }

    }
}
