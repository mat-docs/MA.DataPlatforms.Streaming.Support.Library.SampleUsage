// <copyright file="InterpolationResultHandler.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using MA.DataPlatforms.Streaming.Support.Lib.Core.Modules.Interpolation.Abstractions;
using MA.DataPlatforms.Streaming.Support.Lib.Core.Modules.Interpolation.Contracts;
using MA.DataPlatforms.Streaming.Support.Lib.Core.Shared.Abstractions;

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation;

internal class InterpolationResultHandler : IBatchResultHandler
{
    private readonly ILogger logger;
    private readonly string subscriptionKey;
    private readonly ISqlSessionManager sessionManager;

    public InterpolationResultHandler(ILogger logger, string subscriptionKey, ISqlSessionManager sessionManager)
    {
        this.logger = logger;
        this.subscriptionKey = subscriptionKey;
        this.sessionManager = sessionManager;
    }

    /// <summary>
    /// This is a batch of results from the interpolation from the processor.
    /// Depending on the ProcessResult it can be cast to the final Result object for analysis. 
    /// </summary>
    /// <param name="obj">Batched Result from the support library after it has been processed by the processor.</param>
    public void Handle(BatchResult obj)
    {
        if (obj.SubscriptionKey != this.subscriptionKey)
        {
            this.logger.Info($"Mismatch subscription key found. Received subscription key: {obj.SubscriptionKey}. Expected subscription key: {this.subscriptionKey}.");
            return;
        }

        var session = this.sessionManager.GetSession(obj.SessionKey);
        if (session is null)
        {
            return;
        }

        var results = obj.Results.Select(x => (DefaultResult)x).ToList();

        foreach (var result in results)
        {
            var parameterName = result.Identifier.Split(':')[0];
            var channelIds = new List<uint>
            {
                session.ParameterChannelDictionary[parameterName + ParameterConstants.InterpolatedMin],
                session.ParameterChannelDictionary[parameterName + ParameterConstants.InterpolatedMax],
                session.ParameterChannelDictionary[parameterName + ParameterConstants.InterpolatedFirst],
                session.ParameterChannelDictionary[parameterName + ParameterConstants.InterpolatedLast],
                session.ParameterChannelDictionary[parameterName + ParameterConstants.InterpolatedMean]
            };
            var data = new List<byte>();
            for (var i = 0; i < 5; i++)
            {
                var sampleBytes = i switch
                {
                    0 => BitConverter.GetBytes(result.Min ?? 0),
                    1 => BitConverter.GetBytes(result.Max ?? 0),
                    2 => BitConverter.GetBytes(result.First ?? 0),
                    3 => BitConverter.GetBytes(result.Last ?? 0),
                    4 => BitConverter.GetBytes(result.Mean ?? 0),
                    _ => []
                };
                data.AddRange(sampleBytes);
            }

            session.ClientSession.Session.AddRowData(result.IntervalStartTimeNano.ConvertTimestamp(), channelIds, data.ToArray());
        }
    }
}