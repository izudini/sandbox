// Example usage of NavEntity with Python visualizer integration
#include "NavEntity.h"
#include "Position.h"
#include <iostream>
#include <thread>
#include <chrono>

int main()
{
    std::cout << "NavSim with Python Visualizer Example" << std::endl;
    
    // Create a navigation entity
    NavEntity entity;
    entity.Initialize();
    
    // Start the visualizer
    entity.StartWithVisualizer();
    
    // Create start and destination positions
    Position start(1, 37.7749, -122.4194, 1000.0, 45.0);  // San Francisco
    Position destination(1, 34.0522, -118.2437, 1500.0, 90.0);  // Los Angeles
    
    std::cout << "Starting flight simulation with visualization..." << std::endl;
    
    // Start the flight simulation with real-time visualization
    entity.simulateFlight(start, destination, 500);  // 500 mph
    
    // Wait for simulation to complete
    std::this_thread::sleep_for(std::chrono::seconds(30));
    
    // Stop the flight if still running
    entity.stopFlight();
    
    // Cleanup
    entity.Shutdown();
    
    std::cout << "Flight simulation completed." << std::endl;
    
    return 0;
}