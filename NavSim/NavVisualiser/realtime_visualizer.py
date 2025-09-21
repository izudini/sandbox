#!/usr/bin/env python3
"""
Real-time NavSim Flight Visualizer

Reads flight data from CSV file and displays real-time flight tracking on a map.
"""

import os
import sys
import time
import pandas as pd
import matplotlib.pyplot as plt
import matplotlib.animation as animation
from matplotlib.patches import FancyBboxPatch
import numpy as np

class RealtimeFlightVisualizer:
    def __init__(self, data_file="flight_data.csv"):
        self.data_file = data_file
        self.fig, self.ax = plt.subplots(figsize=(12, 8))
        self.last_update_time = 0
        self.flight_paths = {}
        self.current_positions = {}
        
        # Set up the plot
        self.setup_plot()
        
    def setup_plot(self):
        """Set up the matplotlib plot for flight visualization."""
        self.ax.set_title('NavSim Real-time Flight Tracker', fontsize=16, fontweight='bold')
        self.ax.set_xlabel('Longitude', fontsize=12)
        self.ax.set_ylabel('Latitude', fontsize=12)
        self.ax.grid(True, alpha=0.3)
        
        # Set initial bounds (will update based on data)
        self.ax.set_xlim(-125, -115)
        self.ax.set_ylim(32, 40)
        
        # Equal aspect ratio
        self.ax.set_aspect('equal', adjustable='box')
        
        # Add legend placeholder
        self.legend_elements = []
        
    def read_flight_data(self):
        """Read flight data from CSV file."""
        if not os.path.exists(self.data_file):
            return pd.DataFrame()
            
        try:
            # Check if file has been updated
            file_mod_time = os.path.getmtime(self.data_file)
            if file_mod_time <= self.last_update_time:
                return pd.DataFrame()  # No new data
                
            self.last_update_time = file_mod_time
            
            # Read the CSV file
            df = pd.read_csv(self.data_file)
            
            if df.empty:
                return df
                
            # Ensure required columns exist
            required_cols = ['entity_id', 'latitude', 'longitude', 'altitude', 'heading']
            if not all(col in df.columns for col in required_cols):
                print(f"Warning: CSV file missing required columns: {required_cols}")
                return pd.DataFrame()
                
            return df
            
        except Exception as e:
            print(f"Error reading flight data: {e}")
            return pd.DataFrame()
    
    def update_visualization(self, frame):
        """Update the visualization with new flight data."""
        df = self.read_flight_data()
        
        if df.empty:
            return []
        
        # Clear previous plots but keep the setup
        self.ax.clear()
        self.setup_plot()
        
        # Process data by entity
        entities = df['entity_id'].unique()
        colors = plt.cm.Set1(np.linspace(0, 1, len(entities)))
        
        updated_elements = []
        
        for i, entity_id in enumerate(entities):
            entity_data = df[df['entity_id'] == entity_id].sort_index()
            
            if len(entity_data) < 1:
                continue
                
            lats = entity_data['latitude'].values
            lons = entity_data['longitude'].values
            
            # Plot flight path
            if len(entity_data) > 1:
                line, = self.ax.plot(lons, lats, color=colors[i], linewidth=2, 
                                   alpha=0.7, label=f'Entity {entity_id} Path')
                updated_elements.append(line)
            
            # Plot current position (last point)
            current_lat = lats[-1]
            current_lon = lons[-1]
            current_alt = entity_data['altitude'].iloc[-1]
            current_heading = entity_data['heading'].iloc[-1]
            
            # Aircraft symbol
            scatter = self.ax.scatter(current_lon, current_lat, s=200, c=colors[i], 
                                    marker='^', edgecolors='black', linewidth=2,
                                    zorder=5)
            updated_elements.append(scatter)
            
            # Add entity label with info
            info_text = f'Entity {entity_id}\\nAlt: {current_alt:.0f}ft\\nHdg: {current_heading:.0f}°'
            text = self.ax.annotate(info_text, (current_lon, current_lat),
                                  xytext=(10, 10), textcoords='offset points',
                                  bbox=dict(boxstyle='round,pad=0.3', facecolor=colors[i], alpha=0.7),
                                  fontsize=9, fontweight='bold')
            updated_elements.append(text)
        
        # Update plot bounds based on data
        if not df.empty:
            lat_margin = (df['latitude'].max() - df['latitude'].min()) * 0.1 or 1
            lon_margin = (df['longitude'].max() - df['longitude'].min()) * 0.1 or 1
            
            self.ax.set_xlim(df['longitude'].min() - lon_margin, 
                           df['longitude'].max() + lon_margin)
            self.ax.set_ylim(df['latitude'].min() - lat_margin, 
                           df['latitude'].max() + lat_margin)
        
        # Add legend if multiple entities
        if len(entities) > 1:
            self.ax.legend(loc='upper right', fontsize=10)
        
        # Add status info
        status_text = f'Last Update: {time.strftime("%H:%M:%S")}\\nEntities: {len(entities)}\\nData Points: {len(df)}'
        self.ax.text(0.02, 0.98, status_text, transform=self.ax.transAxes,
                    verticalalignment='top', bbox=dict(boxstyle='round', facecolor='wheat', alpha=0.8),
                    fontsize=9)
        
        return updated_elements
    
    def start_visualization(self):
        """Start the real-time visualization."""
        print("Starting NavSim Real-time Flight Visualizer...")
        print(f"Monitoring file: {self.data_file}")
        print("Close the window to exit.")
        
        # Set up animation
        ani = animation.FuncAnimation(self.fig, self.update_visualization, 
                                    interval=1000, blit=False, cache_frame_data=False)
        
        # Show the plot
        plt.tight_layout()
        plt.show()

def main():
    """Main entry point."""
    data_file = "flight_data.csv"
    
    # Check if custom data file specified
    if len(sys.argv) > 1:
        data_file = sys.argv[1]
    
    print("NavSim Real-time Flight Visualizer")
    print("==================================")
    print(f"Data file: {data_file}")
    
    # Create and start visualizer
    visualizer = RealtimeFlightVisualizer(data_file)
    
    try:
        visualizer.start_visualization()
    except KeyboardInterrupt:
        print("\\nVisualization stopped by user.")
    except Exception as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    main()