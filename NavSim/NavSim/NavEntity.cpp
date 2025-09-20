#include "NavEntity.h"
#include <chrono>
#include <cmath>

NavEntity::NavEntity() : m_isInitialized(false), currentPosition(), m_destination(), m_speed_mph(0), simulationRate_Hz(60), m_isSimulating(false)
{
}

NavEntity::~NavEntity()
{
    if (m_isInitialized)
    {
        Shutdown();
    }
    if (m_isSimulating.load())
    {
        m_isSimulating = false;
        if (m_simulationThread.joinable())
        {
            m_simulationThread.join();
        }
    }
}

void NavEntity::Initialize()
{
    if (!m_isInitialized)
    {
        m_isInitialized = true;
    }
}

void NavEntity::Update()
{
    if (m_isInitialized)
    {
    }
}

void NavEntity::Shutdown()
{
    if (m_isInitialized)
    {
        m_isInitialized = false;
    }
}

void NavEntity::simulateFlight(const Position& start, const Position& destination, int speed_mph)
{
    if (m_isSimulating.load())
    {
        m_isSimulating = false;
        if (m_simulationThread.joinable())
        {
            m_simulationThread.join();
        }
    }
    
    currentPosition = start;
    m_destination = destination;
    m_speed_mph = speed_mph;
    m_isSimulating = true;
    
    m_simulationThread = std::thread(&NavEntity::simulatingFlight, this);
}

void NavEntity::simulatingFlight()
{
    while (m_isSimulating.load())
    {
        double deltaTime = 1.0 / simulationRate_Hz;
        
        double latDiff = m_destination.GetLatitude() - currentPosition.GetLatitude();
        double lonDiff = m_destination.GetLongitude() - currentPosition.GetLongitude();
        double altDiff = m_destination.GetAltitude() - currentPosition.GetAltitude();
        
        double distance = std::sqrt(latDiff * latDiff + lonDiff * lonDiff);
        
        if (distance < 0.0001)
        {
            currentPosition = m_destination;
            m_isSimulating = false;
            break;
        }
        
        double speed_deg_per_sec = (m_speed_mph / 3600.0) / 69.0;
        double moveDistance = speed_deg_per_sec * deltaTime;
        
        if (moveDistance >= distance)
        {
            currentPosition = m_destination;
            m_isSimulating = false;
            break;
        }
        
        double ratio = moveDistance / distance;
        
        double newLat = currentPosition.GetLatitude() + (latDiff * ratio);
        double newLon = currentPosition.GetLongitude() + (lonDiff * ratio);
        double newAlt = currentPosition.GetAltitude() + (altDiff * ratio);
        
        currentPosition.SetLatitude(newLat);
        currentPosition.SetLongitude(newLon);
        currentPosition.SetAltitude(newAlt);
        
        std::this_thread::sleep_for(std::chrono::milliseconds(1000 / simulationRate_Hz));
    }
}

void NavEntity::stopFlight()
{
    if (m_isSimulating.load())
    {
        m_isSimulating = false;
        if (m_simulationThread.joinable())
        {
            m_simulationThread.join();
        }
    }
}