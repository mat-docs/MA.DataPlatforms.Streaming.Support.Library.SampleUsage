# MA DataPlatforms Streaming Support Library - Sample Usage

This repository contains sample code and documentation for using the **MA DataPlatforms Streaming Support Library**, available in both **C#** and **Python**. The library enables applications to interact with Motion Applied's streaming telemetry data platform for reading, writing, and processing real-time motorsport data.

## 📚 Documentation

Complete documentation is available in **MkDocs format**.

### View Documentation

```powershell
# Install dependencies
pip install -r docs-requirements.txt

# Serve documentation locally
mkdocs serve
```

Open your browser to: **http://127.0.0.1:8000**

For more details, see [MKDOCS_SETUP.md](MKDOCS_SETUP.md).

## Quick Start

Choose your implementation and follow the quick start guide:

### C# Implementation
**Location**: `MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation/`  
**Features**: Full feature set including buffering, interpolation, and SQL Race integration

See [C# Sample Guide](MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation/README.md) for detailed instructions.

### Python Implementation
**Location**: `Python/`  
**Features**: Core features (session management, packet reading/writing)

See [Python Sample Guide](Python/README.md) for detailed instructions.

## Repository Structure

```
MA.DataPlatforms.Streaming.Support.Library.SampleUsage/
│
├── docs/                               # MkDocs documentation
├── MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation/
│   ├── Program.cs                      # C# sample application
│   ├── README.md                       # C# documentation
│   ├── Buffering/                      # Buffering examples
│   ├── Interpolation/                  # Interpolation examples
│   └── SqlRace/                        # SQL Race integration
│
└── Python/
    ├── README.md                       # Python documentation
    ├── sample_code/
        ├── sample_reader/              # Reader example
        └── sample_writer/              # Writer example
```

## Feature Comparison

| Feature | C# | Python |
|---------|:--:|:------:|
| Session Management | ✅ | ✅ |
| Data Format Management | ✅ | ✅ |
| Packet Reading | ✅ | ✅ |
| Packet Writing | ✅ | ✅ |
| Buffering | ✅ | ❌ |
| Interpolation | ✅ | ❌ |
| SQL Race Integration | ✅ | ❌ |

## Platform Requirements

### C# Application
- **.NET**: 8.0 or later
- **Platform**: Cross-platform (Windows, Linux, macOS)
- **SQL Race**: Required for interpolation samples

### Python Application
- **Python**: 3.8 or later
- **Platform**: Windows x64 only (native DLL dependency)

## Additional Resources

- **Full Documentation**: Run `mkdocs serve` to view complete documentation
- **C# Samples**: [Detailed C# Guide](MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation/README.md)
- **Python Samples**: [Detailed Python Guide](Python/README.md)
- **Setup Instructions**: [MKDOCS_SETUP.md](MKDOCS_SETUP.md)

## License & Support

This sample code is provided by **Motion Applied Ltd.** for demonstration purposes.

For support or questions, please contact Motion Applied support.

---

**Version**: 2025 R03 Release
