# Quick Start

Get up and running with the MA DataPlatforms Streaming Support Library in minutes!

## Choose Your Path

=== "Python"
    Perfect for rapid development and scripting scenarios.
    
    [:material-language-python: Start with Python](#python-quick-start)

=== "C#"
    Best for high-performance applications with full feature access.
    
    [:material-language-csharp: Start with C#](#c-quick-start)

---

## Python Quick Start

### 1. Setup Environment

```powershell
# Navigate to Python directory
cd Python

# Create and activate virtual environment
python -m venv venv
.\venv\Scripts\activate

# Install dependencies
python -m pip install -r Requirements.txt
```

### 2. Run Sample Reader

```powershell
cd sample_code\sample_reader
python main.py
```

When prompted, choose:

- `L` - Read **Live** sessions (waits for new sessions)
- `H` - Read **Historic** sessions (from existing data)

### 3. Run Sample Writer

```powershell
cd ..\sample_writer
python main.py
```


---

## C# Quick Start

### 1. Open Solution

```powershell
# Open in Visual Studio
start MA.DataPlatforms.Streaming.Support.Library.SampleUsage.sln
```

### 2. Configure

Edit `Program.cs` and update:

```csharp
// Update broker address
var streamApiConfig = new StreamingApiConfiguration(
    strategy: StreamCreationStrategy.TopicBased,
    brokerList: "localhost:9094",  // Your broker
    additionalProperties: []
);

// Update SQL Race connection (if using buffering/interpolation)
const string ConnectionString = 
    @"DbEngine=SQLite;Data Source=C:\Your\Path\Demo.ssndb;PRAGMA journal_mode=WAL;";

// Update parameters to monitor
var subscribedParameters = new List<string>
{
    "vCar:Chassis",     // Modify as needed
    "gLat:Chassis"
};
```

### 3. Build and Run

```powershell
# Using Visual Studio: Press F5

# Or using command line:
dotnet build
dotnet run --project MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation
```

### 4. Your First C# Application

Create a simple console app:

```csharp
using MA.DataPlatforms.Streaming.Support.Lib.Core.Abstractions;
using MA.Streaming.Core.Configs;

var logger = new Logger(LoggingLevel.Info);

var streamApiConfig = new StreamingApiConfiguration(
    StreamCreationStrategy.TopicBased,
    "localhost:9094",
    []
);

var supportLib = new SupportLibApiFactory().Create(logger, streamApiConfig);
supportLib.Initialise();
supportLib.Start();

Console.WriteLine("Support Library initialized successfully!");

supportLib.Stop();
```

---

## Common First Steps
### Initialize Support Library

=== "Python"
    ```python                   
    # Bootstrap the support library (connects Python to C# via FFI)
    support_lib_api_factory = SupportLibraryBootstrapper.bootstrap(
        streaming_api_configuration, 
        logger
    )
    
    # Create and start the support library
    support_lib = support_lib_api_factory.create()
    support_lib.initialise()
    support_lib.start()
    
    ```

=== "C#"
    ```csharp
    var supportLib = new SupportLibApiFactory().Create(logger, streamApiConfig);
    supportLib.Initialise();
    supportLib.Start();
    
    Console.WriteLine("✓ Successfully connected to broker");
    ```

### List Available Sessions

=== "Python"
    ```python
    # Get session management service
    session_mgmt = support_lib.get_session_manager_api().create_service().data
    session_mgmt.initialise()
    session_mgmt.start()
    
    # List sessions
    sessions_response = session_mgmt.get_all_sessions()
    if sessions_response.success:
        for session in sessions_response.data:
            print(f"Session: {session.identifier}")
    ```

=== "C#"
    ```csharp
    var sessionMgmtModule = supportLib.GetSessionManagerApi();
    var sessionMgmt = sessionMgmtModule.CreateService().Data;
    sessionMgmt.Initialise();
    sessionMgmt.Start();
    
    var sessions = sessionMgmt.GetAllSessions();
    foreach (var session in sessions.Data)
    {
        Console.WriteLine($"Session: {session.Identifier}");
    }
    ```

### Read Parameter Data

=== "Python"
    See [Sample Reader Guide](../python/sample-reader.md) for complete example.

=== "C#"
    See [C# Buffering Guide](../csharp/buffering.md) for complete example.

---

## Next Steps by Use Case

### I want to... Monitor Live Data

**Recommended**: Python Sample Reader or C# Packet Reader

- [:material-language-python: Python Reader Guide](../python/sample-reader.md)
- [:material-language-csharp: C# Packet Reader](../csharp/index.md)

### I want to... Write Data to Broker

**Recommended**: Python Sample Writer or C# Packet Writer

- [:material-language-python: Python Writer Guide](../python/sample-writer.md)
- [:material-language-csharp: C# Packet Writer](../csharp/index.md)

### I want to... Buffer and Process Data

**Recommended**: C# Buffering & Interpolation (Python coming soon)

- [:material-language-csharp: C# Buffering Guide](../csharp/buffering.md)
- [:material-language-csharp: C# Interpolation Guide](../csharp/interpolation.md)

### I want to... Create Custom Processors

**Recommended**: C# Custom Processors

- [:material-language-csharp: Custom Processor Guide](../csharp/custom-processors.md)

---

## Troubleshooting Quick Fixes

!!! failure "Connection Timeout"
    **Check**: Is Kafka running?
    ```powershell
    # If using Docker
    docker ps | findstr kafka
    
    # Test connection
    Test-NetConnection -ComputerName localhost -Port 9092
    ```

!!! failure "DLL Load Failed (Python)"
    **Solution**: Install VC++ Redistributables
    
    Download from: [Microsoft VC++ Downloads](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist)

!!! failure "NuGet Restore Failed (C#)"
    **Solution**: Clear cache and restore
    ```powershell
    dotnet nuget locals all --clear
    dotnet restore
    ```

---

## Getting Help

!!! question "Need More Help?"
    - [:material-book-open: Full Documentation](../python/index.md)
    - [:material-help-circle: Troubleshooting Guides](../python/troubleshooting.md)
    - [:material-email: Contact Support](../support.md)

---

**Congratulations!** You're now ready to build streaming telemetry applications! 🎉
