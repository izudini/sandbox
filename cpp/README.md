# Shared Pointer Demonstration

A comprehensive C++ project demonstrating `std::shared_ptr` usage and best practices for passing smart pointers into functions.

## Overview

This project showcases:
- Basic `shared_ptr` creation and reference counting
- Different ways to pass `shared_ptr` to functions
- When to use each passing method
- Ownership semantics and shared ownership
- Memory management and automatic cleanup

## Project Structure

```
.
├── CMakeLists.txt    # Build configuration
├── Resource.h        # Header for demo class
├── Resource.cpp      # Implementation of demo class
├── main.cpp          # Main demonstration program
└── README.md         # This file
```

## Examples Covered

### 1. **Passing by Value**
```cpp
void passByValue(std::shared_ptr<Resource> ptr);
```
- Increments reference count during function call
- Use when function needs to share ownership

### 2. **Passing by Const Reference**
```cpp
void passByConstRef(const std::shared_ptr<Resource>& ptr);
```
- More efficient - no reference count increment
- Use when function only needs to access the object

### 3. **Passing by Reference**
```cpp
void passByRef(std::shared_ptr<Resource>& ptr);
```
- Allows modifying the pointer itself
- Use when function needs to reset or reassign the pointer

### 4. **Returning from Function**
```cpp
std::shared_ptr<Resource> createResource();
```
- Factory pattern for creating managed objects

### 5. **Shared Ownership**
- Multiple owners can hold references to the same object
- Object is destroyed only when last owner releases it

### 6. **Passing Raw Pointer**
```cpp
void processResource(Resource* res);
```
- Use when function doesn't need ownership
- Most efficient for simple operations

## Building the Project

### Using CMake (Linux/WSL/Mac)

```bash
# Create build directory
mkdir build
cd build

# Configure and build
cmake ..
make

# Run the demo
./shared_ptr_demo
```

### Using CMake (Windows with Visual Studio)

```bash
# Create build directory
mkdir build
cd build

# Configure
cmake ..

# Build
cmake --build .

# Run the demo
.\Debug\shared_ptr_demo.exe
```

### Manual Compilation (g++)

```bash
g++ -std=c++17 -Wall main.cpp Resource.cpp -o shared_ptr_demo
./shared_ptr_demo
```

## Key Takeaways

1. **Use `std::make_shared<T>()`** to create shared pointers - it's more efficient
2. **Pass by const reference** when function only needs to observe
3. **Pass by value** when function needs to share ownership
4. **Use raw pointers** (`ptr.get()`) when function doesn't need ownership
5. **Reference counting is automatic** - no manual memory management needed
6. **Object is destroyed** when last `shared_ptr` goes out of scope

## Best Practices

- Prefer `std::make_shared<T>()` over `std::shared_ptr<T>(new T())`
- Pass by const reference for observer functions
- Use raw pointers for non-owning parameters
- Avoid circular references (use `std::weak_ptr` if needed)
- Check for nullptr before dereferencing

## Sample Output

The program produces detailed output showing:
- Constructor/destructor calls
- Reference count changes
- Object lifecycle
- When objects are actually destroyed

## Requirements

- C++17 or later
- CMake 3.10 or later (if using CMake)
- Any modern C++ compiler (g++, clang++, MSVC)
