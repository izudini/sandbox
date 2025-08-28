cmake_minimum_required(VERSION 3.12)
project(PositionDistance)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Create executable first to generate project files
add_executable(position_distance 
    main.cpp 
)

# Include directories
target_include_directories(position_distance PRIVATE 
    ${CMAKE_CURRENT_SOURCE_DIR}
)

# Add math library for Unix-like systems
if(UNIX)
    target_link_libraries(position_distance PRIVATE m)
endif()

# Try to find Python3 and pybind11 - make them optional for project generation
find_package(Python3 COMPONENTS Interpreter Development QUIET)
find_package(pybind11 CONFIG QUIET)
if(NOT pybind11_FOUND)
    find_package(pybind11 QUIET)
endif()

# Only add Python/pybind11 dependencies if both are found
if(Python3_FOUND AND pybind11_FOUND)
    message(STATUS "Found Python3 and pybind11 - enabling Python integration")
    
    target_include_directories(position_distance PRIVATE 
        ${Python3_INCLUDE_DIRS}
    )
    
    target_link_libraries(position_distance PRIVATE
        pybind11::embed
        ${Python3_LIBRARIES}
    )
    
    target_compile_definitions(position_distance PRIVATE PYTHON_ENABLED)
    
    # Windows-specific settings for Python
    if(WIN32 AND Python3_RUNTIME_LIBRARY_DIRS)
        # Copy Python DLLs to output directory
        add_custom_command(TARGET position_distance POST_BUILD
            COMMAND ${CMAKE_COMMAND} -E copy_if_different
            "${Python3_RUNTIME_LIBRARY_DIRS}/*.dll"
            "$<TARGET_FILE_DIR:position_distance>"
            COMMENT "Copying Python DLLs"
            COMMAND_EXPAND_LISTS
        )
    endif()
else()
    if(NOT Python3_FOUND)
        message(STATUS "Python3 not found - using C++ fallback")
    endif()
    if(NOT pybind11_FOUND)
        message(STATUS "pybind11 not found - using C++ fallback")
    endif()
    target_compile_definitions(position_distance PRIVATE PYTHON_DISABLED)
endif()

# Windows-specific Visual Studio settings
if(WIN32)
    # Set startup project for Visual Studio
    set_property(DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR} PROPERTY VS_STARTUP_PROJECT position_distance)
    
    # Add _USE_MATH_DEFINES for M_PI on Windows
    target_compile_definitions(position_distance PRIVATE _USE_MATH_DEFINES)
endif()

# Set working directory for debugging in Visual Studio
set_target_properties(position_distance PROPERTIES
    VS_DEBUGGER_WORKING_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}"
)