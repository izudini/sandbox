#ifndef SENSOR_STATE_H
#define SENSOR_STATE_H

enum class SensorState {
    Off,
    Initializing,
    Operational,
    Declaring,
    Degraded
};

#endif // SENSOR_STATE_H