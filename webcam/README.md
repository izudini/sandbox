# IP Webcam Motion Detection

A Python application that displays an IP webcam feed and alerts when movement is detected in a specified region.

## Features

- Connect to IP webcam feeds
- Interactive region selection for motion detection
- Real-time motion detection using background subtraction
- Visual and audio alerts when motion is detected
- Alert cooldown to prevent spam notifications

## Installation

```bash
pip install -r requirements.txt
```

## Usage

```bash
python motion_detector.py
```

Enter your IP camera URL when prompted (e.g., `http://192.168.1.100:8080/video`).

## Controls

- **Mouse**: Click and drag to set detection region
- **'r'**: Reset detection region to full frame  
- **'q'**: Quit application

## Camera URL Examples

- IP Webcam (Android): `http://192.168.1.100:8080/video`
- DroidCam: `http://192.168.1.100:4747/video`
- Generic MJPEG: `http://camera_ip:port/mjpeg`