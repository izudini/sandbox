#include "NavEntity.h"
#include "Position.h"
#include <iostream>
#include <thread>
#include <chrono>

int main()
{
    std::cout << "NavSim with Python Map Visualizer" << std::endl;
    std::cout << "===================================" << std::endl;
    
    // Create a navigation entity
    NavEntity entity;
    entity.Initialize();
    
    std::cout << "Initializing Python visualizer..." << std::endl;
    
    // Start the visualizer
    entity.StartWithVisualizer();
    
    // Create start and destination positions
    Position start(1, 37.7749, -122.4194, 1000.0, 45.0);  // San Francisco
    Position destination(1, 34.0522, -118.2437, 1500.0, 90.0);  // Los Angeles
    
    std::cout << std::endl;
    std::cout << "Flight Plan:" << std::endl;
    std::cout << "- Start: San Francisco (37.7749, -122.4194)" << std::endl;
    std::cout << "- Destination: Los Angeles (34.0522, -118.2437)" << std::endl;
    std::cout << "- Speed: 500 mph" << std::endl;
    std::cout << std::endl;
    
    std::cout << "Starting flight simulation with real-time visualization..." << std::endl;
    std::cout << "Watch the Python map window for real-time updates!" << std::endl;
    std::cout << std::endl;
    
    // Start the flight simulation with real-time visualization
    entity.simulateFlight(start, destination, 500);  // 500 mph
    
    // Let simulation run for a reasonable time
    std::cout << "Simulation running... (Press Ctrl+C to stop early)" << std::endl;
    
    // Wait for simulation to complete or timeout after 60 seconds
    for (int i = 0; i < 60 && true; ++i)
    {
        std::this_thread::sleep_for(std::chrono::seconds(1));
        std::cout << ".";
        if (i % 10 == 9) std::cout << " " << (i + 1) << "s" << std::endl;
    }
    
    std::cout << std::endl;
    
    // Stop the flight
    entity.stopFlight();
    
    std::cout << std::endl;
    std::cout << "Flight simulation completed!" << std::endl;
    std::cout << "Check 'flight_data.csv' for recorded flight path." << std::endl;
    
    // Cleanup
    entity.Shutdown();
    
    std::cout << std::endl;
    std::cout << "Press Enter to exit..." << std::endl;
    std::cin.get();
    
    return 0;
}