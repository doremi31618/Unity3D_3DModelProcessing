using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleData{
    public Vector3[] vertices;
    public Vector3 normal;
    public Vector3 center;

    public TriangleData(Vector3[] _vertices, Vector3 _normal){
        vertices = _vertices;
        normal = _normal;
        foreach(var v in vertices){
            center += v/3;
        }
    }
}


/// <summary>
/// It can be optimize by removing duplicate vertex and triangles 
/// /// </summary>
public class SurfaceData
{
    //Use in Surface Splitor
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector3> normals;
    public List<Vector2> UVs;

    //Use in Surface Data Manager
    public int triangleCount = 0;
    public Vector3 averageNormal = Vector3.zero;
    public Vector3[] verticesArray;
    public int[] trianglesArray;
    public Vector3[] normalsArray;
    public Vector2[] UVsArray;

    bool isTransform = false;

    public Vector3 getLastNormal
    {
        get
        {
            return normals[normals.Count - 1];
        }
    }



    int getTriangleCount
    {
        get
        {
            return triangles.Count;
        }
    }

    //constructor
    public SurfaceData(Vector3[] first_vertices, Vector2[] first_UV)
    {
        vertices = new List<Vector3>(first_vertices);
        UVs = new List<Vector2>(first_UV);
        triangles = new List<int>(new int[] { 0, 1, 2 });
        normals = new List<Vector3>();
        normals.Add(Vector3.Cross(first_vertices[1] - first_vertices[0], first_vertices[2] - first_vertices[0]));
    }
    public SurfaceData(Vector3[] first_vertices, int[] first_triangle, Vector2[] first_UV)
    {
        vertices = new List<Vector3>(first_vertices);
        UVs = new List<Vector2>(first_UV);
        triangles = new List<int>(first_triangle);
        normals = new List<Vector3>();
        normals.Add(Vector3.Cross(first_vertices[1] - first_vertices[0], first_vertices[2] - first_vertices[0]));
    }

    //Add Data function
    public void AddData(Vector3[] first_vertices,int[] first_triangle, Vector2[] first_UV)
    {
        UVs.AddRange(first_UV);
        vertices.AddRange(first_vertices);
        triangles.AddRange(first_triangle);
        normals.Add(Vector3.Cross(first_vertices[1] - first_vertices[0], first_vertices[2] - first_vertices[0]));
    }

    public void AddData(Vector3[] first_vertices, Vector2[] first_UV)
    {
        UVs.AddRange(first_UV);
        vertices.AddRange(first_vertices);
        triangles.AddRange(newTrianglesData(first_vertices.Length));
        normals.Add(Vector3.Cross(first_vertices[1] - first_vertices[0], first_vertices[2] - first_vertices[0]));
    }
    
    //Execute when generate surface GameObject
    public void TransformToArrayFormat(){
        verticesArray = vertices.ToArray();
        UVsArray = UVs.ToArray();
        trianglesArray = triangles.ToArray();
        normalsArray = normals.ToArray();

        vertices.Clear();
        UVs.Clear();
        triangles.Clear();
        normals.Clear();

        triangleCount = trianglesArray.Length / 3;
        isTransform = true;
    }

    //
    public TriangleData GetTriangleData(int index){
        if (!isTransform) return null;
        return new TriangleData(
            new Vector3[]{
                verticesArray[trianglesArray[index*3]],
                verticesArray[trianglesArray[index*3+1]],
                verticesArray[trianglesArray[index*3+2]],
            },
            normalsArray[index]
        );
    }

    //tools 
    int[] newTrianglesData(int num)
    {
        int[] _triangles = new int[num];
        int start_index = getTriangleCount;
        for (int i = 0; i < num; i++)
        {
            _triangles[i] = i + start_index;
        }
        return _triangles;
    }
}

[RequireComponent(typeof(MeshFilter))]
public class SurfaceSplitor : MonoBehaviour
{
    [Range(0.01f, 0.5f)] public float normalLength = 0.2f;
    [Range(0,360)] public int threadAngle = 90;
    public Material material;
    public GameObject[] surfaceHandle;
    int[] triangles;
    MeshFilter meshFilter;

    #region Model Process



    //this algorithm is for continuous triangle
    void SplitleContinuousSurface()
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        int[] triangles = meshFilter.mesh.triangles;
        Vector2[] uvs = meshFilter.mesh.uv;
        int trianglesCount = triangles.Length / 3;

        List<SurfaceData> surfaceDataCollection = new List<SurfaceData>();

        //create first surface 
        Vector2[] firstUV = new Vector2[] {
            uvs[triangles[0]],
                uvs[triangles[1]],
                uvs[triangles[2]]
            };
        Vector3[] firstVector = new Vector3[] { vertices[triangles[0]], vertices[triangles[1]], vertices[triangles[2]] };
        SurfaceData new_surface = new SurfaceData(firstVector, firstUV);
        surfaceDataCollection.Add(new_surface);

