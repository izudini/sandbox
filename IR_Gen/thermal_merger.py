import numpy as np
import cv2
import argparse
import os
import sys


class ThermalFrameMerger:
    def __init__(self):
        self.supported_formats = ['.mp4', '.avi', '.mov', '.mkv']
        
    def load_video_frames(self, video_path):
        """Load all frames from a video file"""
        if not os.path.exists(video_path):
            raise FileNotFoundError(f"Video file not found: {video_path}")
            
        cap = cv2.VideoCapture(video_path)
        if not cap.isOpened():
            raise ValueError(f"Could not open video file: {video_path}")
        
        frames = []
        fps = cap.get(cv2.CAP_PROP_FPS)
        
        while True:
            ret, frame = cap.read()
            if not ret:
                break
            # Convert to grayscale if needed
            if len(frame.shape) == 3:
                frame = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
            frames.append(frame)
        
        cap.release()
        return np.array(frames), fps
    
    def resize_frames(self, frames1, frames2):
        """Resize frame sets to have matching dimensions"""
        if len(frames1) == 0 or len(frames2) == 0:
            raise ValueError("One or both video files contain no frames")
        
        # Get dimensions of both frame sets
        h1, w1 = frames1[0].shape
        h2, w2 = frames2[0].shape
        
        # Use the smaller dimensions to ensure both fit
        target_height = min(h1, h2)
        target_width = min(w1, w2)
        
        # Resize frames1 if needed
        if h1 != target_height or w1 != target_width:
            resized_frames1 = []
            for frame in frames1:
                resized = cv2.resize(frame, (target_width, target_height))
                resized_frames1.append(resized)
            frames1 = np.array(resized_frames1)
        
        # Resize frames2 if needed
        if h2 != target_height or w2 != target_width:
            resized_frames2 = []
            for frame in frames2:
                resized = cv2.resize(frame, (target_width, target_height))
                resized_frames2.append(resized)
            frames2 = np.array(resized_frames2)
        
        return frames1, frames2
    
    def synchronize_frame_counts(self, frames1, frames2):
        """Ensure both frame sets have the same number of frames"""
        min_frames = min(len(frames1), len(frames2))
        return frames1[:min_frames], frames2[:min_frames]
    
    def scale_heat_circles(self, frames, start_size, end_size):
        """Scale heat circles in frames from start_size to end_size"""
        if len(frames) == 0:
            return frames
            
        if start_size == end_size:
            return frames
            
        scaled_frames = []
        num_frames = len(frames)
        
        for frame_idx, frame in enumerate(frames):
            # Calculate current scale factor based on frame position
            if num_frames == 1:
                scale_factor = end_size / start_size
            else:
                progress = frame_idx / (num_frames - 1)  # 0.0 to 1.0
                current_size = start_size + progress * (end_size - start_size)
                scale_factor = current_size / start_size
            
            if abs(scale_factor - 1.0) < 0.01:  # No significant scaling needed
                scaled_frames.append(frame)
                continue
            
            # Find heat sources (bright spots) in the frame
            # Threshold to identify hot regions
            threshold = np.mean(frame) + 2 * np.std(frame)
            heat_mask = frame > threshold
            
            if not np.any(heat_mask):
                # No heat sources found, keep original frame
                scaled_frames.append(frame)
                continue
            
            # Create scaled frame
            scaled_frame = frame.copy().astype(np.float32)
            
            # Find connected components (heat sources)
            num_labels, labels = cv2.connectedComponents(heat_mask.astype(np.uint8))
            
            for label in range(1, num_labels):  # Skip background (label 0)
                component_mask = (labels == label)
                if not np.any(component_mask):
                    continue
                
                # Find center of heat source
                y_coords, x_coords = np.where(component_mask)
                center_x = int(np.mean(x_coords))
                center_y = int(np.mean(y_coords))
                
                # Create distance map from center
                h, w = frame.shape
                y, x = np.ogrid[:h, :w]
                distance_from_center = np.sqrt((x - center_x)**2 + (y - center_y)**2)
                
                # Find current extent of heat source
                max_distance = np.max(distance_from_center[component_mask])
                if max_distance == 0:
                    continue
                
                # Calculate new extent
                new_max_distance = max_distance * scale_factor
                
                # Create new heat pattern
                new_heat_mask = distance_from_center <= new_max_distance
                
                # Get background temperature around this heat source
                background_region = (distance_from_center > max_distance * 1.5) & (distance_from_center < max_distance * 2.5)
                if np.any(background_region):
                    background_temp = np.mean(frame[background_region])
                else:
                    background_temp = np.mean(frame)
                
                # Get peak temperature of original heat source
                peak_temp = np.max(frame[component_mask])
                
                # Reset the area around this heat source to background
                reset_region = distance_from_center <= max(max_distance, new_max_distance) * 1.2
                scaled_frame[reset_region] = background_temp
                
                # Apply new heat pattern
                if scale_factor > 1.0:
                    # Expanding: create larger heat source
                    heat_intensity = np.maximum(0, 1 - distance_from_center / new_max_distance)
                    heat_addition = heat_intensity * (peak_temp - background_temp)
                    scaled_frame[new_heat_mask] = background_temp + heat_addition[new_heat_mask]
                else:
                    # Shrinking: create smaller heat source
                    heat_intensity = np.maximum(0, 1 - distance_from_center / new_max_distance)
                    heat_addition = heat_intensity * (peak_temp - background_temp)
                    scaled_frame[new_heat_mask] = background_temp + heat_addition[new_heat_mask]
            
            # Ensure we maintain proper data type and range
            scaled_frame = np.clip(scaled_frame, 0, 255).astype(frame.dtype)
            scaled_frames.append(scaled_frame)
        
        return np.array(scaled_frames)
    
    def merge_frames_side_by_side(self, frames1, frames2, gap=10):
        """Merge two sets of frames side by side with optional gap"""
        frames1, frames2 = self.resize_frames(frames1, frames2)
        frames1, frames2 = self.synchronize_frame_counts(frames1, frames2)
        
        if len(frames1) == 0:
            raise ValueError("No frames to merge after synchronization")
        
        height, width = frames1[0].shape
        
        # Create merged frames
        merged_frames = []
        for i in range(len(frames1)):
            # Create combined frame with gap
            merged_width = width * 2 + gap
            merged_frame = np.zeros((height, merged_width), dtype=frames1[0].dtype)
            
            # Place first frame on the left
            merged_frame[:, :width] = frames1[i]
            
            # Place second frame on the right (after gap)
            merged_frame[:, width + gap:] = frames2[i]
            
            merged_frames.append(merged_frame)
        
        return np.array(merged_frames)
    
    def merge_frames_overlay(self, frames1, frames2, alpha=0.5):
        """Merge two sets of frames by overlaying them"""
        frames1, frames2 = self.resize_frames(frames1, frames2)
        frames1, frames2 = self.synchronize_frame_counts(frames1, frames2)
        
        if len(frames1) == 0:
            raise ValueError("No frames to merge after synchronization")
        
        # Normalize frames to same data type and range
        frames1_norm = frames1.astype(np.float32) / 255.0
        frames2_norm = frames2.astype(np.float32) / 255.0
        
        # Create overlay
        merged_frames = []
        for i in range(len(frames1)):
            merged = (alpha * frames1_norm[i] + (1 - alpha) * frames2_norm[i])
            merged = (merged * 255).astype(np.uint8)
            merged_frames.append(merged)
        
        return np.array(merged_frames)
    
    def save_merged_video(self, merged_frames, output_path, fps=30):
        """Save merged frames as a video file"""
        if len(merged_frames) == 0:
            raise ValueError("No frames to save")
        
        height, width = merged_frames[0].shape
        
        # Ensure output has video extension
        if not any(output_path.lower().endswith(ext) for ext in self.supported_formats):
            output_path += '.mp4'
        
        # Create video writer
        fourcc = cv2.VideoWriter_fourcc(*'mp4v')
        out = cv2.VideoWriter(output_path, fourcc, fps, (width, height), isColor=False)
        
        # Write frames
        for frame in merged_frames:
            out.write(frame)
        
        out.release()
        return output_path
    
    def save_individual_frames(self, merged_frames, output_dir):
        """Save individual merged frames as bitmap files"""
        os.makedirs(output_dir, exist_ok=True)
        
        for i, frame in enumerate(merged_frames):
            frame_path = os.path.join(output_dir, f"merged_frame_{i:06d}.bmp")
            cv2.imwrite(frame_path, frame)
        
        return output_dir
    
    def merge_thermal_recordings(self, video1_path, video2_path, output_path, 
                                merge_mode='side_by_side', gap=10, alpha=0.5, 
                                save_frames=False, scale1_start=None, scale1_end=None,
                                scale2_start=None, scale2_end=None):
        """
        Main function to merge two thermal recordings
        
        Args:
            video1_path: Path to first video file
            video2_path: Path to second video file
            output_path: Path for merged output video
            merge_mode: 'side_by_side' or 'overlay'
            gap: Gap between videos in side_by_side mode (pixels)
            alpha: Blending factor for overlay mode (0.0-1.0)
            save_frames: Whether to save individual frames
            scale1_start: Starting size for video1 heat circles (pixels)
            scale1_end: Ending size for video1 heat circles (pixels)
            scale2_start: Starting size for video2 heat circles (pixels)
            scale2_end: Ending size for video2 heat circles (pixels)
        """
        print(f"Loading frames from {video1_path}...")
        frames1, fps1 = self.load_video_frames(video1_path)
        
        print(f"Loading frames from {video2_path}...")
        frames2, fps2 = self.load_video_frames(video2_path)
        
        # Use the lower fps for output
        output_fps = min(fps1, fps2)
        
        print(f"Video 1: {len(frames1)} frames at {fps1} FPS, resolution: {frames1[0].shape}")
        print(f"Video 2: {len(frames2)} frames at {fps2} FPS, resolution: {frames2[0].shape}")
        
        # Apply heat circle scaling if specified
        if scale1_start is not None and scale1_end is not None:
            print(f"Scaling video 1 heat circles from {scale1_start}px to {scale1_end}px...")
            frames1 = self.scale_heat_circles(frames1, scale1_start, scale1_end)
            
        if scale2_start is not None and scale2_end is not None:
            print(f"Scaling video 2 heat circles from {scale2_start}px to {scale2_end}px...")
            frames2 = self.scale_heat_circles(frames2, scale2_start, scale2_end)
        
        # Merge frames
        if merge_mode == 'side_by_side':
            print(f"Merging frames side by side with {gap}px gap...")
            merged_frames = self.merge_frames_side_by_side(frames1, frames2, gap)
        elif merge_mode == 'overlay':
            print(f"Merging frames with overlay (alpha={alpha})...")
            merged_frames = self.merge_frames_overlay(frames1, frames2, alpha)
        else:
            raise ValueError(f"Unknown merge mode: {merge_mode}")
        
        print(f"Merged {len(merged_frames)} frames, resolution: {merged_frames[0].shape}")
        
        # Save merged video
        print(f"Saving merged video to {output_path}...")
        final_output_path = self.save_merged_video(merged_frames, output_path, output_fps)
        
        # Save individual frames if requested
        if save_frames:
            frames_dir = os.path.splitext(final_output_path)[0] + '_frames'
            print(f"Saving individual frames to {frames_dir}...")
            self.save_individual_frames(merged_frames, frames_dir)
        
        print(f"Merge complete! Output saved as: {final_output_path}")
        return final_output_path


