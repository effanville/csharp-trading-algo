using NUnit.Framework;
using TradingConsole;

namespace TC_Tests
{
    public class EndToEndTests
    {
        [Test]
        public void BasicRun()
        {
            string argsFlat = "Simulate --StockFilePath \"C:\\Users\\masdoc\\source\\repos\\StockTradingConsole\\bin\\NewTextDocument.xml\" --StartDate 1/1/2019 --EndDate 28/2/2020 --StartingCash 20000";
            string[] args = argsFlat.Split(' ');
            Program.Main(args);
        }
    }
}
