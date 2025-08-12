cmake_minimum_required(VERSION 3.10)

project(SensorController)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_executable(SensorController main.cpp Sensor.cpp UDPSocketListener.cpp SensorState.h Sensor.h UDPSocketListener.h)

# Link threading library
find_package(Threads REQUIRED)
target_link_libraries(SensorController Threads::Threads)

# Link socket libraries on Windows
if(WIN32)
    target_link_libraries(SensorController ws2_32)
endif()