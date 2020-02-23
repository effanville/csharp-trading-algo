namespace TradingConsole.InputParser
{
    public class TextToken
    {
        public TextTokenType TokenType;
        public string Value;

        public TextToken(TextTokenType type, string value)
        {
            TokenType = type;
            Value = value;
        }
    }
}
