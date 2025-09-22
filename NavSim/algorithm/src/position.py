"""
Position class wrapper for the protobuf Position message.
Provides a more Pythonic interface for working with position data.
"""

import position_pb2


class Position:
    """
    A wrapper class for the protobuf Position message.
    
    Represents latitude, longitude, altitude, heading, and entity ID
    for navigation simulation purposes.
    """
    
    def __init__(self, entity_id=0, latitude=0.0, longitude=0.0, altitude=0.0, heading=0.0):
        """
        Initialize a Position object.
        
        Args:
            entity_id (int): Unique identifier for the entity
            latitude (float): Latitude in decimal degrees (-90 to 90)
            longitude (float): Longitude in decimal degrees (-180 to 180)
            altitude (float): Altitude in meters above sea level
            heading (float): Heading in degrees (0-360, where 0 is North)
        """
        self._pb_position = position_pb2.Position()
        self.entity_id = entity_id
        self.latitude = latitude
        self.longitude = longitude
        self.altitude = altitude
        self.heading = heading
    
    @property
    def entity_id(self):
        """Get the entity ID."""
        return self._pb_position.entity_id
    
    @entity_id.setter
    def entity_id(self, value):
        """Set the entity ID."""
        self._pb_position.entity_id = int(value)
    
    @property
    def latitude(self):
        """Get the latitude in decimal degrees."""
        return self._pb_position.latitude
    
    @latitude.setter
    def latitude(self, value):
        """Set the latitude in decimal degrees (-90 to 90)."""
        if not -90 <= value <= 90:
            raise ValueError("Latitude must be between -90 and 90 degrees")
        self._pb_position.latitude = float(value)
    
    @property
    def longitude(self):
        """Get the longitude in decimal degrees."""
        return self._pb_position.longitude
    
    @longitude.setter
    def longitude(self, value):
        """Set the longitude in decimal degrees (-180 to 180)."""
        if not -180 <= value <= 180:
            raise ValueError("Longitude must be between -180 and 180 degrees")
        self._pb_position.longitude = float(value)
    
    @property
    def altitude(self):
        """Get the altitude in meters above sea level."""
        return self._pb_position.altitude
    
    @altitude.setter
    def altitude(self, value):
        """Set the altitude in meters above sea level."""
        self._pb_position.altitude = float(value)
    
    @property
    def heading(self):
        """Get the heading in degrees."""
        return self._pb_position.heading
    
    @heading.setter
    def heading(self, value):
        """Set the heading in degrees (0-360, where 0 is North)."""
        if not 0 <= value <= 360:
            raise ValueError("Heading must be between 0 and 360 degrees")
        self._pb_position.heading = float(value)
    
    def to_protobuf(self):
        """
        Convert to protobuf Position message.
        
        Returns:
            position_pb2.Position: The protobuf representation
        """
        return self._pb_position
    
    @classmethod
    def from_protobuf(cls, pb_position):
        """
        Create Position from protobuf message.
        
        Args:
            pb_position (position_pb2.Position): Protobuf Position message
            
        Returns:
            Position: New Position instance
        """
        position = cls()
        position._pb_position.CopyFrom(pb_position)
        return position
    
    def serialize(self):
        """
        Serialize the position to bytes.
        
        Returns:
            bytes: Serialized position data
        """
        return self._pb_position.SerializeToString()
    
    @classmethod
    def deserialize(cls, data):
        """
        Deserialize position from bytes.
        
        Args:
            data (bytes): Serialized position data
            
        Returns:
            Position: Deserialized Position instance
        """
        pb_position = position_pb2.Position()
        pb_position.ParseFromString(data)
        return cls.from_protobuf(pb_position)
    
    def __str__(self):
        """String representation of the position."""
        return (f"Position(entity_id={self.entity_id}, "
                f"lat={self.latitude:.6f}, "
                f"lon={self.longitude:.6f}, "
                f"alt={self.altitude:.2f}m, "
                f"heading={self.heading:.1f}°)")
    
    def __repr__(self):
        """Detailed string representation."""
        return (f"Position(entity_id={self.entity_id}, "
                f"latitude={self.latitude}, "
                f"longitude={self.longitude}, "
                f"altitude={self.altitude}, "
                f"heading={self.heading})")
    
    def __eq__(self, other):
        """Check equality with another Position."""
        if not isinstance(other, Position):
            return False
        return (self.entity_id == other.entity_id and
                self.latitude == other.latitude and
                self.longitude == other.longitude and
                self.altitude == other.altitude and
                self.heading == other.heading)