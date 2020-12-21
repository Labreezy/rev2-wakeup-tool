using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GGXrdWakeupDPUtil.Library;
using NUnit.Framework;

namespace GGXrdWakeupDPUtil.UnitTests
{
    [TestFixture]
    public class SlotInputTests
    {
        [TestCase("6,2,!3H", 2)]
        [TestCase("6,2,!3h", 2)]
        [TestCase("!3h", 0)]
        [TestCase("6,2,3h", -1)]
        [TestCase("6,!2,3h", 1)]
        [TestCase("5,5,5,5,5,5,5,5,5,5,5,5,5,5,!5,5,5,5,5,5,5,5,5,5,5", 14)]
        public void ReversalFrameIndex_Test(string input, int reversalFrameIndex)
        {
            //Arrange
            SlotInput slotInput = new SlotInput(input);

            //Act
            var result = slotInput.ReversalFrameIndex;

            //Assert 
            Assert.AreEqual(reversalFrameIndex, result);

        }

        [TestCase("6,2,!3H", "6,2,!3H")]
        [TestCase("6,2,!3h", "6,2,!3H")]
        [TestCase("!3h", "!3H")]
        [TestCase("6,2,3h", "6,2,3H")]
        [TestCase("6,!2,3h", "6,!2,3H")]
        [TestCase("5,5,5,5,5,5,5,5,5,5,5,5,5,5,!5,5,5,5,5,5,5,5,5,5,5", "5,5,5,5,5,5,5,5,5,5,5,5,5,5,!5,5,5,5,5,5,5,5,5,5,5")]
        [TestCase("6    ,2,!3H", "6,2,!3H")]
        [TestCase("6,    2,!3H", "6,2,!3H")]
        [TestCase("6,2,!3H   ", "6,2,!3H")]
        public void Input_Test(string input, string expectedInput)
        {
            //Arrange
            SlotInput slotInput = new SlotInput(input);

            //Act
            var result = slotInput.Input;

            //Assert 
            Assert.AreEqual(expectedInput, result);
        }


        [TestCase("1", new ushort[] { 0b0110 })]
        [TestCase("2", new ushort[] { 0b0010 })]
        [TestCase("3", new ushort[] { 0b1010 })]
        [TestCase("4", new ushort[] { 0b0100 })]
        [TestCase("5", new ushort[] { 0b0000 })]
        [TestCase("6", new ushort[] { 0b1000 })]
        [TestCase("7", new ushort[] { 0b0101 })]
        [TestCase("8", new ushort[] { 0b0001 })]
        [TestCase("9", new ushort[] { 0b1001 })]
        [TestCase("5P", new ushort[] { 0x10 })]
        [TestCase("5K", new ushort[] { 0x20 })]
        [TestCase("5S", new ushort[] { 0x40 })]
        [TestCase("5H", new ushort[] { 0x80 })]
        [TestCase("5D", new ushort[] { 0x100 })]
        [TestCase("5PK", new ushort[] { 0x30 })]
        [TestCase("5PKS", new ushort[] { 0x70 })]
        [TestCase("5PKSH", new ushort[] { 0xF0 })]
        [TestCase("5PKSHD", new ushort[] { 0x1F0 })]

        [TestCase("6,2,3H", new ushort[] { 0b1000, 0b0010, 0x8A })]
        [TestCase("6,2,!3H", new ushort[] { 0b1000, 0b0010, 0x8A })]


        public void InputList_Test(string input, IEnumerable<ushort> inputList)
        {
            //Arrange
            SlotInput slotInput = new SlotInput(input);

            //Act
            var result = slotInput.InputList;

            //Assert 
            Assert.AreEqual(inputList, result);
        }

