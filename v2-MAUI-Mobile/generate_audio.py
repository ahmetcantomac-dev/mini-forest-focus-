import math
import wave
import struct
import os

def generate_chime():
    sample_rate = 44100
    duration = 2.0
    
    # Ensure directory exists
    os.makedirs("Platforms/Android/Resources/raw", exist_ok=True)
    
    with wave.open("Platforms/Android/Resources/raw/success_chime.wav", "w") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(sample_rate)
        
        for i in range(int(sample_rate * duration)):
            t = float(i) / sample_rate
            
            # A warm major chord arpeggio
            # C5
            vol1 = math.exp(-2.5 * t)
            val1 = vol1 * math.sin(2 * math.pi * 523.25 * t)
            
            # E5
            t2 = max(0, t - 0.15)
            vol2 = math.exp(-2.5 * t2) if t > 0.15 else 0
            val2 = vol2 * math.sin(2 * math.pi * 659.25 * t2)
            
            # G5
            t3 = max(0, t - 0.3)
            vol3 = math.exp(-2.5 * t3) if t > 0.3 else 0
            val3 = vol3 * math.sin(2 * math.pi * 783.99 * t3)
            
            # C6
            t4 = max(0, t - 0.45)
            vol4 = math.exp(-2.5 * t4) if t > 0.45 else 0
            val4 = vol4 * math.sin(2 * math.pi * 1046.50 * t4)
            
            val = (val1 + val2 + val3 + val4) / 4.0
            
            # Smooth attack to avoid clicking
            if t < 0.05:
                val *= (t / 0.05)
                
            sample = int(val * 32767.0 * 0.9)
            sample = max(-32768, min(32767, sample))
            w.writeframesraw(struct.pack("<h", sample))

generate_chime()
print("Audio generated successfully.")
