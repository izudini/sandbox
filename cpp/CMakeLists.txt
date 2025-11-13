cmake_minimum_required(VERSION 3.10)
project(SharedPointerDemo)

# Set C++ standard
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Add executable
add_executable(shared_ptr_demo
    main.cpp
    Resource.cpp
)

# Enable warnings
if(MSVC)
    target_compile_options(shared_ptr_demo PRIVATE /W4)
else()
    target_compile_options(shared_ptr_demo PRIVATE -Wall -Wextra -Wpedantic)
endif()