        [TestCase("1", new ushort[] { 0, 0, 1, 0, 0b0110 })]
        [TestCase("2", new ushort[] { 0, 0, 1, 0, 0b0010 })]
        [TestCase("3", new ushort[] { 0, 0, 1, 0, 0b1010 })]
        [TestCase("4", new ushort[] { 0, 0, 1, 0, 0b0100 })]
        [TestCase("5", new ushort[] { 0, 0, 1, 0, 0b0000 })]
        [TestCase("6", new ushort[] { 0, 0, 1, 0, 0b1000 })]
        [TestCase("7", new ushort[] { 0, 0, 1, 0, 0b0101 })]
        [TestCase("8", new ushort[] { 0, 0, 1, 0, 0b0001 })]
        [TestCase("9", new ushort[] { 0, 0, 1, 0, 0b1001 })]
        [TestCase("5P", new ushort[] { 0, 0, 1, 0, 0x10 })]
        [TestCase("5K", new ushort[] { 0, 0, 1, 0, 0x20 })]
        [TestCase("5S", new ushort[] { 0, 0, 1, 0, 0x40 })]
        [TestCase("5H", new ushort[] { 0, 0, 1, 0, 0x80 })]
        [TestCase("5D", new ushort[] { 0, 0, 1, 0, 0x100 })]
        [TestCase("5PK", new ushort[] { 0, 0, 1, 0, 0x30 })]
        [TestCase("5PKS", new ushort[] { 0, 0, 1, 0, 0x70 })]
        [TestCase("5PKSH", new ushort[] { 0, 0, 1, 0, 0xF0 })]
        [TestCase("5PKSHD", new ushort[] { 0, 0, 1, 0, 0x1F0 })]

        [TestCase("6,2,3H", new ushort[] { 0, 0, 3, 0, 0b1000, 0b0010, 0x8A })]
        [TestCase("6,2,!3H", new ushort[] { 0, 0, 3, 0, 0b1000, 0b0010, 0x8A })]


        public void Content_Test(string input, IEnumerable<ushort> content)
        {
            //Arrange
            SlotInput slotInput = new SlotInput(input);

            //Act
            var result = slotInput.Content;

            //Assert 
            Assert.AreEqual(content, result);
        }

        [TestCase("6,2,!3H", true)]
        [TestCase("6,2,!3h", true)]
        [TestCase("!6,2,!3H", false)]
        [TestCase("6,,!3H", false)]
        [TestCase("6,2,!3PKSHD", true)]
        [TestCase("6,2,3H", false)]
        [TestCase("64,2,!3H", false)]
        [TestCase("6 ,2,!3H", true)]
        [TestCase("6,2        ,!3H", true)]
        [TestCase("6,2        ,3H", false)]
        [TestCase("6,2,3H", false)]
        [TestCase("6!,2,3H", false)]
        [TestCase("6,!2,3H", true)]
        [TestCase("!6,2,3H", true)]
        [TestCase("!6,2,!3H", false)]
        public void IsReversalValid_Test(string input, bool isReversalValid)
        {
            //Arrange
            SlotInput slotInput = new SlotInput(input);

            //Act
            var result = slotInput.IsReversalValid;

            //Assert

            Assert.AreEqual(isReversalValid, result);
        }

        [TestCase("6,2,!3H", true)]
        [TestCase("6,2,!3h", true)]
        [TestCase("!6,2,!3H", false)]
        [TestCase("6,,!3H", false)]
        [TestCase("6,2,!3PKSHD", true)]
        [TestCase("6,2,3H", true)]
        [TestCase("64,2,!3H", false)]
        [TestCase("6 ,2,!3H", true)]
        [TestCase("6,2        ,!3H", true)]
        [TestCase("6,2        ,3H", true)]
        [TestCase("6,2,3H", true)]
        [TestCase("6,2,!3H", true)]
        [TestCase("6!,2,3H", false)]
        [TestCase("6,!2,3H", true)]
        [TestCase("!6,2,3H", true)]
        [TestCase("!6,2,!3H", false)]
        public void IsValid_Test(string input, bool isValid)
        {
            //Arrange
            SlotInput slotInput = new SlotInput(input);

            //Act
            var result = slotInput.IsValid;

            //Assert

            Assert.AreEqual(isValid, result);
        }
    }
}
