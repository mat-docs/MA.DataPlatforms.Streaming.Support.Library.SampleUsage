﻿// <copyright file="TimestampDataHandler.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System;
using System.Collections.Generic;
using System.Linq;

using MA.DataPlatforms.Streaming.Support.Lib.Core.Contracts.BufferingModule;
using MA.DataPlatforms.Streaming.Support.Lib.Core.Contracts.BufferingModule.Abstractions;
using MA.DataPlatforms.Streaming.Support.Lib.Core.Shared.Abstractions;
using MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.SqlRace;

using MESL.SqlRace.Domain;

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.Buffering;

internal class TimestampDataHandler : IHandler<ITimestampData>
{
    private readonly ILogger logger;
    private readonly ISqlSessionManager sessionManager;
    private readonly List<string> subscribedParameters;

    public TimestampDataHandler(ILogger logger, ISqlSessionManager sessionManager, List<string> subscribedParameters)
    {
        this.logger = logger;
        this.sessionManager = sessionManager;
        this.subscribedParameters = subscribedParameters;
    }

    /// <summary>
    ///     Here data is split per "Timestamp". Each timestamp contains samples for multiple parameters.
    /// </summary>
    /// <param name="obj">TimestampData from support library.</param>
    public void Handle(ITimestampData obj)
    {
        SqlRaceSession session;
        switch (obj)
        {
            case StartTimestampData startTimeStampData:
            {
                session = this.sessionManager.CreateSession(startTimeStampData.SessionKey);
                break;
            }
            case EndTimestampData endTimeStampData:
            {
                this.sessionManager.StopSession(endTimeStampData.SessionKey);
                return;
            }
            default:
            {
                var foundSession = this.sessionManager.GetSession(obj.SessionKey);
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
        var markers = obj.TimeColumns
            .SelectMany(x => x.SampleValues
                .Where(y => y.ValueType == typeof(MarkerData))
                .Select(z => z.Cast<MarkerData>()))
            .ToList();

        foreach (var marker in markers.OfType<MarkerData>())
        {
            try
            {
                session.ClientSession.Session.LapCollection.Add(new Lap(marker.Timestamp.ConvertTimestamp(), (short)marker.Value, 0, marker.Label, false));
            }
            catch (InvalidOperationException ex)
            {
                this.logger.Warning($"Unable to add laps due to {ex}");
            }
        }

        foreach (var timeColumn in obj.TimeColumns)
        {
            var channelIds = new List<uint>();
            var data = new List<byte>();
            foreach (var value in timeColumn.SampleValues)
            {
                if (!this.subscribedParameters.Contains(value.Identifier))
                {
                    continue;
                }

                var parameterIdentifier = value.Identifier.Split(':')[0] + ParameterConstants.BufferedTimestamp;
                channelIds.Add(session.ParameterChannelDictionary[parameterIdentifier]);
                data.AddRange(BitConverter.GetBytes(value.Cast<double>()));
            }

            if (channelIds.Count == 0)
            {
                continue;
            }

            session.ClientSession.Session.AddRowData(timeColumn.Timestamp.ConvertTimestamp(), channelIds, data.ToArray());
        }
    }
}