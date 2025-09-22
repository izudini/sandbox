CXX = g++
CXXFLAGS = -std=c++17 -Wall -Wextra -fPIC
LDFLAGS = -shared
TARGET = libNavSimulator.so
OBJECTS = position.pb.o simulator.o python_interface.o

# Default target
all: $(TARGET)

# Build the shared library
$(TARGET): $(OBJECTS)
	$(CXX) $(LDFLAGS) -o $@ $^

# Object file rules
%.o: %.cpp
	$(CXX) $(CXXFLAGS) -c $< -o $@

# Clean build artifacts
clean:
	rm -f $(OBJECTS) $(TARGET)

# Phony targets
.PHONY: all clean

# Dependencies
position.pb.o: position.pb.cpp position.pb.h
simulator.o: simulator.cpp simulator.h position.pb.h python_interface.h
python_interface.o: python_interface.cpp python_interface.h position.pb.h