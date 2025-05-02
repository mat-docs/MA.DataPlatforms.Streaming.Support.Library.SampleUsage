// <copyright file="SqlRaceSession.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

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