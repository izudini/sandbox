#include <iostream>
#include <iomanip>
#include "position.h"
#include "python_interface.cpp"

int main() {
    try {
        // Create the Python interface
        PythonDistanceCalculator calculator;
        
        // Create two position structs
        Position pos1(40.7128, -74.0060, 10.0);  // New York City
        Position pos2(34.0522, -118.2437, 100.0); // Los Angeles
        
        std::cout << "Position 1: " << std::fixed << std::setprecision(4) 
                  << "(" << pos1.latitude << ", " << pos1.longitude 
                  << ", " << pos1.altitude << ")" << std::endl;
                  
        std::cout << "Position 2: " << std::fixed << std::setprecision(4)
                  << "(" << pos2.latitude << ", " << pos2.longitude 
                  << ", " << pos2.altitude << ")" << std::endl;
        
        // Calculate distance using Python function
        double distance = calculator.calculateDistance(pos1, pos2);
        
        std::cout << "\nDistance between positions: " << std::fixed << std::setprecision(2)
                  << distance << " meters (" << distance / 1000.0 << " km)" << std::endl;
                  
        // Test with closer positions
        Position pos3(40.7589, -73.9851, 50.0);  // Times Square
        Position pos4(40.7614, -73.9776, 75.0);  // Central Park
        
        std::cout << "\nTesting with closer positions:" << std::endl;
        std::cout << "Position 3: " << std::fixed << std::setprecision(4)
                  << "(" << pos3.latitude << ", " << pos3.longitude 
                  << ", " << pos3.altitude << ")" << std::endl;
                  
        std::cout << "Position 4: " << std::fixed << std::setprecision(4)
                  << "(" << pos4.latitude << ", " << pos4.longitude 
                  << ", " << pos4.altitude << ")" << std::endl;
        
        double close_distance = calculator.calculateDistance(pos3, pos4);
        
        std::cout << "Distance: " << std::fixed << std::setprecision(2)
                  << close_distance << " meters" << std::endl;
                  
    } catch (const std::exception& e) {
        std::cerr << "Error: " << e.what() << std::endl;
        return 1;
    }
    
    return 0;
}