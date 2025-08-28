cmake_minimum_required(VERSION 3.12)
project(PositionDistance)

set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Find Python
find_package(Python3 COMPONENTS Interpreter Development REQUIRED)

# Find pybind11
find_package(pybind11 REQUIRED)

# Include directories
include_directories(${Python3_INCLUDE_DIRS})

# Create executable
add_executable(position_distance 
    main.cpp 
    python_interface.cpp
)

# Link libraries
target_link_libraries(position_distance 
    pybind11::embed
    ${Python3_LIBRARIES}
)

# Set Python path environment variable for running
set_target_properties(position_distance PROPERTIES
    ENVIRONMENT "PYTHONPATH=${CMAKE_CURRENT_SOURCE_DIR}"
)