        for (int i = 1; i < trianglesCount; i++)
        {

            int index = i * 3;
            Vector3[] triPos = new Vector3[3];
            triPos[0] = vertices[triangles[index]];
            triPos[1] = vertices[triangles[index + 1]];
            triPos[2] = vertices[triangles[index + 2]];

            Vector2[] triUV = new Vector2[3]{
                uvs[triangles[index]],
                uvs[triangles[index+1]],
                uvs[triangles[index+2]]
            };

            SurfaceData last_surface = surfaceDataCollection[surfaceDataCollection.Count - 1];
            Vector3 normal = Vector3.Cross(triPos[1] - triPos[0], triPos[2] - triPos[0]);
            Vector3 lastNormal = last_surface.getLastNormal;
            last_surface.averageNormal += normal;

            //if the angle between two vector are more than 90 degree 
            if (Vector3.Dot(Vector3.Normalize(normal), Vector3.Normalize(lastNormal)) < 0f)
            {
                //Compute average normal
                last_surface.averageNormal /= last_surface.normals.Count;

                //build a new surface data
                SurfaceData _new_surface = new SurfaceData(triPos, triUV);
                surfaceDataCollection.Add(_new_surface);
            }
            //Add data to current surface data
            else
            {
                last_surface.AddData(triPos, triUV);
                
            }


        }


        surfaceHandle = new GameObject[surfaceDataCollection.Count];
        //Generate New SurfaceMesh
        for (int i = 0; i < surfaceHandle.Length; i++)
        {
            surfaceHandle[i] = new GameObject("Surface Handle");
            surfaceHandle[i].transform.parent = this.gameObject.transform;
            surfaceHandle[i].transform.localPosition = Vector3.zero;
            surfaceHandle[i].AddComponent<MeshFilter>().mesh = GenerateMesh(surfaceDataCollection[i]);
            surfaceHandle[i].AddComponent<MeshRenderer>().material = material;
            surfaceHandle[i].AddComponent<SurfaceDataManager>().Initialize(surfaceDataCollection[i]);
        }

        /*
        int last_id = 0;
        surfaceHandle = new List<GameObject>();
        //Generate New SurfaceMesh
        for (int i = 0; i < surfaceDataCollection.Count; i++)
        {

            if (surfaceHandle.Count == 0)
            {
                GameObject newSurfaceHandle = new GameObject("Surface Handle");
                newSurfaceHandle.transform.parent = this.gameObject.transform;
                newSurfaceHandle.transform.localPosition = Vector3.zero;
                newSurfaceHandle.AddComponent<MeshFilter>().mesh = GenerateMesh(surfaceDataCollection[i]);
                newSurfaceHandle.AddComponent<MeshRenderer>().material = material;
                newSurfaceHandle.AddComponent<SurfaceDataManager>().Initialize(surfaceDataCollection[i]);
                surfaceHandle.Add(newSurfaceHandle);
                continue;
            }


            Vector3 last_averageNormal = surfaceDataCollection[last_id].averageNormal;
            Vector3 curr_averageNormal = surfaceDataCollection[i].averageNormal;
            bool isNEWSurface = Vector3.Dot(last_averageNormal, curr_averageNormal) < 0;

            if (isNEWSurface)
            {
                GameObject newSurfaceHandle = new GameObject("Surface Handle");
                newSurfaceHandle.transform.parent = this.gameObject.transform;
                newSurfaceHandle.transform.localPosition = Vector3.zero;
                newSurfaceHandle.AddComponent<MeshFilter>().mesh = GenerateMesh(surfaceDataCollection[i]);
                newSurfaceHandle.AddComponent<MeshRenderer>().material = material;
                newSurfaceHandle.AddComponent<SurfaceDataManager>().Initialize(surfaceDataCollection[i]);
                surfaceHandle.Add(newSurfaceHandle);
                last_id += 1;
            }
            else
            {
                int last_mesh = surfaceHandle.Count - 1;
                CombineInstance[] combine = new CombineInstance[2];
                Mesh curr = new Mesh();
                curr = surfaceHandle[last_mesh].GetComponent<MeshFilter>().sharedMesh;
                combine[0].mesh = curr;
                combine[0].transform = surfaceHandle[last_mesh].transform.localToWorldMatrix;
                combine[1].mesh = GenerateMesh(surfaceDataCollection[i]);
                combine[1].transform = surfaceHandle[last_mesh].transform.localToWorldMatrix;
                
                surfaceHandle[last_mesh].GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            }
        }
        */
        
    }

    Mesh GenerateMesh(SurfaceData surface)
    {
        Mesh new_surface_mesh = new Mesh();
        surface.TransformToArrayFormat();
        new_surface_mesh.vertices = surface.verticesArray;
        new_surface_mesh.uv = surface.UVsArray;
        new_surface_mesh.triangles = surface.trianglesArray;

        //important
        new_surface_mesh.RecalculateNormals();
        new_surface_mesh.RecalculateBounds();
        return new_surface_mesh;
    }

    #endregion

    #region Unity function
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }
    void OnEnable()
    {
        // VisualizeTriangleNormal();
        GetComponent<MeshRenderer>().enabled = false;
        SplitleContinuousSurface();
    }
    void OnDisable()
    {
        GetComponent<MeshRenderer>().enabled = true;
        foreach(var gobj in surfaceHandle){
            DestroyImmediate(gobj);
        }
        surfaceHandle = null;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
}
