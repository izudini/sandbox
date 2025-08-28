#include <iostream>
#include <iomanip>
#include <cmath>
#include "position.h"

#ifdef PYTHON_ENABLED
#include "python_interface.cpp"
#endif

// Fallback C++ distance calculation using Haversine formula
class CppDistanceCalculator {
private:
    static constexpr double EARTH_RADIUS = 6371000.0; // meters
    
    static double toRadians(double degrees) {
        return degrees * M_PI / 180.0;
    }
    
public:
    double calculateDistance(const Position& pos1, const Position& pos2) {
        double lat1 = toRadians(pos1.latitude);
        double lon1 = toRadians(pos1.longitude);
        double lat2 = toRadians(pos2.latitude);
        double lon2 = toRadians(pos2.longitude);
        
        double dlat = lat2 - lat1;
        double dlon = lon2 - lon1;
        
        double a = sin(dlat/2) * sin(dlat/2) + 
                  cos(lat1) * cos(lat2) * sin(dlon/2) * sin(dlon/2);
        double c = 2 * atan2(sqrt(a), sqrt(1-a));
        
        double distance = EARTH_RADIUS * c;
        
        // Add altitude difference using 3D distance
        double altitude_diff = pos2.altitude - pos1.altitude;
        return sqrt(distance * distance + altitude_diff * altitude_diff);
    }
};

int main() {
    try {
        // Create two position structs
        Position pos1(40.7128, -74.0060, 10.0);  // New York City
        Position pos2(34.0522, -118.2437, 100.0); // Los Angeles
        
        std::cout << "Position 1: " << std::fixed << std::setprecision(4) 
                  << "(" << pos1.latitude << ", " << pos1.longitude 
                  << ", " << pos1.altitude << ")" << std::endl;
                  
        std::cout << "Position 2: " << std::fixed << std::setprecision(4)
                  << "(" << pos2.latitude << ", " << pos2.longitude 
                  << ", " << pos2.altitude << ")" << std::endl;
        
        // Calculate distance
        double distance;
#ifdef PYTHON_ENABLED
        std::cout << "\nUsing Python distance calculation..." << std::endl;
        PythonDistanceCalculator python_calculator;
        distance = python_calculator.calculateDistance(pos1, pos2);
#else
        std::cout << "\nUsing C++ distance calculation..." << std::endl;
        CppDistanceCalculator cpp_calculator;
        distance = cpp_calculator.calculateDistance(pos1, pos2);
#endif
        
        std::cout << "Distance between positions: " << std::fixed << std::setprecision(2)
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
        
#ifdef PYTHON_ENABLED
        double close_distance = python_calculator.calculateDistance(pos3, pos4);
#else
        double close_distance = cpp_calculator.calculateDistance(pos3, pos4);
#endif
        
        std::cout << "Distance: " << std::fixed << std::setprecision(2)
                  << close_distance << " meters" << std::endl;
                  
    } catch (const std::exception& e) {
        std::cerr << "Error: " << e.what() << std::endl;
        return 1;
    }
    
    return 0;
}