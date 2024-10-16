# Cellular-Automata-Granular-Synthesis
A generative music system combining Cellular Automata and granular synthesis, implemented in C# and MaxMSP. This project explores algorithmic sound generation, mapping grid-based automata states to audio synthesis (granular synth), providing the user interface for creating experimental and evolving sound as a tool for musician, sound designer or anyone working with sound.

## System Requirements
- **macOS** (required for the application the run)
- **Max 8** or later installed (required for the application the run)
- .NET Framework on macOS (e.g. Avalonia) (required for modifying the Cellular Automata app)

## Usage
1. Launch the **CellularAutomataUI.app** to control the grid generation.
2. Open the **MaxMSP patch**: granularMain.maxpat to manage audio processing and granular synthesis.
3. Begin by sending the CA state data from the app to Max patch. The alive and dead cells in the grid will trigger audio grains, which you can modify in real time by interacting with the app.
4. In the max patch, explore different **sample selections** (drum loops, percussive sounds, etc.) to see how different sounds interact with this granular engine and Cellular Automata.

## Known Limitations
- Requires **MaxMSP** for audio processing.
- Current implementation is only tested on **macOS**.
- Setting large grid sizes of the Cellular Automata grid may lead to delays in processing due to the "poly~" object's limitations of processing 1028 voices maxmimum in Max.

## License
This project is licensed under the MIT License.

## Acknowledgements
- **SharpOSC** library for handling OSC communication
- **MaxMSP** for providing the granular synthesis environment

