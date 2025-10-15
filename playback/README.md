# Playback - Protobuf Deserializer

A Python script for deserializing Google Protocol Buffers from `.pb` files.

## Setup

1. Install dependencies:
```bash
pip install -r requirements.txt
```

## Usage

```bash
python protobuf_deserializer.py <file.pb>
```

### Options

- `--output-format`: Choose output format (`text`, `json`, or `both`)

### Examples

```bash
# Basic usage
python protobuf_deserializer.py data.pb

# JSON output only
python protobuf_deserializer.py data.pb --output-format json

# Text output only
python protobuf_deserializer.py data.pb --output-format text
```

## Features

- Automatically detects common protobuf message types
- Handles FileDescriptorSet files (compiled .proto files)
- Provides both text and JSON output formats
- Shows raw protobuf field analysis for unknown message types
- Error handling for invalid files

## Note

For custom message types, you may need the corresponding `.proto` definition files to properly deserialize the data.