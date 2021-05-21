using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using MeshDecimator;
using MeshDecimator.Math;
using MeshDecimatorTool;

public delegate void BroadcastInfoHandler(string info);
/// <summary>
/// the mesh simplify algorithm is based on the " Fast Quadric Mesh Simplification algorithm "
/// and use the library from : https://github.com/Whinarn/UnityMeshSimplifier
/// </summary>
public class ProcessMeshSimplify : MonoBehaviour
{
    public event BroadcastInfoHandler simplifyInfo;
    public Task simplifyTask;
    [Range(0, 1)] public float quality = 0.05f;
    public float percentage = 0;
    public string info;
    public async void SimplifyObj(string sourcePath, string destPath, float quality)
    {
        simplifyTask = new Task(() =>
        {
            quality = MathHelper.Clamp01(quality);
            ObjMesh sourceObjMesh = new ObjMesh();
            sourceObjMesh.ReadFile(sourcePath);
            var sourceVertices = sourceObjMesh.Vertices;
            var sourceNormals = sourceObjMesh.Normals;
            var sourceTexCoords2D = sourceObjMesh.TexCoords2D;
            var sourceTexCoords3D = sourceObjMesh.TexCoords3D;
            var sourceSubMeshIndices = sourceObjMesh.SubMeshIndices;

            var sourceMesh = new MeshDecimator.Mesh(sourceVertices, sourceSubMeshIndices);
            sourceMesh.Normals = sourceNormals;

            if (sourceTexCoords2D != null)
            {
                sourceMesh.SetUVs(0, sourceTexCoords2D);
            }
            else if (sourceTexCoords3D != null)
            {
                sourceMesh.SetUVs(0, sourceTexCoords3D);
            }

            int currentTriangleCount = 0;
            for (int i = 0; i < sourceSubMeshIndices.Length; i++)
            {
                currentTriangleCount += (sourceSubMeshIndices[i].Length / 3);
            }

            int targetTriangleCount = (int)Math.Ceiling(currentTriangleCount * quality);
            string info_1 = string.Format("Input: {0} vertices, {1} triangles (target {2}) \n",
                sourceVertices.Length, currentTriangleCount, targetTriangleCount);

            // Console.WriteLine(info_1);
            simplifyInfo.Invoke(info_1);

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();

            var algorithm = MeshDecimation.CreateAlgorithm(Algorithm.Default);
            algorithm.Verbose = true;
            MeshDecimator.Mesh destMesh = MeshDecimation.DecimateMesh(algorithm, sourceMesh, targetTriangleCount);
            stopwatch.Stop();

            var destVertices = destMesh.Vertices;
            var destNormals = destMesh.Normals;
            var destIndices = destMesh.GetSubMeshIndices();

            ObjMesh destObjMesh = new ObjMesh(destVertices, destIndices);
            destObjMesh.Normals = destNormals;
            destObjMesh.MaterialLibraries = sourceObjMesh.MaterialLibraries;
            destObjMesh.SubMeshMaterials = sourceObjMesh.SubMeshMaterials;

            
            if (sourceTexCoords2D != null)
            {
                var destUVs = destMesh.GetUVs2D(0);
                destObjMesh.TexCoords2D = destUVs;
            }
            else if (sourceTexCoords3D != null)
            {
                var destUVs = destMesh.GetUVs3D(0);
                destObjMesh.TexCoords3D = destUVs;
            }

            destObjMesh.WriteFile(destPath);

            int outputTriangleCount = 0;
            for (int i = 0; i < destIndices.Length; i++)
            {
                outputTriangleCount += (destIndices[i].Length / 3);
            }

            float reduction = (float)outputTriangleCount / (float)currentTriangleCount;
            float timeTaken = (float)stopwatch.Elapsed.TotalSeconds;

            string info_2 = string.Format("Output: {0} vertices, {1} triangles ({2} reduction; {3:0.0000} sec) \n",
                destVertices.Length, outputTriangleCount, reduction, timeTaken);
            // Console.WriteLine(info_2);
            simplifyInfo.Invoke(info_2);
            simplifyInfo.Invoke("finish");
            DisposeTask();
        });
        simplifyTask.Start();
    }
    public void DisposeTask(){
        simplifyTask.Dispose();
    }

    void SimplifyMesh()
    {
        UnityEngine.Mesh sourceMesh = GetComponent<MeshFilter>().mesh;
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(sourceMesh);
        meshSimplifier.SimplifyMesh(quality);
        GetComponent<MeshFilter>().mesh = meshSimplifier.ToMesh();
    }
    // Start is called before the first frame update
    void Awake()
    {
        // SimplifyMesh();
    }
    void ExportObjFile()
    {
        // ObjExporter.MeshToFile(GetComponent<MeshFilter>(), Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Simplify");
    }

    //On Test use
    void OnGUI()
    {
        // Rect btn = new Rect(50, 50, 150, 50);
        // if (GUI.Button(btn, "Export model"))
        // {
        //     ExportObjFile();
        // }
    }
}
