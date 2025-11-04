# Overview

## Introduction

The **MA DataPlatforms Streaming Support Library** enables applications to interact with streaming telemetry data for motorsport and automotive applications. The library is available in two forms:

- **C# Library**: Available as a NuGet package for .NET 8.0+ (cross-platform: Windows, Linux, macOS)
- **Python FFI Wrapper**: Python bindings that call the C# library via Foreign Function Interface (Windows x64 only)

Both implementations provide high-performance data streaming, session management, and data processing capabilities.

## What is the Streaming Support Library?

The library provides a comprehensive API for:

- **Reading** live and historic telemetry sessions from streaming brokers
- **Writing** telemetry data and sessions to streaming brokers
- **Managing** session lifecycle and metadata
- **Processing** data through buffering and interpolation (C# only)
- **Integrating** with SQL Race for data visualization in ATLAS

## Architecture Overview

![Architecture Overview](../images/architecture-overview.svg)

## Key Concepts

### Service Pattern

All functionality is accessed through a consistent service pattern:

1. **Bootstrap** - Initialize the Support Library with configuration
2. **Get Module API** - Obtain the factory for desired functionality  
3. **Create Service** - Create a service instance from the module
4. **Initialize** - Call `Initialise()` on the service
5. **Start** - Call `Start()` to begin operations
6. **Use** - Interact with the service
7. **Stop** - Call `Stop()` when finished

### Available Modules

| Module | Purpose | C# | Python |
|--------|---------|:--:|:------:|
| **Session Management** | Create, update, end sessions | ✅ | ✅ |
| **Data Format Management** | Manage parameter definitions | ✅ | ✅ |
| **Packet Reader** | Read telemetry data | ✅ | ✅ |
| **Packet Writer** | Write telemetry data | ✅ | ✅ |
| **Sample Reader (Buffering)** | Time-windowed data aggregation | ✅ | 🚧 |
| **Data Reader (Interpolation)** | Statistical processing | ✅ | 🚧 |

### Data Flow

#### Reading Data

The reading process follows a structured sequence:

1. Create and initialize Packet Reader service
2. Set up packet handler to process received data
3. Start the reader service
4. Subscribe to Kafka broker streams
5. Receive packets from broker (continuous stream)
6. Handler processes each packet (extract and parse data)
7. Stop reader when finished

#### Writing Data

The writing process follows a structured sequence:

1. Create session via Session Manager
2. Write NewSession packet to broker
3. Write Configuration packet with parameter definitions
4. Write Data packets (continuous loop)
5. Write EndSession packet
6. End session via Session Manager

## Core Components

### 1. Session Management

Handles the complete lifecycle of telemetry sessions:

- Creating new sessions
- Updating session metadata
- Ending sessions
- Querying available sessions

### 2. Data Format Management

Manages parameter definitions and data formats:

- Creating data format IDs for parameter sets
- Retrieving parameter lists from format IDs
- Managing parameter metadata

### 3. Packet Reader/Writer

Handles data transmission to/from the broker:

- Reading live sessions (real-time streaming)
- Reading historic sessions (playback)
- Writing data packets
- Managing packet serialization

### 4. Buffering (C# Only)

Aggregates streaming data into time-based windows:

- Configurable window sizes
- Two modes: SampleData and TimestampData
- Support for markers, events, errors, CAN data
- Custom merge strategies

### 5. Interpolation (C# Only)

Processes buffered data at configurable frequencies:

- Built-in statistical processor (First, Last, Mean, Min, Max)
- Custom processor support
- Configurable subscription and delivery frequencies
- Batch result handling

## Open Data Protocol

The library uses **Protocol Buffers (protobuf)** for data serialization, supporting various packet types:

- **NewSessionPacket** - Session start marker
- **SessionInfo** - Session metadata updates
- **DataFormatDefinitionPacket** - Parameter and event format definitions
- **PeriodicDataPacket** - Time-series sample data at regular intervals
- **RowDataPacket** - Timestamped row data
- **MarkerPacket** - Event markers (laps, sectors, etc.)
- **EventPacket** - Event occurrences
- **EndOfSessionPacket** - Session completion marker
- **CoverageCursorInfoPacket** - Coverage information

## Platform Requirements

### C# Requirements

- .NET 8.0 or later (cross-platform)
- Kafka broker access
- SQL Race (optional, for visualization features)

### Python Requirements

!!! warning "Platform Limitation"
    The Python package uses FFI to call native Windows DLLs and **only supports Windows 10/11 (x64)**.

- Windows 10/11 (x64) - **Required**
- Python 3.8 or later
- Kafka broker access

## Feature Comparison

| Feature | C# | Python | Notes |
|---------|:--:|:------:|-------|
| Session Management | ✅ | ✅ | Full support both platforms |
| Data Format Management | ✅ | ✅ | Full support both platforms |
| Packet Reading | ✅ | ✅ | Live and historic sessions |
| Packet Writing | ✅ | ✅ | All packet types supported |
| Buffering | ✅ | 🚧 | C# only (Python planned) |
| Interpolation | ✅ | 🚧 | C# only (Python planned) |
| SQL Race Integration | ✅ | ❌ | C# only |
| Custom Processors | ✅ | 🚧 | C# only (Python planned) |

## Next Steps

<div class="grid cards" markdown>

-   :material-download:{ .lg .middle } __Installation__

    ---

    Install the library for your platform

    [:octicons-arrow-right-24: Install Now](installation.md)

-   :material-rocket-launch:{ .lg .middle } __Quick Start__

    ---

    Get up and running quickly

    [:octicons-arrow-right-24: Quick Start](quick-start.md)

-   :material-language-python:{ .lg .middle } __Python Guide__

    ---

    Python-specific documentation

    [:octicons-arrow-right-24: Python Docs](../python/index.md)

-   :material-language-csharp:{ .lg .middle } __C# Guide__

    ---

    C#-specific documentation

    [:octicons-arrow-right-24: C# Docs](../csharp/index.md)

</div>
