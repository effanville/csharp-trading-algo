using Effanville.FinancialStructures.Stocks.Signals;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Pricing;

namespace Effanville.TradingStructures.Signals;

public sealed class SignalEventHandler<TSignal, TState, TSignalResponse>
    : ISignalEventHandler<TSignal, TState, TSignalResponse>, IDisposable
    where TSignal : ISignal<TState>
    where TState : ISignalState
{
    private readonly TSignal _signal;
    private readonly TState _state;
    private readonly IClock _clock;
    private readonly IPriceService _priceService;
    private bool _disposedValue;

    public SignalEventHandler(IClock clock, IPriceService priceService)
    {
        _clock = clock;
        _priceService = priceService;
        _priceService.PriceChanged += OnPriceChanged;
    }

    private void OnPriceChanged(object? sender, PriceUpdateEventArgs e)
        => _signal.OnCandleFinished(_clock.Now(), e.Candle, _state);

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _priceService.PriceChanged -= OnPriceChanged;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void Publish() => throw new NotImplementedException();
}
