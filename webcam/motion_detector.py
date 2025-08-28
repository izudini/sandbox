import cv2
import numpy as np
import time
import threading
from datetime import datetime

class MotionDetector:
    def __init__(self, camera_url, region_coords=None):
        self.camera_url = camera_url
        self.region_coords = region_coords
        self.cap = None
        self.background_subtractor = cv2.createBackgroundSubtractorMOG2(detectShadows=True)
        self.motion_detected = False
        self.alert_cooldown = 3
        self.last_alert_time = 0
        self.running = False
        
    def connect_camera(self):
        self.cap = cv2.VideoCapture(self.camera_url)
        if not self.cap.isOpened():
            print(f"Error: Cannot connect to camera at {self.camera_url}")
            return False
        return True
    
    def draw_region(self, frame):
        if self.region_coords:
            x, y, w, h = self.region_coords
            cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)
            cv2.putText(frame, "Detection Zone", (x, y - 10), 
                       cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 255, 0), 2)
        return frame
    
    def detect_motion(self, frame):
        if self.region_coords:
            x, y, w, h = self.region_coords
            roi = frame[y:y+h, x:x+w]
        else:
            roi = frame
            
        fg_mask = self.background_subtractor.apply(roi)
        
        kernel = np.ones((5, 5), np.uint8)
        fg_mask = cv2.morphologyEx(fg_mask, cv2.MORPH_OPEN, kernel)
        fg_mask = cv2.morphologyEx(fg_mask, cv2.MORPH_CLOSE, kernel)
        
        contours, _ = cv2.findContours(fg_mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        
        motion_area = 0
        for contour in contours:
            area = cv2.contourArea(contour)
            if area > 500:
                motion_area += area
                
        return motion_area > 1000
    
    def trigger_alert(self):
        current_time = time.time()
        if current_time - self.last_alert_time > self.alert_cooldown:
            timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
            print(f"\n🚨 MOTION DETECTED! - {timestamp}")
            
            def beep():
                for _ in range(3):
                    print("\a", end="", flush=True)
                    time.sleep(0.2)
            
            threading.Thread(target=beep, daemon=True).start()
            self.last_alert_time = current_time
    
    def run(self):
        if not self.connect_camera():
            return
            
        self.running = True
        print("Motion detection started. Press 'q' to quit.")
        print("Click and drag to set detection region, or press 'r' to reset region.")
        
        mouse_data = {"drawing": False, "start_pos": None, "end_pos": None}
        
        def mouse_callback(event, x, y, flags, param):
            if event == cv2.EVENT_LBUTTONDOWN:
                mouse_data["drawing"] = True
                mouse_data["start_pos"] = (x, y)
            elif event == cv2.EVENT_MOUSEMOVE and mouse_data["drawing"]:
                mouse_data["end_pos"] = (x, y)
            elif event == cv2.EVENT_LBUTTONUP:
                mouse_data["drawing"] = False
                if mouse_data["start_pos"] and mouse_data["end_pos"]:
                    x1, y1 = mouse_data["start_pos"]
                    x2, y2 = mouse_data["end_pos"]
                    self.region_coords = (min(x1, x2), min(y1, y2), 
                                        abs(x2 - x1), abs(y2 - y1))
                    print(f"Detection region set: {self.region_coords}")
        
        cv2.namedWindow("IP Camera Motion Detection")
        cv2.setMouseCallback("IP Camera Motion Detection", mouse_callback)
        
        while self.running:
            ret, frame = self.cap.read()
            if not ret:
                print("Error: Cannot read frame from camera")
                break
            
            frame = self.draw_region(frame)
            
            if mouse_data["drawing"] and mouse_data["start_pos"] and mouse_data["end_pos"]:
                x1, y1 = mouse_data["start_pos"]
                x2, y2 = mouse_data["end_pos"]
                cv2.rectangle(frame, (x1, y1), (x2, y2), (255, 0, 0), 2)
            
            motion_detected = self.detect_motion(frame)
            
            status_color = (0, 0, 255) if motion_detected else (0, 255, 0)
            status_text = "MOTION DETECTED!" if motion_detected else "No Motion"
            cv2.putText(frame, status_text, (10, 30), 
                       cv2.FONT_HERSHEY_SIMPLEX, 1, status_color, 2)
            
            timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
            cv2.putText(frame, timestamp, (10, frame.shape[0] - 10), 
                       cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 255, 255), 1)
            
            if motion_detected:
                self.trigger_alert()
            
            cv2.imshow("IP Camera Motion Detection", frame)
            
            key = cv2.waitKey(1) & 0xFF
            if key == ord('q'):
                break
            elif key == ord('r'):
                self.region_coords = None
                print("Detection region reset to full frame")
        
        self.cleanup()
    
    def cleanup(self):
        if self.cap:
            self.cap.release()
        cv2.destroyAllWindows()
        self.running = False

def main():
    camera_url = input("Enter IP camera URL (e.g., http://192.168.1.100:8080/video): ").strip()
    
    if not camera_url:
        camera_url = "http://192.168.1.100:8080/video"
        print(f"Using default URL: {camera_url}")
    
    detector = MotionDetector(camera_url)
    
    try:
        detector.run()
    except KeyboardInterrupt:
        print("\nShutting down...")
        detector.cleanup()

if __name__ == "__main__":
    main()