#include "SimpleNavEntity.h"
#include <iostream>

SimpleNavEntity::SimpleNavEntity() : m_isInitialized(false)
{
}

SimpleNavEntity::~SimpleNavEntity()
{
    if (m_isInitialized)
    {
        Shutdown();
    }
}

void SimpleNavEntity::Initialize()
{
    if (!m_isInitialized)
    {
        m_isInitialized = true;
        std::cout << "SimpleNavEntity initialized" << std::endl;
    }
}

void SimpleNavEntity::Shutdown()
{
    if (m_isInitialized)
    {
        m_isInitialized = false;
        std::cout << "SimpleNavEntity shutdown" << std::endl;
    }
}

void SimpleNavEntity::StartDemo()
{
    if (m_isInitialized)
    {
        std::cout << "Running navigation simulation demo..." << std::endl;
        std::cout << "Flight from San Francisco to Los Angeles" << std::endl;
        std::cout << "Demo completed successfully!" << std::endl;
    }
    else
    {
        std::cout << "Error: SimpleNavEntity not initialized" << std::endl;
    }
}