import numpy as np
import matplotlib.pyplot as plt
from matplotlib.animation import FuncAnimation
import cv2
import argparse
import os


class IRObjectGenerator:
    def __init__(self, image_size, object_radius=10, background_temp=20, object_temp=100):
        self.width, self.height = image_size
        self.object_radius = object_radius
        self.background_temp = background_temp
        self.object_temp = object_temp
        
    def generate_frame(self, object_center):
        """Generate a single IR frame with object at given position"""
        # Create background at ambient temperature
        frame = np.full((self.height, self.width), self.background_temp, dtype=np.float32)
        
        # Create coordinate grids
        y, x = np.ogrid[:self.height, :self.width]
        
        # Calculate distance from object center
        center_x, center_y = object_center
        distance = np.sqrt((x - center_x)**2 + (y - center_y)**2)
        
        # Create circular hot object with temperature gradient
        mask = distance <= self.object_radius
        
        # Add thermal gradient (hotter in center, cooler at edges)
        gradient = np.maximum(0, 1 - distance / self.object_radius)
        temp_addition = gradient * (self.object_temp - self.background_temp)
        
        frame += temp_addition
        
        return frame
    
    def generate_sequence(self, start_pos, direction, speed_pps, duration_seconds, fps=30):
        """Generate sequence of IR frames showing moving object"""
        total_frames = int(duration_seconds * fps)
        frames = []
        
        # Normalize direction vector
        direction = np.array(direction, dtype=float)
        if np.linalg.norm(direction) > 0:
            direction = direction / np.linalg.norm(direction)
        
        for frame_num in range(total_frames):
            time_elapsed = frame_num / fps
            displacement = direction * speed_pps * time_elapsed
            current_pos = (start_pos[0] + displacement[0], start_pos[1] + displacement[1])
            
            # Only generate frame if object is within image bounds
            if (0 <= current_pos[0] < self.width and 0 <= current_pos[1] < self.height):
                frame = self.generate_frame(current_pos)
                frames.append(frame)
            else:
                # Object moved out of frame
                break
                
        return np.array(frames)
    
    def save_single_frame(self, object_center, filename):
        """Save a single IR frame as black and white image (black=low IR, white=high IR)"""
        frame = self.generate_frame(object_center)
        
        # Normalize to 0-255 for saving (black=low emissions, white=high emissions)
        normalized = ((frame - frame.min()) / (frame.max() - frame.min()) * 255).astype(np.uint8)
        
        # Save as grayscale image (black and white)
        cv2.imwrite(filename, normalized)
        
        return frame
    
    def save_animation(self, start_pos, direction, speed_pps, duration_seconds, filename, fps=30):
        """Save animated sequence as black and white video file and individual bitmap frames"""
        frames = self.generate_sequence(start_pos, direction, speed_pps, duration_seconds, fps)
        
        if len(frames) == 0:
            print("No valid frames generated - object immediately out of bounds")
            return
        
        # Create folder for individual frames
        base_name = os.path.splitext(filename)[0]
        frames_folder = f"{base_name}_frames"
        os.makedirs(frames_folder, exist_ok=True)
        
        # Setup video writer for grayscale
        fourcc = cv2.VideoWriter_fourcc(*'mp4v')
        out = cv2.VideoWriter(filename, fourcc, fps, (self.width, self.height), isColor=False)
        
        for i, frame in enumerate(frames):
            # Normalize to grayscale (black=low emissions, white=high emissions)
            normalized = ((frame - frame.min()) / (frame.max() - frame.min()) * 255).astype(np.uint8)
            
            # Write frame to video
            out.write(normalized)
            
            # Save individual frame as bitmap
            frame_filename = os.path.join(frames_folder, f"frame_{i:06d}.bmp")
            cv2.imwrite(frame_filename, normalized)
        
        out.release()
        print(f"Animation saved as {filename}")
        print(f"Individual frames saved in folder: {frames_folder}")
        
    def display_animation(self, start_pos, direction, speed_pps, duration_seconds, fps=30):
        """Display animated sequence as black and white using matplotlib"""
        frames = self.generate_sequence(start_pos, direction, speed_pps, duration_seconds, fps)
        
        if len(frames) == 0:
            print("No valid frames generated - object immediately out of bounds")
            return
        
        fig, ax = plt.subplots(figsize=(10, 8))
        im = ax.imshow(frames[0], cmap='gray', interpolation='bilinear', vmin=frames.min(), vmax=frames.max())
        ax.set_title('Infrared Object Movement (Black=Low IR, White=High IR)')
        plt.colorbar(im, label='Temperature (°C)')
        
        def animate(frame_idx):
            im.set_array(frames[frame_idx])
            return [im]
        
        anim = FuncAnimation(fig, animate, frames=len(frames), interval=1000/fps, blit=True)
        plt.show()
        
        return anim


def main():
    parser = argparse.ArgumentParser(description='Generate infrared images of moving objects')
    parser.add_argument('--width', type=int, default=640, help='Image width in pixels')
    parser.add_argument('--height', type=int, default=480, help='Image height in pixels')
    parser.add_argument('--start-x', type=float, default=100, help='Starting X position')
    parser.add_argument('--start-y', type=float, default=100, help='Starting Y position')
    parser.add_argument('--dir-x', type=float, default=1, help='Direction X component')
    parser.add_argument('--dir-y', type=float, default=0, help='Direction Y component')
    parser.add_argument('--speed', type=float, default=50, help='Speed in pixels per second')
    parser.add_argument('--duration', type=float, default=5, help='Duration in seconds')
    parser.add_argument('--radius', type=int, default=15, help='Object radius in pixels')
    parser.add_argument('--output', type=str, default='ir_object.mp4', help='Output filename')
    parser.add_argument('--single-frame', action='store_true', help='Generate single frame instead of animation')
    parser.add_argument('--display', action='store_true', help='Display animation instead of saving')
    
    args = parser.parse_args()
    
    # Create generator
    generator = IRObjectGenerator(
        image_size=(args.width, args.height),
        object_radius=args.radius
    )
    
    start_position = (args.start_x, args.start_y)
    direction = (args.dir_x, args.dir_y)
    
    if args.single_frame:
        # Generate single frame - use appropriate extension
        if args.output.endswith('.mp4'):
            output_filename = args.output.replace('.mp4', '.png')
        elif not args.output.endswith(('.png', '.jpg', '.jpeg', '.bmp', '.tiff')):
            output_filename = args.output + '.png'
        else:
            output_filename = args.output
        frame = generator.save_single_frame(start_position, output_filename)
        print(f"Single frame saved as {output_filename}")
    elif args.display:
        # Display animation
        generator.display_animation(start_position, direction, args.speed, args.duration)
    else:
        # Save animation - ensure video extension
        if not args.output.endswith(('.mp4', '.avi', '.mov', '.mkv')):
            output_filename = args.output + '.mp4'
        else:
            output_filename = args.output
        generator.save_animation(start_position, direction, args.speed, args.duration, output_filename)


if __name__ == "__main__":
    main()