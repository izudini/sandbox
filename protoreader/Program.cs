using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System.Text;

namespace protoReader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: protoReader <file.pb> [--format json|text|both]");
                Console.WriteLine("Deserializes Google Protocol Buffers from .pb files");
                return;
            }

            string filePath = args[0];
            string outputFormat = "both";

            // Parse command line arguments
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] == "--format" && i + 1 < args.Length)
                {
                    outputFormat = args[i + 1].ToLower();
                    i++; // Skip the next argument
                }
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File '{filePath}' not found.");
                return;
            }

            if (!filePath.EndsWith(".pb", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Warning: File does not have .pb extension");
            }

            try
            {
                Console.WriteLine($"Reading protobuf file: {filePath}");
                byte[] data = File.ReadAllBytes(filePath);
                Console.WriteLine($"File size: {data.Length} bytes");

                bool success = TryDeserializeMessage(data, outputFormat);

                if (!success)
                {
                    Console.WriteLine("Could not deserialize as a known protobuf message type.");
                    Console.WriteLine("This might be a custom message type that requires the corresponding .proto file.");
                    AnalyzeRawProtobuf(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static bool TryDeserializeMessage(byte[] data, string outputFormat)
        {
            // Try common well-known types
            var wellKnownTypes = new List<(Func<IMessage> factory, string name)>
            {
                (() => new Any(), "Any"),
                (() => new Struct(), "Struct"),
                (() => new Value(), "Value"),
                (() => new ListValue(), "ListValue"),
                (() => new StringValue(), "StringValue"),
                (() => new Int32Value(), "Int32Value"),
                (() => new Int64Value(), "Int64Value"),
                (() => new BoolValue(), "BoolValue"),
                (() => new DoubleValue(), "DoubleValue"),
                (() => new FloatValue(), "FloatValue"),
                (() => new BytesValue(), "BytesValue"),
                (() => new UInt32Value(), "UInt32Value"),
                (() => new UInt64Value(), "UInt64Value"),
                (() => new Timestamp(), "Timestamp"),
                (() => new Duration(), "Duration"),
                (() => new Empty(), "Empty")
            };

            foreach (var (factory, typeName) in wellKnownTypes)
            {
                try
                {
                    var message = factory();
                    message.MergeFrom(data);

                    Console.WriteLine($"Successfully deserialized as: {typeName}");
                    PrintMessageInfo(message, outputFormat);
                    return true;
                }
                catch (InvalidProtocolBufferException)
                {
                    // Continue to next type
                }
                catch (Exception)
                {
                    // Continue to next type
                }
            }

            // Try FileDescriptorSet
            try
            {
                var descriptorSet = FileDescriptorSet.Parser.ParseFrom(data);
                Console.WriteLine("Successfully deserialized as: FileDescriptorSet");
                PrintMessageInfo(descriptorSet, outputFormat);
                return true;
            }
            catch (InvalidProtocolBufferException)
            {
                // Not a FileDescriptorSet
            }

            return false;
        }

        static void PrintMessageInfo(IMessage message, string outputFormat)
        {
            if (outputFormat == "text" || outputFormat == "both")
            {
                Console.WriteLine("\n--- Message Content (Text) ---");
                Console.WriteLine(message.ToString());
            }

            if (outputFormat == "json" || outputFormat == "both")
            {
                Console.WriteLine("\n--- JSON Representation ---");
                try
                {
                    string jsonString = JsonFormatter.Default.Format(message);
                    // Pretty print JSON
                    var jsonObject = JsonConvert.DeserializeObject(jsonString);
                    string prettyJson = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                    Console.WriteLine(prettyJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not convert to JSON: {ex.Message}");
                }
            }

            // Show field information
            Console.WriteLine("\n--- Message Fields ---");
            var descriptor = message.Descriptor;
            Console.WriteLine($"Message Type: {descriptor.FullName}");
            Console.WriteLine($"Field Count: {descriptor.Fields.Count}");

            foreach (var field in descriptor.Fields.InFieldNumberOrder())
            {
                var accessor = field.Accessor;
                var value = accessor.GetValue(message);
                Console.WriteLine($"  {field.Name} ({field.FieldNumber}): {value ?? "null"}");
            }
        }

        static void AnalyzeRawProtobuf(byte[] data)
        {
            Console.WriteLine($"\nRaw data (first 100 bytes): {Convert.ToHexString(data.Take(100).ToArray())}");

            Console.WriteLine("\n--- Raw Protobuf Analysis ---");
            try
            {
                using var stream = new MemoryStream(data);
                using var reader = new CodedInputStream(stream);

                int fieldCount = 0;
                while (!reader.IsAtEnd && fieldCount < 10) // Limit to first 10 fields
                {
                    try
                    {
                        uint tag = reader.ReadTag();
                        if (tag == 0) break;

                        int fieldNumber = WireFormat.GetTagFieldNumber(tag);
                        WireFormat.WireType wireType = WireFormat.GetTagWireType(tag);

                        Console.WriteLine($"Field {fieldNumber}, Wire Type {wireType}");
                        fieldCount++;

                        // Skip the value based on wire type
                        switch (wireType)
                        {
                            case WireFormat.WireType.Varint:
                                reader.ReadUInt64();
                                break;
                            case WireFormat.WireType.Fixed64:
                                reader.ReadFixed64();
                                break;
                            case WireFormat.WireType.LengthDelimited:
                                reader.ReadBytes();
                                break;
                            case WireFormat.WireType.Fixed32:
                                reader.ReadFixed32();
                                break;
                            default:
                                Console.WriteLine($"Unknown wire type: {wireType}");
                                return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error analyzing field: {ex.Message}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not analyze raw protobuf structure: {ex.Message}");
            }
        }
    }
}