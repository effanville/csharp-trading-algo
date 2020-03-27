using FinancialStructures.ReportLogging;
using StringFunctions;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (tokens.Any(tokens => tokens.TokenType == TextTokenType.Error))
            {
                return new UserInputOptions();
            }

            return GenerateOptionsFromInputs(tokens, reportLogger);
        }

        /// <summary>
        /// From command line inputs, converts into types of input and the value specified.
        /// </summary>
        private static List<TextToken> ParseInput(string[] args, LogReporter reportLogger)
        {
            var outputTokens = new List<TextToken>();

            outputTokens.Add(DetermineProgramType(args[0]));

            for (int i = 1; i < args.Length; i++)
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

        private static TextToken DetermineProgramType(string argument)
        {
            foreach (ProgramType type in Enum.GetValues(typeof(ProgramType)))
            {
                if (argument == type.ToString())
                {
                    return new TextToken(TextTokenType.ProgramType, argument);
                }
            }

            return new TextToken(TextTokenType.Error, argument);
        }

        private static UserInputOptions GenerateOptionsFromInputs(List<TextToken> inputTokens, LogReporter reportLogger)
        {
            var inputs = new UserInputOptions();
            foreach (var token in inputTokens)
            {
                switch (token.TokenType)
                {
                    case (TextTokenType.ProgramType):
                        {
                            foreach (ProgramType type in Enum.GetValues(typeof(ProgramType)))
                            {
                                if (token.Value == type.ToString())
                                {
                                    inputs.funtionType = type;
                                }
                            }
                            break;
                        }
                    case TextTokenType.ParameterFilePath:
                        {
                            break;
                        }
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
                    case TextTokenType.StartingCash:
                        {
                            inputs.StartingCash = double.Parse(token.Value);
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
                    case TextTokenType.TradingGap:
                        {
                            inputs.TradingGap = TimeSpan.Parse(token.Value);
                            break;
                        }
                    case TextTokenType.DecisionSystemType:
                        {
                            inputs.DecisionType = token.Value.ToEnum<DecisionSystemType>();
                            break;
                        }
                    case TextTokenType.BuySellType:
                        {
                            inputs.BuyingSellingType = token.Value.ToEnum<BuySellType>();
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
