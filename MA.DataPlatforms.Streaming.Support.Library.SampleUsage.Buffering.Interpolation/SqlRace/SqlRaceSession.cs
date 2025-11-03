// <copyright file="SqlRaceSession.cs" company="Motion Applied Ltd.">
// Copyright (c) Motion Applied Ltd.</copyright>

using System.Collections.Generic;

using MESL.SqlRace.Domain;

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.SqlRace;

internal class SqlRaceSession
{
    public readonly Dictionary<string, uint> ParameterChannelDictionary;
    public readonly IClientSession ClientSession;

    public SqlRaceSession(IClientSession clientSession, Dictionary<string, uint> parameterChannelDictionary)
    {
        this.ClientSession = clientSession;
        this.ParameterChannelDictionary = parameterChannelDictionary;
    }
}