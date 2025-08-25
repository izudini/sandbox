cmake_minimum_required(VERSION 3.10)

project(SensorController)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Build as shared library (DLL)
add_library(SensorController SHARED SensorControllerAPI.cpp Sensor.cpp UDPSocketListener.cpp TCPServer.cpp SensorState.h Sensor.h UDPSocketListener.h TCPServer.h SensorControllerAPI.h)

# Define the export macro
target_compile_definitions(SensorController PRIVATE SENSOR_CONTROLLER_EXPORTS)

# Link threading library
find_package(Threads REQUIRED)
target_link_libraries(SensorController Threads::Threads)

# Link socket libraries on Windows
if(WIN32)
    target_link_libraries(SensorController ws2_32)
endif()

# Optional: Build the original executable as well
add_executable(SensorControllerApp main.cpp Sensor.cpp UDPSocketListener.cpp TCPServer.cpp SensorState.h Sensor.h UDPSocketListener.h TCPServer.h)
target_link_libraries(SensorControllerApp Threads::Threads)
if(WIN32)
    target_link_libraries(SensorControllerApp ws2_32)
endif()