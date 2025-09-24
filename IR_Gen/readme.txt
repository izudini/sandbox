 Created ir_object_generator.py with the following features:

  Key Parameters:
  - Image size (width, height)
  - Object location (start position)
  - Direction vector (x, y components)
  - Speed in pixels per second
  - Object radius and temperature

  Usage Examples:
  # Basic animation (object moving right at 50 px/s)
  python ir_object_generator.py --width 800 --height 600 --start-x 100 --start-y 300 --dir-x 1 --dir-y 0 --speed 50

  # Diagonal movement
  python ir_object_generator.py --start-x 50 --start-y 50 --dir-x 1 --dir-y 1 --speed 75 --duration 8

  # Single frame
  python ir_object_generator.py --single-frame --start-x 200 --start-y 150

  # Display instead of save
  python ir_object_generator.py --display --speed 100

  Features:
  - Generates realistic IR thermal gradients
  - Supports video output (MP4) or single frames (PNG)
  - Real-time display option
  - Configurable object temperature and size
  - Automatic boundary checking

  Required dependencies: numpy, matplotlib, opencv-python