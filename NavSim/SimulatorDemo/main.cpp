#include <iostream>
#include <thread>
#include <chrono>
#include "../Simulator/simulator.h"
#include "../Simulator/position.pb.h"

int main() {
    std::cout << "NavSim Simulator Demo" << std::endl;
    std::cout << "=====================" << std::endl;
    
    // Create simulator instance
    navsim::Simulator simulator;
    
    // Create start and destination positions
    navsim::Position startPos;
    startPos.set_entity_id(1);
    startPos.set_latitude(40.7589);  // New York City
    startPos.set_longitude(-73.9851);
    startPos.set_altitude(100.0);
    startPos.set_heading(90.0);  // East
    
    navsim::Position destPos;
    destPos.set_entity_id(1);
    destPos.set_latitude(40.7614);  // Central Park
    destPos.set_longitude(-73.9776);
    destPos.set_altitude(120.0);
    destPos.set_heading(45.0);  // Northeast
    
    // Display start and destination
    std::cout << std::endl << "Starting Position:" << std::endl;
    std::cout << "  Entity ID: " << startPos.entity_id() << std::endl;
    std::cout << "  Latitude: " << startPos.latitude() << std::endl;
    std::cout << "  Longitude: " << startPos.longitude() << std::endl;
    std::cout << "  Altitude: " << startPos.altitude() << " meters" << std::endl;
    std::cout << "  Heading: " << startPos.heading() << " degrees" << std::endl;
    
    std::cout << std::endl << "Destination Position:" << std::endl;
    std::cout << "  Entity ID: " << destPos.entity_id() << std::endl;
    std::cout << "  Latitude: " << destPos.latitude() << std::endl;
    std::cout << "  Longitude: " << destPos.longitude() << std::endl;
    std::cout << "  Altitude: " << destPos.altitude() << " meters" << std::endl;
    std::cout << "  Heading: " << destPos.heading() << " degrees" << std::endl;
    
    // Start simulation with 25 mph speed
    int speed_mph = 25;
    std::cout << std::endl << "Starting simulation at " << speed_mph << " mph..." << std::endl;
    std::cout << "Press Ctrl+C to stop the simulation." << std::endl;
    std::cout << std::endl;
    
    // Start the simulation
    simulator.start(startPos, destPos, speed_mph);
    
    // Let the simulation run for 10 seconds as a demo
    std::this_thread::sleep_for(std::chrono::seconds(10));
    
    std::cout << std::endl << "Demo completed!" << std::endl;
    std::cout << "Note: In a real application, you would implement proper shutdown mechanisms." << std::endl;
    
    return 0;
}