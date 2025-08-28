#ifndef SENSOR_CONTROLLER_API_H
#define SENSOR_CONTROLLER_API_H

#ifdef _WIN32
    #ifdef SENSOR_CONTROLLER_EXPORTS
        #define SENSOR_API __declspec(dllexport)
    #else
        #define SENSOR_API __declspec(dllimport)
    #endif
#else
    #define SENSOR_API
#endif

extern "C" {
    SENSOR_API bool StartSensorController();
    SENSOR_API bool StopSensorController();
    SENSOR_API bool IsRunning();
}

#endif // SENSOR_CONTROLLER_API_H