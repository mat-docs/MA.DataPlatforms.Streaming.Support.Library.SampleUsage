// <copyright file="LinearInterpolationHandler.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using MA.DataPlatforms.Streaming.Support.Lib.Core.Contracts.InterpolationModule;
using MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.SqlRace;

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.Interpolation.Custom;

internal sealed class LinearInterpolationHandler : IBatchResultHandler
{
    private readonly string subscriptionKey;
    private readonly ISqlSessionManager sessionManager;

    public LinearInterpolationHandler(string subscriptionKey, ISqlSessionManager sessionManager)
    {
        this.subscriptionKey = subscriptionKey;
        this.sessionManager = sessionManager;
    }

    /// <summary>
    /// A Custom handler that handles the custom LinearInterpolationResult DTO.
    /// </summary>
    /// <param name="obj">Batch Results from the Support Library.</param>
    public void Handle(BatchResult obj)
    {
        if (obj.SubscriptionKey != this.subscriptionKey)
        {
            return;
        }

        var session = this.sessionManager.GetSession(obj.SessionKey);
        if (session is null)
        {
            return;
        }

        var linearInterpolationResults = obj.Results
            .OfType<LinearInterpolationResult>()
            .ToList();

        foreach (var result in linearInterpolationResults)
        {
            if (result.InterpolatedSamples.Count == 0)
            {
                continue;
            }

            var parameterName = result.Identifier.Split(':')[0] + ParameterConstants.LinearInterpolation;
            var channelId = session.ParameterChannelDictionary[parameterName];
            var data = result.InterpolatedSamples.SelectMany(BitConverter.GetBytes).ToArray();
            var timestamps = result.Timestamps.Select(x => x.ConvertTimestamp()).ToArray();
            session.ClientSession.Session.AddRowData(channelId, timestamps, data, sizeof(double), false);
        }
    }
}