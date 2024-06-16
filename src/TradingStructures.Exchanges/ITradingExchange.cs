using System;

using Effanville.TradingStructures.Common.Services;

namespace Effanville.TradingStructures.Exchanges;

public interface ITradingExchange : IService
{
    event EventHandler<ExchangeStatusChangedEventArgs>? ExchangeStatusChanged;
}