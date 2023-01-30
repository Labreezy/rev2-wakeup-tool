using System;
using System.Linq;
using GGXrdWakeupDPUtil.Library.Memory;
using NUnit.Framework;

namespace GGXrdWakeupDPUtil.UnitTests
{
    [TestFixture]
    public class MemoryPointerTests
    {

        [Test]
        public void IsMemoryPointer_Instantiated_Correctly()
        {
            var pointerName = "testPointer";
            var pointerValue = "0x123456|0x789|0x999";
            var memoryPointer = new MemoryPointer(pointerName, pointerValue);
            
            Assert.NotNull(memoryPointer);
            Assert.AreEqual(pointerName, memoryPointer.Name);
            Assert.AreEqual(new IntPtr(0x123456), memoryPointer.Pointer);
            CollectionAssert.AreEqual(new[] { 0x789, 0x999 }.AsEnumerable(), memoryPointer.Offsets);

            var pointerName2 = "testPointer2";
            var pointerValue2 = "0x123";
            var memoryPointer2 = new MemoryPointer(pointerName2, pointerValue2);
            
            Assert.NotNull(memoryPointer2);
            Assert.AreEqual(pointerName2, memoryPointer2.Name);
            Assert.AreEqual(new IntPtr(0x123), memoryPointer2.Pointer);
            Assert.IsNotNull(memoryPointer2.Offsets);
            Assert.IsEmpty(memoryPointer2.Offsets);
        }

        [Test]
        public void MemoryPointerShouldReturnExceptionWhenArgumentIsInvalid()
        {

            Assert.Throws<ArgumentException>(() =>
            {
                var memoryPointer = new MemoryPointer("", "0x123456|0x789|0x999");
            });

            Assert.Throws<ArgumentException>(() =>
            {
                var memoryPointer = new MemoryPointer("testPointer", "");
            });

            Assert.Throws<ArgumentException>(() =>
            {
                var memoryPointer = new MemoryPointer("testPointer", " ");
            });


            Assert.Throws<ArgumentException>(() =>
            {
                var memoryPointer = new MemoryPointer("testPointer", "0x123456,0x789,0x999");
            });
        }
    }
}