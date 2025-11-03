# Feature Comparison

This page provides a detailed comparison of features available in the C# and Python implementations of the MA DataPlatforms Streaming Support Library.

## Platform Support

| Platform | C# | Python | Notes |
|----------|:--:|:------:|-------|
| Windows 10/11 (x64) | ✅ | ✅ | Required for both |
| Linux | ❌ | ❌ | Not supported (native DLL requirement) |
| macOS | ❌ | ❌ | Not supported (native DLL requirement) |

## Core Features

| Feature | C# | Python | Status |
|---------|:--:|:------:|--------|
| **Session Management** | ✅ | ✅ | Full parity |
| Create Sessions | ✅ | ✅ | |
| Update Sessions | ✅ | ✅ | |
| End Sessions | ✅ | ✅ | |
| Query Sessions | ✅ | ✅ | |
| **Data Format Management** | ✅ | ✅ | Full parity |
| Create Format IDs | ✅ | ✅ | |
| Get Parameter Lists | ✅ | ✅ | |
| **Packet Reading** | ✅ | ✅ | Full parity |
| Live Sessions | ✅ | ✅ | |
| Historic Sessions | ✅ | ✅ | |
| Stream Filtering | ✅ | ✅ | |
| Event Subscription | ✅ | ✅ | |
| **Packet Writing** | ✅ | ✅ | Full parity |
| Write Data Packets | ✅ | ✅ | |
| Write Session Info | ✅ | ✅ | |
| All Packet Types | ✅ | ✅ | |

## Advanced Features

| Feature | C# | Python | Status |
|---------|:--:|:------:|--------|
| **Buffering** | ✅ | 🚧 | Python: Planned for future release |
| Time-windowed Aggregation | ✅ | 🚧 | |
| SampleData Mode | ✅ | 🚧 | |
| TimestampData Mode | ✅ | 🚧 | |
| Custom Merge Strategies | ✅ | 🚧 | |
| Include Markers | ✅ | 🚧 | |
| Include Events | ✅ | 🚧 | |
| **Interpolation** | ✅ | 🚧 | Python: Planned for future release |
| Statistical Processing | ✅ | 🚧 | |
| Custom Processors | ✅ | 🚧 | |
| Configurable Frequencies | ✅ | 🚧 | |
| Batch Result Handling | ✅ | 🚧 | |
| **SQL Race Integration** | ✅ | ❌ | Python: Not planned |
| Direct Database Storage | ✅ | ❌ | |
| ATLAS Visualization | ✅ | ❌ | |

## Performance Characteristics

| Characteristic | C# (Native NuGet) | Python (FFI Wrapper) |
|----------------|:-----------------:|:--------------------:|
| **Execution Speed** | Native .NET | FFI Overhead |
| **Memory Efficiency** | High | Good |
| **Throughput** | Very High | High |
| **Latency** | Very Low | Low-Medium |
| **CPU Usage** | Optimized | Moderate |
| **Library Access** | Direct Assembly | FFI Marshalling |

### Performance Notes

- **C# (Native)**: Uses NuGet package directly - zero FFI overhead, optimal for high-throughput scenarios
- **Python (FFI)**: Calls C# library via Foreign Function Interface - adds marshalling overhead but suitable for most use cases
- C# buffering/interpolation can handle higher data rates due to native execution
- Python is excellent for scripting, prototyping, and moderate workloads where ease of use is prioritized

## Development Experience

| Aspect | C# | Python |
|--------|:--:|:------:|
| **IDE Support** | Excellent (VS, VS Code) | Excellent (VS Code, PyCharm) |
| **Debugging** | Full native debugging | FFI debugging limitations |
| **Type Safety** | Strong typing | Dynamic typing (with hints) |
| **Async/Await** | Native support | Via asyncio |
| **Package Management** | NuGet | pip |
| **Build Tools** | MSBuild, dotnet CLI | pip, setup.py |

## API Consistency

Both implementations follow the same service pattern:

| Step | C# | Python | Notes |
|------|:--:|:------:|-------|
| Bootstrap | ✅ | ✅ | Slightly different syntax |
| Get Module API | ✅ | ✅ | Same pattern |
| Create Service | ✅ | ✅ | Same pattern |
| Initialize | ✅ | ✅ | Same method names |
| Start | ✅ | ✅ | Same method names |
| Stop | ✅ | ✅ | Same method names |

## Use Case Recommendations

### When to Use C#

✅ **Best for:**

- High-throughput real-time processing
- Buffering and interpolation requirements
- SQL Race/ATLAS integration
- Production systems requiring maximum performance
- Applications needing custom processors
- Long-running services

### When to Use Python

✅ **Best for:**

- Rapid development and prototyping
- Scripting and automation
- Data analysis workflows
- Integration with Python data science stack
- Simple read/write operations
- Moderate data rates

## Migration Path

### Python to C#

**Reasons to migrate:**

- Need buffering or interpolation
- Require SQL Race integration
- Performance requirements exceed Python capabilities
- Need custom processors

**Difficulty**: Medium

- Similar API patterns make migration straightforward
- Main differences are syntax and type system
- Most concepts transfer directly

[:material-swap-horizontal: Contact Support for Migration Assistance](../support.md)

### C# to Python

**Reasons to migrate:**

- Prefer Python ecosystem
- Simpler deployment
- Integration with Python tools
- Don't need buffering/interpolation

**Difficulty**: Easy (if features are available)

- Python API is simpler in some areas
- Less boilerplate code
- Dynamic typing can speed development

## Version Compatibility

| Component | C# Version | Python Version |
|-----------|------------|----------------|
| .NET Runtime | 8.0+ | N/A |
| Python | N/A | 3.8+ |
| Library Package | Via NuGet | Via pip/GitHub |
| Kafka | 2.0+ | 2.0+ |
| Protocol Buffers | Included | protobuf 6.31.1+ |

## Future Roadmap

### Planned for Python

🚧 **In Development:**

- Buffering module (time-windowed aggregation)
- Interpolation module (statistical processing)
- Custom processor support

### Potential Future Features

💡 **Under Consideration:**

- Cross-platform support (Linux, macOS)
- Additional broker support beyond Kafka
- Enhanced async support
- Performance optimizations

---

!!! info "Feature Requests"
    Have a feature you'd like to see? Contact Motion Applied support with your use case.
