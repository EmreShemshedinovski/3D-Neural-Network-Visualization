# 3D Neural Grid Visualizer (Unity + iPad)

A real-time synchronization system between a mobile drawing interface and a Unity 3D environment. This project was recognized as the top-performing implementation in a class of 100 students.

# Technical Implementation
* **Internal C# Server:** Unity acts as the host, using a non-blocking HTTP Listener to process incoming JSON pixel data.
* **Gimbal Lock Prevention:** Implemented custom Quaternion-based rotation logic to ensure mathematical stability across all axes.
* **Real-time Sync:** Leverages ngrok tunneling for low-latency communication between the iPad client and the local engine.
* **Architecture:** 784-voxel grid (28x28) with dynamic emission and color mapping.
* ** The servers are Python as a Library to reach and hold the Draw.html file, 8081 is the server that Unity Listens and Answers.

# How to Run
1. Open the project in Unity 2022.x or newer.
2. Ensure the C# script is listening on the correct port.
3. Start an ngrok tunnel to your local port.
4. Copy and Paste the NGROK https link to Draw.html in the Fetch section
5. Run pyton -m http.server 5500 on the terminal.
6. Get your IPV4 with typing ipconfig in CMD, and paste it to the browser of your device that will give the input such as an iPad   
7. Send pixel data from the client to Unity.
8. If errors encountered check firewall permissions of Unity app.