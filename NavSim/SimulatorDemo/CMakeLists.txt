cmake_minimum_required(VERSION 3.15)
project(SimulatorDemo)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Add the simulator library directory
add_subdirectory(../Simulator NavSimulator)

# Create the executable
add_executable(SimulatorDemo
    main.cpp
)

# Link against the simulator library
target_link_libraries(SimulatorDemo 
    NavSimulator
)

# Include directories
target_include_directories(SimulatorDemo PRIVATE
    ../Simulator
)

# Set compiler flags
if(MSVC)
    target_compile_options(SimulatorDemo PRIVATE /W4)
else()
    target_compile_options(SimulatorDemo PRIVATE -Wall -Wextra -Wpedantic)
endif()