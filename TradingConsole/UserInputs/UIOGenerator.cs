using FinancialStructures.ReportLogging;
using System;
using System.Collections.Generic;

namespace TradingConsole.InputParser
{
    public static class UIOGenerator
    {
        public static string parameterArg = "--";

        private static TextTokenType TokenTypeSelector(string arg)
        {
            foreach (TextTokenType type in Enum.GetValues(typeof(TextTokenType)))
            {
                string parameter = arg.Remove(0, 2);
                if (parameter == type.ToString())
                {
                    return type;
                }
            }

            return TextTokenType.Error;
        }

        public static UserInputOptions ParseUserInput(string[] args, LogReporter reportLogger)
        {
            if (args.Length == 0)
            {
                return new UserInputOptions();
            }

            List<TextToken> tokens = ParseInput(args, reportLogger);

            return GenerateOptionsFromInputs(tokens, reportLogger);
        }

        /// <summary>
        /// From command line inputs, converts into types of input and the value specified.
        /// </summary>
        private static List<TextToken> ParseInput(string[] args, LogReporter reportLogger)
        {
            var outputTokens = new List<TextToken>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith(parameterArg))
                {
                    if (i < args.Length - 1 && !args[i + 1].StartsWith(parameterArg))
                    {
                        outputTokens.Add(new TextToken(TokenTypeSelector(args[i]), args[i + 1]));
                    }
                    else
                    {
                        outputTokens.Add(new TextToken(TextTokenType.Error, TokenTypeSelector(args[i]).ToString() + " - NoValueSelected"));
                        reportLogger.LogError("Parsing", "Token does not have proper value");
                    }
                }
            }

            return outputTokens;
        }

        private static UserInputOptions GenerateOptionsFromInputs(List<TextToken> inputTokens, LogReporter reportLogger)
        {
            var inputs = new UserInputOptions();
            foreach (var token in inputTokens)
            {
                switch (token.TokenType)
                {
                    case (TextTokenType.StockFilePath):
                        {
                            inputs.StockFilePath = token.Value;
                            break;
                        }
                    case (TextTokenType.PortfolioFilePath):
                        {
                            inputs.PortfolioFilePath = token.Value;
                            break;
                        }
                    case (TextTokenType.Download):
                        {
                            if (token.Value == "all")
                            {
                                inputs.funtionType = ProgramType.DownloadAll;
                            }
                            else
                            {
                                inputs.funtionType = ProgramType.DownloadLatest;
                            }
                            break;
                        }
                    case (TextTokenType.Configure):
                        {
                            inputs.funtionType = ProgramType.Configure;
                            inputs.StockFilePath = token.Value;
                            break;
                        }
                    case (TextTokenType.Simulate):
                        {
                            inputs.funtionType = ProgramType.Simulate;
                            break;
                        }
                    case TextTokenType.StartDate:
                        {
                            inputs.StartDate = DateTime.Parse(token.Value);
                            break;
                        }
                    case TextTokenType.EndDate:
                        {
                            inputs.EndDate = DateTime.Parse(token.Value);
                            break;
                        }
                    case TextTokenType.StartingCash:
                        {
                            inputs.StartingCash = double.Parse(token.Value);
                            break;
                        }
                    case TextTokenType.TradingGap:
                        {
                            inputs.TradingGap = TimeSpan.Parse(token.Value);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            return inputs;
        }
    }
}
