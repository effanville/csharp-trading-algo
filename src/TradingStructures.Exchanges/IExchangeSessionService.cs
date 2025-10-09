using System;

using Effanville.TradingStructures.Common.Services;

namespace Effanville.TradingStructures.Exchanges;

public interface IExchangeSessionService : IService
{
    event EventHandler<ExchangeStatusChangedEventArgs>? ExchangeStatusChanged;
}