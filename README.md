# ITDxDDA-ASG1

## 🎮 Game Overview
This game is a mobile AR app for the cafe Chagee. you can scan QR codes to spawn 3D models of your food that you can move around and customise, before taking a picture and placing an order at the same time.

## 🛠️ Installation Guide
**For running the app in Unity Editor on Windows:**
1. Download and install Unity from https://unity.com/download
2. Download and install GitHub Desktop from https://github.com/apps/desktop
3. In GitHub Desktop
    1. Go to File -> Clone Repository -> URL
    2. Input the URL https://github.com/CheangWeiCheng/ITD-DDA-ASG1.git
    3. Choose your local path
    4. Click Clone.
4. In Unity Hub:
    1. Select "Open" → "Add project from disk"
    2. Navigate to the cloned repository folder
5. Ensure these packages are installed:
    1. AR Foundation
    2. ARCore XR Plugin
    3. Firebase Database SDK

**For running the app on an Android phone:**
1. Download ITDxDDA_ASG1.apk from the Builds folder in the GitHub repository at https://github.com/CheangWeiCheng/ITD-DDA-ASG1.git
2. Run the .apk file

## 🎮 How to Play the Game
1. Either sign up with a new account (you can use a fake email address) or login with a preexisting account
2. Scan the QR code for either the coffe or the donut. A 3D model should appear
3. Click the button on the model, and adjust the settings of your food item and position it however you want (the QR code must remain on screen)
4. (Optional) Select a photo frame with the buttons at the bottom of the screen
5. (Optional) When scanning the two types of QR codes at once, drag the 2 3D models together to join them together and create a set meal
6. Press the order button on screen to "take a picture of the models" and place your order at the same time.

## 🖥️ System Requirements
Platform: Windows / Android

**Windows**
| Component      | Recommended                                   |
|----------------|-----------------------------------------------|
| **OS**         | Windows 11 64-bit                             |
| **Processor**	 | 1200 Mhz, 14 Core(s), 18 Logical Processor(s) |
| **CPU**        | Intel(R) Core(TM) Ultra 5 125H                |
| **GPU**        | Intel® ARC Graphics                           |
| **RAM**        | 16GB                                          |

**Android**
| Component      | Recommended                                               |
|----------------|-----------------------------------------------------------|
| **OS**         | Android 14 (One UI 6.1)                                   |
| **Chipset**	 | Samsung Exynos 1480 (4 nm)                                |
| **CPU**        | Octa-core (4x2.75 GHz Cortex-A78 + 4x2.05 GHz Cortex-A55) |
| **GPU**        | Xclipse 530                                               |
| **RAM**        | 8 GB to 12 GB                                             |
| **AR Support** | ARCore compatible                                         |

## 🕹️ Key Controls
**Windows**
| Action       | Keybind           |
|--------------|-------------------|
| Move         | WASD              |
| Look         | Right Mouse Button|
| Interact     | Left Mouse Button |

## ⚠️ Known Limitations
### Current Bugs
- The Android build version has a non-functional login button and image tracking does not work (neither of these bugs are present in the Unity Editor version).

### Absent Features
- Pressing the back button does not reset the AR objects

## 📚 Asset Credits
### Images
**QR code images**
https://commons.wikimedia.org/wiki/File:Rickrolling_QR_code.png
https://www.redbubble.com/i/photographic-print/Smash-Mouth-s-All-Star-QR-Code-by-manu142/49703199.6Q0TX

**Picture Frames**
*Wooden Frame*
https://gallery.yopriceville.com/Free-Clipart-Pictures/Decorative-Elements-PNG/Wooden_Frame_Border_PNG_Clipart

*Flower Frame*
https://creazilla.com/media/clipart/7829964/flower-frame

### Audio
**Menu Selection SFX:**  
*Free UI Soundpack*  
https://assetstore.unity.com/packages/audio/sound-fx/free-ui-soundpack-239372

## Additional Credits
Deepseek AI was used at times to optimise the code and fix certain errors. However, it was not used to write the code entirely. 