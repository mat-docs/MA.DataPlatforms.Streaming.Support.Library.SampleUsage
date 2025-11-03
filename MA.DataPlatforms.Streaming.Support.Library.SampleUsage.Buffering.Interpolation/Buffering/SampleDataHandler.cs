// <copyright file="SampleDataHandler.cs" company="Motion Applied Ltd.">
// Copyright (c) Motion Applied Ltd.</copyright>

using System;
using System.Collections.Generic;
using System.Linq;

using MA.DataPlatforms.Streaming.Support.Lib.Core.Contracts.BufferingModule;
using MA.DataPlatforms.Streaming.Support.Lib.Core.Contracts.BufferingModule.Abstractions;
using MA.DataPlatforms.Streaming.Support.Lib.Core.Shared.Abstractions;
using MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.SqlRace;

using MESL.SqlRace.Domain;

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.Buffering;

internal class SampleDataHandler : IHandler<ISampleData>
{
    private readonly ILogger logger;
    private readonly ISqlSessionManager sqlSessionManager;
    private readonly List<string> subscribedParameters;

    public SampleDataHandler(ILogger logger, ISqlSessionManager sqlSessionManager, List<string> subscribedParameters)
    {
        this.logger = logger;
        this.sqlSessionManager = sqlSessionManager;
        this.subscribedParameters = subscribedParameters;
    }

    /// <summary>
    ///     Here data is split per "Parameter". Each SampleData is tied to one parameter and contains multiple samples and
    ///     timestamps.
    /// </summary>
    /// <param name="obj">Sample Data from the support library.</param>
    public void Handle(ISampleData obj)
    {
        SqlRaceSession session;
        switch (obj)
        {
            case StartSampleData sampleStartData:
            {
                session = this.sqlSessionManager.CreateSession(sampleStartData.SessionKey);
                break;
            }
            case EndSampleData sampleEndData:
            {
                this.sqlSessionManager.StopSession(sampleEndData.SessionKey);
                return;
            }
            default:
            {
                var foundSession = this.sqlSessionManager.GetSession(obj.SessionKey);
                if (foundSession is null)
                {
                    return;
                }

                session = foundSession;
                break;
            }
        }

        // Marker, Event, Errors, and CAN data can be included as part of the merged stream.
        // This is how you can filter them.
        var markers = obj.Values
            .Where(x => x.Value.ValueType == typeof(MarkerData))
            .Select(x => x.Cast<MarkerData>())
            .ToList();

        foreach (var marker in markers.OfType<MarkerData>())
        {
            try
            {
                session.ClientSession.Session.LapCollection.Add(new Lap(marker.Timestamp.ConvertTimestamp(), (short)marker.Value, 0, marker.Label, false));
            }
            catch (InvalidOperationException ex)
            {
                this.logger.Warning($"Unable to add laps to the session due to {ex}.");
            }
        }

        if (!this.subscribedParameters.Contains(obj.Identifier))
        {
            return;
        }

        var parameterName = obj.Identifier.Split(':')[0] + ParameterConstants.BufferedParameter;
        var channelId = session.ParameterChannelDictionary[parameterName];

        var values = obj.Values
            .Where(x => x.Value.ValueType != typeof(MarkerData)).ToList();

        var timestamps = values.Select(x => x.Timestamp.ConvertTimestamp()).ToArray();
        var samples = values.SelectMany(x => BitConverter.GetBytes(x.Value.Cast<double>())).ToArray();

        session.ClientSession.Session.AddRowData(channelId, timestamps, samples, sizeof(double), false);
    }
}