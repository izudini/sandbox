# Position Distance Calculator

A C++ project that uses Python for geographic distance calculations via pybind11.

## Overview

This project demonstrates how to:
- Define position data using Protocol Buffers
- Implement distance calculations in Python using the haversine formula
- Call Python functions from C++ using pybind11
- Pass C++ structs to Python and receive results back

## Files

- `position.proto` - Protocol Buffer definition for position data
- `distance_calculator.py` - Python module with distance calculation logic
- `position.h` - C++ Position struct definition
- `python_interface.cpp` - Pybind11 wrapper for Python integration
- `main.cpp` - Demo program showing usage
- `CMakeLists.txt` - Build configuration

## Prerequisites

- CMake (3.12 or higher)
- C++ compiler with C++14 support
- Python 3.x with development headers
- pybind11

### Installing pybind11

```bash
# Using pip
pip install pybind11

# Or using conda
conda install pybind11

# Or using package manager (Ubuntu/Debian)
sudo apt-get install pybind11-dev
```

## Building

```bash
mkdir build
cd build
cmake ..
make
```

## Running

From the build directory:

```bash
export PYTHONPATH=..
./position_distance
```

The `PYTHONPATH=..` tells Python to look in the parent directory for the `distance_calculator.py` module.

## Example Output

```
Position 1: (40.7128, -74.0060, 10.0000)
Position 2: (34.0522, -118.2437, 100.0000)

Distance between positions: 3944208.50 meters (3944.21 km)

Testing with closer positions:
Position 3: (40.7589, -73.9851, 50.0000)
Position 4: (40.7614, -73.9776, 75.0000)
Distance: 864.54 meters
```

## How It Works

1. C++ creates Position structs with latitude, longitude, and altitude
2. The Python interface converts C++ structs to Python Position objects
3. Python calculates the 3D distance using the haversine formula
4. The result is returned to C++ as a double

The distance calculation accounts for both the great circle distance on Earth's surface and the altitude difference between points.