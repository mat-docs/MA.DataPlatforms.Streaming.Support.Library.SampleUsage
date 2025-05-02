// <copyright file="LinearInterpolationProcessor.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using MA.DataPlatforms.Streaming.Support.Lib.Core.Contracts.InterpolationModule;

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.Interpolation.Custom;

internal sealed class LinearInterpolationProcessor : ISubscriptionProcessor
{
    private readonly ulong interpolationPeriod;

    public LinearInterpolationProcessor(ulong interpolationPeriodNano)
    {
        this.interpolationPeriod = interpolationPeriodNano;
    }

    /// <summary>
    /// An example on how to create a custom processor.
    /// Any custom processors need to implement the ISubscriptionProcessor interface
    /// and output DTOs that implement the IProcessResult interface.
    /// </summary>
    /// <param name="context">ProcessContext which contains the data from the Support Library.
    /// The data has been merged through the Buffering Module.</param>
    /// <returns>The result of the processing.</returns>
    public IProcessResult Process(ProcessContext context)
    {
        var dataPoints = context.IntervalDataPoints;
        var interpolatedDataPoints = new List<double>();
        var timestamps = new List<ulong>();
        for (var i = 1; i < dataPoints.Count; i += 2)
        {
            var dataPointInitial = dataPoints[i - 1];
            var dataPointFinal = dataPoints[i];

            for (var interpolatedTimestamp = dataPointInitial.TimestampNanoseconds;
                 interpolatedTimestamp < dataPointFinal.TimestampNanoseconds;
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