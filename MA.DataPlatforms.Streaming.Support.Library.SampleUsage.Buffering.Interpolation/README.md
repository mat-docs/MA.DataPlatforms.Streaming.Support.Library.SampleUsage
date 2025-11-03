# C# Buffering and Interpolation Sample

This sample demonstrates how to use the **Buffering** and **Interpolation** modules of the MA DataPlatforms Streaming Support Library in C# via the **native NuGet package**. The application listens to a Stream API (e.g., Kafka), processes streaming telemetry data through buffering and interpolation pipelines, and stores the results in a SQL Race session for visualization and analysis in ATLAS.

## Overview

This C# implementation uses the **native NuGet package** (not FFI) to provide advanced data processing capabilities:

- **Buffering Module**: Aggregates and merges streaming data using configurable time windows
- **Interpolation Module**: Processes buffered data to extract statistical values (min, max, mean, first, last) at configurable frequencies
- **SQL Race Integration**: Stores processed results in SQLite/SQL Server databases for visualization
- **Custom Processors**: Extensible architecture supporting custom interpolation algorithms

### Native NuGet Package

Unlike the Python implementation which uses **FFI (Foreign Function Interface)**, these samples consume the library as a standard **NuGet package**:

- ✅ Direct .NET assembly integration - zero FFI overhead
- ✅ Native performance for high-throughput scenarios
- ✅ Full type safety and IntelliSense support
- ✅ Access to all advanced features (buffering, interpolation, SQL Race)
- ✅ Standard NuGet package management with versioning

## Features

### Buffering
- **Time-based windowing**: Configurable window length with sliding window support
- **Two data formats**: Sample Data (one parameter per packet) and Timestamp Data (multiple parameters per timestamp)
- **Multi-data type support**: Handle samples, markers, events, errors, and CAN data
- **Custom merge strategies**: Inject your own data merging logic

### Interpolation
- **Statistical processing**: Built-in processor for First, Last, Mean, Min, Max calculations
- **Configurable frequencies**: Independent subscription and delivery frequencies
- **Custom processors**: Implement `ISubscriptionProcessor` for custom interpolation algorithms
- **Batch result handling**: Process multiple parameters efficiently

### SQL Race Integration
- **Real-time storage**: Stream processed data directly to SQL Race sessions
- **ATLAS visualization**: Visualize buffered and interpolated data in ATLAS
- **Multiple database support**: SQLite and SQL Server

## Architecture

```
Stream API (Kafka)
       ↓
Packet Reader Service
       ↓
Sample Reader Service (Buffering)
    ↓              ↓
SampleData    TimestampData
Handlers       Handlers
       ↓
Data Reader Service (Interpolation)
    ↓              ↓
Default         Custom
Processor      Processor
       ↓
Result Handlers → SQL Race Session
```

## Prerequisites

- **.NET 8.0** or later
- **SQL Race** installed and configured
- Access to a **Stream API broker** (e.g., Kafka)
- **MA DataPlatforms Streaming Support Library** NuGet package
- **ATLAS** (optional, for visualization)

## Configuration

### 1. Streaming API Configuration

Configure the connection to your Stream API broker:

```csharp
var streamApiConfig = new StreamingApiConfiguration(
    strategy: StreamCreationStrategy.TopicBased,
    brokerList: "localhost:9094",
    additionalProperties: []
);
```

**Parameters:**
- `strategy`: How streams are created (`TopicBased` or `StreamBased`)
- `brokerList`: Kafka broker address(es)
- `additionalProperties`: Additional Kafka configuration properties

### 2. Packet Reading Configuration

Configure how sessions are read from the broker:

```csharp
var packetReadingConfig = new PacketReadingConfiguration(
    sessionIdentifierPattern: "*",           // Match all sessions
    readingType: ReadingType.Live,           // Live or Historic
    streams: new List<string> { "Chassis" }  // Specific streams to read
);
```

