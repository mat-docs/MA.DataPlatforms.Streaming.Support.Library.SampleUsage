// <copyright file="ISqlSessionManager.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.SqlRace;

internal interface ISqlSessionManager
{
    void StopSession(string sessionKey);

    SqlRaceSession CreateSession(string sessionKey);

    SqlRaceSession? GetSession(string sessionKey);

    void Stop();
}