"""
NavVisualiser - Navigation Visualization Tool

A Python package for visualizing navigation data from the NavSim project.
"""

__version__ = "1.0.0"
__author__ = "NavSim Project"
__description__ = "Navigation Visualization Tool"

from .main import Position, NavVisualiser

__all__ = ["Position", "NavVisualiser"]