**Parameters:**
- `sessionIdentifierPattern`: Glob pattern for session names (`"*"` = all)
- `readingType`: `ReadingType.Live` or `ReadingType.Historic`
- `streams`: List of stream names to subscribe to (empty = all streams)

### 3. Buffering Configuration

Configure the buffering behavior:

```csharp
var bufferingConfig = new BufferingConfiguration(
    subscribedParameters: new List<string> 
    { 
        "vCar:Chassis",
        "gLat:Chassis",
        "gLong:Chassis"
    },
    includeMarkerData: true,
    bufferingWindowLength: 3000  // 3000 milliseconds
);
```

**Configuration Options:**

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `SubscribedParameters` | `List<string>` | `null` | List of parameters to subscribe to. `null` = all parameters |
| `BufferingWindowLength` | `int` | `3000` | Window length in milliseconds |
| `SlidingWindowPercentage` | `double` | `5%` | Percentage of window length for sliding interval |
| `MergeStrategy` | `MergeStrategy?` | `null` | Merge strategy: `ReadingMerge` (recommended) or `WritingMerge` |
| `IncludeMarkerData` | `bool` | `false` | Include marker data in buffered stream |
| `IncludeEventData` | `bool` | `false` | Include event data in buffered stream |
| `IncludeErrorData` | `bool` | `false` | Include error data in buffered stream |
| `IncludeCanData` | `bool` | `false` | Include CAN data in buffered stream |

### 4. SQL Race Configuration

Configure the SQL Race database connection:

```csharp
const string ConnectionString = 
    @"DbEngine=SQLite;Data Source=C:\Motion Applied\SupportFilesDemo\SupportFilesDemo.ssndb;PRAGMA journal_mode=WAL;";

Core.Initialize();
Core.ConfigureServer(true, IPEndPoint.Parse("127.0.0.1:7380"));

var recorderConfiguration = RecordersConfiguration.GetRecordersConfiguration();
recorderConfiguration.AddConfiguration(
    Guid.NewGuid(),
    nameof(DbEngine.SQLServer),
    "SupportLib",
    "test",
    ConnectionString,
    false
);
```

### 5. Parameter Subscription

Define which parameters to monitor:

```csharp
var subscribedParameters = new List<string>
{
    "vCar:Chassis",        // Vehicle speed
    "sLap:Chassis",        // Lap distance
    "aSteerWheel:Chassis", // Steering angle
    "gLat:Chassis",        // Lateral G-force
    "gLong:Chassis"        // Longitudinal G-force
};
```

## Buffering Module

### Two Buffering Modes

#### 1. Sample Data Mode

Each `SampleData` object contains **one parameter** with multiple samples and timestamps.

**Use Cases:**
- Processing individual parameter streams
- Handling events, markers, CAN data separately
- When you need parameter-specific processing logic

**Example Handler:**

```csharp
public class SampleDataHandler : IHandler<SampleData>
{
    public void Handle(SampleData data)
    {
        var parameterIdentifier = data.ParameterIdentifier;
        var timestamps = data.Timestamps;  // Nanosecond timestamps
        var values = data.Samples;         // Double values
        
        for (int i = 0; i < timestamps.Count; i++)
        {
            var timestamp = timestamps[i];
            var value = values[i];
            
            // Store in SQL Race, log, or process further
            Console.WriteLine($"{parameterIdentifier}: {timestamp} -> {value}");
        }
    }
}
```

**Handler Registration:**
```csharp
var sampleDataHandler = new SampleDataHandler(logger, sqlSessionManager, subscribedParameters);
sampleReaderApi.AddHandler(sampleDataHandler);
```

#### 2. Timestamp Data Mode

Each `TimestampData` object contains **multiple parameters** per timestamp.

**Use Cases:**
- Synchronous multi-parameter analysis
- When you need aligned data across multiple channels
- Correlation analysis between parameters

**Example Handler:**

