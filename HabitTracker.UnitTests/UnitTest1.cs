using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace HabitTracker.UnitTests
{
    public class Tests
    {
        [Test]
        public void UpdateEntryTest()
        {
            string input = "5\n1\n3\n10\n";
            using var strRdr = new StringReader(input);
            Console.SetIn(strRdr);

            HabitTracker.Program.Main(Array.Empty<string>());

            Assert.Pass();
        }
    }
}
