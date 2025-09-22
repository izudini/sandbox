#pragma once
#include <string>
#include <memory>

namespace navsim {

class Position {
public:
    Position();
    ~Position();
    
    // Copy constructor and assignment operator
    Position(const Position& other);
    Position& operator=(const Position& other);
    
    // Entity ID
    void set_entity_id(int32_t value);
    int32_t entity_id() const;
    
    // Latitude
    void set_latitude(double value);
    double latitude() const;
    
    // Longitude
    void set_longitude(double value);
    double longitude() const;
    
    // Altitude
    void set_altitude(double value);
    double altitude() const;
    
    // Heading
    void set_heading(double value);
    double heading() const;
    
    // Serialization
    std::string SerializeAsString() const;
    bool ParseFromString(const std::string& data);
    
    // Clear all fields
    void Clear();

private:
    int32_t entity_id_;
    double latitude_;
    double longitude_;
    double altitude_;
    double heading_;
};

} // namespace navsim