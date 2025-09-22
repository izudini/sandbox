#define _USE_MATH_DEFINES
#include "simulator.h"
#include <iostream>
#include <chrono>
#include <thread>
#include <cmath>
#include <iomanip>

#ifndef M_PI
#define M_PI 3.14159265358979323846
#endif

namespace navsim {

Simulator::Simulator() : simulationFrequency_hz(60), speedMph(0), startPosition(), destinationPosition(), currentPosition(), simulationRunning(false) {
    // Initialize Python interface
    if (!pythonInterface.initialize()) {
        std::cerr << "Warning: Failed to initialize Python interface" << std::endl;
    }
}

Simulator::~Simulator() {
    simulationRunning = false;
    if (simulationThread.joinable()) {
        simulationThread.join();
    }
}

void Simulator::start(const Position& start, const Position& destination, int speed_mph) {
    startPosition = start;
    destinationPosition = destination;
    currentPosition = start;  // Start at the starting position
    speedMph = speed_mph;
    simulationRunning = true;
    simulationThread = std::thread(&Simulator::runSimulation, this);
}

void Simulator::runSimulation() {
    auto interval = std::chrono::microseconds(1000000 / simulationFrequency_hz);
    
    // Calculate total distance and flight time
    double totalDistance = calculateDistance(startPosition, destinationPosition);
    double speedMetersPerSecond = speedMph * 0.44704; // Convert mph to m/s
    double totalFlightTime = totalDistance / speedMetersPerSecond; // seconds
    
    std::cout << "Flight started!" << std::endl;
    std::cout << "Total distance: " << totalDistance << " meters" << std::endl;
    std::cout << "Speed: " << speedMph << " mph (" << speedMetersPerSecond << " m/s)" << std::endl;
    std::cout << "Estimated flight time: " << totalFlightTime << " seconds" << std::endl;
    std::cout << std::endl;
    
    auto startTime = std::chrono::high_resolution_clock::now();
    
    while (simulationRunning) {
        auto currentTime = std::chrono::high_resolution_clock::now();
        auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(currentTime - startTime).count() / 1000.0;
        
        // Calculate progress (0.0 to 1.0)
        double progress = elapsed / totalFlightTime;
        
        if (progress >= 1.0) {
            // Flight completed
            currentPosition = destinationPosition;
            std::cout << "Flight completed! Arrived at destination." << std::endl;
            std::cout << "Final position - Lat: " << currentPosition.latitude() 
                      << ", Lon: " << currentPosition.longitude()
                      << ", Alt: " << currentPosition.altitude() << "m"
                      << ", Heading: " << currentPosition.heading() << "°" << std::endl;
            simulationRunning = false;
            break;
        }
        
        // Calculate current position
        currentPosition = calculateIntermediatePosition(startPosition, destinationPosition, progress);
        
        // Call Python NavListener with current position
        pythonInterface.callNavListener(currentPosition);
        
        // Print current position
        std::cout << "Time: " << std::fixed << std::setprecision(1) << elapsed << "s"
                  << " | Progress: " << std::setprecision(1) << (progress * 100.0) << "%"
                  << " | Lat: " << std::setprecision(6) << currentPosition.latitude()
                  << ", Lon: " << currentPosition.longitude()
                  << ", Alt: " << std::setprecision(1) << currentPosition.altitude() << "m"
                  << ", Heading: " << currentPosition.heading() << "°" << std::endl;
        
        // Sleep for the interval
        std::this_thread::sleep_for(interval);
    }
}

double Simulator::calculateDistance(const Position& pos1, const Position& pos2) const {
    // Haversine formula for calculating distance between two lat/lon points
    const double R = 6371000; // Earth's radius in meters
    
    double lat1Rad = pos1.latitude() * M_PI / 180.0;
    double lat2Rad = pos2.latitude() * M_PI / 180.0;
    double deltaLatRad = (pos2.latitude() - pos1.latitude()) * M_PI / 180.0;
    double deltaLonRad = (pos2.longitude() - pos1.longitude()) * M_PI / 180.0;
    
    double a = sin(deltaLatRad / 2) * sin(deltaLatRad / 2) +
               cos(lat1Rad) * cos(lat2Rad) *
               sin(deltaLonRad / 2) * sin(deltaLonRad / 2);
    double c = 2 * atan2(sqrt(a), sqrt(1 - a));
    
    double horizontalDistance = R * c;
    
    // Add altitude difference
    double altitudeDifference = pos2.altitude() - pos1.altitude();
    
    return sqrt(horizontalDistance * horizontalDistance + altitudeDifference * altitudeDifference);
}

Position Simulator::calculateIntermediatePosition(const Position& start, const Position& end, double progress) const {
    Position intermediate;
    
    // Linear interpolation for all position components
    intermediate.set_entity_id(start.entity_id());
    intermediate.set_latitude(start.latitude() + (end.latitude() - start.latitude()) * progress);
    intermediate.set_longitude(start.longitude() + (end.longitude() - start.longitude()) * progress);
    intermediate.set_altitude(start.altitude() + (end.altitude() - start.altitude()) * progress);
    intermediate.set_heading(start.heading() + (end.heading() - start.heading()) * progress);
    
    return intermediate;
}

} // namespace navsim