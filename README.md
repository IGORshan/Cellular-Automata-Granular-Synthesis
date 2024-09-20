# Cellular-Automata-Granular-Synthesis
A generative music system combining Cellular Automata and granular synthesis, implemented in C# and MaxMSP. This project explores algorithmic sound generation, mapping grid-based automata states to audio synthesis (granular synth), providing a good user interface for creating experimental and evolving sonic textures.

Here's a sample structure and content for your **README.md** file that provides a clear overview of your project:

## System Requirements
- macOS (required)
- **Max 8** or later installed
- .NET Framework on macOS (e.g. Avalonia) for running the Cellular Automata app

## Usage
1. Launch the **Cellular Automata** app to control the grid generation.
2. Open the **MaxMSP patch** to manage audio processing and granular synthesis.
3. Begin by sending the CA state data from the app to Max. The alive and dead cells in the grid will trigger audio grains, which you can modify in real time.
4. Explore different **sample selections** (drum loops, percussive sounds, etc.) to see how they interact with the granular engine.

## Known Limitations
- Requires **MaxMSP** for audio processing.
- Current implementation is only tested on **macOS**.
- Large grid sizes may lead to delays in processing due to Max's limitations.

## License
This project is licensed under the MIT License.

## Acknowledgements
- **SharpOSC** library for handling OSC communication
- **MaxMSP** for providing the granular synthesis environment

