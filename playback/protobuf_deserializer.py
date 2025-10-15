#!/usr/bin/env python3
"""
Protobuf Deserializer Script

This script deserializes Google Protocol Buffers from .pb files.
It can handle any protobuf message type by using dynamic message parsing.
"""

import argparse
import sys
from pathlib import Path
from google.protobuf.descriptor_pb2 import FileDescriptorSet
from google.protobuf.message import DecodeError
from google.protobuf.json_format import MessageToJson
import json


def read_pb_file(file_path):
    """Read binary data from a .pb file."""
    try:
        with open(file_path, 'rb') as f:
            return f.read()
    except FileNotFoundError:
        print(f"Error: File '{file_path}' not found.")
        sys.exit(1)
    except Exception as e:
        print(f"Error reading file: {e}")
        sys.exit(1)


def try_deserialize_as_descriptor_set(data):
    """Try to deserialize data as a FileDescriptorSet."""
    try:
        descriptor_set = FileDescriptorSet()
        descriptor_set.ParseFromString(data)
        return descriptor_set
    except DecodeError:
        return None


def deserialize_generic_message(data):
    """Attempt to deserialize data as a generic protobuf message."""
    from google.protobuf.any_pb2 import Any
    from google.protobuf.struct_pb2 import Struct
    from google.protobuf.wrappers_pb2 import StringValue, Int32Value, Int64Value, BoolValue, DoubleValue, FloatValue

    # List of common protobuf message types to try
    message_types = [
        Any,
        Struct,
        StringValue,
        Int32Value,
        Int64Value,
        BoolValue,
        DoubleValue,
        FloatValue,
    ]

    for message_type in message_types:
        try:
            message = message_type()
            message.ParseFromString(data)
            return message, message_type.__name__
        except DecodeError:
            continue

    return None, None


def print_message_info(message, message_type_name=None):
    """Print information about the deserialized message."""
    if message_type_name:
        print(f"Successfully deserialized as: {message_type_name}")

    print("\n--- Message Content ---")
    print(message)

    print("\n--- JSON Representation ---")
    try:
        json_str = MessageToJson(message, preserving_proto_field_name=True)
        # Pretty print JSON
        json_obj = json.loads(json_str)
        print(json.dumps(json_obj, indent=2))
    except Exception as e:
        print(f"Could not convert to JSON: {e}")


def main():
    parser = argparse.ArgumentParser(
        description="Deserialize Google Protocol Buffers from .pb files"
    )
    parser.add_argument(
        "file_path",
        help="Path to the .pb file to deserialize"
    )
    parser.add_argument(
        "--output-format",
        choices=["text", "json", "both"],
        default="both",
        help="Output format (default: both)"
    )

    args = parser.parse_args()

    # Validate file extension
    if not args.file_path.endswith('.pb'):
        print("Warning: File does not have .pb extension")

    # Check if file exists
    if not Path(args.file_path).exists():
        print(f"Error: File '{args.file_path}' does not exist.")
        sys.exit(1)

    print(f"Reading protobuf file: {args.file_path}")
    data = read_pb_file(args.file_path)
    print(f"File size: {len(data)} bytes")

    # First, try to deserialize as a FileDescriptorSet
    descriptor_set = try_deserialize_as_descriptor_set(data)
    if descriptor_set:
        print("File appears to be a FileDescriptorSet (compiled .proto files)")
        print_message_info(descriptor_set, "FileDescriptorSet")
        return

    # Try to deserialize as common message types
    message, message_type = deserialize_generic_message(data)
    if message:
        print_message_info(message, message_type)
        return

    # If all else fails, show raw data info
    print("Could not deserialize as a known protobuf message type.")
    print("This might be a custom message type that requires the corresponding .proto file.")
    print(f"Raw data (first 100 bytes): {data[:100]}")

    # Try to show some basic protobuf field analysis
    print("\n--- Raw Protobuf Analysis ---")
    try:
        from google.protobuf.internal.decoder import _DecodeVarint32
        pos = 0
        field_count = 0
        while pos < len(data) and field_count < 10:  # Limit to first 10 fields
            try:
                tag, pos = _DecodeVarint32(data, pos)
                field_number = tag >> 3
                wire_type = tag & 0x7
                print(f"Field {field_number}, Wire Type {wire_type}")
                field_count += 1

                # Skip the value based on wire type
                if wire_type == 0:  # Varint
                    _, pos = _DecodeVarint32(data, pos)
                elif wire_type == 1:  # 64-bit
                    pos += 8
                elif wire_type == 2:  # Length-delimited
                    length, pos = _DecodeVarint32(data, pos)
                    pos += length
                elif wire_type == 5:  # 32-bit
                    pos += 4
                else:
                    break
            except:
                break
    except Exception as e:
        print(f"Could not analyze raw protobuf structure: {e}")


if __name__ == "__main__":
    main()