def main():
    parser = argparse.ArgumentParser(description='Merge frames from two thermal recordings')
    parser.add_argument('video1', help='Path to first thermal recording')
    parser.add_argument('video2', help='Path to second thermal recording')
    parser.add_argument('-o', '--output', default='merged_thermal.mp4', 
                       help='Output video filename (default: merged_thermal.mp4)')
    parser.add_argument('-m', '--mode', choices=['side_by_side', 'overlay'], 
                       default='side_by_side', help='Merge mode (default: side_by_side)')
    parser.add_argument('-g', '--gap', type=int, default=10, 
                       help='Gap between videos in side_by_side mode (default: 10)')
    parser.add_argument('-a', '--alpha', type=float, default=0.5, 
                       help='Blending factor for overlay mode 0.0-1.0 (default: 0.5)')
    parser.add_argument('-f', '--save-frames', action='store_true', 
                       help='Save individual merged frames as bitmap files')
    parser.add_argument('--scale1-start', type=float, 
                       help='Starting size for video1 heat circles (pixels)')
    parser.add_argument('--scale1-end', type=float, 
                       help='Ending size for video1 heat circles (pixels)')
    parser.add_argument('--scale2-start', type=float, 
                       help='Starting size for video2 heat circles (pixels)')
    parser.add_argument('--scale2-end', type=float, 
                       help='Ending size for video2 heat circles (pixels)')
    
    args = parser.parse_args()
    
    # Validate inputs
    if not os.path.exists(args.video1):
        print(f"Error: First video file not found: {args.video1}")
        sys.exit(1)
    
    if not os.path.exists(args.video2):
        print(f"Error: Second video file not found: {args.video2}")
        sys.exit(1)
    
    if args.mode == 'overlay' and not (0.0 <= args.alpha <= 1.0):
        print(f"Error: Alpha value must be between 0.0 and 1.0, got {args.alpha}")
        sys.exit(1)
    
    # Validate scaling parameters
    if (args.scale1_start is not None) != (args.scale1_end is not None):
        print("Error: Both --scale1-start and --scale1-end must be specified together")
        sys.exit(1)
    
    if (args.scale2_start is not None) != (args.scale2_end is not None):
        print("Error: Both --scale2-start and --scale2-end must be specified together")
        sys.exit(1)
    
    if args.scale1_start is not None and args.scale1_start <= 0:
        print(f"Error: scale1-start must be positive, got {args.scale1_start}")
        sys.exit(1)
    
    if args.scale1_end is not None and args.scale1_end <= 0:
        print(f"Error: scale1-end must be positive, got {args.scale1_end}")
        sys.exit(1)
    
    if args.scale2_start is not None and args.scale2_start <= 0:
        print(f"Error: scale2-start must be positive, got {args.scale2_start}")
        sys.exit(1)
    
    if args.scale2_end is not None and args.scale2_end <= 0:
        print(f"Error: scale2-end must be positive, got {args.scale2_end}")
        sys.exit(1)
    
    # Create merger and process
    try:
        merger = ThermalFrameMerger()
        merger.merge_thermal_recordings(
            args.video1, args.video2, args.output,
            merge_mode=args.mode, gap=args.gap, alpha=args.alpha,
            save_frames=args.save_frames,
            scale1_start=args.scale1_start, scale1_end=args.scale1_end,
            scale2_start=args.scale2_start, scale2_end=args.scale2_end
        )
    except Exception as e:
        print(f"Error during merge: {str(e)}")
        sys.exit(1)


if __name__ == "__main__":
    main()