```csharp
public class TimestampDataHandler : IHandler<TimestampData>
{
    public void Handle(TimestampData data)
    {
        var timestamps = data.Timestamps;  // Shared timestamps
        var parameterData = data.Data;     // Dictionary<string, List<double>>
        
        for (int i = 0; i < timestamps.Count; i++)
        {
            var timestamp = timestamps[i];
            
            foreach (var (paramId, samples) in parameterData)
            {
                var value = samples[i];
                // All parameters share the same timestamp
                Console.WriteLine($"{timestamp}: {paramId} = {value}");
            }
        }
    }
}
```

### Subscribing to Buffering

```csharp
// Subscribe to specific parameters
sampleReaderApi.Subscribe(subscribedParameters);

// Or subscribe to all parameters
sampleReaderApi.SubscribeAll();

// Unsubscribe when done
sampleReaderApi.Unsubscribe(subscribedParameters);
// or
sampleReaderApi.UnsubscribeAll();
```

## Interpolation Module

The interpolation module processes buffered data at configurable frequencies to extract statistical values or apply custom algorithms.

### Default Interpolation Processor

The built-in processor calculates:
- **First**: First sample value in the time window
- **Last**: Last sample value in the time window
- **Mean**: Average of all samples
- **Min**: Minimum sample value
- **Max**: Maximum sample value

### Interpolation Configuration

```csharp
var subscriptionKey = Guid.NewGuid().ToString();
var interpolationResultHandler = new InterpolationResultHandler(logger, subscriptionKey, sqlSessionManager);

dataReaderApi.Subscribe(
    subscriptionKey: subscriptionKey,
    parameterIdentifiers: subscribedParameters,
    subscriptionFrequencyHz: 2,        // Process data at 2 Hz
    handler: interpolationResultHandler,
    deliveryFrequencyHz: 2             // Deliver results at 2 Hz (optional)
);
```

**Parameters:**
- `subscriptionKey`: Unique identifier for this interpolation subscription
- `parameterIdentifiers`: List of parameters to process
- `subscriptionFrequencyHz`: Frequency at which data is sent to the processor (Hz)
- `handler`: Object implementing `IBatchResultHandler` to receive results
- `deliveryFrequencyHz`: Frequency at which results are delivered (defaults to subscription frequency)
- `processor`: Optional custom processor (defaults to built-in statistical processor)

### Handling Interpolation Results

```csharp
public class InterpolationResultHandler : IBatchResultHandler
{
    private readonly string _subscriptionKey;
    private readonly ISqlSessionManager _sqlSessionManager;

    public void Handle(IReadOnlyList<IProcessResult> results)
    {
        foreach (var result in results)
        {
            if (_subscriptionKey != result.SubscriptionKey)
                continue;

            var parameterIdentifier = result.ParameterIdentifier;
            var timestamp = result.Timestamp;
            
            // Access statistical values
            var first = result.First;
            var last = result.Last;
            var mean = result.Mean;
            var min = result.Min;
            var max = result.Max;
            
            // Store in SQL Race
            _sqlSessionManager.WriteInterpolatedData(parameterIdentifier, timestamp, mean);
        }
    }
}
```

### Custom Interpolation Processor

Create custom interpolation algorithms by implementing `ISubscriptionProcessor`:

```csharp
public class LinearInterpolationProcessor : ISubscriptionProcessor
{
    private readonly ulong _interpolationInterval;

    public LinearInterpolationProcessor(ulong interpolationIntervalNs)
    {
        _interpolationInterval = interpolationIntervalNs;
    }

    public IReadOnlyList<IProcessResult> Process(
        string subscriptionKey,
        IReadOnlyDictionary<string, TimestampValuePair> inputData,
        ulong startTime,
        ulong endTime)
    {
        var results = new List<IProcessResult>();
        
        foreach (var (parameterId, data) in inputData)
        {
            // Implement linear interpolation logic
            var interpolatedValue = PerformLinearInterpolation(
                data.Timestamps, 
                data.Values, 
                startTime, 
                endTime
            );
            
            results.Add(new LinearInterpolationResult(
                subscriptionKey,
                parameterId,
                startTime,
                interpolatedValue
            ));
        }
        
        return results;
    }
}
```

