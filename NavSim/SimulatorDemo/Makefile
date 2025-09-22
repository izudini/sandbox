CXX = g++
CXXFLAGS = -std=c++17 -Wall -Wextra -I../Simulator
SIMULATOR_LIB = ../Simulator/libNavSimulator.so
TARGET = SimulatorDemo
SOURCE = main.cpp

# Default target
all: $(TARGET)

# Build the executable
$(TARGET): $(SOURCE) $(SIMULATOR_LIB)
	$(CXX) $(CXXFLAGS) -o $@ $< -L../Simulator -lNavSimulator -Wl,-rpath,../Simulator

# Ensure the simulator library is built
$(SIMULATOR_LIB):
	$(MAKE) -C ../Simulator

# Clean build artifacts
clean:
	rm -f $(TARGET)

# Clean everything including simulator
clean-all: clean
	$(MAKE) -C ../Simulator clean

# Phony targets
.PHONY: all clean clean-all