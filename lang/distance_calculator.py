import math
from typing import NamedTuple

class Position(NamedTuple):
    latitude: float
    longitude: float
    altitude: float

def calculate_distance(pos1: Position, pos2: Position) -> float:
    """
    Calculate the distance between two positions using the haversine formula
    for great circle distance, plus the difference in altitude.
    
    Args:
        pos1: First position (latitude, longitude, altitude)
        pos2: Second position (latitude, longitude, altitude)
    
    Returns:
        Distance in meters
    """
    # Earth's radius in meters
    EARTH_RADIUS = 6371000
    
    # Convert latitude and longitude from degrees to radians
    lat1_rad = math.radians(pos1.latitude)
    lon1_rad = math.radians(pos1.longitude)
    lat2_rad = math.radians(pos2.latitude)
    lon2_rad = math.radians(pos2.longitude)
    
    # Haversine formula for great circle distance
    dlat = lat2_rad - lat1_rad
    dlon = lon2_rad - lon1_rad
    
    a = (math.sin(dlat / 2) ** 2 + 
         math.cos(lat1_rad) * math.cos(lat2_rad) * math.sin(dlon / 2) ** 2)
    c = 2 * math.atan2(math.sqrt(a), math.sqrt(1 - a))
    
    # Horizontal distance
    horizontal_distance = EARTH_RADIUS * c
    
    # Vertical distance (altitude difference)
    altitude_difference = abs(pos2.altitude - pos1.altitude)
    
    # Total 3D distance using Pythagorean theorem
    total_distance = math.sqrt(horizontal_distance ** 2 + altitude_difference ** 2)
    
    return total_distance

# Example usage
if __name__ == "__main__":
    # Example positions
    pos1 = Position(latitude=40.7128, longitude=-74.0060, altitude=10.0)  # New York
    pos2 = Position(latitude=34.0522, longitude=-118.2437, altitude=100.0)  # Los Angeles
    
    distance = calculate_distance(pos1, pos2)
    print(f"Distance between positions: {distance:.2f} meters ({distance/1000:.2f} km)")