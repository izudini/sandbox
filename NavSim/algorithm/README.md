# Algorithm Project

A Python project for navigation simulation algorithms using protobuf for data serialization.

## Structure

- `src/` - Source code
  - `position.py` - Position class wrapper
  - `position_pb2.py` - Generated protobuf classes
- `proto/` - Protocol buffer definitions
- `requirements.txt` - Python dependencies

## Usage

```python
from src.position import Position

# Create a position
pos = Position(entity_id=1, latitude=40.7128, longitude=-74.0060, altitude=10.0, heading=90.0)

# Serialize to bytes
data = pos.serialize()

# Deserialize from bytes
restored_pos = Position.deserialize(data)
```