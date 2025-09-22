"""
NavListener class for handling position updates in navigation simulation.
"""

from .position import Position


class NavListener:
    """
    A listener class for navigation position updates.
    
    Handles incoming position data and provides logging/monitoring capabilities.
    """
    
    def __init__(self):
        """Initialize the NavListener."""
        pass
    
    def on_position_update(self, position):
        """
        Handle a position update by printing it to the console.
        
        Args:
            position (Position): The position object to process
        """
        if not isinstance(position, Position):
            raise TypeError("Expected Position object")
        
        print(f"Position Update: {position}")