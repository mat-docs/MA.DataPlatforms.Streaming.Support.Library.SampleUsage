// <copyright file="LinearInterpolationProcessor.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System.Collections.Generic;

using MA.DataPlatforms.Streaming.Support.Lib.Core.Contracts.InterpolationModule;

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.Interpolation.Custom;

internal sealed class LinearInterpolationProcessor : ISubscriptionProcessor
{
    private readonly ulong interpolationPeriod;
    private readonly Dictionary<string, DataPoint> lastBatchDataPoints = new();

    /// <summary>
    ///     Creates a linearly interpolated data between two sample points.
    /// </summary>
    /// <param name="interpolationPeriodNano">The frequency of the interpolated data</param>
    public LinearInterpolationProcessor(ulong interpolationPeriodNano)
    {
        this.interpolationPeriod = interpolationPeriodNano;
    }

    /// <summary>
    ///     An example on how to create a custom processor.
    ///     Any custom processors need to implement the ISubscriptionProcessor interface
    ///     and output DTOs that implement the IProcessResult interface.
    /// </summary>
    /// <param name="context">
    ///     ProcessContext which contains the data from the Support Library.
    ///     The data has been merged through the Buffering Module.
    /// </param>
    /// <returns>The result of the processing.</returns>
    public IProcessResult Process(ProcessContext context)
    {
        var dataPoints = context.IntervalDataPoints;
        var interpolatedDataPoints = new List<double>();
        var timestamps = new List<ulong>();
        for (var i = 0; i < dataPoints.Count; i++)
        {
            var dataPointFinal = dataPoints[i];

            // Depending on what you are doing, you may need to cache the last process context's data point
            // as shown here to ensure all the data is processed together continuously.
            DataPoint dataPointInitial;
            switch (i)
            {
                case 0 when !this.lastBatchDataPoints.TryGetValue(dataPointFinal.ParameterIdentifier, out dataPointInitial):
                    // Continue to the next sample
                    continue;
                case 0 when this.lastBatchDataPoints.TryGetValue(dataPointFinal.ParameterIdentifier, out dataPointInitial):
                    break;
                default:
                    dataPointInitial = dataPoints[i - 1];
                    break;
            }

            for (var interpolatedTimestamp = dataPointInitial.TimestampNanoseconds;
                 interpolatedTimestamp <= dataPointFinal.TimestampNanoseconds;
                 interpolatedTimestamp += this.interpolationPeriod)
            {
                interpolatedDataPoints.Add(
                    GetInterpolatedSample(
                        dataPointInitial.Value,
                        dataPointInitial.TimestampNanoseconds,
                        dataPointFinal.Value,
                        dataPointFinal.TimestampNanoseconds,
                        interpolatedTimestamp));
                timestamps.Add(interpolatedTimestamp);
            }

            this.lastBatchDataPoints[dataPointFinal.ParameterIdentifier] = dataPointFinal;
        }

        return new LinearInterpolationResult(
            context.ParameterId,
            context.IntervalStartTimeNano,
            context.IntervalEndTimeNano,
            interpolatedDataPoints,
            timestamps);
    }

    private static double GetInterpolatedSample(double initialSample, ulong initialTimestamp, double finalSample, ulong finalTimestamp, ulong interpolatedTimestamp)
    {
        var weight = (interpolatedTimestamp - initialTimestamp) / (double)(finalTimestamp - initialTimestamp);
        return double.Lerp(initialSample, finalSample, weight);
    }
}