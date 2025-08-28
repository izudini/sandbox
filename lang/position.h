#ifndef POSITION_H
#define POSITION_H

struct Position {
    double latitude;
    double longitude;
    double altitude;
    
    Position(double lat, double lon, double alt) 
        : latitude(lat), longitude(lon), altitude(alt) {}
};

#endif // POSITION_H