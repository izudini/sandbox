# NavSim Build Instructions

## Prerequisites

1. **Visual Studio 2022** with C++ development tools
2. **Python 3.11** (optional, for enhanced visualizer)
3. **Windows 10/11**

## Build Steps

### Option 1: Visual Studio IDE

1. **Open the solution:**
   ```
   NavSim\NavSim.sln
   ```

2. **Set build configuration:**
   - Configuration: Debug
   - Platform: x64

3. **Set startup project:**
   - Right-click on "NavSimExample" in Solution Explorer
   - Select "Set as Startup Project"

4. **Build and run:**
   - Press F5 or click "Start Debugging"
   - Visual Studio will build NavSim.dll first, then NavSimExample.exe

### Option 2: Command Line (Developer Command Prompt)

1. Open "Developer Command Prompt for VS 2022"
2. Navigate to the NavSim directory:
   ```cmd
   cd "C:\Users\izudin\Desktop\sandbox\NavSim"
   ```
3. Build the solution:
   ```cmd
   msbuild NavSim\NavSim.sln /p:Configuration=Debug /p:Platform=x64
   ```
4. Run the example:
   ```cmd
   NavSimExample\x64\Debug\NavSimExample.exe
   ```

## Project Structure

- **NavSim** - Core DLL library with flight simulation and Python integration
- **NavSimExample** - Console application demonstrating the functionality
- **NavVisualiser** - Python visualization module

## Troubleshooting

### Build Errors

1. **Missing Python dependencies:**
   - Install Python 3.11 if you want full pybind11 integration
   - Current version works without Python for basic functionality

2. **Platform mismatch:**
   - Ensure you're building for x64 platform
   - Both projects are configured for x64 only

3. **Missing dependencies:**
   - Make sure Visual Studio C++ development tools are installed
   - Windows SDK should be installed with Visual Studio

### Runtime Issues

1. **DLL not found:**
   - Make sure NavSim.dll is in the same directory as NavSimExample.exe
   - Build process should copy it automatically

2. **Python visualizer not starting:**
   - Current implementation uses basic file output instead of live visualization
   - Check console output for status messages

## Example Output

When running successfully, you should see:
```
NavSim with Python Visualizer Example
Python visualizer initialized successfully
Starting flight simulation with visualization...
Visualizing current position...
Flight simulation completed.
```

## Next Steps

- To enable full pybind11 integration, install pybind11: `pip install pybind11`
- Modify the project to re-enable Python includes and libraries
- Implement real-time visualization updates