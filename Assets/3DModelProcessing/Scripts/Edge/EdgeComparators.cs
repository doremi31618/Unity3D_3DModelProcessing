using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// using OCJ Algorithmm 
/// reference from : https://www.researchgate.net/publication/320723538_An_Efficient_Query_Algorithm_for_Trajectory_Similarity_Based_on_Frechet_Distance_Threshold
/// </summary>
namespace ThreeDModelProcessing.Edge
{
    public class EdgeComparators : MonoBehaviour
    {
        public EdgeData model_1;
        public EdgeData model_2;
        public float threshold;


        void CompareEdgeSimilarity(List<Vector3> targetEdge, List<Vector3> candidateEdge)
        {

            float targetLength = Vector3.Distance(targetEdge[0], targetEdge[targetEdge.Count - 1]);
            float candidateLength = Vector3.Distance(candidateEdge[0], candidateEdge[candidateEdge.Count - 1]);
        }
        Vector3 FindFarestPointFromLine(Vector3 line_point_1, Vector3 line_point_2, List<Vector3> points)
        {
            Vector3 farPoint = Vector3.zero;
            float farestDistance = -1;
            foreach (var p in points)
            {
                float distance = fromLineToPoint(line_point_1, line_point_2, p).magnitude;
                if (distance > farestDistance)
                {
                    farestDistance = distance;
                    farPoint = p;
                }
            }
            return farPoint;
        }

        /// <summary>
        ///   ^       p3(mid)
        ///   |       ^ ^
        ///   |      /  | 
        ///   |     /   |  
        ///   | v1 /  v4|   
        ///   |   /     |    
        ///   |  /  v3  |  V2    
        ///  p1 -------->-----> p2
        /// </summary>
        /// <param name="line_poin_1"></param>
        /// <param name="line_point_2"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        Vector3 fromLineToPoint(Vector3 line_poin_1, Vector3 line_point_2, Vector3 point)
        {
            Vector3 v1 = point - line_poin_1;
            Vector3 v2 = line_point_2 - line_poin_1;
            Vector3 v3 = Vector3.Dot(v1.normalized, v2.normalized) * v1.magnitude * v2.normalized;
            Vector3 v4 = v1 - v3;
            return v4;
        }

        /// <summary>
        /// (0,1,0)
        ///   ^       p3(mid)
        ///   |      /  | \
        ///   |     /   |  \
        ///   | v1 /  v4|   \ 
        ///   |   /     |    \
        ///   |  /  v3  V2    \
        ///  p1 ------------ p2(tail)------>(1,0,0)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="scale">Range from 0 ~ 1</param>
        /// <returns></returns>
        Matrix4x4 transformToNormalSpace(Vector3 p1, Vector3 p2, Vector3 p3, float scale)
        {

            //get transform matrix 

            Vector3 v2 = p2 - p1;
            Vector3 X_axis = v2.normalized;
            Vector3 Y_axis = fromLineToPoint(p1, p2, p3).normalized;
            Vector3 Z_axis = Vector3.Cross(X_axis, Y_axis).normalized;

            Matrix4x4 matrix = Matrix4x4.identity;
            matrix[0, 0] = matrix[1, 1] = matrix[2, 2] = scale * (1.0f / v2.magnitude);

            //creating rotation matrix 
            Matrix4x4 rotMatrix = Matrix4x4.identity;
            rotMatrix.SetColumn(0, new Vector4(X_axis.x, X_axis.y, X_axis.z, 0));
            rotMatrix.SetColumn(1, new Vector4(Y_axis.x, Y_axis.y, Y_axis.z, 0));
            rotMatrix.SetColumn(2, new Vector4(Z_axis.x, Z_axis.y, Z_axis.z, 0));
            rotMatrix.SetColumn(3, new Vector4(0, 0, 0, 1));
            rotMatrix = rotMatrix.inverse;

            return rotMatrix * matrix;
        }

        List<Vector3> TanslateFirstTransformByMatrix(Matrix4x4 matrix, List<Vector3> points)
        {
            if (points.Count == 0)
                return points;

            Vector3 head = points[0];
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = points[i] - head;
            }
            

