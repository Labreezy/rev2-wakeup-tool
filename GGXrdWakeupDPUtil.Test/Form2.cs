using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using GGXrdWakeupDPUtil.Library;
using GGXrdWakeupDPUtil.Library.Memory.Implementations;
using GGXrdWakeupDPUtil.Library.Scenarios;
using GGXrdWakeupDPUtil.Library.Scenarios.Events.Implementations;

namespace GGXrdWakeupDPUtil.Test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReversalTool2 reversalTool2 = new ReversalTool2();



            var process = Process.GetProcessesByName("GuiltyGearXrd").FirstOrDefault();
            var memoryReader = new MemoryReader(process);
            var scenarioEvent = new WakeupEvent(memoryReader);
            var scenarioAction = new PlayReversalAction();
            var scenarioFrequency = new PercentageFrequency(100);
            
            Scenario scenario = new Scenario(scenarioEvent, scenarioAction, scenarioFrequency);
            
            scenario.Run();
        }
    }
}