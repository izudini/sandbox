# protoReader - C# Protobuf Deserializer

A C# console application for deserializing Google Protocol Buffers from `.pb` files.

## Prerequisites

- .NET 6.0 or later
- Visual Studio or Visual Studio Code (optional, for development)

## Build

```bash
dotnet build
```

## Usage

```bash
dotnet run <file.pb> [--format json|text|both]
```

### Examples

```bash
# Basic usage (shows both text and JSON)
dotnet run data.pb

# JSON output only
dotnet run data.pb --format json

# Text output only
dotnet run data.pb --format text
```

## Features

- Automatically detects common protobuf well-known types:
  - Any, Struct, Value, ListValue
  - StringValue, Int32Value, Int64Value, BoolValue, etc.
  - Timestamp, Duration, Empty
- Handles FileDescriptorSet files (compiled .proto files)
- Provides both text and JSON output formats
- Shows detailed field information
- Raw protobuf field analysis for unknown message types
- Comprehensive error handling

## Dependencies

- Google.Protobuf (3.25.1)
- Newtonsoft.Json (13.0.3)

## Note

For custom message types, you may need the corresponding `.proto` definition files and generated C# classes to properly deserialize the data.