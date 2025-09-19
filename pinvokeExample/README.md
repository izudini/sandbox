# P/Invoke Example Project

This project demonstrates how to create a C++ DLL with proper `__declspec(dllexport)` and `__declspec(dllimport)` declarations for use with P/Invoke from .NET applications.

## Project Structure

```
pinvokeExample/
├── MathLibrary/               # C++ DLL Project
│   ├── MathLibrary.h         # Header with dllexport/dllimport declarations
│   ├── MathLibrary.cpp       # Implementation
│   ├── MathLibrary.vcxproj   # Visual Studio project file
│   └── MathLibrary.vcxproj.filters
├── ClientApp/                # C++ Client Application
│   ├── main.cpp              # Example usage of the DLL
│   ├── ClientApp.vcxproj     # Visual Studio project file
│   └── ClientApp.vcxproj.filters
└── PInvokeExample.sln        # Visual Studio solution file
```

## Key Features

### DLL Export/Import Configuration
- Uses `MATHLIBRARY_EXPORTS` preprocessor definition to control export/import
- When building the DLL: `__declspec(dllexport)` exports functions
- When using the DLL: `__declspec(dllimport)` imports functions

### Exported Functions
The DLL exports both C-style functions and a C++ class:

#### C-Style Functions (extern "C")
- `int Add(int a, int b)`
- `int Subtract(int a, int b)`
- `double Multiply(double a, double b)`
- `double Divide(double a, double b)`
- `int GetVersion()`
- `const char* GetLibraryName()`

#### C++ Class
- `Calculator` class with mathematical operations
- Maintains state (last result)
- Demonstrates C++ class export

## Building the Project

1. Open `PInvokeExample.sln` in Visual Studio
2. Build the solution (Build → Build Solution)
3. The MathLibrary.dll will be created in the output directory
4. The ClientApp.exe will demonstrate usage of the DLL

## P/Invoke Usage

For .NET applications, you can use P/Invoke to call the exported functions:

```csharp
[DllImport("MathLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
public static extern int Add(int a, int b);

[DllImport("MathLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
public static extern double Multiply(double a, double b);

[DllImport("MathLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
public static extern IntPtr GetLibraryName();
```

## Notes

- The project is configured for Visual Studio 2022 (v143 toolset)
- Supports both x86 and x64 platforms
- Debug and Release configurations available
- The DLL uses Unicode character set
- Client application automatically links to the import library