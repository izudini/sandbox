#include "position.pb.h"
#include <sstream>
#include <cstring>

namespace navsim {

Position::Position() 
    : entity_id_(0)
    , latitude_(0.0)
    , longitude_(0.0)
    , altitude_(1000.0)  // Default from proto
    , heading_(0.0)      // Default from proto
{
}

Position::~Position() {
}

Position::Position(const Position& other) 
    : entity_id_(other.entity_id_)
    , latitude_(other.latitude_)
    , longitude_(other.longitude_)
    , altitude_(other.altitude_)
    , heading_(other.heading_)
{
}

Position& Position::operator=(const Position& other) {
    if (this != &other) {
        entity_id_ = other.entity_id_;
        latitude_ = other.latitude_;
        longitude_ = other.longitude_;
        altitude_ = other.altitude_;
        heading_ = other.heading_;
    }
    return *this;
}

void Position::set_entity_id(int32_t value) {
    entity_id_ = value;
}

int32_t Position::entity_id() const {
    return entity_id_;
}

void Position::set_latitude(double value) {
    latitude_ = value;
}

double Position::latitude() const {
    return latitude_;
}

void Position::set_longitude(double value) {
    longitude_ = value;
}

double Position::longitude() const {
    return longitude_;
}

void Position::set_altitude(double value) {
    altitude_ = value;
}

double Position::altitude() const {
    return altitude_;
}

void Position::set_heading(double value) {
    heading_ = value;
}

double Position::heading() const {
    return heading_;
}

std::string Position::SerializeAsString() const {
    std::ostringstream oss;
    oss << entity_id_ << "," << latitude_ << "," << longitude_ << "," << altitude_ << "," << heading_;
    return oss.str();
}

bool Position::ParseFromString(const std::string& data) {
    std::istringstream iss(data);
    std::string token;
    
    try {
        if (std::getline(iss, token, ',')) {
            entity_id_ = std::stoi(token);
        }
        if (std::getline(iss, token, ',')) {
            latitude_ = std::stod(token);
        }
        if (std::getline(iss, token, ',')) {
            longitude_ = std::stod(token);
        }
        if (std::getline(iss, token, ',')) {
            altitude_ = std::stod(token);
        }
        if (std::getline(iss, token, ',')) {
            heading_ = std::stod(token);
        }
        return true;
    } catch (...) {
        return false;
    }
}

void Position::Clear() {
    entity_id_ = 0;
    latitude_ = 0.0;
    longitude_ = 0.0;
    altitude_ = 1000.0;
    heading_ = 0.0;
}

} // namespace navsim