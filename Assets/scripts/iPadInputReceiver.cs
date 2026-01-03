using UnityEngine;
using System.Net;
using System.Threading;
using System.IO;

public class iPadInputReceiver : MonoBehaviour {
    private HttpListener listener;
    private Thread thread;
    public float[] currentPixels = new float[784];
    public bool newDataReceived = false; 
    private bool isRunning = false;

    // We store the raw string here first (Thread Safe)
    private string pendingJSON = ""; 
    private bool hasPendingData = false;
    private object dataLock = new object(); // Prevents reading/writing conflicts

    void Start() {
        isRunning = true;
        // Reset pixels to 0
        currentPixels = new float[784]; 
        
        thread = new Thread(Listen);
        thread.Start();
        Debug.Log("Server Started on Port 8081...");
    }

    void Update() {
        // 1. Check if the Thread gave us new data
        if (hasPendingData) {
            string jsonToProcess = "";
            
            // Lock safely to grab the string
            lock (dataLock) {
                jsonToProcess = pendingJSON;
                hasPendingData = false;
            }

            // 2. NOW we convert it (On the Main Thread - Safe!)
            if (!string.IsNullOrEmpty(jsonToProcess)) {
                ProcessJSON(jsonToProcess);
            }
        }
    }

    void ProcessJSON(string json) {
        try {
            // Log the raw data so you KNOW it arrived
            // Debug.Log("Raw JSON received: " + json.Substring(0, 50) + "..."); 

            PixelWrapper data = JsonUtility.FromJson<PixelWrapper>(json);
            
            if (data != null && data.pixels != null && data.pixels.Length > 0) {
                currentPixels = data.pixels;
                newDataReceived = true;
                Debug.Log("SUCCESS! Pixels Updated. Value at 100: " + currentPixels[100]);
            } else {
                Debug.LogWarning("JSON parsed but data was empty.");
            }
        } catch (System.Exception e) {
            Debug.LogError("JSON Error: " + e.Message);
        }
    }

    void Listen() {
        try {
            listener = new HttpListener();
            // This + allows ngrok to talk to it
            listener.Prefixes.Add("http://+:8081/"); 
            listener.Start();

            while (isRunning) {
                var context = listener.GetContext();
                
                // Allow CORS so the browser doesn't complain
                context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                context.Response.AddHeader("Access-Control-Allow-Methods", "POST");
                context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

                if (context.Request.HttpMethod == "POST") {
                    using (var reader = new StreamReader(context.Request.InputStream)) {
                        string json = reader.ReadToEnd();
                        
                        // Pass the raw string to the main Update() loop
                        lock (dataLock) {
                            pendingJSON = json;
                            hasPendingData = true;
                        }
                    }
                }
                
                context.Response.StatusCode = 200;
                context.Response.Close();
            }
        } catch (System.Exception e) {
            Debug.LogError("Socket Error: " + e.Message);
        }
    }

    // The Wrapper class must match your JSON EXACTLY
    [System.Serializable] 
    public class PixelWrapper { 
        public float[] pixels; 
    }

    void OnApplicationQuit() { 
        isRunning = false; 
        if (listener != null) listener.Stop(); 
        if (thread != null) thread.Abort(); 
    }
}