**Using Custom Processor:**

```csharp
var customProcessor = new LinearInterpolationProcessor(interpolationInterval: 2000000UL); // 500 Hz
var customHandler = new LinearInterpolationHandler(subscriptionKey, sqlSessionManager);

dataReaderApi.Subscribe(
    subscriptionKey,
    subscribedParameters,
    subscriptionFrequencyHz: 2,
    handler: customHandler,
    deliveryFrequencyHz: 2,
    processor: customProcessor  // Inject custom processor
);
```

## Service Initialization Pattern

All services follow the same lifecycle pattern:

```csharp
// 1. Create the Support Library
var supportLibApi = new SupportLibApiFactory().Create(logger, streamApiConfig);
supportLibApi.Initialise();
supportLibApi.Start();

// 2. Get Module API
var packetReaderModuleApi = supportLibApi.GetReadingPacketApi();

// 3. Create Service
var packetReaderApiResponse = packetReaderModuleApi.CreateService(packetReadingConfig);
if (!packetReaderApiResponse.Success || packetReaderApiResponse.Data is null)
{
    logger.Error("Failed to create packet reader service");
    return;
}

var packetReaderService = packetReaderApiResponse.Data;

// 4. Initialize and Start Service
packetReaderService.Initialise();
packetReaderService.Start();

// 5. Use the service
// ...

// 6. Stop when done
packetReaderService.Stop();
supportLibApi.Stop();
```

## Complete Example Workflow

```csharp
// 1. Initialize SQL Race
Core.Initialize();
var sqlSessionManager = new SqlSessionManager(connectionString, subscribedParameters, logger);

// 2. Configure Support Library
var streamApiConfig = new StreamingApiConfiguration(...);
var packetReadingConfig = new PacketReadingConfiguration(...);
var bufferingConfig = new BufferingConfiguration(...);

// 3. Create handlers
var sampleDataHandler = new SampleDataHandler(logger, sqlSessionManager, subscribedParameters);
var timestampDataHandler = new TimestampDataHandler(logger, sqlSessionManager, subscribedParameters);
var interpolationHandler = new InterpolationResultHandler(logger, subscriptionKey, sqlSessionManager);

// 4. Initialize Support Library
var supportLibApi = new SupportLibApiFactory().Create(logger, streamApiConfig);
supportLibApi.Initialise();
supportLibApi.Start();

// 5. Create Packet Reader Service
var packetReaderService = supportLibApi.GetReadingPacketApi()
    .CreateService(packetReadingConfig).Data;
packetReaderService.Initialise();
packetReaderService.Start();

// 6. Create Sample Reader Service (Buffering)
var sampleReaderApi = supportLibApi.GetSampleReaderApi()
    .CreateService(bufferingConfig).Data;
sampleReaderApi.SetReaderService(packetReaderService);
sampleReaderApi.AddHandler(sampleDataHandler);
sampleReaderApi.AddHandler(timestampDataHandler);
sampleReaderApi.Initialise();
sampleReaderApi.Start();
sampleReaderApi.Subscribe(subscribedParameters);

// 7. Create Data Reader Service (Interpolation)
var dataReaderApi = supportLibApi.GetDataReaderApi().CreateService().Data;
dataReaderApi.SetSampleReaderService(sampleReaderApi);
dataReaderApi.Initialise();
dataReaderApi.Start();
dataReaderApi.Subscribe(subscriptionKey, subscribedParameters, 2, interpolationHandler, 2);

// 8. Wait for data processing
Console.ReadLine();

// 9. Cleanup
sampleReaderApi.RemoveHandler(sampleDataHandler);
sampleReaderApi.RemoveHandler(timestampDataHandler);
sampleReaderApi.Unsubscribe(subscribedParameters);
dataReaderApi.Unsubscribe(subscriptionKey);
dataReaderApi.Stop();
sampleReaderApi.Stop();
packetReaderService.Stop();
supportLibApi.Stop();
sqlSessionManager.Stop();
```

