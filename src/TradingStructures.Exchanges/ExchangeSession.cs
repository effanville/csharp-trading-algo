namespace Effanville.TradingStructures.Exchanges
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