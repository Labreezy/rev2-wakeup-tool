using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;
using Binarysharp.MemoryManagement;
using GGXrdWakeupDPUtil.Library.Enums;

namespace GGXrdWakeupDPUtil.Library
{
    public class ReversalTool : IDisposable
    {
        private readonly Dispatcher _dispatcher;
        private readonly string _ggprocname = ConfigurationManager.AppSettings.Get("GGProcessName");


        private readonly List<NameWakeupData> _nameWakeupDataList = new List<NameWakeupData>
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

        private char FrameDelimiter = ',';
        private char WakeUpFrameDelimiter = '!';
        const int WakeupFrameMask = 0x200;

        #region Offsets
        private readonly IntPtr _p2IdOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2IdOffset"), 16));
        private readonly int _recordingSlotOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("RecordingSlotOffset"), 16);
        private readonly IntPtr _p1AnimStringPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1AnimStringPtr"), 16));
        private readonly int _p1AnimStringPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1AnimStringPtrOffset"), 16);
        private readonly IntPtr _p2AnimStringPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2AnimStringPtr"), 16));
        private readonly int _p2AnimStringPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2AnimStringPtrOffset"), 16);
        private readonly IntPtr _frameCountOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("FrameCountOffset"), 16));
        private readonly IntPtr _scriptOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("ScriptOffset"), 16));
        #endregion

        private readonly string FaceDownAnimation = "CmnActFDown2Stand";
        private readonly string FaceUpAnimation = "CmnActBDown2Stand";

        private const int RecordingSlotSize = 4808;


        private MemorySharp _memorySharp;

        private Frida.Script _script;
        private Frida.DeviceManager _deviceManager;
        private Frida.Device _device;
        private Frida.Session _session;

        private static bool _runReversalThread;
        private static readonly object RunReversalThreadLock = new object();

        #region Constructors
        public ReversalTool(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
        #endregion



        public void AttachToProcess()
        {
            var process = Process.GetProcessesByName(_ggprocname).FirstOrDefault();

            if (process == null)
            {
                throw new Exception("GG process not found!");
            }

            _memorySharp = new MemorySharp(process);


            CreateScript(_dispatcher, _memorySharp.Pid);
        }

        public NameWakeupData GetDummy()
        {
            try
            {
                var index = _memorySharp.Read<byte>(_p2IdOffset);

                var result = _nameWakeupDataList[index];

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public SlotInput SetInputInSlot(int slotNumber, string input)
        {
            List<string> inputList = GetInputList(input);
            int wakeupFrameIndex = inputList.FindLastIndex(x => x.StartsWith(WakeUpFrameDelimiter.ToString()));

            if (wakeupFrameIndex < 0)
            {
                throw new Exception("No ! frame specified.  See the readme for more information.");
            }

            IEnumerable<short> inputShorts = GetInputShorts(inputList);

            var enumerable = inputShorts as short[] ?? inputShorts.ToArray();
            OverwriteSlot(slotNumber, enumerable);

            return new SlotInput(input, enumerable, wakeupFrameIndex);
        }



        public void PlayReversal()
        {
            _script.Post("{\"type\": \"playback\"}");
        }



        public void StartReversalLoop(SlotInput slotInput)
        {
            lock (RunReversalThreadLock)
            {
                _runReversalThread = true;
            }

            Thread reversalThread = new Thread(() =>
            {
                var currentDummy = GetDummy();
                bool localRunReversalThread = true;
                while (localRunReversalThread)
                {
                    int wakeupTiming = GetWakeupTiming(currentDummy);


                    if (wakeupTiming != 0)
                    {
                        Thread waitThread = new Thread(() =>
                        {
                            int fc = FrameCount();
                            var frames = wakeupTiming - slotInput.WakeupFrameIndex - 1;
                            while (FrameCount() < fc + frames)
                            {
                            }
                        })
                        { Name = "waitThread" };
                        waitThread.Start();
                        waitThread.Join();


                        PlayReversal();
                    }


                    lock (RunReversalThreadLock)
                    {
                        localRunReversalThread = _runReversalThread;
                    }

                    Thread.Sleep(1);
                }
            })
            { Name = "reversalThread" };

            reversalThread.Start();
        }

        public void StopReversalLoop()
        {
            lock (RunReversalThreadLock)
            {
                _runReversalThread = false;
            }
        }

        public bool CheckValidInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var inputList = GetInputList(input);



            return inputList.All(x =>
            {
                Regex regex = new Regex(@"!{0,1}[1-9][PKSHD]*(?:,|$)");

                return regex.IsMatch(x);
            });
        }

        #region Private
        private List<string> GetInputList(string input)
        {
            Regex whitespaceregex = new Regex(@"\s+");

            var text = whitespaceregex.Replace(input, "");

            string[] splittext = text.Split(FrameDelimiter);

            return splittext.ToList();
        }

        private IEnumerable<short> GetInputShorts(List<string> inputList)
        {
            List<short> result = inputList.Select(x =>
            {
                var value = SingleInputParse(x);

                if (value < 0)
                {
                    throw new Exception("Invalid input with input '" + value + "'.  Read the README for formatting information.");
                }

                return value;
            }).ToList();

            int wakeupFrameIndex = inputList.FindLastIndex(x => x.StartsWith(WakeUpFrameDelimiter.ToString()));

            result[wakeupFrameIndex] -= WakeupFrameMask;

            return result;
        }

        private short SingleInputParse(string input)
        {
            Regex inputregex = new Regex(WakeUpFrameDelimiter + @"?[1-9]{1}[PKSHD]{0,5}");

            int[] directions = { 0b0110, 0b0010, 0b1010, 0b0100, 0b0000, 0b1000, 0b0101, 0b0001, 0b1001 };

            if (inputregex.IsMatch(input))
            {
                var result = 0;
                if (input[0] == WakeUpFrameDelimiter)
                {
                    result += WakeupFrameMask;
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
            }
            else
            {
                return -1;
            }
        }

        private void OverwriteSlot(int slotNumber, IEnumerable<short> inputs)
        {
            var addr = _recordingSlotOffset + RecordingSlotSize * (slotNumber - 1);
            var inputList = inputs as short[] ?? inputs.ToArray();
            var header = new List<short> { 0, 0, (short)inputList.Length, 0 };

            _memorySharp.Write((IntPtr)addr, header.Concat(inputList).ToArray(), false);

        }

        private string ReadAnimationString(int player)
        {
            if (player == 1)
            {
                var ptr = _memorySharp[_p1AnimStringPtr].Read<IntPtr>();

                return _memorySharp.ReadString(ptr + _p1AnimStringPtrOffset, false, 32);
            }

            if (player == 2)
            {
                var ptr = _memorySharp[_p2AnimStringPtr].Read<IntPtr>();

                return _memorySharp.ReadString(ptr + _p2AnimStringPtrOffset, false, 32);
            }

            return string.Empty;
        }

        private void CreateScript(Dispatcher dispatcher, int pid)
        {
            if (_script == null)
            {
                _deviceManager = new Frida.DeviceManager(dispatcher);
                _device = _deviceManager.EnumerateDevices().FirstOrDefault(x => x.Type == Frida.DeviceType.Local);



                if (_device == null)
                {
                    throw new Exception("Local device not found.This application will now close.");
                }

                _session = _device.Attach((uint)pid);


                var src =
                    @"var xrdbase = Module.findBaseAddress('GuiltyGearXrd.exe');
                    var hookaddr = xrdbase.add(" + "0x" + _scriptOffset.ToString("x") + @");
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
                        }, 0);";

                _script = _session.CreateScript(src);
                _script.Load();


            }
        }

        private int FrameCount()
        {
            return _memorySharp.Read<int>(_frameCountOffset);
        }
        private int GetWakeupTiming(NameWakeupData currentDummy)
        {
            var animationString = ReadAnimationString(2);

            if (animationString == FaceDownAnimation)
            {
                return currentDummy.FaceDownFrames;
            }
            if (animationString == FaceUpAnimation)
            {
                return currentDummy.FaceUpFrames;
            }

            return 0;

        }
        #endregion

        #region Dispose Members
        public void Dispose()
        {
            StopReversalLoop();

            _memorySharp?.Dispose();


            _script.Post("{\"type\": \"quit\"}");
            _script.Post("{\"type\": \"playback\"}");
            _script.Unload();
            _session.Detach();


            _script?.Dispose();
            _deviceManager?.Dispose();
            _device?.Dispose();
            _session?.Dispose();
        }
        #endregion


    }
}
