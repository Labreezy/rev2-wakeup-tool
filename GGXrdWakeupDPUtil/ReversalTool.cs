using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Binarysharp.MemoryManagement;

namespace GGXrdWakeupDPUtil
{
    public class ReversalTool
    {
        private readonly string _ggprocname = "GuiltyGearXrd";
        private readonly MemorySharp _memorySharp;

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

        #region Offsets

        private readonly IntPtr _p1AnimStringPtr = new IntPtr(0x1B18C78);
        private readonly int _p1AnimStringPtrOffset = 0x244C;

        private const int P2IdOffset = 0x1BDBE08;
        private readonly IntPtr _p2AnimStringPtr = new IntPtr(0x1B18C7C);
        private readonly int _p2AnimStringPtrOffset = 0x244C;


        const int RecordingSlotSize = 4808;

        #endregion

        const int WakeupFrameMask = 0x200;


        public ReversalTool()
        {
            var process = Process.GetProcessesByName(_ggprocname).FirstOrDefault();

            if (process != null)
            {
                _memorySharp = new MemorySharp(process);
            }
        }


        public bool CheckGuiltyGearXrdProcess()
        {
            return _memorySharp != null;
        }

        public NameWakeupData GetDummy()
        {
            var idx = _memorySharp.Read<byte>((IntPtr)P2IdOffset);

            var result = _nameWakeupDataList[idx];

            return result;
        }


        public string ReadAnimString(int player)
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

        public void OverwriteSlot(int slotNumber, IEnumerable<short> inputs)
        {
            var addr = 0x1BE4F58 + RecordingSlotSize * slotNumber;
            var inputList = inputs as short[] ?? inputs.ToArray();
            var header = new List<short> { 0, 0, (short)inputList.Length, 0 };

            _memorySharp.Write((IntPtr)addr, header.Concat(inputList).ToArray(), false);

        }

        public int OverwriteSlot(int slotNumber, string input)
        {
            List<short> inputList = CreateInputList(input, out var wakeupframeidx);

            OverwriteSlot(slotNumber, inputList);

            return wakeupframeidx;
        }


        //TODO set private
        public List<short> CreateInputList(string text, out int wakeupframeidx)
        {
            Regex _whitespaceregex = new Regex(@"\s+");
            char FrameDelimiter = ',';

            text = _whitespaceregex.Replace(text, "");
            string[] splittext = text.Split(new char[] { FrameDelimiter });
            List<short> inputnums = new List<short> { };
            foreach (string inputstr in splittext)
            {
                inputnums.Add(SingleInputParse(inputstr));
                if (inputnums.Last() < 0)
                {
                    throw new Exception("Invalid input with input '" + inputstr + "'.  Read the README for formatting information.");
                }
            }

            wakeupframeidx = inputnums.FindLastIndex(x => x >= 0x200);


            if (wakeupframeidx < 0)
            {
                throw new Exception("No ! frame specified.  See the readme for more information.");
            }
            inputnums[wakeupframeidx] -= WakeupFrameMask;

            return inputnums;
        }

        private short SingleInputParse(string input)
        {
            Regex _inputregex = new Regex(@"!?[1-9]{1}[PKSHD]{0,5}");

            int[] _directions = { 0b0110, 0b0010, 0b1010, 0b0100, 0b0000, 0b1000, 0b0101, 0b0001, 0b1001 };

            if (_inputregex.IsMatch(input))
            {
                var result = 0;
                if (input[0] == '!')
                {
                    result += WakeupFrameMask;
                    input = input.Substring(1);
                }
                var direction = Int32.Parse(input.Substring(0, 1));
                result |= _directions[direction - 1];
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

        public int FrameCount()
        {
            return _memorySharp.Read<int>((IntPtr)0x1BD1F90);
        }

        //TODO Remove
        public int GetPid()
        {
            return _memorySharp.Pid;
        }
    }
}
