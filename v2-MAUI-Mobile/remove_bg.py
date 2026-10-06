import os
from rembg import remove
from PIL import Image

def process_images(directory):
    for filename in os.listdir(directory):
        if filename.endswith(".png"):
            input_path = os.path.join(directory, filename)
            output_path = os.path.join(directory, "temp_" + filename)
            
            try:
                # Open image
                input_img = Image.open(input_path)
                
                # Try to remove background
                output_img = remove(input_img)
                
                # Save as transparent PNG
                output_img.save(output_path, "PNG")
                
                # Replace original with processed
                os.replace(output_path, input_path)
                print(f"Processed: {filename}")
                
            except Exception as e:
                print(f"Failed to process {filename}: {e}")

if __name__ == "__main__":
    img_dir = "Resources/Images"
    print(f"Processing images in {img_dir}...")
    process_images(img_dir)
    print("Done!")
