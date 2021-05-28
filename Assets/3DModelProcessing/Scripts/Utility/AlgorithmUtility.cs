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

    /// <summary>
    /// Min Binary Heap
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T>
    {
        List<KeyValuePair<T, float>> element = new List<KeyValuePair<T, float>>();
        public PriorityQueue()
        {
            element = new List<KeyValuePair<T, float>>();
        }
        public PriorityQueue(T firstItem, float value)
        {
            element.Add(new KeyValuePair<T, float>(firstItem, value));
        }

        public int Count
        {
            get { return element.Count; }
        }

        public void Add(T newItem, float value)
        {
            //check if the item is repeated
            element.Add(new KeyValuePair<T, float>(newItem, value));

            int currentIndex = element.Count - 1;
            int nextIndex = ((currentIndex) - 1) / 2;
            while (nextIndex >= 0)
            {
                float nextPriority = element[nextIndex].Value;
                if (value < nextPriority)
                {
                    KeyValuePair<T, float> tempValue = new KeyValuePair<T, float>(element[nextIndex].Key, element[nextIndex].Value);
                    element[nextIndex] = element[currentIndex];
                    element[currentIndex] = tempValue;

                    currentIndex = nextIndex;
                    nextIndex = (nextIndex - 1) / 2;
                }
                else
                {
                    break;
                }
            }
        }

        public KeyValuePair<T, float> Dequeue()
        {

            KeyValuePair<T, float> first = new KeyValuePair<T, float>(element[0].Key, element[0].Value);
            int length = element.Count;
            int currentIndex = 0;

            element[0] = element[length - 1];
            element.RemoveAt(length - 1);

            while (currentIndex > length - 1)
            {
                int leftChild = currentIndex * 2 + 1;
                int rightChild = currentIndex * 2 + 2;

                //need to check if the child is exist
                if (leftChild < length - 1 && rightChild > length - 1)
                {
                    //if parent value > child value , swich them
                    if (element[currentIndex].Value > element[leftChild].Value)
                    {
                        KeyValuePair<T, float> tempValue = new KeyValuePair<T, float>(element[currentIndex].Key, element[currentIndex].Value);
                        element[leftChild] = element[currentIndex];
                        element[currentIndex] = tempValue;

                        currentIndex = leftChild;
                    }
                    else
                        break;
                }

                else if (leftChild > length - 1 && rightChild < length - 1)
                {
                    if (element[currentIndex].Value > element[rightChild].Value)
                    {
                        KeyValuePair<T, float> tempValue = new KeyValuePair<T, float>(element[currentIndex].Key, element[currentIndex].Value);
                        element[rightChild] = element[currentIndex];
                        element[currentIndex] = tempValue;

                        currentIndex = rightChild;
                    }
                    else
                        break;
                }

                else if (leftChild > length - 1 && rightChild > length - 1) break;

                if (element[leftChild].Value < element[rightChild].Value)
                {
                    //if parent value > child value , swich them
                    if (element[currentIndex].Value > element[leftChild].Value)
                    {
                        KeyValuePair<T, float> tempValue = new KeyValuePair<T, float>(element[currentIndex].Key, element[currentIndex].Value);
                        element[leftChild] = element[currentIndex];
                        element[currentIndex] = tempValue;

                        currentIndex = leftChild;
                    }
                    else
                        break;
                }
                else
                {
                    if (element[currentIndex].Value > element[rightChild].Value)
                    {
                        KeyValuePair<T, float> tempValue = new KeyValuePair<T, float>(element[currentIndex].Key, element[currentIndex].Value);
                        element[rightChild] = element[currentIndex];
                        element[currentIndex] = tempValue;

                        currentIndex = rightChild;
                    }
                    else
                        break;
                }
            }
            return first;
        }

    }

    public class AStartSearch
    {
        class Node
        {
            public Vector3 position;
            public Node parent_node;
            public float currentCost;
            public float estimateCost;
            public override string ToString()
            {
                string returnValue = string.Format("Position {0} Current Cost {1} Estimate Cost {2}", position, currentCost, estimateCost);
                return returnValue;
            }
        }
        public int MAX_STEPS = 10000;

        List<Vector3> finalPath;
        private Dictionary<Vector3, Node> allNode;
        public List<Vector3> getShortestPath { get { return finalPath; } }
        public AStartSearch(Vector3 startPoint, Vector3 destination, EdgeGraph graph)
        {
            // Debug.Log("Start AStar Algorithm");
            // Debug.Log("start from" + startPoint);
            // Debug.Log("destination" + destination);
            Node firstNode = new Node();
            finalPath = new List<Vector3>();
            allNode = new Dictionary<Vector3, Node>();

            firstNode.position = startPoint;
            firstNode.parent_node = null;
            firstNode.currentCost = 0;
            firstNode.estimateCost = PathCostEstimate(startPoint, destination);
            allNode.Add(startPoint, firstNode);
            
            PriorityQueue<Vector3> openList = new PriorityQueue<Vector3>(startPoint, firstNode.estimateCost);
            List<Vector3> closedList = new List<Vector3>();

            int stepCounter = 0;
            while (openList.Count != 0 || stepCounter > MAX_STEPS)
            {

                var nextItem = openList.Dequeue();
                closedList.Add(nextItem.Key);
                Node nextNode = allNode[nextItem.Key];
                // Debug.Log("Dequeue node [key] : " + nextItem.Key + " [Value] :" + nextItem.Value);
                // Debug.Log("next node [vertex] : " + nextNode.position + " [Value] : " + nextNode.estimateCost);
                if (nextNode.position == destination)
                {
                    // Debug.Log("Find destination : " + destination);
                    //find all path and update final Path
                    while (nextNode.parent_node != null)
                    {
                        finalPath.Add(nextNode.position);
                        nextNode = nextNode.parent_node;
                        if (nextNode == null)
                            break;
                    }
                    finalPath.Add(nextNode.position);
                        
                }
                else
                {
                    //search for adjacent node 
                    // Debug.Log("step index : " + stepCounter);
                    // Debug.Log("search vertex : " + nextNode.position);
                    var adjacentList = graph.getAdjacentVertexList(nextNode.position);
                    foreach (var vertex in adjacentList)
                    {

                        //chcek if node were already in open list -> skip
                        if (allNode.ContainsKey(vertex))
                        {
                            // Debug.Log("already in allNodes");
                            continue;
                        }

                        //check if they were in closed list -> skip
                        if (closedList.Contains(vertex))
                        {
                            // Debug.Log("already in ClosedList");
                            continue;
                        }

                        //if not -> create new node 
                        Node newNode = new Node();
                        // Debug.Log("add vertex to node: " + vertex);
                        newNode.position = vertex;
                        newNode.parent_node = nextNode;
                        newNode.currentCost = nextNode.currentCost + PathCostEstimate(nextNode.position, vertex);
                        newNode.estimateCost = PathCostEstimate(vertex, destination);
                        // Debug.Log(newNode);
                        allNode.Add(vertex, newNode);
                        openList.Add(vertex, newNode.estimateCost);
                    }
                }

                if (finalPath.Count != 0) break;
                stepCounter += 1;
            }
        }

        public virtual float PathCostEstimate(Vector3 startPoint, Vector3 destination)
        {
            float estimateCost = (int)Vector3.Distance(startPoint, destination);
            // Debug.Log("estimate cost : " + estimateCost);
            return estimateCost;
        }
    }
}

