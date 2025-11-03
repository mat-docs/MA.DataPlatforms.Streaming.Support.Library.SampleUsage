# MA DataPlatforms Streaming Support Library - Sample Usage

This repository contains comprehensive sample code and documentation for using the **MA DataPlatforms Streaming Support Library**, available in both **C#** and **Python**. The library enables applications to interact with Motion Applied's streaming telemetry data platform for reading, writing, buffering, and processing real-time motorsport data.

## 📚 Documentation

The complete documentation is now available in **MkDocs format** for easy browsing and navigation.

### View Documentation Locally

```powershell
# Install MkDocs and dependencies
pip install -r docs-requirements.txt

# Serve documentation locally
mkdocs serve
```

Then open your browser to: **http://127.0.0.1:8000**

See [MKDOCS_SETUP.md](MKDOCS_SETUP.md) for detailed MkDocs usage instructions.

### Sync Core Library Documentation

To include the core C# library documentation, copy the contents from:

**Source:** `../MA.DataPlatforms.Streaming.Support.Library/docs`  
**Destination:** `docs/core-library/`

This will add the complete core library API documentation to your local documentation site.
See [CORE_DOCS_SYNC.md](CORE_DOCS_SYNC.md) for detailed instructions.

## Overview

The MA DataPlatforms Streaming Support Library is available in two implementations:

- **C# Native Library**: Consumed directly as a NuGet package - full feature set with maximum performance
- **Python FFI Wrapper**: Python bindings that call the C# library via Foreign Function Interface

Both provide integration with streaming brokers (Kafka) for real-time telemetry data using the Open Data Protocol.

## Available Implementations

This repository contains sample code for both implementations:

