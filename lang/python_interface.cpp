#include <pybind11/pybind11.h>
#include <pybind11/embed.h>
#include "position.h"

namespace py = pybind11;

class PythonDistanceCalculator {
private:
    py::module distance_module;
    py::function calculate_distance_func;
    py::object Position_class;
    
public:
    PythonDistanceCalculator() {
        py::initialize_interpreter();
        
        // Import the distance_calculator module
        distance_module = py::module::import("distance_calculator");
        calculate_distance_func = distance_module.attr("calculate_distance");
        Position_class = distance_module.attr("Position");
    }
    
    ~PythonDistanceCalculator() {
        py::finalize_interpreter();
    }
    
    double calculateDistance(const Position& pos1, const Position& pos2) {
        // Create Python Position objects
        py::object py_pos1 = Position_class(pos1.latitude, pos1.longitude, pos1.altitude);
        py::object py_pos2 = Position_class(pos2.latitude, pos2.longitude, pos2.altitude);
        
        // Call the Python function
        py::object result = calculate_distance_func(py_pos1, py_pos2);
        
        // Convert result back to C++ double
        return result.cast<double>();
    }
};