            for (int i = 0; i < points.Count; i++)
            {
                Vector4 newPoint = matrix * new Vector4(points[i] .x, points[i] .y, points[i] .z, 1);//
                points[i] = new Vector3(newPoint.x, newPoint.y, newPoint.z);
            }

            

            return points;
        }
        public List<Vector3> TransformEdgeToNormalSpace(List<Vector3> selectedEdge)
        {
            Vector3 farestPoint = FindFarestPointFromLine(selectedEdge[0], selectedEdge[selectedEdge.Count - 1], selectedEdge);
            Matrix4x4 transformMatrix = transformToNormalSpace(selectedEdge[0], selectedEdge[selectedEdge.Count - 1], farestPoint, 1);
            List<Vector3> transformEdge = TanslateFirstTransformByMatrix(transformMatrix, selectedEdge);
            return transformEdge;
        }


        /// <summary>
        /// The input of two trajectory should transform to normal space (0 ~ 1)
        /// </summary>
        /// <param name="targetTrajectory"></param>
        /// <param name="candidateTrajectory"></param>
        /// <returns></returns>
        public bool OCJSearch(List<Vector3> targetTrajectory, List<Vector3> candidateTrajectory)
        {
            int targetLength = targetTrajectory.Count;
            int candinateLength = candidateTrajectory.Count;
            
            List<List<int>> link = new List<List<int>>();
            for (int i = 0; i < targetLength; i++)
            {
                link.Add(new List<int>());
                for (int j = 0; j < candinateLength; j++)
                {
                    if (LessThanThreshold(targetTrajectory[i], candidateTrajectory[j]))
                        link[i].Add(j);
                    else
                        break;
                }
                print(link[i].Count);
            }

            //check if the target (head, tail) pair could match condinate (head, tail) pair
            int finalIndex = link.Count - 1;
            // if (link[finalIndex].Count - 1 < 0){
            //     print("link[finalIndex].Count - 1");
            //     return false;
            // }
            // if (link[0][0] > 0 || (link[finalIndex][link[finalIndex].Count - 1] < candidateTrajectory.Count - 1))
            //     return false;

            //make sure there won't be any edge has large 
            for (int i = 1; i < link.Count - 1; i++)
            {
                int final_index_Of_Link_i_minus_1 = link[i - 1].Count - 1;
                if (link[i].Count == 0 || final_index_Of_Link_i_minus_1<0){
                    print("final_index_Of_Link_i_minus_1" + i);
                    return false;
                }
                if (link[i][0] > link[i - 1][final_index_Of_Link_i_minus_1])
                    return false;
            }
            return true;
        }

        bool LessThanThreshold(Vector3 target, Vector3 candidate)
        {
            print("target :"　+ target + " candidate :" +candidate);
            return (Vector3.Distance(target, candidate) < 1);
        }
        // public void StartTransform(){

        // }
        private void OnDrawGizmos()
        {
            if (model_1.selectedEdge.Count == 0) return;
            for (int i = 0; i < model_1.selectedEdge.Count; i++)
            {
                Gizmos.DrawSphere(model_1.selectedEdge[i], 0.01f);
            }

            for (int i = 0; i < model_2.selectedEdge.Count; i++)
            {
                Gizmos.DrawSphere(model_2.selectedEdge[i], 0.01f);
            }
        }
        private void OnGUI()
        {
            Rect butRect = new Rect(500, 50, 150, 50);
            if (GUI.Button(butRect, "Start Transform"))
            {
                if (model_1.selectedEdge.Count == 0) return;
                // print("distance from head to tail (before):" + Vector3.Distance(model_1.selectedEdge[0], model_1.selectedEdge[model_1.selectedEdge.Count - 1]));
                model_1.selectedEdge = TransformEdgeToNormalSpace(model_1.selectedEdge);
                model_2.selectedEdge = TransformEdgeToNormalSpace(model_2.selectedEdge);
                bool similarity = OCJSearch(model_1.selectedEdge, model_2.selectedEdge);
                print(similarity);
                // print("distance from head to tail (after):" + Vector3.Distance(model_1.selectedEdge[0], model_1.selectedEdge[model_1.selectedEdge.Count - 1]));
                // model_2.selectedEdge = TransformEdgeToNormalSpace(model_2.selectedEdge);
            }
        }
    }
}

