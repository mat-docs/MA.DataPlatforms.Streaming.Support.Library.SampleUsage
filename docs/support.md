# Support

## Getting Help

If you need assistance with the MA DataPlatforms Streaming Support Library, there are several resources available:

### Documentation

- **This Documentation Site** - Comprehensive guides and API reference
- **Sample Code** - Working examples in both Python and C#
- **Main Library Documentation** - Detailed technical documentation

### Contact Support

For technical support, bug reports, or feature requests:

!!! info "Motion Applied Support"
    Contact Motion Applied support team for assistance with:
    
    - Library installation and configuration
    - Technical issues and troubleshooting
    - Feature requests and enhancements
    - Integration support
    - Performance optimization

### GitHub Repository

Visit our GitHub repository for:

- Source code for sample applications
- Issue tracking
- Contributions and pull requests

[:material-github: GitHub Repository](https://github.com/mat-docs/MA.DataPlatforms.Streaming.Support.Library.SampleUsage)

## Common Questions

### General Questions

??? question "What platforms are supported?"
    Currently, only **Windows 10/11 (x64)** is supported due to native DLL dependencies. Both Python and C# require Windows x64.

??? question "Can I use this on Linux or macOS?"
    No, the library uses native Windows x64 DLLs and cannot run on Linux or macOS at this time.

??? question "What's the difference between Python and C# versions?"
    The C# version has full functionality including Buffering and Interpolation. The Python version uses FFI to wrap the C# library and currently supports Session Management, Data Format Management, and Packet Reading/Writing. Buffering and Interpolation will be added to Python in the future.

### Python Questions

??? question "Why do I get 'DLL load failed' error?"
    This error occurs when:
    
    1. You're not on Windows x64
    2. Missing Visual C++ Redistributables
    3. Python architecture doesn't match (must be x64)
    
    Install [VC++ Redistributables](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist) and ensure 64-bit Python.

??? question "Can I use Python 3.7 or earlier?"
    No, Python 3.8 or later is required.

??? question "When will Buffering and Interpolation be available in Python?"
    These features are planned for future releases. Check the main library documentation for roadmap updates.

### C# Questions

??? question "Do I need Visual Studio?"
    Visual Studio is recommended but not required. You can use VS Code with C# extension or build from command line using .NET CLI.

??? question "What .NET version is required?"
    .NET 8.0 or later is required.

??? question "Is SQL Race required?"
    SQL Race is only required if you want to write to SQL Race databases. For basic reading/writing, it's not needed.

### Kafka/Broker Questions

??? question "What Kafka version is supported?"
    Kafka 2.0 or later is supported.

??? question "Can I use other message brokers instead of Kafka?"
    Currently, the library is designed for Kafka. Support for other brokers is not available.

??? question "How do I run Kafka locally for development?"
    Use Docker with the provided docker-compose.yml example in the [Installation](getting-started/installation.md#kafka-broker-setup) guide.

### Performance Questions

??? question "What are the performance characteristics?"
    Performance depends on:
    
    - **C#**: Direct native performance, optimal for high-throughput scenarios
    - **Python**: FFI overhead adds latency but suitable for most use cases
    - **Buffering window size**: Larger windows use more memory
    - **Interpolation frequency**: Higher frequencies increase CPU usage

??? question "Can I process data in real-time?"
    Yes, the library is designed for real-time telemetry streaming with low latency.

## Reporting Issues

### Bug Reports

When reporting bugs, please include:

1. **Environment Information**
    - OS version (Windows 10/11)
    - Python version (if applicable)
    - .NET version (if applicable)
    - Library version

2. **Steps to Reproduce**
    - Clear, step-by-step instructions
    - Minimal code example
    - Expected vs actual behavior

3. **Error Messages**
    - Complete error messages and stack traces
    - Relevant log output

4. **Configuration**
    - Broker configuration
    - Relevant settings and parameters

### Feature Requests

For feature requests, describe:

1. **Use Case** - What problem are you trying to solve?
2. **Proposed Solution** - How would you like it to work?
3. **Alternatives** - What workarounds have you considered?
4. **Impact** - How would this help your project?

## Additional Resources

### External Documentation

- [Apache Kafka Documentation](https://kafka.apache.org/documentation/)
- [Protocol Buffers Documentation](https://developers.google.com/protocol-buffers)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Python Documentation](https://docs.python.org/3/)

### Learning Resources

- [Kafka Tutorial](https://kafka.apache.org/quickstart)
- [gRPC and Protocol Buffers](https://grpc.io/docs/what-is-grpc/introduction/)
- [Async Programming in C#](https://docs.microsoft.com/en-us/dotnet/csharp/async)
- [Python Best Practices](https://docs.python-guide.org/)

## Community

Stay connected with the Motion Applied developer community:

- Share your projects using the library
- Contribute improvements and bug fixes
- Help other developers with questions

---

!!! tip "Can't find what you're looking for?"
    Contact Motion Applied support for personalized assistance.
