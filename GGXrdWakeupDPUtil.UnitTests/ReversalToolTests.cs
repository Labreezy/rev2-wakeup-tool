using System.Windows.Threading;
using GGXrdWakeupDPUtil.Library;
using NUnit.Framework;

namespace GGXrdWakeupDPUtil.UnitTests
{
    [TestFixture]
    public class ReversalToolTests
    {
        [TestCase("6,2,!3H", true)]
        [TestCase("6,2,!3h", true)]
        [TestCase("!6,2,!3H", false)]
        [TestCase("6,,!3H", false)]
        [TestCase("6,2,!3PKSHD", true)]
        [TestCase("6,2,3H", false)]
        [TestCase("64,2,!3H", false)]
        public void CheckValidInput_Test(string input, bool isValid)
        {
            //Arrange
            ReversalTool reversalTool = new ReversalTool(Dispatcher.CurrentDispatcher);

            //Act
            var result = reversalTool.CheckValidInput(input);

            //Assert

            Assert.AreEqual(isValid, result);
        }
    }
}
