using NUnit.Framework;

namespace Generic.IrcClient.Tests
{
    [TestFixture]
    public class ProgramTest
    {
        [Test]
        public void Program()
        {
            var program = new Program();
            program.Main();
        }
    }
}
