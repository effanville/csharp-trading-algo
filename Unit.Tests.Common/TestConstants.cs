using System;

namespace TradingConsole.Tests
{
    public sealed class TestConstants
    {
        public static readonly string CurrentPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        public const string ExampleFilesFolder = "ExampleFiles";

        public static readonly string ExampleFilesLocation = $"{CurrentPath}/{ExampleFilesFolder}";
    }
}
