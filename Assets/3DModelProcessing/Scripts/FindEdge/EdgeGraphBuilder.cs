using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine;
using ThreeDModelProcessing;
public class EdgeGraphBuilder : MonoBehaviour
{
    public EdgeReader edgeReader;
    public EdgeRawData rawData;
    public SimplifyModelData simplifyModelData;


    // Start is called before the first frame update
    void Start()
    {
        
        edgeReader = GetComponent<EdgeReader>();
        InitGraphBuilder();
    }

    void InitGraphBuilder()
    {
        rawData = edgeReader.edgeList;
        simplifyModelData = new SimplifyModelData(rawData);
    }

    void StartBuild()
    {
        StartCoroutine(_StartBuild());
    }

    IEnumerator _StartBuild()
    {
        InitGraphBuilder();
        int edgeNumber = rawData.getEdgeNumber;
        Stopwatch system_timer = new Stopwatch();
        float nextTime = system_timer.ElapsedMilliseconds + 500;
        system_timer.Start();
        //buil edge iterator 
        for (int i = 0; i < edgeNumber; i++)
        {
            Vector2Int edge = rawData.getEdge(i);
            simplifyModelData.AddEdge(edge.x, edge.y);
            // print("Add Edge : " + edge.x +", " +edge.y);
            if (system_timer.ElapsedMilliseconds > nextTime)
            {
                nextTime = system_timer.ElapsedMilliseconds + 500f;
                yield return new WaitForEndOfFrame();
            }
        }
        simplifyModelData.edgeGraph.OutputGraph(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + "EdgeGraph.txt");


    }
    void OnGUI(){
        Rect btn = new Rect(50, 200, 150, 50);
        if (GUI.Button(btn, "Build Edge")){
            StartCoroutine(_StartBuild());
        }
    }
}
