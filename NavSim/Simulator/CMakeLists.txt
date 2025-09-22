cmake_minimum_required(VERSION 3.15)
project(NavSimulator)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Find Python components
find_package(Python3 COMPONENTS Interpreter Development REQUIRED)

# Create shared library (DLL on Windows)
add_library(NavSimulator SHARED
    position.pb.cpp
    simulator.cpp
    simulator_exports.cpp
    python_interface.cpp
)

# Set the output name for the DLL
set_target_properties(NavSimulator PROPERTIES
    OUTPUT_NAME "NavSimulator"
    PREFIX ""
)

# Include directories
target_include_directories(NavSimulator PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${Python3_INCLUDE_DIRS}
)

# Link Python libraries
target_link_libraries(NavSimulator PRIVATE
    ${Python3_LIBRARIES}
)

# Export symbols for DLL
if(WIN32)
    target_compile_definitions(NavSimulator PRIVATE NAVSIM_EXPORTS)
    target_compile_definitions(NavSimulator INTERFACE NAVSIM_IMPORTS)
endif()

# Set compiler flags
if(MSVC)
    target_compile_options(NavSimulator PRIVATE /W4)
else()
    target_compile_options(NavSimulator PRIVATE -Wall -Wextra -Wpedantic)
endif()