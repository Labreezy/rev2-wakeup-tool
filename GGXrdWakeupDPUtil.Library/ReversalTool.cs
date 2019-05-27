﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            new NameWakeupData("Dizzy", 30, 24),
            new NameWakeupData("Baiken", 25, 21),
            new NameWakeupData("Answer", 25, 25)
        };

        private char FrameDelimiter = ',';
        private char WakeUpFrameDelimiter = '!';
        private const int WakeupFrameMask = 0x200;

        #region Offsets

        private readonly IntPtr _p2IdOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2IdOffset"), 16));
        private readonly IntPtr _recordingSlotPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("RecordingSlotPtr"), 16));
        private readonly IntPtr _p1AnimStringPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1AnimStringPtr"), 16));
        private readonly int _p1AnimStringPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1AnimStringPtrOffset"), 16);
        private readonly IntPtr _p2AnimStringPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2AnimStringPtr"), 16));
        private readonly int _p2AnimStringPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2AnimStringPtrOffset"), 16);
        private readonly IntPtr _frameCountOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("FrameCountOffset"), 16));
        private readonly IntPtr _scriptOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("ScriptOffset"), 16));

        #endregion Offsets

        private readonly string FaceDownAnimation = "CmnActFDown2Stand";
        private readonly string FaceUpAnimation = "CmnActBDown2Stand";

        private const int RecordingSlotSize = 4808;
        private byte[] _remoteCodeAOB;
        private MemorySharp _memorySharp;
        private Binarysharp.MemoryManagement.Memory.RemoteAllocation _newmem;
        private Binarysharp.MemoryManagement.Memory.RemoteAllocation _flagmem;
        private IntPtr _newmembase;
        private IntPtr _flagmembase;
        private static bool _runReversalThread;
        private static readonly object RunReversalThreadLock = new object();
        private IntPtr _nonRelativeScriptOffset;
        private static bool _written = false;

        public void AttachToProcess()
        {
            var process = Process.GetProcessesByName(_ggprocname).FirstOrDefault();

            if (process == null)
            {
                throw new Exception("GG process not found!");
            }

            _memorySharp = new MemorySharp(process);
            _nonRelativeScriptOffset = IntPtr.Add(_memorySharp.Modules.MainModule.BaseAddress, (int)_scriptOffset);
            _newmem = _memorySharp.Memory.Allocate(128);
            _newmembase = _newmem.Information.AllocationBase;
            _flagmem = _memorySharp.Memory.Allocate(128);
            _flagmembase = _flagmem.Information.AllocationBase;
            var remoteASMstring = String.Format("mov ebp,[eax+0x40]\n" + "mov ebp,[ebp+0x0C]\n" + "cmp edi,3\n" + "jne 0x{0}\n" + "cmp BYTE [0x{2}], 1\n" + "je 0x{3}\n" +
                "mov DWORD [0x{4}], 0x200\n" + "and DWORD [0x{4}], eax\n" + "cmp DWORD [0x{4}], 0x200\n" + "jne 0x{0}\n" + "mov DWORD [0x{4}], eax\n" + "mov BYTE [0x{2}], 1\n" + "jmp 0x{0}\n" +
                "cmp DWORD [0x{4}], eax\n" + "jne 0x{0}\n" + "cmp BYTE [0x{1}],0\n" + "jne 0x{0}\n" + "mov ebp,[edx]\n" + "mov BYTE [0x{1}], 1\n" + "jmp 0x{0}",
                (_nonRelativeScriptOffset.ToInt32() + 6).ToString("X8"), _flagmembase.ToString("X8"), IntPtr.Add(_flagmembase, 1).ToString("X8"), IntPtr.Add(_newmembase, 0x49).ToString("X8"), IntPtr.Add(_flagmembase, 4).ToString("X8"));
            _remoteCodeAOB = _memorySharp.Assembly.Assembler.Assemble(remoteASMstring, _newmembase);
            _memorySharp.Write<byte>(_newmembase, _remoteCodeAOB, false);
        }

        public NameWakeupData GetDummy()
        {
            var index = _memorySharp.Read<byte>(_p2IdOffset);

            var result = _nameWakeupDataList[index];

            return result;
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

        public void waitAndReversal(SlotInput slotInput, int wakeupTiming)
        {
            int fc = FrameCount();
            var frames = wakeupTiming - slotInput.WakeupFrameIndex - 1;
            while (FrameCount() < fc + frames)
            {
            }
            lock (_memorySharp)
            {
#if DEBUG
                Console.WriteLine("Reversal!");
#endif
                _memorySharp.Write<byte>(_flagmembase, 0, false);
                Thread.Sleep(320); //20 frames, approximately, it's actually 333.333333333 ms.  Nobody should be able to be knocked down and get up in this time, causing the code to execute again.
#if DEBUG
                Console.WriteLine("Reversal Wait Finished!");
#endif
            }
        }

        public void StartReversalLoop(SlotInput slotInput, Action errorAction = null)
        {
            lock (RunReversalThreadLock)
            {
                _runReversalThread = true;
            }

            Thread reversalThread = new Thread(() =>
            {
                var currentDummy = GetDummy();
                bool localRunReversalThread = true;
                _memorySharp.Write<byte>(_flagmembase, 1, false);
                _written = false;
                _memorySharp.Assembly.Inject(String.Format("jmp 0x{0}\nnop", _newmembase.ToString("X8")), _nonRelativeScriptOffset);
                while (localRunReversalThread)
                {
                    try
                    {
                        int wakeupTiming = GetWakeupTiming(currentDummy);

                        if (wakeupTiming != 0 && !_written)
                        {
                            waitAndReversal(slotInput, wakeupTiming);
                        }
                    }
                    catch (Win32Exception)
                    {
                        errorAction?.Invoke();
                    }

                    lock (RunReversalThreadLock)
                    {
                        localRunReversalThread = _runReversalThread;
                    }

                    Thread.Sleep(1);
                }

#if DEBUG
                Console.WriteLine("reversalThread ended");
#endif
            })
            { Name = "reversalThread" };

            reversalThread.Start();

            _memorySharp.Windows.MainWindow.Activate();
        }

        public void StopReversalLoop()
        {
            lock (RunReversalThreadLock)
            {
                _runReversalThread = false;
                _memorySharp.Assembly.Inject(new string[] { "mov ebp,[eax+0x40]", "mov ebp,[ebp+0x0C]" }, _nonRelativeScriptOffset);
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
                Regex regex = new Regex(@"^!{0,1}[1-9][PKSHDpksdh]*(?:,|$)");

                return regex.IsMatch(x);
            })
            && inputList.Where(x =>
            {
                Regex regex = new Regex(@"^!{1}[1-9][PKSHDpksdh]*(?:,|$)");
                return regex.IsMatch(x);
            }).Count() == 1
            ;
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
                        case 'p':
                            result |= (int)Buttons.P;
                            break;

                        case 'K':
                        case 'k':
                            result |= (int)Buttons.K;
                            break;

                        case 'S':
                        case 's':
                            result |= (int)Buttons.S;
                            break;

                        case 'H':
                        case 'h':
                            result |= (int)Buttons.H;
                            break;

                        case 'D':
                        case 'd':
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
            var ptr = _memorySharp[_recordingSlotPtr].Read<IntPtr>();

            var slotAddr = ptr + RecordingSlotSize * (slotNumber - 1);

            var inputList2 = inputs as short[] ?? inputs.ToArray();
            var header2 = new List<short> { 0, 0, (short)inputList2.Length, 0 };

            _memorySharp.Write(slotAddr, header2.Concat(inputList2).ToArray(), false);
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

        #endregion Private

        #region Dispose Members

        public void Dispose()
        {
            StopReversalLoop();
            _memorySharp?.Dispose();
        }

        #endregion Dispose Members
    }
}