## Usage

1. **Update Configuration**: Modify `Program.cs` with your Stream API, database, and parameter settings
2. **Build**: Build the solution in Visual Studio or via command line
3. **Run**: Execute the application
4. **Monitor**: Data is processed in real-time and stored in SQL Race
5. **Visualize**: Open the SQL Race session in ATLAS to visualize buffered and interpolated data

## Key Classes

| Class | Purpose |
|-------|---------|
| `SampleDataHandler` | Handles buffered sample data (one parameter per packet) |
| `TimestampDataHandler` | Handles buffered timestamp data (multiple parameters per timestamp) |
| `InterpolationResultHandler` | Handles default interpolation results (First, Last, Mean, Min, Max) |
| `LinearInterpolationProcessor` | Example custom interpolation processor |
| `LinearInterpolationHandler` | Handles custom linear interpolation results |
| `SqlSessionManager` | Manages SQL Race session and writes processed data |

## Best Practices

1. **Service Lifecycle**: Always call `Initialise()`, `Start()`, and `Stop()` in the correct order
2. **Handler Cleanup**: Remove handlers before stopping services to prevent memory leaks
3. **Subscription Management**: Always unsubscribe before stopping services
4. **Error Handling**: Check `ApiResponse.Success` before using `ApiResponse.Data`
5. **Logging**: Use the logger to track service lifecycle and data processing
6. **Resource Disposal**: Stop all services in reverse order of creation

## Extending the Sample

### Using Different Merge Strategies

Choose between two merge strategies for buffering:

```csharp
// ReadingMerge (recommended for better performance)
var bufferingConfig = new BufferingConfiguration(
    subscribedParameters,
    mergeStrategy: MergeStrategy.ReadingMerge
);

// WritingMerge
var bufferingConfig = new BufferingConfiguration(
    subscribedParameters,
    mergeStrategy: MergeStrategy.WritingMerge
);
```

**Merge Strategy Options:**
- `MergeStrategy.ReadingMerge`: Default strategy, optimized for reading performance (recommended)
- `MergeStrategy.WritingMerge`: Attempts merge at buffer write time (not recommended)

### Add More Interpolation Subscriptions

You can have multiple interpolation subscriptions with different frequencies and processors:

```csharp
// High-frequency statistical interpolation
dataReaderApi.Subscribe("high-freq", params, 100, handler1, 100);

// Low-frequency custom interpolation
dataReaderApi.Subscribe("low-freq", params, 1, handler2, 1, customProcessor);
```

### Process Different Data Types

Enable markers, events, errors in buffering configuration:

```csharp
var bufferingConfig = new BufferingConfiguration(
    subscribedParameters,
    includeMarkerData: true,
    includeEventData: true,
    includeErrorData: true,
    includeCanData: true
);
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| No data received | Check broker connection, session pattern, and stream names |
| SQL Race connection fails | Verify connection string and SQL Race initialization |
| Service creation fails | Ensure services are initialized in correct order |
| Memory leaks | Remove handlers and unsubscribe before stopping services |
| Interpolation not working | Verify subscription frequency and ensure buffering is active |

## Additional Resources

- **Python Sample**: See `Python/` folder for Python FFI usage examples
- **SQL Race Documentation**: Refer to SQL Race SDK documentation
- **ATLAS**: Use ATLAS for visualizing processed telemetry data

---

**Note:** This sample demonstrates the full capabilities of the Streaming Support Library's Buffering and Interpolation modules. These features are currently available in C# only; Python FFI support is planned for future releases.

