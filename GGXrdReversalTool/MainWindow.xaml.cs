using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GGXrdReversalTool.Memory.Implementations;
using GGXrdReversalTool.Scenarios;
using GGXrdReversalTool.Scenarios.Action.Implementations;
using GGXrdReversalTool.Scenarios.Event;
using GGXrdReversalTool.Scenarios.Event.Implementations;
using GGXrdReversalTool.Scenarios.Frequency.Implementations;

namespace GGXrdReversalTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //TODO Remove
            
            
            
            
            
            // ReversalTool2 reversalTool2 = new ReversalTool2();



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