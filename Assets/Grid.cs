using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {


	public bool displayGridGizmos;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Awake() {
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		CreateGrid();
	}

	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

	void CreateGrid() {
		grid = new Node[gridSizeX,gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
				grid[x,y] = new Node(walkable,worldPoint, x,y);
			}
		}
	}

	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}
	
		public List<Node> GetNeighbours_Manhattan(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0 || x == -1 && y == -1 || x == -1 && y == 1 || x == 1 && y == -1 || x == 1 && y == 1)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}
	
	

	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}

	public List<Node> path_euclidian;
	public List<Node> path_manhattan;
	public List<Node> path_DFS;
	public List<Node> path_BFS;
	public List<Node> path_UCS;

	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));

		if (grid != null) {
			foreach (Node n in grid) {
				Gizmos.color = (n.walkable)?Color.white:Color.grey;

				if (path_DFS != null)
					if (path_DFS.Contains(n))
						Gizmos.color = Color.yellow; //COLOR HERE
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));

				if (path_euclidian != null)
					if (path_euclidian.Contains(n))
						Gizmos.color = Color.blue; //COLOR HERE
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));

				if (path_manhattan != null)
					if (path_manhattan.Contains(n))
						Gizmos.color = Color.black; //COLOR HERE
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));

				if (path_BFS != null)
					if (path_BFS.Contains(n))
						Gizmos.color = Color.green; //COLOR HERE
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));

				if (path_UCS != null)
					if (path_UCS.Contains(n))
						Gizmos.color = Color.red; //COLOR HERE
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));


			}
		}
	}
}