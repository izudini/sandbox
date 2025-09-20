#!/usr/bin/env python3
"""
NavVisualiser - Navigation Visualization Tool

A Python application for visualizing navigation data from the NavSim project.
"""

import sys
from typing import Optional, Dict, List
import matplotlib.pyplot as plt
import matplotlib.animation as animation
import numpy as np


class Position:
    """Position class representing entity ID, latitude, longitude, altitude, and heading."""
    
    def __init__(self, entity_id: int = 0, latitude: float = 0.0, longitude: float = 0.0, 
                 altitude: float = 0.0, heading: float = 0.0):
        self.entity_id = entity_id
        self.latitude = latitude
        self.longitude = longitude
        self.altitude = altitude
        self.heading = heading
    
    def __str__(self) -> str:
        return f"Position(id={self.entity_id}, lat={self.latitude}, lon={self.longitude}, alt={self.altitude}, heading={self.heading})"


class NavVisualiser:
    """Main visualizer class for navigation data."""
    
    def __init__(self):
        self.entities: Dict[int, List[Position]] = {}
        self.fig = None
        self.ax = None
        self.lat_bounds = [-90, 90]
        self.lon_bounds = [-180, 180]
    
    def update_position(self, position: Position) -> None:
        """Update the current position and add to history."""
        if position.entity_id not in self.entities:
            self.entities[position.entity_id] = []
        self.entities[position.entity_id].append(position)
    
    def updateEntity(self, position: Position) -> None:
        """Update entity position (alias for update_position)."""
        self.update_position(position)
    
    def start(self) -> None:
        """Start the 2D visualization panel with latitude and longitude tickmarks."""
        # Force a specific backend that works better with Windows
        import matplotlib
        import os
        
        # Check if we're running on Windows (Git Bash, PowerShell, etc.)
        if os.name == 'nt' or 'WINDIR' in os.environ:
            try:
                matplotlib.use('TkAgg')  # Best for Windows
                print("Using TkAgg backend (Windows)")
            except ImportError:
                try:
                    matplotlib.use('Qt5Agg')
                    print("Using Qt5Agg backend (Windows)")
                except ImportError:
                    matplotlib.use('Agg')  # Fallback non-interactive
                    print("Warning: Using non-interactive backend")
        else:
            # WSL2/Linux
            try:
                matplotlib.use('TkAgg')
                print("Using TkAgg backend (Linux/WSL2)")
            except ImportError:
                try:
                    matplotlib.use('Qt5Agg')
                    print("Using Qt5Agg backend (Linux/WSL2)")
                except ImportError:
                    print("Using default backend")
        
        plt.style.use('seaborn-v0_8' if 'seaborn-v0_8' in plt.style.available else 'default')
        
        # Enable interactive mode first
        plt.ion()
        
        # Smaller figure size (50% reduction from 12x8 to 6x4)
        self.fig, self.ax = plt.subplots(figsize=(6, 4))
        
        # Make the window resizable and movable
        self.fig.canvas.manager.set_window_title('NavSim Visualizer')
        
        # Remove all labels to maximize map space
        self.ax.set_xlabel('')
        self.ax.set_ylabel('')
        
        # Set initial bounds
        self.ax.set_xlim(self.lon_bounds)
        self.ax.set_ylim(self.lat_bounds)
        
        # Configure grid and tickmarks - reduced density
        self.ax.grid(True, alpha=0.3)
        
        # Set major ticks for longitude (every 60 degrees for less clutter)
        lon_major_ticks = np.arange(-180, 181, 60)
        self.ax.set_xticks(lon_major_ticks)
        
        # Set major ticks for latitude (every 45 degrees for less clutter)
        lat_major_ticks = np.arange(-90, 91, 45)
        self.ax.set_yticks(lat_major_ticks)
        
        # Customize grid appearance - simplified
        self.ax.grid(which='major', alpha=0.4, linewidth=0.6)
        
        # Smaller tick labels with minimal padding
        self.ax.tick_params(axis='both', which='major', labelsize=6, pad=2)
        
        # Set equal aspect ratio to avoid distortion
        self.ax.set_aspect('equal', adjustable='box')
        
        # Minimize whitespace and margins - remove left/right padding
        plt.subplots_adjust(left=0.02, right=0.99, top=0.98, bottom=0.05)
        
        # Show the plot with proper event handling
        plt.show(block=False)
        plt.draw()
        
        # Force GUI update to make window responsive
        self.fig.canvas.flush_events()
        
        print("NavVisualiser started. 2D panel is now active.")
        print("Use updateEntity() to add position data.")
        print("The window should now be movable and resizable.")
    
    def visualize(self) -> None:
        """Visualize the navigation data."""
        if not self.fig:
            print("Visualization not started. Call start() first.")
            return
            
        self.ax.clear()
        # Remove all labels to maximize map space
        self.ax.set_xlabel('')
        self.ax.set_ylabel('')
        
        colors = plt.cm.tab10(np.linspace(0, 1, len(self.entities)))
        
        for i, (entity_id, positions) in enumerate(self.entities.items()):
            if positions:
                lats = [pos.latitude for pos in positions]
                lons = [pos.longitude for pos in positions]
                
                # Plot trajectory with thinner lines
                self.ax.plot(lons, lats, color=colors[i], alpha=0.7, linewidth=1.5, label=f'{entity_id}')
                
                # Plot current position with smaller markers
                if positions:
                    current_pos = positions[-1]
                    self.ax.scatter(current_pos.longitude, current_pos.latitude, 
                                  color=colors[i], s=60, marker='o', edgecolors='black', linewidth=1.5)
                    
                    # Add smaller entity ID label
                    self.ax.annotate(f'{entity_id}', 
                                   (current_pos.longitude, current_pos.latitude),
                                   xytext=(3, 3), textcoords='offset points',
                                   fontsize=8, fontweight='bold')
        
        # Update bounds based on data
        if self.entities:
            all_positions = [pos for positions in self.entities.values() for pos in positions]
            if all_positions:
                lats = [pos.latitude for pos in all_positions]
                lons = [pos.longitude for pos in all_positions]
                
                lat_margin = (max(lats) - min(lats)) * 0.1 or 1
                lon_margin = (max(lons) - min(lons)) * 0.1 or 1
                
                self.ax.set_xlim(min(lons) - lon_margin, max(lons) + lon_margin)
                self.ax.set_ylim(min(lats) - lat_margin, max(lats) + lat_margin)
        
        # Reconfigure grid and ticks for compact view
        self.ax.grid(True, alpha=0.3)
        self.ax.tick_params(axis='both', which='major', labelsize=6, pad=2)
        
        # Compact legend if multiple entities
        if len(self.entities) > 1:
            self.ax.legend(fontsize=7, loc='upper right', frameon=True, 
                          fancybox=False, shadow=False, framealpha=0.8)
        
        plt.draw()
        self.fig.canvas.flush_events()  # Force GUI update
        plt.pause(0.01)
    
    def close(self) -> None:
        """Close the visualization window."""
        if self.fig:
            plt.close(self.fig)
            self.fig = None
            self.ax = None
            plt.ioff()  # Turn off interactive mode


def main():
    """Main entry point for the NavVisualiser application."""
    print("Starting NavVisualiser...")
    
    visualizer = NavVisualiser()
    visualizer.start()
    
    # Example usage
    test_position1 = Position(1, 37.7749, -122.4194, 100.0, 45.0)
    test_position2 = Position(2, 37.7849, -122.4094, 120.0, 90.0)
    
    visualizer.updateEntity(test_position1)
    visualizer.updateEntity(test_position2)
    visualizer.visualize()
    
    # Keep the window open
    try:
        input("Press Enter to exit...")
    finally:
        visualizer.close()


if __name__ == "__main__":
    main()