# KC-135 Sensor Simulator with C++ Backend Integration

This project integrates a C++ sensor controller backend with a C# Windows Forms frontend.

## Project Structure

- `KC-135/` - C# Windows Forms application
- `sensorSimBackend/` - C++ sensor controller that can be built as a DLL

## Setup Instructions

### 1. Build the C++ DLL

To build the C++ backend as a DLL, you'll need:
- Visual Studio 2019 or later with C++ development tools
- CMake (optional) or direct compilation

#### Option A: Using Visual Studio Developer Command Prompt
```cmd
cd sensorSimBackend
cl /LD /EHsc /DSENSOR_CONTROLLER_EXPORTS /std:c++17 SensorControllerAPI.cpp Sensor.cpp UDPSocketListener.cpp TCPServer.cpp /Fe:SensorController.dll ws2_32.lib
```

#### Option B: Using CMake (if available)
```cmd
cd sensorSimBackend
mkdir build
cd build
cmake ..
cmake --build . --config Release
```

### 2. Copy DLL to C# Output Directory

After building the DLL, copy `SensorController.dll` to:
- `KC-135/bin/Debug/net8.0-windows/` (for debug builds)
- `KC-135/bin/Release/net8.0-windows/` (for release builds)

### 3. Build and Run C# Application

```cmd
cd KC-135
dotnet build
dotnet run
```

## Features Added

### C++ Backend (`SensorControllerAPI.cpp`)
- `StartSensorController()` - Starts the TCP server and sensor simulation
- `StopSensorController()` - Stops the server and cleans up resources  
- `IsRunning()` - Returns current server status

### C# Frontend (`Form1.cs`)
- **Start Button** (Green) - Calls the C++ StartSensorController function
- **Stop Button** (Red) - Calls the C++ StopSensorController function
- **Status Label** - Shows current controller state (Running/Stopped/DLL Not Found)
- **Error Handling** - Shows message boxes for DLL errors

### Integration (`SensorControllerInterface.cs`)
- P/Invoke declarations for calling C++ DLL functions from C#
- Proper calling convention and error handling

## Usage

1. Run the C# application
2. Click the **Start** button to begin the sensor controller backend
3. The TCP server will start on port 8080 and begin broadcasting sensor data
4. Click the **Stop** button to shut down the backend
5. The status label shows the current state of the controller

## Notes

- The DLL must be in the same directory as the C# executable or in the system PATH
- If the DLL is not found, the buttons will be disabled and status shows "DLL Not Found"
- The C++ backend runs the TCP server in a separate thread to avoid blocking the UI
- Both start and stop operations include proper error handling and user feedback