# Installation

This guide covers the installation process for both Python and C# implementations of the MA DataPlatforms Streaming Support Library.

## Prerequisites

!!! warning "Platform Requirement"
    **Windows 10/11 (x64)** is required for both Python and C# implementations due to native DLL dependencies.

### Common Requirements

- Access to a **Kafka broker** (for streaming functionality)
- **Administrative privileges** (for initial setup)
- **Internet connection** (for package downloads)

## Python Installation

### Step 1: Python Environment

Ensure you have Python 3.8 or later installed:

```powershell
# Check Python version
python --version
```

### Step 2: Create Virtual Environment (Recommended)

```powershell
# Navigate to Python sample directory
cd Python

# Create virtual environment
python -m venv venv

# Activate virtual environment
.\venv\Scripts\activate
```

### Step 3: Install Dependencies

```powershell
# Install from requirements file
python -m pip install -r Requirements.txt
```

This installs:

- `ma_dataplatforms_streaming_support_library` - The Support Library Python package
- `protobuf` - Protocol Buffers for data serialization

### Step 4: Verify Installation

```python
# Test import
python -c "from ma_dataplatforms_streaming_support_library.core.base.support_library_bootstrapper import SupportLibraryBootstrapper; print('Installation successful!')"
```

## C# Installation

### Step 1: Development Environment

Install the following:

- [**.NET 8.0 SDK**](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- **Visual Studio 2022** (recommended) or VS Code with C# extension
- **SQL Race** (for buffering/interpolation features)

### Step 2: Open Solution

```powershell
# Open the solution file
start MA.DataPlatforms.Streaming.Support.Library.SampleUsage.sln
```

Or open in Visual Studio manually.

### Step 3: Restore NuGet Packages

Visual Studio will automatically restore packages. Or manually:

```powershell
dotnet restore
```

### Step 4: Configure SQL Race

Update the connection string in `Program.cs`:

```csharp
const string ConnectionString = 
    @"DbEngine=SQLite;Data Source=C:\Motion Applied\SupportFilesDemo\SupportFilesDemo.ssndb;PRAGMA journal_mode=WAL;";
```

### Step 5: Build Solution

```powershell
# Build using dotnet CLI
dotnet build

# Or use Visual Studio: Build > Build Solution (Ctrl+Shift+B)
```

## Kafka Broker Setup

### Local Development (Docker)

The easiest way to run Kafka locally for development:

```powershell
# Using Docker Compose (create docker-compose.yml)
docker-compose up -d
```

Example `docker-compose.yml`:

```yaml
version: '3'
services:
  zookeeper:
    image: confluentinc/cp-zookeeper:latest
    environment:
      ZOOKEEPER_CLIENT_PORT: 2181
      ZOOKEEPER_TICK_TIME: 2000
    ports:
      - 2181:2181

  kafka:
    image: confluentinc/cp-kafka:latest
    depends_on:
      - zookeeper
    ports:
      - 9092:9092
      - 9094:9094
    environment:
      KAFKA_BROKER_ID: 1
      KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://localhost:9092,PLAINTEXT_HOST://localhost:9094
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT
      KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
```

### Production/Remote Broker

Update the broker configuration in your code:

=== "Python"
    ```python
    streaming_api_config = StreamingApiConfiguration(
        broker_list="your-broker:9092",  # Update this
        stream_creation_strategy=StreamCreationStrategy.TOPIC_BASE,
        additional_properties=[],
        timeout_seconds=10
    )
    ```

=== "C#"
    ```csharp
    var streamApiConfig = new StreamingApiConfiguration(
        strategy: StreamCreationStrategy.TopicBased,
        brokerList: "your-broker:9092",  // Update this
        additionalProperties: []
    );
    ```

## Verify Installation

### Python Verification

```powershell
# Run the sample reader
cd Python\sample_code\sample_reader
python main.py
```

### C# Verification

```powershell
# Run the application
cd MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation\bin\Debug\net8.0-windows
.\MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.exe
```

## Troubleshooting

### Python Issues

!!! failure "Import Error: DLL load failed"
    **Cause**: Not running on Windows x64 or missing VC++ redistributables
    
    **Solution**: 
    
    1. Verify Windows x64: `systeminfo | findstr /C:"System Type"`
    2. Install [VC++ Redistributables](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist)

!!! failure "Module Not Found Error"
    **Cause**: Package not installed or wrong virtual environment
    
    **Solution**:
    ```powershell
    # Ensure virtual environment is activated
    .\venv\Scripts\activate
    
    # Reinstall packages
    python -m pip install -r Requirements.txt --force-reinstall
    ```

### C# Issues

!!! failure "Build Failed: Missing NuGet Packages"
    **Cause**: NuGet packages not restored
    
    **Solution**:
    ```powershell
    # Clear NuGet cache
    dotnet nuget locals all --clear
    
    # Restore packages
    dotnet restore
    ```

!!! failure "SQL Race Connection Failed"
    **Cause**: SQL Race not installed or incorrect connection string
    
    **Solution**:
    
    1. Install SQL Race
    2. Verify connection string path exists
    3. Check database file permissions

### Kafka Issues

!!! failure "Connection Timeout"
    **Cause**: Kafka broker not running or incorrect address
    
    **Solution**:
    ```powershell
    # Check if Kafka is running (Docker)
    docker ps | findstr kafka
    
    # Check network connectivity
    Test-NetConnection -ComputerName localhost -Port 9092
    ```

## Next Steps

<div class="grid cards" markdown>

-   :material-rocket-launch:{ .lg .middle } __Quick Start__

    ---

    Get up and running with your first application

    [:octicons-arrow-right-24: Quick Start](quick-start.md)

-   :material-language-python:{ .lg .middle } __Python Guide__

    ---

    Explore Python-specific features

    [:octicons-arrow-right-24: Python Docs](../python/index.md)

-   :material-language-csharp:{ .lg .middle } __C# Guide__

    ---

    Explore C#-specific features

    [:octicons-arrow-right-24: C# Docs](../csharp/index.md)

</div>
