// <copyright file="LinearInterpolationResult.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System.Collections.Generic;

using MA.DataPlatforms.Streaming.Support.Lib.Core.Contracts.InterpolationModule;

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.Interpolation.Custom;

/// <summary>
/// Custom DTO that implements the IProcessResult interface which allows it to be used by the Support Library.
/// </summary>
internal sealed class LinearInterpolationResult : IProcessResult
{
    public LinearInterpolationResult(
        string identifier,
        ulong intervalStartTimeNano,
        ulong intervalEndTimeNano,
        IList<double> interpolatedSamples,
        IList<ulong> timestamps)
    {
        this.Identifier = identifier;
        this.IntervalStartTimeNano = intervalStartTimeNano;
        this.IntervalEndTimeNano = intervalEndTimeNano;
        this.InterpolatedSamples = interpolatedSamples;
        this.Timestamps = timestamps;
    }

    public IList<double> InterpolatedSamples { get; }

    public IList<ulong> Timestamps { get; }

    public string Identifier { get; }

    public ulong IntervalStartTimeNano { get; }

    public ulong IntervalEndTimeNano { get; }
}