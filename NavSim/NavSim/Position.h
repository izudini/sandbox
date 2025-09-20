#pragma once

#ifdef NAVSIM_EXPORTS
#define NAVSIM_API __declspec(dllexport)
#else
#define NAVSIM_API __declspec(dllimport)
#endif

class NAVSIM_API Position
{
public:
    Position();
    Position(int entityID, double latitude, double longitude, double altitude, double heading);
    Position(double latitude, double longitude, double altitude, double heading);
    ~Position();

    double GetLatitude() const;
    void SetLatitude(double latitude);

    double GetLongitude() const;
    void SetLongitude(double longitude);

    double GetAltitude() const;
    void SetAltitude(double altitude);

    double GetHeading() const;
    void SetHeading(double heading);

    int GetEntityID() const;
    void SetEntityID(int entityID);

private:
    int m_entityID;
    double m_latitude;
    double m_longitude;
    double m_altitude;
    double m_heading;
};