// <copyright file="ISqlSessionManager.cs" company="Motion Applied Ltd.">
// Copyright (c) Motion Applied Ltd.</copyright>

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.SqlRace;

internal interface ISqlSessionManager
{
    void StopSession(string sessionKey);

    SqlRaceSession CreateSession(string sessionKey);

    SqlRaceSession? GetSession(string sessionKey);

    void Stop();
}