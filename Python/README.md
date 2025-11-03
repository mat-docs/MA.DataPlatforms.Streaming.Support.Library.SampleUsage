# Python Streaming Support Library - Sample Code

This repository contains comprehensive sample code demonstrating the usage of the **MA DataPlatforms Streaming Support Library** for Python. The library provides a **Python FFI (Foreign Function Interface) wrapper** around the native C# library, enabling Python developers to work with Motion Applied's streaming data platform.

## Overview

The Python FFI wrapper enables applications to:
- **Read** live and historic telemetry sessions from streaming data brokers (e.g., Kafka)
- **Write** telemetry data and sessions to streaming data brokers
- **Manage** session lifecycle (create, start, update, end sessions)
- **Handle** data formats and parameter definitions
- Work with the **Open Data Protocol** for telemetry data exchange

### FFI Architecture

The Python library uses **FFI (Foreign Function Interface)** to call the C# library:
- Python code makes function calls to the underlying C# DLL
- Data is marshalled between Python and C# types
- Provides a Pythonic API while leveraging C# library capabilities
- Small performance overhead compared to native C# but suitable for most use cases

## Current Features

The Python package currently supports the following modules:

✅ **Session Management** - Create, update, and manage telemetry sessions  
✅ **Data Format Management** - Handle parameter definitions and data format IDs  
✅ **Packet Reading** - Read live and historic session data from the broker  
✅ **Packet Writing** - Write telemetry data to the broker  