### C# Samples (Native NuGet Package)
- **Location**: `MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation/`
- **Implementation**: Uses the native C# library directly via NuGet (no FFI)
- **Features**: Full feature set including buffering, interpolation, and SQL Race integration  
- **Performance**: Maximum throughput with zero FFI overhead
- **Documentation**: [C# README](MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation/README.md)

### Python Samples (FFI Wrapper)
- **Location**: `Python/`
- **Implementation**: Python FFI wrapper that calls the C# library underneath
- **Features**: Core features (session management, packet reading/writing)
- **Performance**: Good performance with small FFI marshalling overhead
- **Documentation**: [Python README](Python/README.md)

## Repository Structure

```
MA.DataPlatforms.Streaming.Support.Library.SampleUsage/
│
├── MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation/
│   ├── Program.cs                          # Main C# sample application
│   ├── README.md                           # C# detailed documentation
│   ├── Buffering/
│   │   ├── SampleDataHandler.cs            # Sample data buffering handler
│   │   └── TimestampDataHandler.cs         # Timestamp data buffering handler
│   ├── Interpolation/
│   │   ├── InterpolationResultHandler.cs   # Default interpolation result handler
│   │   └── Custom/
│   │       ├── LinearInterpolationHandler.cs    # Custom interpolation handler
│   │       ├── LinearInterpolationProcessor.cs  # Custom interpolation logic
│   │       └── LinearInterpolationResult.cs     # Custom result format
│   └── SqlRace/
│       ├── SqlSessionManager.cs            # SQL Race session management
│       └── SqlRaceExtensions.cs            # Helper extensions
│
└── Python/
    ├── README.md                           # Python detailed documentation
    ├── Requirements.txt                    # Python dependencies
    └── sample_code/
        ├── sample_reader/
        │   ├── main.py                     # Reader application entry point
        │   ├── received_packet_dto_handler.py   # Packet processing handler
        │   └── session_notifier.py         # Event subscription example
        └── sample_writer/
            ├── main.py                     # Writer application entry point
            ├── mock_data_writer.py         # Session creation and data writing
            ├── periodic_packet_generator.py     # Sample data generator
            └── packet_id_generator.py      # Packet ID management
```

## Feature Comparison

| Feature | C# | Python | Status |
|---------|:--:|:------:|--------|
| **Session Management** | ✅ | ✅ | Create, update, end sessions |
| **Data Format Management** | ✅ | ✅ | Parameter definitions, format IDs |
| **Packet Reading** | ✅ | ✅ | Live and historic session reading |
| **Packet Writing** | ✅ | ✅ | Write telemetry data to broker |
| **Buffering** | ✅ | 🚧 | Time-windowed data aggregation |
| **Interpolation** | ✅ | 🚧 | Statistical and custom processing |
| **SQL Race Integration** | ✅ | ❌ | Direct database storage |

✅ Available | 🚧 Coming Soon | ❌ Not Planned

## Quick Start

### C# Sample (Buffering & Interpolation)

**Prerequisites:**
- .NET 8.0 or later
- SQL Race installed
- Access to Kafka broker

**Setup:**
1. Open the solution in Visual Studio
2. Update configuration in `Program.cs`:
   - Stream API broker address
   - SQL Race connection string
   - Subscribed parameters
3. Build and run

**What it does:**
- Connects to Kafka broker
- Buffers streaming data with 3-second windows
- Performs interpolation at 2 Hz (First, Last, Mean, Min, Max)
- Stores results in SQL Race for ATLAS visualization

See [C# README](MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation/README.md) for detailed documentation.

### Python Sample (Reader & Writer)

**Prerequisites:**
- Python 3.8+
- Windows x64 (for native DLL)
- Access to Kafka broker

**Setup:**
1. Create virtual environment:
   ```bash
   python -m venv venv
   venv\Scripts\activate
   ```

2. Install dependencies:
   ```bash
   cd Python
   python -m pip install -r Requirements.txt
   ```

3. Update broker configuration in sample code

**Run Reader:**
```bash
cd sample_code/sample_reader
python main.py
```
Choose Live (L) or Historic (H) session reading.

**Run Writer:**
```bash
cd sample_code/sample_writer
python main.py
```
Creates a session with a sine wave parameter.

See [Python README](Python/README.md) for detailed documentation.

## Core Concepts

### Architecture

```
┌─────────────────────────────────────────┐
│   Application Layer                     │
│   (C# or Python)                        │
└───────────────┬─────────────────────────┘
                │
                │ C# API / Python FFI
                │
┌───────────────▼─────────────────────────┐
│   MA Streaming Support Library          │
│   (Native C# - Windows x64 DLL)         │
│                                          │
│   ┌──────────────────────────────────┐  │
│   │  Session Management Module       │  │
│   ├──────────────────────────────────┤  │
│   │  Data Format Management Module   │  │
│   ├──────────────────────────────────┤  │
│   │  Packet Reader Module            │  │
│   ├──────────────────────────────────┤  │
│   │  Packet Writer Module            │  │
│   ├──────────────────────────────────┤  │
│   │  Sample Reader Module (Buffering)│  │
│   ├──────────────────────────────────┤  │
│   │  Data Reader Module (Interpolate)│  │
│   └──────────────────────────────────┘  │
└───────────────┬─────────────────────────┘
                │
                │ Stream API Protocol
                │
┌───────────────▼─────────────────────────┐
│   Stream Broker (Kafka)                 │
└─────────────────────────────────────────┘
```

### Service Initialization Pattern

Both C# and Python follow the same service lifecycle pattern:

1. **Bootstrap** - Initialize the Support Library with configuration
2. **Get Module API** - Obtain the module factory for the desired functionality
3. **Create Service** - Create a service instance from the module
4. **Initialize** - Call `Initialise()` on the service
5. **Start** - Call `Start()` to begin operations
6. **Use** - Interact with the service
7. **Stop** - Call `Stop()` when finished

**C# Example:**
```csharp
var supportLibApi = new SupportLibApiFactory().Create(logger, streamApiConfig);
supportLibApi.Initialise();
supportLibApi.Start();

var moduleApi = supportLibApi.GetSessionManagerApi();
var serviceResponse = moduleApi.CreateService();
var service = serviceResponse.Data;

service.Initialise();
service.Start();
// Use service...
service.Stop();
supportLibApi.Stop();
```

**Python Example:**
```python
support_lib_factory = SupportLibraryBootstrapper.bootstrap(streaming_api_config, logger)
support_lib = support_lib_factory.create()
support_lib.initialise()
support_lib.start()

module_api = support_lib.get_session_manager_api()
service_response = module_api.create_service()
service = service_response.data

service.initialise()
service.start()
# Use service...
service.stop()
support_lib.stop()
```

### Available Modules

| Module | C# API Method | Python API Method | Purpose |
|--------|---------------|-------------------|---------|
| **Session Management** | `GetSessionManagerApi()` | `get_session_manager_api()` | Create, update, end sessions |
| **Data Format Management** | `GetDataFormatManagerApi()` | `get_data_format_manager_api()` | Manage parameter definitions |
| **Packet Reader** | `GetReadingPacketApi()` | `get_reading_packet_api()` | Read telemetry data |
| **Packet Writer** | `GetWritingPacketApi()` | `get_writing_packet_api()` | Write telemetry data |
| **Sample Reader** | `GetSampleReaderApi()` | N/A | Buffering (C# only) |
| **Data Reader** | `GetDataReaderApi()` | N/A | Interpolation (C# only) |

## Use Cases

### 1. Real-Time Data Monitoring (Python/C#)

Monitor live telemetry sessions and extract specific parameter values:

- Subscribe to live sessions
- Process incoming data packets
- Extract parameter samples
- Display or log values in real-time

**Best Implementation:** Python Sample Reader or C# Packet Reader

### 2. Data Recording (Python/C#)

Create and write telemetry sessions to the broker:

- Create new session
- Define parameters
- Generate or stream data
- End session for recording

**Best Implementation:** Python Sample Writer or C# Packet Writer

### 3. Data Buffering & Aggregation (C# only)

Aggregate streaming data into time-based windows:

- Configure buffering windows
- Merge data from multiple streams
- Handle markers, events, errors
- Two modes: SampleData or TimestampData

**Best Implementation:** C# Buffering Sample

### 4. Statistical Analysis (C# only)

Perform real-time statistical processing:

- Configure interpolation frequency
- Extract First, Last, Mean, Min, Max
- Store results in SQL Race
- Visualize in ATLAS

**Best Implementation:** C# Interpolation Sample

### 5. Custom Data Processing (C# only)

Implement custom interpolation algorithms:

- Create custom processor (e.g., Linear Interpolation)
- Define custom result format
- Process at configurable frequencies
- Integrate with SQL Race

**Best Implementation:** C# Custom Interpolation Processor

## Data Flow Examples

### Reading Live Data

```
Kafka Broker
     │
     │ Live Session Data
     │
     ▼
Packet Reader Service
     │
     │ ReceivedPacketDto
     │
     ▼
Packet Handler
     │
     │ Parse Open Data Protocol
     │
     ▼
Extract Parameter Samples
     │
     ▼
Process/Display/Store
```

### Writing Data with Buffering & Interpolation (C#)

```
Application
     │
     │ Generate Data
     │
     ▼
Packet Writer Service ──────► Kafka Broker
                                    │
                                    │
                                    ▼
                              Packet Reader ◄── Other Applications
                                    │
                                    ▼
                              Sample Reader (Buffering)
                              - 3s time windows
                              - Merge strategies
                                    │
                        ┌───────────┴───────────┐
                        │                       │
                        ▼                       ▼
                  SampleData              TimestampData
                   Handler                  Handler
                        │                       │
                        └───────────┬───────────┘
                                    ▼
                              Data Reader (Interpolation)
                              - 2 Hz frequency
                              - Statistical processing
                                    │
                                    ▼
                          Interpolation Handler
                                    │
                                    ▼
                              SQL Race Session
                                    │
                                    ▼
                            ATLAS Visualization
```

## Configuration Reference

### Streaming API Configuration

**C#:**
```csharp
var streamApiConfig = new StreamingApiConfiguration(
    strategy: StreamCreationStrategy.TopicBased,
    brokerList: "localhost:9094",
    additionalProperties: []
);
```

**Python:**
```python
streaming_api_config = StreamingApiConfiguration(
    broker_list="localhost:9094",
    stream_creation_strategy=StreamCreationStrategy.TOPIC_BASE,
    additional_properties=[],
    timeout_seconds=10
)
```

### Packet Reading Configuration

**C#:**
```csharp
var packetReadingConfig = new PacketReadingConfiguration(
    sessionIdentifierPattern: "*",
    readingType: ReadingType.Live,
    streams: new List<string> { "Chassis" }
);
```

**Python:**
```python
packet_reading_config = PacketReadingConfiguration(
    session_key="",
    timeout_seconds=10,
    raise_exception_on_timeout=False,
    data_source="Default",
    session_identifier_pattern="*",
    reading_type=PacketReadingType.LIVE,
    streams=["Chassis"]
)
```

### Buffering Configuration (C# only)

```csharp
var bufferingConfig = new BufferingConfiguration(
    subscribedParameters: new List<string> { "vCar:Chassis", "gLat:Chassis" },
    includeMarkerData: true,
    bufferingWindowLength: 3000  // milliseconds
);
```

## Best Practices

### 1. Service Lifecycle Management

✅ **Do:**
- Always call `Initialise()` before `Start()`
- Stop services in reverse order of creation
- Use try-finally or using statements for cleanup
- Check `ApiResponse.Success` before using `ApiResponse.Data`

❌ **Don't:**
- Use services before initialization
- Forget to stop services (causes resource leaks)
- Ignore error responses

### 2. Event Subscription (Python)

✅ **Do:**
```python
service.event.subscribe(handler)
# ... use service
service.event.unsubscribe(handler)
```

❌ **Don't:**
- Forget to unsubscribe (causes memory leaks)
- Subscribe multiple times to the same event

### 3. Handler Management

✅ **Do:**
```csharp
sampleReaderApi.AddHandler(handler);
// ... use
sampleReaderApi.RemoveHandler(handler);
```

❌ **Don't:**
- Leave handlers registered after stopping
- Share mutable state between handlers without synchronization

### 4. Parameter Subscription

✅ **Do:**
- Subscribe to specific parameters when possible
- Unsubscribe before stopping services
- Use `null` or empty list for all parameters only when needed

❌ **Don't:**
- Subscribe to all parameters unnecessarily (performance impact)
- Forget to unsubscribe

### 5. Error Handling

✅ **Do:**
```python
response = service.some_operation()
if not response.success or response.data is None:
    logger.error(f"Operation failed: {response.error_message}")
    return
data = response.data
```

❌ **Don't:**
- Access `response.data` without checking `success`
- Ignore error messages

## Troubleshooting

### Common Issues

| Issue | Possible Cause | Solution |
|-------|----------------|----------|
| Connection timeout | Broker not running or incorrect address | Verify broker is running on specified address |
| Service creation fails | Not initialized or incorrect config | Check initialization order and configuration |
| No data received | Wrong session pattern or data source | Verify session identifier pattern and data source name |
| Memory leaks | Services/handlers not cleaned up | Always stop services and remove handlers |
| Python FFI errors | Wrong platform or missing DLLs | Ensure Windows x64 and check DLL availability |
| SQL Race connection fails | Incorrect connection string | Verify SQL Race installation and connection string |

### Debugging Tips

1. **Enable verbose logging**: Set log level to Debug/Info
2. **Check service responses**: Always validate `ApiResponse.Success`
3. **Verify broker connectivity**: Test Kafka connection independently
4. **Monitor resource usage**: Check for memory/handle leaks
5. **Test with simple scenarios**: Start with minimal configuration

## Performance Considerations

### Buffering
- **Window size**: Larger windows = more memory, less frequent updates
- **Sliding percentage**: Lower = more updates, higher CPU usage
- **Parameter count**: More parameters = higher memory usage

### Interpolation
- **Frequency**: Higher frequencies = more CPU usage
- **Custom processors**: Can significantly impact performance
- **Handler efficiency**: Keep handlers lightweight

### Python FFI
- **FFI overhead**: Python has additional overhead vs. C#
- **Data marshaling**: Large data transfers can be slower
- **Threading**: Be aware of GIL (Global Interpreter Lock) limitations

## Migration Guide: Python to C# (for advanced features)

If you need Buffering/Interpolation:

1. **Convert service initialization**: Similar pattern, different syntax
2. **Implement handlers**: Translate Python handlers to C# classes
3. **Add buffering configuration**: Configure windows and merge strategies
4. **Add interpolation subscriptions**: Set up frequencies and processors
5. **Integrate SQL Race**: Add database storage for visualization

See both README files for detailed API comparisons.

## Additional Resources

- **C# Detailed Documentation**: [C# README](MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation/README.md)
- **Python Detailed Documentation**: [Python README](Python/README.md)
- **Open Data Protocol**: Refer to protobuf definitions in library packages
- **SQL Race**: SQL Race SDK documentation
- **ATLAS**: Use ATLAS for visualization of processed data

## System Requirements

### C# Application
- **OS**: Windows 10/11 (x64)
- **Runtime**: .NET 8.0 or later
- **SQL Race**: Latest version installed
- **Memory**: 4GB+ recommended
- **Broker**: Kafka 2.0+

### Python Application
- **OS**: Windows 10/11 (x64) - Required for native DLL
- **Python**: 3.8 or later
- **Memory**: 2GB+ recommended
- **Broker**: Kafka 2.0+

## License & Support

This sample code is provided by Motion Applied Ltd. for demonstration purposes.

For support, issues, or questions regarding the Streaming Support Library, please contact Motion Applied support.

---

**Quick Navigation:**
- [C# Buffering & Interpolation Sample](MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation/README.md)
- [Python Reader & Writer Samples](Python/README.md)

**Version Information:**
- Library: Check NuGet/PyPI package version
- Sample Code: 2025 R03 Release
