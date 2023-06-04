namespace TradingSystem.ExchangeStructures
{
    public enum ExchangeSession : byte
    {
        OpenAuction,
        Continuous,
        IntraDayClose,
        IntraDayAuction,
        CloseAuction,
        Closed
    }
}