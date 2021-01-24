using System;
using NUnit.Framework;
using TradingConsole.InputParser;

namespace TC_Tests
{
    public class InputParserTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BasicInputsWork()
        {
            string inputArgs = "Simulate --StockFilePath \"C:\\Users\\masdoc\\source\\repos\\StockTradingConsole\\bin\\NewTextDocument.xml\" --StartDate 1/1/2019 --EndDate 28/2/2020 --StartingCash 20000";
            string[] args = inputArgs.Split(' ');
            UserInputParser parser = new UserInputParser(TestHelper.ReportLogger);
            UserInputOptions tokens = parser.ParseUserInput(args);
            Assert.AreEqual(ProgramType.Simulate, tokens.FuntionType);
            Assert.AreEqual("\"C:\\Users\\masdoc\\source\\repos\\StockTradingConsole\\bin\\NewTextDocument.xml\"", tokens.StockFilePath);
            Assert.AreEqual(new DateTime(2019, 1, 1), tokens.StartDate);
            Assert.AreEqual(new DateTime(2020, 2, 28), tokens.EndDate);
            Assert.AreEqual(20000, tokens.StartingCash);
        }
    }
}