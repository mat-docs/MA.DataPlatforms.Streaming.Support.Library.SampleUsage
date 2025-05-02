// <copyright file="LinearInterpolationResult.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using MA.DataPlatforms.Streaming.Support.Lib.Core.Contracts.InterpolationModule;

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.Interpolation.Custom;

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