using UnityEngine;
using System.Collections.Generic;

public class NeuralNetworkVisualization : MonoBehaviour {
    [Header("Prefabs & Materials")]
    public GameObject neuronPrefab;
    public Material lineMaterial;
    public iPadInputReceiver receiver; 

    [Header("Thickness Controls")]
    public float outerFrameThickness = 0.06f; 
    public float internalSignalThickness = 0.02f; 

    [Header("Visual Settings")]
    public Color themeColor = new Color(0f, 0.75f, 1f);
    public float rotationSpeed = 20f; 

    private List<GameObject> inputLayer = new List<GameObject>();
    private List<GameObject> hiddenLayer = new List<GameObject>();
    private List<LineRenderer> activeLines = new List<LineRenderer>();

    void Start() {
        // The 3D Neural Structure
        inputLayer = CreateLayer(28, 28, 0f, 1.1f);
        hiddenLayer = CreateLayer(10, 10, 12f, 3.5f); 
        
        // Box Frame
        CreateClosedBoxFrame(); 

        // Line material setup for internal signal
        for (int i = 0; i < 200; i++) {
            GameObject lineObj = new GameObject("NeuralStream");
            lineObj.transform.SetParent(this.transform);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.positionCount = 2;
            lr.enabled = false; 
            activeLines.Add(lr);
        }
    }

    void Update() {
        if (receiver == null || receiver.currentPixels == null || receiver.currentPixels.Length < 784) return;

        //(Multi-Axis Rotation)
        float tiltX = Mathf.Sin(Time.time * 0.5f) * 10f; 
        float tiltY = Time.time * rotationSpeed;       
        float tiltZ = Mathf.Cos(Time.time * 0.3f) * 5f;  
        transform.rotation = Quaternion.Euler(tiltX, tiltY, tiltZ);

        // AUTO-CLEAR Section 
        foreach (GameObject neuron in inputLayer) {
            neuron.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
        }

        int lineUsageIndex = 0;

        // Current Pixels of Activation
        for (int y = 0; y < 28; y++) {
            for (int x = 0; x < 28; x++) {
                int dataIndex = y * 28 + x; 
                
                // The coordinate flip
                int unityIndex = (27 - y) * 28 + x; 

                float val = receiver.currentPixels[dataIndex];
                
                if (val > 0.05f && unityIndex < inputLayer.Count) {
                    Renderer r = inputLayer[unityIndex].GetComponent<Renderer>();
                    r.material.SetColor("_EmissionColor", themeColor * val * 20f);
                    
                    if (lineUsageIndex < activeLines.Count && val > 0.5f) {
                        LineRenderer lr = activeLines[lineUsageIndex];
                        lr.enabled = true;
                        lr.SetPosition(0, inputLayer[unityIndex].transform.position);
                        lr.SetPosition(1, hiddenLayer[Random.Range(0, hiddenLayer.Count)].transform.position);
                        
                        // Internal lines 
                        lr.widthMultiplier = internalSignalThickness; 
                        lineUsageIndex++;
                    }
                }
            }
        }
        
        for (int j = lineUsageIndex; j < activeLines.Count; j++) {
            activeLines[j].enabled = false;
        }
    }

    void CreateClosedBoxFrame() {
        int[] frontCorners = { 0, 27, 783, 756 };
        int[] backCorners = { 0, 9, 99, 90 }; 

        for (int i = 0; i < 4; i++) {
            int next = (i + 1) % 4;
            // The cage structure 
            CreateStaticLine(inputLayer[frontCorners[i]].transform.position, hiddenLayer[backCorners[i]].transform.position);
            CreateStaticLine(inputLayer[frontCorners[i]].transform.position, inputLayer[frontCorners[next]].transform.position);
            CreateStaticLine(hiddenLayer[backCorners[i]].transform.position, hiddenLayer[backCorners[next]].transform.position);
        }
    }

    void CreateStaticLine(Vector3 start, Vector3 end) {
        GameObject frameObj = new GameObject("BoxEdge");
        frameObj.transform.SetParent(this.transform); 
        LineRenderer lr = frameObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.positionCount = 2;
        
        // Outer cage 
        lr.widthMultiplier = outerFrameThickness; 
        lr.useWorldSpace = false;
        lr.SetPosition(0, transform.InverseTransformPoint(start));
        lr.SetPosition(1, transform.InverseTransformPoint(end));
        
        lr.startColor = themeColor * 0.4f;
        lr.endColor = themeColor * 0.4f;
    }

    List<GameObject> CreateLayer(int w, int h, float z, float gap) {
        List<GameObject> nList = new List<GameObject>();
        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                Vector3 pos = new Vector3(x - w/2f, y - h/2f, z) * gap;
                GameObject n = Instantiate(neuronPrefab, pos, Quaternion.identity, transform);
                nList.Add(n);
            }
        }
        return nList;
    }
}