🚧 **Coming Soon**: Buffering and Interpolation modules (available in C# version)

## Architecture

```
┌─────────────────────────────────────┐
│   Python Application                │
│   (Your Code)                       │
└──────────────┬──────────────────────┘
               │
               │ Python FFI Bindings
               │
┌──────────────▼──────────────────────┐
│  MA Streaming Support Library       │
│  (Native C# DLL - Windows x64)      │
└──────────────┬──────────────────────┘
               │
               │ Streaming API
               │
┌──────────────▼──────────────────────┐
│  Stream Broker (e.g., Kafka)        │
└─────────────────────────────────────┘
```

## Installation

### Prerequisites
- **Python 3.8+**
- **Windows x64** (required for native DLL support)
- Access to a **Stream API broker** (e.g., Kafka)
- **Virtual environment** (recommended)

### Setup Steps

1. **Create a virtual environment** (recommended):
   ```bash
   python -m venv venv
   venv\Scripts\activate
   ```

2. **Install dependencies**:
   ```bash
   python -m pip install -r Requirements.txt
   ```

3. **Configure your broker connection** in the sample code (see Configuration section below)

## Sample Applications

### 1. Sample Reader (`sample_reader/`)

Demonstrates how to **read telemetry data** from live or historic sessions.

**Features:**
- Connect to the streaming broker
- Read live sessions as they are being recorded
- Read historic (completed) sessions from the broker
- Parse Open Data Protocol packets (Periodic Data, Markers, Events, etc.)
- Subscribe to session lifecycle events
- Extract parameter samples with timestamps

**Key Components:**
- `main.py` - Entry point with configuration and service initialization
- `received_packet_dto_handler.py` - Handles incoming data packets and extracts parameter values
- `session_notifier.py` - Subscribes to and logs session lifecycle events

**Usage:**
```bash
cd sample_code/sample_reader
python main.py
```

When prompted, choose:
- `L` for **Live** sessions (waits for new sessions to start)
- `H` for **Historic** sessions (lists and reads completed sessions)

The sample reads the `vCar:Chassis` parameter and logs sample values with timestamps.

### 2. Sample Writer (`sample_writer/`)

Demonstrates how to **write telemetry data** to create sessions on the broker.

**Features:**
- Create new telemetry sessions
- Define custom parameters and configurations
- Write periodic sample data (sine wave example)
- Send session markers and metadata
- Properly start and end sessions
- Generate data in real-time with correct timing

**Key Components:**
- `main.py` - Entry point with service initialization
- `mock_data_writer.py` - Orchestrates session creation and data writing
- `periodic_packet_generator.py` - Generates periodic sample data (sine wave)
- `packet_id_generator.py` - Generates unique packet IDs

**Usage:**
```bash
cd sample_code/sample_writer
python main.py
```

The sample creates a session with a `Sin:MyApp` parameter generating a sine wave that can be recorded and visualized in ATLAS.

## Core Concepts

### 1. Support Library Bootstrapping

Every application must bootstrap the Support Library to establish the Python-to-C# FFI connection:

```python
from ma_dataplatforms_streaming_support_library.core.base.support_library_bootstrapper import SupportLibraryBootstrapper
from ma_dataplatforms_streaming_support_library.core.base.logger import Logger
from ma_dataplatforms_streaming_support_library.contracts.shared.stream_api_configuration import StreamingApiConfiguration

# Configure connection to broker
streaming_api_config = StreamingApiConfiguration(
    broker_list="localhost:9092",
    stream_creation_strategy=StreamCreationStrategy.TOPIC_BASE,
    additional_properties=[],
    timeout_seconds=10
)

# Create logger
logger = Logger()

# Bootstrap the library (CRITICAL: do this before any other operations)
support_lib_factory = SupportLibraryBootstrapper.bootstrap(streaming_api_config, logger)

# Create and start the support library
support_lib = support_lib_factory.create()
support_lib.initialise()
support_lib.start()
```

### 2. Service Pattern

All functionality is accessed through **Module APIs** that create **Services**:

```python
# Get the module API
session_management_module = support_lib.get_session_manager_api()

# Create a service from the module
service_response = session_management_module.create_service()

if service_response.success and service_response.data is not None:
    service = service_response.data
    
    # Always initialize and start services
    service.initialise()
    service.start()
    
    # Use the service...
    
    # Always stop services when done
    service.stop()
```

### 3. Available Module APIs

| Module API | Purpose | Creates |
|------------|---------|---------|
| `get_session_manager_api()` | Session lifecycle management | Session Management Service |
| `get_data_format_manager_api()` | Parameter definitions and data formats | Data Format Management Service |
| `get_reading_packet_api()` | Read data from broker | Packet Reader Service |
| `get_writing_packet_api()` | Write data to broker | Packet Writer Service |

### 4. Data Format Management

Before reading or writing parameters, you need to work with **data format IDs**:

```python
# Get data format management service
data_format_service = support_lib.get_data_format_manager_api().create_service().data
data_format_service.initialise()
data_format_service.start()

# Get data format ID for specific parameters
format_response = data_format_service.get_parameter_data_format_id(
    data_source="Default",
    parameter_identifiers=["vCar:Chassis", "gLat:Chassis"]
)

if format_response.success:
    data_format_id = format_response.data.data_format_id
    
# Get parameter list from a data format ID
params_response = data_format_service.get_parameters_list(
    data_source="Default",
    data_format_id=data_format_id
)

if params_response.success:
    parameters = params_response.data.parameter_list
```

### 5. Reading Sessions

**For Live Sessions:**
```python
from ma_dataplatforms_streaming_support_library.contracts.packet_reading.packet_reading_configuration import PacketReadingConfiguration
from ma_dataplatforms_streaming_support_library.contracts.packet_reading.packet_reading_type import PacketReadingType

# Configure for live reading
packet_reading_config = PacketReadingConfiguration(
    session_key="",  # Empty for pattern matching
    timeout_seconds=10,
    raise_exception_on_timeout=False,
    data_source="Default",
    session_identifier_pattern="*",  # Match all sessions
    reading_type=PacketReadingType.LIVE,
    streams=[]
)

# Create packet reader service
packet_reader = packet_reader_module.create_service_with_config(packet_reading_config).data
packet_reader.initialise()
packet_reader.start()

# Set handler to process incoming packets
packet_reader.set_handler(your_handler)
```

**For Historic Sessions:**
```python
# Get available sessions
session_management_service = support_lib.get_session_manager_api().create_service().data
session_management_service.initialise()
session_management_service.start()

sessions_response = session_management_service.get_all_sessions()
sessions = sessions_response.data

# Read specific session
packet_reader = packet_reader_module.create_service(
    data_source=session.data_source,
    session_key=session.session_key
).data
packet_reader.initialise()
packet_reader.start()
packet_reader.set_handler(your_handler)
```

### 6. Writing Sessions

**Session Creation Workflow:**
```python
from ma_dataplatforms_streaming_support_library.contracts.session_management.session_creation_info import SessionCreationInfo
from ma_dataplatforms_streaming_support_library.protos.open_data_pb2 import Packet, NewSessionPacket, ConfigurationPacket

# 1. Create session
session_creation_info = SessionCreationInfo(
    data_source="Default",
    identifier="MySession",
    type="Session",
    version=1,
    utc_offset=datetime.datetime.now().astimezone().utcoffset(),
    details=[],
    associate_session_keys=[]
)

session_response = session_management_service.create_new_session(session_creation_info)
session_info = session_response.data

# 2. Start session (send NewSessionPacket to each stream)
new_session_packet = NewSessionPacket(...)
packet_writer_service.write_data(
    data_source=session_info.data_source,
    stream="",
    session_key=session_info.session_key,
    packet_bytes=PacketBytes(packet.SerializeToString())
)

# 3. Send configuration packet (parameter definitions)
config_packet = ConfigurationPacket(...)
packet_writer_service.write_data(...)

# 4. Write data packets
periodic_packet = PeriodicDataPacket(...)
packet_writer_service.write_data(...)

# 5. End session
end_session_packet = EndOfSessionPacket(...)
packet_writer_service.write_data(...)
session_management_service.end_session(data_source, session_key)
```

### 7. Event Handling

Services expose events that you can subscribe to:

```python
# Subscribe to events
packet_reader.session_reading_started.subscribe(on_session_started)
packet_reader.session_reading_completed.subscribe(on_session_completed)
packet_reader.stream_reading_started.subscribe(on_stream_started)

# Event handler signature (C# style)
def on_session_started(sender: object, session_info: SessionInfo):
    print(f"Session {session_info.identifier} started")

# Always unsubscribe when done
packet_reader.session_reading_started.unsubscribe(on_session_started)
```

### 8. Working with Open Data Protocol

The library uses Protocol Buffers (protobuf) for data serialization:

```python
from ma_dataplatforms_streaming_support_library.protos.open_data_pb2 import (
    Packet, PeriodicDataPacket, MarkerPacket, EventPacket
)

# Parse packet bytes
packet = Packet()
packet.ParseFromString(received_packet_dto.packet_bytes.data)

# Check packet type
if packet.type == "PeriodicData":
    periodic_packet = PeriodicDataPacket()
    periodic_packet.ParseFromString(packet.content)
    
    # Access samples
    for column in periodic_packet.columns:
        if column.WhichOneof("list") == "double_samples":
            for i, sample in enumerate(column.double_samples.samples):
                timestamp = periodic_packet.start_time + i * periodic_packet.interval
                value = sample.value
                status = sample.status
```

## Configuration

### Streaming API Configuration

```python
StreamingApiConfiguration(
    broker_list="localhost:9092",           # Kafka broker address
    stream_creation_strategy=StreamCreationStrategy.TOPIC_BASE,  # or STREAM_BASE
    additional_properties=[],                # Additional Kafka properties
    timeout_seconds=10                       # Connection timeout
)
```

### Packet Reading Configuration

```python
PacketReadingConfiguration(
    session_key="",                          # Specific session key or ""
    timeout_seconds=10,                      # Read timeout
    raise_exception_on_timeout=False,        # Exception behavior
    data_source="Default",                   # Data source name
    session_identifier_pattern="*",          # Session name pattern
    reading_type=PacketReadingType.LIVE,     # LIVE or HISTORIC
    streams=["Chassis", "Powertrain"]        # Specific streams or []
)
```

## Best Practices

### 1. Resource Management
Always follow the **initialize → start → use → stop** pattern:

```python
service = module.create_service().data
service.initialise()
service.start()

try:
    # Use service
    pass
finally:
    service.stop()
```

### 2. Handler Cleanup
Remove handlers when done to prevent memory leaks:

```python
packet_reader.set_handler(handler)
# ... use reader
packet_reader.remove_handler(handler)
```

### 3. Event Subscription Cleanup
Always unsubscribe from events:

```python
packet_reader.session_reading_started.subscribe(handler)
# ... use events
packet_reader.session_reading_started.unsubscribe(handler)
```

### 4. Error Handling
Check `ApiResult` responses:

```python
response = service.some_operation()
if not response.success or response.data is None:
    logger.error(f"Operation failed: {response.error_message}")
    return

data = response.data
```

### 5. Logger Implementation
Use the built-in logger or implement `ILogger`:

```python
from ma_dataplatforms_streaming_support_library.core.base.logger import Logger, ILogger

# Use default logger
logger = Logger()

# Or implement custom logger
class CustomLogger(ILogger):
    def debug(self, message: str): pass
    def info(self, message: str): pass
    def warning(self, message: str): pass
    def error(self, message: str): pass
```

## Creating Your Own Applications

### Basic Reader Application

```python
from ma_dataplatforms_streaming_support_library.core.base.support_library_bootstrapper import SupportLibraryBootstrapper
from ma_dataplatforms_streaming_support_library.core.base.logger import Logger
from ma_dataplatforms_streaming_support_library.contracts.shared.stream_api_configuration import StreamingApiConfiguration
from ma_dataplatforms_streaming_support_library.contracts.packet_reading.packet_reading_configuration import PacketReadingConfiguration
from ma_dataplatforms_streaming_support_library.contracts.packet_reading.i_handler import IHandler
from ma_dataplatforms_streaming_support_library.contracts.packet_reading.received_packet_dto import ReceivedPacketDto

class MyPacketHandler(IHandler[ReceivedPacketDto]):
    def handle(self, packet: ReceivedPacketDto) -> None:
        # Process packet
        print(f"Received packet for session: {packet.session_key}")

# Configure and bootstrap
config = StreamingApiConfiguration("localhost:9092", StreamCreationStrategy.TOPIC_BASE, [], 10)
logger = Logger()
support_lib_factory = SupportLibraryBootstrapper.bootstrap(config, logger)
support_lib = support_lib_factory.create()
support_lib.initialise()
support_lib.start()

# Create packet reader
packet_reader_module = support_lib.get_reading_packet_api()
reading_config = PacketReadingConfiguration("", 10, False, "Default", "*", PacketReadingType.LIVE, [])
packet_reader = packet_reader_module.create_service_with_config(reading_config).data

packet_reader.initialise()
packet_reader.start()
packet_reader.set_handler(MyPacketHandler())

# Wait for data...
input("Press Enter to stop...")

# Cleanup
packet_reader.stop()
support_lib.stop()
```

### Basic Writer Application

```python
from ma_dataplatforms_streaming_support_library.contracts.session_management.session_creation_info import SessionCreationInfo
from ma_dataplatforms_streaming_support_library.protos.open_data_pb2 import Packet, NewSessionPacket

# Bootstrap (same as reader)...

# Get services
session_mgmt = support_lib.get_session_manager_api().create_service().data
session_mgmt.initialise()
session_mgmt.start()

packet_writer = support_lib.get_writing_packet_api().create_service().data
packet_writer.initialise()
packet_writer.start()

# Create session
session_info_obj = SessionCreationInfo("Default", "MySession", "Test", 1, utc_offset, [], [])
session_response = session_mgmt.create_new_session(session_info_obj)
session_info = session_response.data

# Start session
new_session_pkt = NewSessionPacket(...)
packet = Packet(type="NewSession", session_key=session_info.session_key, ...)
packet_writer.write_data(session_info.data_source, "", session_info.session_key, 
                         PacketBytes(packet.SerializeToString()))

# Write data...

# End session
session_mgmt.end_session(session_info.data_source, session_info.session_key)

# Cleanup
packet_writer.stop()
session_mgmt.stop()
support_lib.stop()
```

## Troubleshooting

### Common Issues

**Issue:** `Failed to create service`  
**Solution:** Ensure you've called `bootstrap()` and the streaming API configuration is correct

**Issue:** `Connection timeout`  
**Solution:** Verify broker address and ensure Kafka is running and accessible

**Issue:** `Failed to get data format ID`  
**Solution:** Ensure data format management service is initialized and the data source exists

**Issue:** `No data received`  
**Solution:** Check that sessions exist on the broker and reading configuration matches session properties

**Issue:** Memory leaks  
**Solution:** Always call `stop()` on services and unsubscribe from events when done

## Limitations

- **Platform**: Currently only supports **Windows x64** (native DLL requirement)
- **Modules**: Buffering and Interpolation modules not yet available in Python (use C# version)
- **Threading**: The FFI layer may have threading limitations; check C# documentation

## Additional Resources

- **C# Documentation**: For complete feature reference, see the C# sample usage
- **Open Data Protocol**: Refer to protobuf definitions in the library package
- **ATLAS**: Use ATLAS Stream Recorder to record and visualize sessions

## Support

For issues, questions, or feature requests related to the Streaming Support Library, please contact Motion Applied support or refer to the main library documentation.

---

**Note:** This Python package wraps a native Windows x64 C# library using FFI. The C# version provides additional features (Buffering, Interpolation) that will be added to Python in future releases.