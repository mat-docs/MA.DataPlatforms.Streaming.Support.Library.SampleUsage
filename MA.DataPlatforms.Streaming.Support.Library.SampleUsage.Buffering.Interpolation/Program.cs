// <copyright file="Program.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System;
using System.Collections.Generic;
using System.Net;

using MA.DataPlatforms.Streaming.Support.Lib.Core.Abstractions;
using MA.DataPlatforms.Streaming.Support.Lib.Core.Contracts.BufferingModule;
using MA.DataPlatforms.Streaming.Support.Lib.Core.Contracts.ReadingModule;
using MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.Buffering;
using MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.Interpolation;
using MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.Interpolation.Custom;
using MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.SqlRace;
using MA.Streaming.Abstraction;
using MA.Streaming.Core.Configs;

using MESL.SqlRace.Domain;

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation;

internal static class Program
{
    private const string ConnectionString = @"DbEngine=SQLite;Data Source=C:\McLaren Applied\SupportFilesDemo\SupportFilesDemo.ssndb;PRAGMA journal_mode=WAL;";

    private static void Main()
    {
        // Initialize SQL Race
        Console.WriteLine("Initialize SQL Race");
        Core.Initialize();
        Core.ConfigureServer(true, IPEndPoint.Parse("127.0.0.1:7380"));
        var recorderConfiguration = RecordersConfiguration.GetRecordersConfiguration();
        recorderConfiguration.AddConfiguration(Guid.NewGuid(), nameof(DbEngine.SQLServer), "SupportLib", "test", ConnectionString, false);

        // List out which parameters we want to subscribe to.
        var subscribedParameters = new List<string>
        {
            "vCar:Chassis",
            "sLap:Chassis",
            "aSteerWheel:Chassis",
            "gLat:Chassis",
            "gLong:Chassis"
        };

        // Configure Support Library
        var streamApiConfig = new StreamingApiConfiguration(StreamCreationStrategy.TopicBased, "localhost:9094", []);
        var packetReadingConfig = new PacketReadingConfiguration(
            sessionIdentifierPattern: "*",
            readingType: ReadingType.Live,
            streams: new List<string>
            {
                "Chassis"
            });
        var bufferingConfig = new BufferingConfiguration(subscribedParameters, includeMarkerData: true, bufferingWindowLength: 3000);

        // Create the handlers.
        var logger = new Logger(LoggingLevel.Info);
        var sqlSessionManager = new SqlSessionManager(ConnectionString, subscribedParameters, logger);
        var sampleDataHandler = new SampleDataHandler(logger, sqlSessionManager, subscribedParameters);
        var timestampDataHandler = new TimestampDataHandler(logger, sqlSessionManager, subscribedParameters);

        // Interpolation requires a subscription key for every handler/processor pairing.
        var subscriptionKeyDefault = Guid.NewGuid().ToString();
        var interpolationResultHandler = new InterpolationResultHandler(logger, subscriptionKeyDefault, sqlSessionManager);

        // Custom linear interpolation.
        var interpolationInterval = 2000000UL; // 500 Hz 
        var subscriptionKeyLinearInterpolation = Guid.NewGuid().ToString();
        var linearInterpolationProcessor = new LinearInterpolationProcessor(interpolationInterval);
        var linearInterpolationHandler = new LinearInterpolationHandler(subscriptionKeyLinearInterpolation, sqlSessionManager);

        // Create the support library.
        var supportLibApi = new SupportLibApiFactory().Create(
            logger,
            streamApiConfig);

        // Initialize and start support library
        supportLibApi.Initialise();
        supportLibApi.Start();

        var packetReaderModuleApi = supportLibApi.GetReadingPacketApi();
        if (packetReaderModuleApi is null)
        {
            logger.Error("Failed to create packet reader module.");
            return;
        }

        var packetReaderApiResponse = packetReaderModuleApi.CreateService(packetReadingConfig);
        if (!packetReaderApiResponse.Success ||
            packetReaderApiResponse.Data is null)
        {
            logger.Error("Failed to create packet reader api.");
            return;
        }

        var packetReaderService = packetReaderApiResponse.Data;
        packetReaderService.Initialise();
        packetReaderService.Start();

        var sampleReaderModule = supportLibApi.GetSampleReaderApi();
        if (sampleReaderModule is null)
        {
            logger.Error("Failed to create sample reader module.");
            return;
        }

        var sampleReaderApiResponse = sampleReaderModule.CreateService(bufferingConfig);
        if (!sampleReaderApiResponse.Success ||
            sampleReaderApiResponse.Data is null)
        {
            logger.Error("Failed to create sample reader api.");
            return;
        }

        var sampleReaderApi = sampleReaderApiResponse.Data;

        // Subscribe to buffering (merging)
        sampleReaderApi.SetReaderService(packetReaderService);
        sampleReaderApi.AddHandler(sampleDataHandler);
        sampleReaderApi.AddHandler(timestampDataHandler);

        sampleReaderApi.Initialise();
        sampleReaderApi.Start();

        sampleReaderApi.Subscribe(subscribedParameters);

        // Subscribe to interpolation. This is also where we can inject your handler and processor for that interpolation.
        var dataReaderModule = supportLibApi.GetDataReaderApi();
        if (dataReaderModule is null)
        {
            logger.Error("Failed to create data reader module.");
            return;
        }

        var dataReaderApiResponse = dataReaderModule.CreateService();
        if (!dataReaderApiResponse.Success ||
            dataReaderApiResponse.Data is null)
        {
            logger.Error("Failed to create data reader api.");
            return;
        }

        var dataReaderApi = dataReaderApiResponse.Data;
        dataReaderApi.SetSampleReaderService(sampleReaderApi);
        dataReaderApi.Initialise();
        dataReaderApi.Start();
        dataReaderApi.Subscribe(subscriptionKeyDefault, subscribedParameters, 2, interpolationResultHandler, 2);

        dataReaderApi.Subscribe(
            subscriptionKeyLinearInterpolation,
            subscribedParameters,
            2,
            linearInterpolationHandler,
            2,
            linearInterpolationProcessor);

        logger.Info("Press enter to stop...");
        Console.ReadLine();
        sampleReaderApi.RemoveHandler(sampleDataHandler);
        sampleReaderApi.RemoveHandler(timestampDataHandler);
        sampleReaderApi.Unsubscribe(subscribedParameters);

        dataReaderApi.Unsubscribe(subscriptionKeyDefault);
        dataReaderApi.Unsubscribe(subscriptionKeyLinearInterpolation);
        dataReaderApi.Stop();

        sampleReaderApi.Stop();
        supportLibApi.Stop();
        packetReaderService.Stop();
        sqlSessionManager.Stop();
        Console.ReadLine();
    }
}