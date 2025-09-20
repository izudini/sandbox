#include "Position.h"

Position::Position() : m_entityID(0), m_latitude(0.0), m_longitude(0.0), m_altitude(0.0), m_heading(0.0)
{
}

Position::Position(int entityID, double latitude, double longitude, double altitude, double heading)
    : m_entityID(entityID), m_latitude(latitude), m_longitude(longitude), m_altitude(altitude), m_heading(heading)
{
}

Position::Position(double latitude, double longitude, double altitude, double heading)
    : m_entityID(0), m_latitude(latitude), m_longitude(longitude), m_altitude(altitude), m_heading(heading)
{
}

Position::~Position()
{
}

double Position::GetLatitude() const
{
    return m_latitude;
}

void Position::SetLatitude(double latitude)
{
    m_latitude = latitude;
}

double Position::GetLongitude() const
{
    return m_longitude;
}

void Position::SetLongitude(double longitude)
{
    m_longitude = longitude;
}

double Position::GetAltitude() const
{
    return m_altitude;
}

void Position::SetAltitude(double altitude)
{
    m_altitude = altitude;
}

double Position::GetHeading() const
{
    return m_heading;
}

void Position::SetHeading(double heading)
{
    m_heading = heading;
}

int Position::GetEntityID() const
{
    return m_entityID;
}

void Position::SetEntityID(int entityID)
{
    m_entityID = entityID;
}