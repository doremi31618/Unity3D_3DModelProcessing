using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeDModelProcessing.Edge;
namespace ThreeDModelProcessing.Algorithm
{
    public class CommonAlgorithm
    {

        /// <summary>
        /// only find first same number in given two arrays
        /// </summary>
        /// <param name="arr1">given array1 </param>
        /// <param name="arr2">given array2 </param>
        /// <returns>return the same number</returns>
        static int FindSameNumberInGivenArrays(int[] arr1, int[] arr2)
        {
            if (arr1.Length <= arr2.Length)
            {
                var hashSet = new HashSet<int>(arr1);
                for (int i = 0; i < arr1.Length; i++)
                {
                    if (hashSet.Contains(arr2[i]))
                        return arr2[i];
                }
            }
            else
            {
                var hashSet = new HashSet<int>(arr2);
                for (int i = 0; i < arr2.Length; i++)
                {
                    if (hashSet.Contains(arr1[i]))
                        return arr1[i];
                }
            }


            //if can't find same number in given arrays , return -1 
            //(Beacause there won't be a negetive number in triangle index)
            return -1;
        }

    }

    public class AStartSearch{
        class Node{
            public Vector3 position;
            public Node parent_node;
            public int currentCost;
            public int estimateCost;
        }
        public int MAX_STEPS = 10000;
        public List<Vector3> finalPath;
        public AStartSearch(Vector3 startPoint, Vector3 destination, EdgeGraph graph){
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();

            Node firstNode = new Node();
            firstNode.position = startPoint;
            firstNode.parent_node = null;
            firstNode.currentCost = 0;
            firstNode.estimateCost = PathCostEstimate(startPoint, destination);

            int stepCounter = 0;
            while(openList.Count != 0 || stepCounter > MAX_STEPS){
                
                Node nextNode = openList[0];
                if (nextNode.position == destination){

                    //find all path and update final Path
                    while(nextNode.parent_node != null){
                        finalPath.Add(nextNode.position);
                        nextNode = nextNode.parent_node;
                        break;
                    }
                }
                else{
                    //search for adjacent node 
                    var adjacentList = graph.getAdjacentVertexList(nextNode.position);
                    foreach(var vertex in adjacentList){
                        
                        //chcek if they were already in open list -> estimate cost -> see if the cost smaller then origin 
                        // if (openList.Contains())
                        //check if they were in closed list -> skip

                        //if not -> create new node 

                        //estimate node cost

                        //add node to openlist

                        //rearange open list

                        //search next node
                    }
                }

                //remove node from open list , and add node to 

                stepCounter += 1;
            }
        }

        public virtual int PathCostEstimate(Vector3 startPoint, Vector3 destination){
            int estimateCost = (int)Vector3.Distance(startPoint, destination);
            Debug.Log("estimate cost : " + estimateCost);
            return estimateCost;
        }
    }
}

