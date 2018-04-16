using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Binarysharp.MemoryManagement;

namespace GGXrdWakeupDPUtil
{
    public class ReversalTool2 : IDisposable
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
            new NameWakeupData("Dizzy", 25, 24),
            new NameWakeupData("Baiken", 22, 21),
            new NameWakeupData("Answer", 24, 24)
        };

        private char FrameDelimiter = ',';
        private char WakeUpFrameDelimiter = '!';
        const int WakeupFrameMask = 0x200;

        #region Offsets
        private readonly int _p2IdOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2IdOffset"), 16);
        private readonly int _recordingSlotOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("RecordingSlotOffset"), 16);
        #endregion

        private const int RecordingSlotSize = 4808;


        private MemorySharp _memorySharp;



        public void AttachToProcess()
        {
            var process = Process.GetProcessesByName(_ggprocname).FirstOrDefault();

            if (process == null)
            {
                throw new Exception("GG process not found!");
            }

            _memorySharp = new MemorySharp(process);
        }

        public NameWakeupData GetDummy()
        {
            var index = _memorySharp.Read<byte>((IntPtr)_p2IdOffset);

            var result = _nameWakeupDataList[index];

            return result;
        }

        #region Dispose Members
        public void Dispose()
        {
            _memorySharp?.Dispose();
        }
        #endregion

        public void SetInputInSlot(int slotNumber, string input, int delay)
        {
            List<string> inputList = GetInputList(input);
            int wakeupFrameIndex = inputList.FindLastIndex(x => x.StartsWith(WakeUpFrameDelimiter.ToString()));

            if (wakeupFrameIndex < 0)
            {
                throw new Exception("No ! frame specified.  See the readme for more information.");
            }

            IEnumerable<short> inputShorts = GetInputShorts(inputList);

            OverwriteSlot(slotNumber, inputShorts);
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
            var addr = _recordingSlotOffset + RecordingSlotSize * slotNumber;
            var inputList = inputs as short[] ?? inputs.ToArray();
            var header = new List<short> { 0, 0, (short)inputList.Length, 0 };

            _memorySharp.Write((IntPtr)addr, header.Concat(inputList).ToArray(), false);

        }
        #endregion
    }
}
