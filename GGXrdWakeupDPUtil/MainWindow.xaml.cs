using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GGXrdWakeupDPUtil
{
    public class NameWakeupData
    {
        public String charName { get; }
        public int faceUpFrames { get; }
        public int faceDownFrames { get; }
        public NameWakeupData(string cn, int fu, int fd)
        {
            charName = cn;
            faceUpFrames = fu;
            faceDownFrames = fd;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int P2IDOffset = 0x1BDCE08;
        private List<int> P1AnimOffsets = new List<int> { 0x1674508, 0x28, 0xD8, 0x4A8, 0x490, 0x484, 0x244C };
        private List<int> P1AnimOffsets_fallback = new List<int> { 0x0167EFCC, 0x370, 0xE4, 0x414, 0x4C, 0x484, 0x244C };
        private List<int> P2AnimOffsets = new List<int> { 0x1674574, 0x28, 0xD8, 0x4A8, 0x494, 0x484, 0x244C };
        private List<int> P2AnimOffsets_fallback = new List<int> { 0x016775F4, 0x2D8, 0xD8, 0x4A8, 0x494, 0x484, 0x244C };
        private List<int> P1P2FlipOffsets = new List<int> { 0x16DF5FC, 0x5A8, 0x42C, 0x34C ,0x18 ,0x400 };
        const int frameCounterOffset = 0x1BD2F6C;
        const int recordingSlotOneOffset = 0x1AC5F58;
        const int recordingSlotSize = 4808;
        const int recFlagOffset = 0x1BD17C0;
        const int playbackFrameCountOffset = 0x1BD438C;
        public MemorySharp msggprocess = null;
        public List<NameWakeupData> nameWakeupDataList = new List<NameWakeupData>
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
        private string ggprocname = "GuiltyGearXrd";
        private string facedown = "CmnActFDown2Stand";
        private string faceup = "CmnActBDown2Stand";
        private const char frameDelimiter = ',';
        private const char wakeupFrame = '!';
        const int playbackFlag = 0x800;
        const int recordFlip = 0;
        Regex inputregex = new Regex(@"!?[1-9]{1}[PKSHD]{0,5}");
        Regex whitespaceregex = new Regex(@"\s+");
        int[] directions = new[] { 0b0110, 0b0010, 0b1010, 0b0100, 0b0000, 0b1000, 0b0101, 0b0001, 0b1001 };
        Frida.DeviceManager devman;
        Frida.Device localdev;
        Frida.Process ggproc;
        Frida.Script script;
        Frida.Session session;
            string scriptsrc = @"var xrdbase = Module.findBaseAddress('GuiltyGearXrd.exe');
            var hookaddr = xrdbase.add(0xB83236);
            var playingback = false;
            var running = true;
            Interceptor.attach(hookaddr, function(args){
            	if(playingback && this.context.edi.equals(ptr('3'))){
                	playingback = false;
                	this.context.ebp = ptr(Memory.readU32(this.context.edx).toString());
                  }
                });
            var quit = recv('quit', function (value) {
               Interceptor.detachAll();
               running = false;
            });
setTimeout( function () {
    while (running){        
        var op = recv('playback', function (value) {
        playingback=true;
            });
        op.wait();
    }
    }, 0);
";
        enum Buttons
        {
            P = 0x10,
            K = 0x20,
            S = 0x40,
            H = 0x80,
            D = 0x100
        }
        const int wakeupFrameMask = 0x200;
        int currentDummyId;
        public bool flip;
        CancellationTokenSource source, idsource;
        CancellationToken token, idtoken;
        private bool fallbackp1 = false;
        private bool fallbackp2 = false;
        private short singleInputParse(string input)
        {
            if (inputregex.IsMatch(input))
            {
                var result = 0;
                if (input[0] == '!')
                {
                    result += wakeupFrameMask;
                    input = input.Substring(1);
                }
                var direction = Int32.Parse(input.Substring(0, 1));
                result |= directions[direction - 1];
                if (input.Length == 1)
                {
                    return (short)result;
                }
                var buttons = input.Substring(1).ToCharArray();
                foreach (char button in buttons)
                {
                    switch (button)
                    {
                        case 'P':
                            result |= (int)Buttons.P;
                            break;
                        case 'K':
                            result |= (int)Buttons.K;
                            break;
                        case 'S':
                            result |= (int)Buttons.S;
                            break;
                        case 'H':
                            result |= (int)Buttons.H;
                            break;
                        case 'D':
                            result |= (int)Buttons.D;
                            break;
                    }
                }
                return (short)result;
            } else
            {
                return -1;
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
            if (source != null && !source.IsCancellationRequested)
            {
                source.Cancel();
            }
            if (idsource != null && !idsource.IsCancellationRequested)
            {
                idsource.Cancel();
            }
            if (script != null)
            {
                script.Post("{\"type\": \"quit\"}");
                script.Post("{\"type\": \"playback\"}");
                script.Unload();
                session.Detach();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.Diagnostics.Process.GetProcessesByName(ggprocname).Length > 0)
            {
                msggprocess = new MemorySharp(System.Diagnostics.Process.GetProcessesByName(ggprocname).First());
                try
                {
                    checkAndSetFallbacks();
                }
                catch (AccessViolationException ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Shutdown();
                    return;
                }
                setDummyID();
            }
            else
            {
                MessageBox.Show("Guilty Gear not found open!  Remember, be in training mode paused when you open this program.  This program will now close.");
                Application.Current.Shutdown();
                return;
            }
            devman = new Frida.DeviceManager(Dispatcher);
            localdev = null;
            foreach (Frida.Device d in devman.EnumerateDevices())
            {
                if (d.Type == Frida.DeviceType.Local)
                {
                    localdev = d;
                    break;
                }
            }
            if(localdev == null)
            {
                MessageBox.Show("Local device not found.  This application will now close.");
                Application.Current.Shutdown();
                return;
            }
            ggproc = null;
            foreach (Frida.Process p in localdev.EnumerateProcesses())
            {
                if(p.Pid == msggprocess.Pid)
                {
                    ggproc = p;
                    break;
                }
            }
            if(ggproc == null)
            {
                MessageBox.Show("Guilty Gear not found open!  Remember, be in training mode paused when you open this program.");
                Application.Current.Shutdown();
                return;
            }
            
            idsource = new CancellationTokenSource();
            idtoken = idsource.Token;
            setDummyID();
            Task.Run(() => updateIDLoop());
        }

        private void checkAndSetFallbacks()
        {
            try
            {
                readAnimString(1);
            }
            catch (Exception)
            {
                try
                {
                    readFallbackAnimString(1);
                    fallbackp1 = true;
                }
                catch (Exception)
                {
                    throw new AccessViolationException("P1 Animation Address Broke.  (for future use)  This program will now close.");
                }
            }
            try
            {
                readAnimString(2);
            }
            catch (Exception)
            {
                try
                {
                    readFallbackAnimString(2);
                    fallbackp2 = true;
                }
                catch (Exception)
                {
                    throw new AccessViolationException("P2 Animation Address Broke.  This program will now close.");
                }
            }
        }


        private void Script_Message(object sender, Frida.ScriptMessageEventArgs e)
        {
            var senderscript = (Frida.Script)sender;
            MessageBox.Show(e.Message);
        }

        private void updateIDLoop()
        {
            while (!idtoken.IsCancellationRequested)
            {
                if(msggprocess.Read<byte>((IntPtr)P2IDOffset) != currentDummyId)
                {
                    setDummyID();
                }
                Thread.Sleep(500);
            }
        }

        private string readAnimString(int player)
        {
            if (player == 1) {
                var addr = (IntPtr)(msggprocess.Read<int>((IntPtr)P1AnimOffsets[0]));
                foreach (int offset in P1AnimOffsets.GetRange(1, 5)) {
                    addr = IntPtr.Add(addr, offset);
                    addr = (IntPtr)msggprocess.Read<int>(addr, false);
                }
                return msggprocess.ReadString(IntPtr.Add(addr, P1AnimOffsets[6]), false, 32);
            }
            if (player == 2)
            {
                var addr = (IntPtr)(msggprocess.Read<int>((IntPtr)P2AnimOffsets[0]));
                foreach (int offset in P2AnimOffsets.GetRange(1, 5))
                {
                    addr = IntPtr.Add(addr, offset);
                    addr = (IntPtr)msggprocess.Read<int>(addr, false);
                }
                return msggprocess.ReadString(IntPtr.Add(addr, P2AnimOffsets[6]), false, 32);
            }
            return "";
        }
        private string readFallbackAnimString(int player)
        { 
            if (player == 1)
            {
                var addr = (IntPtr)(msggprocess.Read<int>((IntPtr)P1AnimOffsets_fallback[0]));
                foreach (int offset in P1AnimOffsets_fallback.GetRange(1, 5))
                {
                    addr = IntPtr.Add(addr, offset);
                    addr = (IntPtr)msggprocess.Read<int>(addr, false);
                }
                return msggprocess.ReadString(IntPtr.Add(addr, P1AnimOffsets_fallback[6]), false, 32);
            }
            if (player == 2)
            {
                var addr = (IntPtr)(msggprocess.Read<int>((IntPtr)P2AnimOffsets_fallback[0]));
                foreach (int offset in P2AnimOffsets_fallback.GetRange(1, 5))
                {
                    addr = IntPtr.Add(addr, offset);
                    addr = (IntPtr)msggprocess.Read<int>(addr, false);
                }
                return msggprocess.ReadString(IntPtr.Add(addr, P2AnimOffsets_fallback[6]), false, 32);
            }

            return "";
        }
        private void setDummyID()
        {
            var idx = msggprocess.Read<byte>((IntPtr)P2IDOffset);
            try
            {
                dummyTextBlock.Text = dummyTextBlock.Text.Split(':')[0] + ": " + nameWakeupDataList[idx].charName;
            } catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Yell at Labryz for 'royally fucking up the id offset'.  This application will now close.");
                Application.Current.Shutdown();
                return;
            }
            currentDummyId = idx;
        }
        private void enableButton_Click(object sender, RoutedEventArgs e)
        {
            var text = inputTextBox.Text;
            text = whitespaceregex.Replace(text, "");
            string[] splittext = text.Split(new char[] { frameDelimiter });
            List<short> inputnums = new List<short> { };
            foreach (string inputstr in splittext)
            {
                inputnums.Add(singleInputParse(inputstr));
                if (inputnums.Last() < 0)
                {
                    MessageBox.Show("Invalid input with input '" + inputstr + "'.  Read the README for formatting information.");
                    return;
                }
            }
            var ggwindow = msggprocess.Windows.MainWindow;
            var wakeupframeidx = inputnums.FindLastIndex(delegate (short n)
            {
                return n >= 0x200;
            });
            if (wakeupframeidx < 0)
            {
                MessageBox.Show("No ! frame specified.  See the readme for more information.");
                return;
            }
            inputnums[wakeupframeidx] -= wakeupFrameMask;
            var slotidx = -1;
            if ((bool)Slot1R.IsChecked)
            {
                slotidx = 0;
            } else if ((bool)Slot2R.IsChecked)
            {
                slotidx = 1;
            }
            else if ((bool)Slot3R.IsChecked)
            {
                slotidx = 2;
            }
            if (slotidx >= 0)
            {
                overwriteSlot(slotidx, inputnums);
            } else
            {
                MessageBox.Show("No slot chosen.  Please make sure you picked a slot to overwrite.");
                return;
            }
            enableButton.IsEnabled = false;
            Slot1R.IsEnabled = false;
            Slot2R.IsEnabled = false;
            Slot3R.IsEnabled = false;
            disableButton.IsEnabled = true;
            try
            {
                checkAndSetFallbacks();
            } catch (AccessViolationException ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
                return;
            }
            session = localdev.Attach((uint)msggprocess.Pid);
            script = session.CreateScript(scriptsrc);
            script.Message += Script_Message;
            script.Load();
            ggwindow.Activate();
            source = new CancellationTokenSource();
            token = source.Token;
            Task.Run(() => enableLoop(fallbackp2, wakeupframeidx));

        }
        private void setFlip()
        {
            var addr = (IntPtr)(msggprocess.Read<int>((IntPtr)P1P2FlipOffsets[0]));
            foreach (int offset in P1P2FlipOffsets.GetRange(1, 4))
            {
                addr = IntPtr.Add(addr, offset);
                addr = (IntPtr)msggprocess.Read<int>(addr, false);
            }
            flip = msggprocess.Read<byte>(IntPtr.Add(addr, P1P2FlipOffsets[5]), false) == 0;

        }
        private void overwriteSlot(int slotidx, List<short> inputs)
        {
            var addr = recordingSlotOneOffset + recordingSlotSize * slotidx;
            var header = new List<short> {0, 0, (short)inputs.Count, 0 };
            msggprocess.Write((IntPtr)addr, header.Concat(inputs).ToArray());
        }
        private void enableLoop(bool useFallback, int wakeupframeidx)
        {
            idsource.Cancel();
            if (useFallback)
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        int wakeuptiming = 0;
                        if (facedown == readFallbackAnimString(2))
                        {
                            wakeuptiming = nameWakeupDataList[currentDummyId].faceDownFrames;
                            Task framewait = Task.Run(() => waitFrames(wakeuptiming - wakeupframeidx - 1));
                            framewait.Wait();
                        }
                        else if (faceup == readFallbackAnimString(2))
                        {
                            wakeuptiming = nameWakeupDataList[currentDummyId].faceUpFrames;
                            Task framewait = Task.Run(() => waitFrames(wakeuptiming - wakeupframeidx - 1));
                            framewait.Wait();
                        }
                        if (wakeuptiming == 0)
                        {
                            continue;
                        }
                        else
                        {
                            script.Post("{\"type\": \"playback\"}");
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("P2's animation address broke/has changed while enabled (using fallback address).  This program will now shut down.  If this issue persists, contact me.");
                        script.Post("{\"type\": \"quit\"}");
                        script.Post("{\"type\": \"playback\"}");
                        script.Unload();
                        session.Detach();
                        Application.Current.Shutdown();
                        return;
                    }
                }
            }
            else
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        int wakeuptiming = 0;
                        if (facedown == readAnimString(2))
                        {
                            wakeuptiming = nameWakeupDataList[currentDummyId].faceDownFrames;
                            Task framewait = Task.Run(() => waitFrames(wakeuptiming - wakeupframeidx - 1));
                            framewait.Wait();
                        }
                        else if (faceup == readAnimString(2))
                        {
                            wakeuptiming = nameWakeupDataList[currentDummyId].faceUpFrames;
                            Task framewait = Task.Run(() => waitFrames(wakeuptiming - wakeupframeidx - 1));
                            framewait.Wait();
                        }
                        if (wakeuptiming == 0)
                        {
                            continue;
                        }
                        else
                        {
                            script.Post("{\"type\": \"playback\"}");
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("P2's animation address broke/has changed while enabled.  This program will now shut down.  If this issue persists, contact me.");
                        script.Post("{\"type\": \"quit\"}");
                        script.Post("{\"type\": \"playback\"}");
                        script.Unload();
                        session.Detach();
                        Application.Current.Shutdown();
                        return;
                    }
                }
            }
            idsource = new CancellationTokenSource();
            idtoken = idsource.Token;
            Task.Run(() => updateIDLoop());
        }

        

        private void waitFrames(int frames)
        {
            int fc = msggprocess.Read<int>((IntPtr)frameCounterOffset);
            while(msggprocess.Read<int>((IntPtr)frameCounterOffset) < fc + frames && !token.IsCancellationRequested)
            {

            }
        } 
        private void disableButton_Click(object sender, RoutedEventArgs e)
        {
            source.Cancel();
            source.Dispose();
            script.Post("{\"type\": \"quit\"}");
            script.Post("{\"type\": \"playback\"}");
            script.Unload();
            session.Detach();
            enableButton.IsEnabled = true;
            Slot1R.IsEnabled = true;
            Slot2R.IsEnabled = true;
            Slot3R.IsEnabled = true;
            disableButton.IsEnabled = false;
        }

      
    }        
}
