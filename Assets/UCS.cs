using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class UCS : MonoBehaviour
{
public Transform seeker, target;
	Grid grid;


	void Awake() {
		grid = GetComponent<Grid> ();
	}

	void Update() {
		FindPath (seeker.position, target.position);
	}


	void FindPath(Vector3 startPos, Vector3 targetPos) {
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		Stopwatch sw = new Stopwatch();
		sw.Start();

		Queue<Node> openSet = new Queue<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Enqueue(startNode);

		while (openSet.Count > 0) {

			Node node = openSet.Peek();
			openSet.Dequeue();
        
			closedSet.Add(node);

			if (node == targetNode) {
				sw.Stop();
				print ("Using UCS, the path is found in: " + sw.ElapsedMilliseconds + " ms");
				print ("Number of nodes in the fringe at goal: "  + openSet.Count);
				print ("Number of expanded nodes: "  + closedSet.Count);

				RetracePath(startNode,targetNode);
				return;
			}
            
            foreach (Node neighbour in grid.GetNeighbours(node)) {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newCostToNeighbour;
					neighbour.parent = node;

                if(!openSet.Contains(neighbour)){
                    neighbour.parent = node;
				    openSet.Enqueue(neighbour);
                }      
			}
		}	
	}

	void RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();
		grid.path_UCS = path;
	}

}
int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}

}