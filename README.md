# TTROverlay

## Overview

This tool enhances the streaming experience for Toontown Rewritten by providing a simple overlay that updates based on the game server. The overlay appears in the client area of the game window and adjusts dynamically without user intervention. This project is developed using Windows Forms and is currently supported only on Windows.
## Installation

1. **Download the Latest Version**: Download the `publish.zip` file from the [releases page](https://github.com/safepre/TTR-Overlay/releases/tag/v1.0.0). Then unzip the file by right-clicking on it and selecting "Extract All" or using your preferred extraction tool. Navigate to the extracted folder, double-click 'publish/'

![publish](https://github.com/user-attachments/assets/7ea5cdf6-d3c5-4f7b-891c-cc1a78b487bf)
   
double click 'setup.exe' to start the installation

![exe](https://github.com/user-attachments/assets/bbf2a68a-107c-4d84-9189-c93acf0e36ab)

and follow the on-screen prompts to complete the installation.

2. **Set Up the Overlay**: (FOLLOW THIS STEP) Make sure to use your toon. As soon as your toon is at a playground, go to Options and turn on 'Companion App Support' under 'Gameplay.' Open the application and click the Connect button.

![2](https://github.com/user-attachments/assets/832a95de-3184-4421-89dc-0cf5441aeca4) <!-- Replace # with the URL of your screenshot -->

![4](https://github.com/user-attachments/assets/f8fcd68a-0f4f-464f-b9bf-3dc6a419d32b)


## Setting Up TTROverlay for OBS

To capture the TTROverlay in OBS (Open Broadcaster Software), follow these steps:

1. **Open OBS**: Launch OBS Studio on your computer.

2. **Add a New Source**:
   - Click the `+` button in the "Sources" box at the bottom of the OBS window.
   - Select `Window Capture` from the list of source types.

3. **Configure Window Capture**:
   - Name your source (e.g., "TTROverlay").
   - Click `OK` to proceed to the next step.

4. **Select the Correct Window**:
   - In the "Window" dropdown menu, find and select `TTREngine64.exe` (or the window title that corresponds to TTROverlay).
   - Make sure the "Capture Method" is set to `Windows 10 (1903 and up)` for the overlay to pop up.
   - Ensure that the "Capture Specific Window" option is selected and the correct window is chosen.
   - Click `OK` to confirm the settings.

