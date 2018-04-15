using Binarysharp.MemoryManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GGXrdWakeupDPUtil
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MemorySharp Msggprocess;
        public List<NameWakeupData> NameWakeupDataList = new List<NameWakeupData>
        {
            new NameWakeupData("Sol", 25, 21),
            new NameWakeupData("Ky", 23, 21),
            new NameWakeupData("May", 25, 22),
            new NameWakeupData("Millia", 25, 23),
            new NameWakeupData("Zato", 25, 22),
            new NameWakeupData("Potemkin", 24, 22),
            new NameWakeupData("Chipp", 30, 24),
            new NameWakeupData("Faust", 25, 29),
            new NameWakeupData("Axl", 25, 21),
            new NameWakeupData("Venom", 21, 26),
            new NameWakeupData("Slayer", 26, 20),
            new NameWakeupData("I-No", 24, 20),
            new NameWakeupData("Bedman", 24, 30),
            new NameWakeupData("Ramlethal", 25, 23),
            new NameWakeupData("Sin", 30, 21),
            new NameWakeupData("Elphelt", 27, 27),
            new NameWakeupData("Leo", 28, 26),
            new NameWakeupData("Johnny", 25, 24),
            new NameWakeupData("Jack-O'", 25, 23),
            new NameWakeupData("Jam", 26, 25),
            new NameWakeupData("Haehyun", 25, 27),
            new NameWakeupData("Raven", 25, 24),
            new NameWakeupData("Dizzy", 25, 24),
            new NameWakeupData("Baiken", 22, 21),
            new NameWakeupData("Answer", 24, 24)

        };

        private readonly string _facedown = "CmnActFDown2Stand";
        private readonly string _faceup = "CmnActBDown2Stand";

        private readonly CancellationToken waitFrameToken;


        //TODO new variables


        private ReversalTool _reversalTool;
        private ScriptInjector _scriptInjector;
        private NameWakeupData _currentDummy;
        private readonly CancellationTokenSource _dummyTokenSource = new CancellationTokenSource();
        private readonly CancellationTokenSource _reversalTokenSource = new CancellationTokenSource();
        
        private CancellationToken _dummyIdCancellationToken;
        private CancellationToken _reversalCancellationToken;

        private void Start()
        {
            _reversalTool = new ReversalTool();

            if (!_reversalTool.CheckGuiltyGearXrdProcess())
            {
                MessageBox.Show("Guilty Gear not found open!  Remember, be in training mode paused when you open this program.  This program will now close.");
                Application.Current.Shutdown();
                return;
            }

            _dummyIdCancellationToken = _dummyTokenSource.Token;
            Task.Run(() => UpdateIdDummy(), _dummyIdCancellationToken);
        }

        private void Stop()
        {
            if (_dummyTokenSource != null && !_dummyIdCancellationToken.IsCancellationRequested)
            {
                _dummyTokenSource.Cancel();
            }
            if (_reversalTokenSource != null && !_reversalCancellationToken.IsCancellationRequested)
            {
                _reversalTokenSource.Cancel();
            }


            _scriptInjector?.Dispose();
        }

        public void UpdateIdDummy()
        {
            while (!_dummyIdCancellationToken.IsCancellationRequested)
            {
                var dummy = _reversalTool.GetDummy();

                if (!Equals(dummy, _currentDummy))
                {
                    SetDummyId();
                }

                Thread.Sleep(500);
            }
        }


        private void SetDummyId()
        {
            try
            {
                var dummy = _reversalTool.GetDummy();

                Dispatcher.Invoke(() =>
                {
                    dummyTextBlock.Text = $"Current Dummy :{dummy.CharName}";
                });
                _currentDummy = dummy;
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Can't read the dummy id offset. This application will now close.");
                Application.Current.Shutdown();
                return;
            }
        }

        private void EnableReversal(int wakeupframeidx)
        {
            if (_scriptInjector == null)
            {
                _scriptInjector = new ScriptInjector(Dispatcher, _reversalTool.GetPid()); 
            }




            while (!_reversalCancellationToken.IsCancellationRequested)
            {
                try
                {
                    int wakeuptiming = 0;

                    if (_reversalTool.ReadAnimString(2) == _facedown)
                    {
                        wakeuptiming = _currentDummy.FaceDownFrames;
                    }
                    else if (_reversalTool.ReadAnimString(2) == _faceup)
                    {
                        wakeuptiming = _currentDummy.FaceDownFrames;
                    }
                    Task framewait = Task.Run(() => WaitFrames(wakeuptiming - wakeupframeidx - 1));
                    framewait.Wait();


                    if (wakeuptiming == 0)
                    {
                    }
                    else
                    {
                        _scriptInjector.Post("{\"type\": \"playback\"}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("P2's animation address broke/has changed while enabled.  This program will now shut down.  If this issue persists, contact me.");
                    Stop();
                    Application.Current.Shutdown();
                    return;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Stop();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Start();
        }
        


        private void Script_Message(object sender, Frida.ScriptMessageEventArgs e)
        {
            var senderscript = (Frida.Script)sender;
            MessageBox.Show(e.Message);
        }

        private void enableButton_Click(object sender, RoutedEventArgs e)
        {
            var input = inputTextBox.Text;
            var slotidx = -1;
            int wakeupframeidx;

            if (Slot1R.IsChecked != null && Slot1R.IsChecked.Value)
            {
                slotidx = 0;
            }
            else if (Slot2R.IsChecked != null && Slot2R.IsChecked.Value)
            {
                slotidx = 1;
            }
            else if (Slot3R.IsChecked != null && Slot3R.IsChecked.Value)
            {
                slotidx = 2;
            }
            if (slotidx >= 0)
            {
                wakeupframeidx = _reversalTool.OverwriteSlot(slotidx, input);
            }
            else
            {
                MessageBox.Show("No slot chosen.  Please make sure you picked a slot to overwrite.");
                return;
            }

            enableButton.IsEnabled = false;
            Slot1R.IsEnabled = false;
            Slot2R.IsEnabled = false;
            Slot3R.IsEnabled = false;
            disableButton.IsEnabled = true;

            //Task.Run(() => EnableReversal(wakeupframeidx), reversalCancellationToken);
            _reversalCancellationToken = _reversalTokenSource.Token;
            Task.Run(() => EnableReversal(wakeupframeidx), _reversalCancellationToken);

        }

        private void WaitFrames(int frames)
        {
            int fc = _reversalTool.FrameCount();
            while (_reversalTool.FrameCount() < fc + frames && !waitFrameToken.IsCancellationRequested)
            {

            }
        }
        private void disableButton_Click(object sender, RoutedEventArgs e)
        {
            _scriptInjector.Disable();
            enableButton.IsEnabled = true;
            Slot1R.IsEnabled = true;
            Slot2R.IsEnabled = true;
            Slot3R.IsEnabled = true;
            disableButton.IsEnabled = false;
        }
    }
}
