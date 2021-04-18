using System;
using System.Collections.Generic;
using System.Linq;
using StructureCommon.Extensions;
using StructureCommon.Reporting;
using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using TradingConsole.Statistics;

namespace TradingConsole.InputParser
{
    /// <summary>
    /// Contains logic for interpreting user inputs.
    /// </summary>
    public class UserInputParser
    {
        private readonly IReportLogger ReportLogger;

        /// <summary>
        /// Default parameter argument extension.
        /// </summary>
        public const string parameterArg = "--";

        /// <summary>
        /// Create an instance.
        /// </summary>
        /// <param name="reportLogger"></param>
        public UserInputParser(IReportLogger reportLogger)
        {
            ReportLogger = reportLogger;
        }

        /// <summary>
        /// Determines if the inputs specified are suitable.
        /// </summary>
        public bool EnsureInputsSuitable(UserInputOptions inputOptions)
        {
            switch (inputOptions.FuntionType)
            {
                case ProgramType.Simulate:
                case ProgramType.Trade:
                {
                    return true;
                }

                default:
                    return false;
            }
        }

        /// <summary>
        /// Analyses the inputs specified and returns the outputs from this.
        /// </summary>
        public UserInputOptions ParseUserInput(string[] args)
        {
            if (args.Length == 0)
            {
                return new UserInputOptions();
            }

            List<TextToken> tokens = ParseInput(args);
            if (tokens.Any(tokens => tokens.TokenType == TextTokenType.Error))
            {
                return new UserInputOptions();
            }

            return GenerateOptionsFromInputs(tokens);
        }

        /// <summary>
        /// From command line inputs, converts into types of input and the value specified.
        /// </summary>
        private List<TextToken> ParseInput(string[] args)
        {
            List<TextToken> outputTokens = new List<TextToken>
            {
                DetermineProgramType(args[0])
            };

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
                        _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Parsing, "Token does not have proper value");
                    }
                }
            }

            return outputTokens;
        }

        private TextTokenType TokenTypeSelector(string arg)
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

        private TextToken DetermineProgramType(string argument)
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

        private UserInputOptions GenerateOptionsFromInputs(List<TextToken> inputTokens)
        {
            UserInputOptions inputs = new UserInputOptions();
            foreach (TextToken token in inputTokens)
            {
                switch (token.TokenType)
                {
                    case (TextTokenType.ProgramType):
                    {
                        foreach (ProgramType type in Enum.GetValues(typeof(ProgramType)))
                        {
                            if (token.Value == type.ToString())
                            {
                                inputs.FuntionType = type;
                                break;
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
                    case TextTokenType.DecSysParams:
                    {
                        List<StatisticType> list = new List<StatisticType>();
                        string[] stringValues = token.Value.Split(',');
                        foreach (string value in stringValues)
                        {
                            StatisticType val = value.ToEnum<StatisticType>();
                            list.Add(val);
                        }
                        inputs.decisionSystemStats = list;
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
