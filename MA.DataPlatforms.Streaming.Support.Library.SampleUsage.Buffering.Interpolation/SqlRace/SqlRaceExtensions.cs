// <copyright file="SqlRaceExtensions.cs" company="Motion Applied Ltd.">
// Copyright (c) Motion Applied Ltd.</copyright>

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.SqlRace;

internal static class SqlRaceExtensions
{
    private const long NumberOfNanosecondsInDay = 86400000000000;

    public static long ConvertTimestamp(this ulong unixTimestamp)
    {
        return (long)unixTimestamp % NumberOfNanosecondsInDay;
    }
}