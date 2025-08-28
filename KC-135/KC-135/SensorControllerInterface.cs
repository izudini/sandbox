using System;
using System.Runtime.InteropServices;

namespace KC_135
{
    public static class SensorControllerInterface
    {
        // Import functions from the C++ DLL
        [DllImport("SensorController.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StartSensorController();

        [DllImport("SensorController.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StopSensorController();

        [DllImport("SensorController.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsRunning();
    }
}