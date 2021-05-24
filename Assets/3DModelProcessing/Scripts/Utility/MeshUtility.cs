using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ThreeDModelProcessing.Utility
{
    public class MeshUtility
    {
        public static void MeshData(Mesh mesh){
            Debug.Log("Dot product" + Vector3.Dot(new Vector3(0, 0, 0), new Vector3(0, 1, 0)));
            Debug.Log(String.Join(",", mesh.vertices));
            Debug.Log(String.Join(",", mesh.triangles));
        } 
        public static int GetTrianglesArrayLength(Mesh mesh)
        {
            return mesh.triangles.Length / 3;
        }
        public static Vector3 GetVertex(Mesh mesh, int index)
        {
            return mesh.vertices[index];
        }

        public static int[] GetTriangle(Mesh mesh, int index)
        {
            return new int[]{
            mesh.triangles[index*3],
            mesh.triangles[index*3+1],
            mesh.triangles[index*3+2]};
        }

        public static Vector3[] GetTriangleVertex(Mesh mesh, int[] indices)
        {
            return new Vector3[]{
            mesh.vertices[indices[0]],
            mesh.vertices[indices[1]],
            mesh.vertices[indices[2]]};
        }

        public static Vector3[] GetTriangleNormalCollection(Mesh mesh, int length)
        {
            Vector3[] triangleNormal = new Vector3[length];
            //traverse all the triangle
            for (int i = 0; i < length; i++)
            {
                //calculate triangle data
                int[] triangleIndex = GetTriangle(mesh, i);
                Vector3[] triangleVertex = GetTriangleVertex(mesh, triangleIndex);
                triangleNormal[i] = GetTriangleNormal(mesh, triangleVertex);
                // print(triangleNormal[i] .normalized);
            }
            return triangleNormal;
        }

        public static Vector3 GetTriangleNormal(Mesh mesh, Vector3[] vertices)
        {
            // print("============GetTriangleNormal Input Vector===========");
            // print(vertices[0]*10000);
            // print(vertices[1]*10000);
            // print(vertices[2]*10000);
            Vector3 vert1 = vertices[1] * 10000 - vertices[0] * 10000;
            Vector3 vert2 = vertices[2] * 10000 - vertices[0] * 10000;
            // print("============substract===========");
            // print(vert1);
            // print(vert2);
            Vector3 crossProduct = Vector3.Cross(vert1.normalized, vert2.normalized).normalized;
            // print("Cross"+crossProduct);
            return crossProduct;
        }

        public static Vector3 GetTriangleCenter(Mesh mesh, Vector3[] vertices)
        {
            Vector3 center = Vector3.zero;
            foreach (var v in vertices)
            {
                center += v / 3;
            }
            return center;

        }

        public static Mesh GenerateProceduralCube()
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[]{
            new Vector3(1,-1, 1),
            new Vector3(1,-1,-1),
            new Vector3(1, 1,-1),
            new Vector3(1, 1, 1),
            new Vector3(-1, -1, 1),
            new Vector3(-1, -1, -1),
            new Vector3(-1, 1, -1),
            new Vector3(-1, 1, 1)
            };
            int[] triangle = new int[]{
            4,0,3,
            4,3,7,
            0,1,2,
            0,2,3,
            1,5,6,
            1,6,2,
            5,4,7,
            5,7,6,
            7,3,2,
            7,2,6,
            0,5,1,
            0,4,5
            };
            mesh.vertices = vertices;
            mesh.triangles = triangle;

            return mesh;

        }

    }

}
