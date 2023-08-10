﻿using Microsoft.Extensions.DependencyInjection;
using FakeItEasy;
using SharedKernel.Events;
using SharedKernel.Features;
using StockTrader.API.Endpoints;
using StockTrader.Core.StockAggregate;
using StockTrader.Infrastructure;

namespace Stocks.Tests.Shared;

using StockTrader.HistoryManager;

public class MockTestHarness
{
    private IServiceProvider _serviceProvider;
    
    public IStockRepository MockStockRepository { get; private set; }
    public IEventBus MockEventBus { get; private set; }

    public MockTestHarness(IFeatureFlags featureFlags, bool useMocks = false)
    {
        var serviceCollection = new ServiceCollection();
        
        // Arrange
        MockStockRepository = A.Fake<IStockRepository>();
        
        MockEventBus = A.Fake<IEventBus>();

        serviceCollection.AddSharedServices(new SharedServiceOptions(true, true, true));

        serviceCollection.AddSingleton(MockStockRepository);
        serviceCollection.AddSingleton(MockEventBus);
        serviceCollection.AddSingleton(featureFlags);
        serviceCollection.AddSingleton<SetStockPriceEndpoint>();
        serviceCollection.AddSingleton<AddStockHistoryFunction>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public T GetService<T>()
    {
        return this._serviceProvider.GetRequiredService<T>();
    }
}