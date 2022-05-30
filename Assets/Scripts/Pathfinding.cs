using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATrinity
{
    // A-star pathfinding method for triangle grid

    public class Pathfinding
    {
        private TriangleGrid<PathNode> grid;
        private List<PathNode> openList;
        private List<PathNode> closedList;

        public TriangleGrid<PathNode> GetGrid() => grid;

        public Pathfinding(int range, float edge)
        {
            grid = new TriangleGrid<PathNode>(range, edge, Vector3.zero, (int a, int b, int c) => new TriangleCell<PathNode>(a,b,c));
        }

        public List<PathNode> FindPath(int startA, int startB, int startC, int endA, int endB, int endC)
        {
            PathNode startNode = grid.GetValue(startA, startB, startC);
            PathNode endNode = grid.GetValue(endA, endB, endC);

            openList = new List<PathNode> { startNode };
            closedList = new List<PathNode>();

            foreach(TriangleCell<PathNode> cell in grid.cellArray)
            {
                PathNode node = cell.value;
                node.g = int.MaxValue;
                node.CalculateF();
                node.prevNode = null;
            }

            startNode.g = 0;
            startNode.h = StepDistance(startNode, endNode);
            startNode.CalculateF();

            while(openList.Count > 0)
            {
                PathNode currentNode = GetLowestF(openList);
                if(currentNode == endNode)
                {
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach(PathNode neighNode in GetNeighbourList(currentNode))
                {
                    if (closedList.Contains(neighNode)) continue;

                    int gCost = currentNode.g + 1;
                    if(gCost < neighNode.g)
                    {
                        neighNode.prevNode = currentNode;
                        neighNode.g = gCost;
                        neighNode.h = StepDistance(neighNode, endNode);
                        neighNode.CalculateF();

                        if (!openList.Contains(neighNode)) openList.Add(neighNode);
                    }
                }
            }

            return null;
        }

        private int StepDistance(PathNode node1, PathNode node2)
        {
            return Mathf.Abs(node1.a - node2.a) + Mathf.Abs(node1.a - node2.a) + Mathf.Abs(node1.a - node2.a);
        }

        private List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);
            PathNode currentNode = endNode;
            while(currentNode.prevNode != null)
            {
                path.Add(currentNode.prevNode);
                currentNode = currentNode.prevNode;
            }
            path.Reverse();
            return path;
        }

        private List<PathNode> GetNeighbourList(PathNode node)
        {
            List<PathNode> neighbourList = new List<PathNode>();
            if(node.a + node.b + node.c == 2) //If peak points up
            {
                neighbourList.Add(GetNode(node.a - 1, node.b, node.c));
                neighbourList.Add(GetNode(node.a, node.b - 1, node.c));
                neighbourList.Add(GetNode(node.a, node.b, node.c - 1));
            }
            else //If peak points down
            {
                neighbourList.Add(GetNode(node.a + 1, node.b, node.c));
                neighbourList.Add(GetNode(node.a, node.b + 1, node.c));
                neighbourList.Add(GetNode(node.a, node.b, node.c + 1));
            }

            return neighbourList;
        }

        private PathNode GetNode(int a, int b, int c)
        {
            return grid.GetValue(a, b, c);
        }

        private PathNode GetLowestF(List<PathNode> pathNodes)
        {
            PathNode lowcostNode = pathNodes[0];
            foreach (PathNode node in pathNodes)
            {
                if (node.f < lowcostNode.f) lowcostNode = node;
            }
            return lowcostNode;
        }
    }


    public class PathNode
    {
        private TriangleGrid<PathNode> grid;
        public Vector3 center { get; private set; }
        public int a, b, c;
        public int g, h, f;

        public PathNode prevNode; // Previous Node

        public PathNode(TriangleGrid<PathNode> grid, int a, int b, int c)
        {
            this.grid = grid;
            this.a = a;
            this.b = b;
            this.c = c;

            center = GetCellCenter();
        }

        public void CalculateF() => f = g + h;

        public override string ToString()
        {
            return a + ":" + b + ":";
        }

        private Vector3 GetCellCenter()
        {
            float x = (a - c) * 0.5f;
            float y = -Mathf.Sqrt(3) / 6 * a + Mathf.Sqrt(3) / 3 * b - Mathf.Sqrt(3) / 6 * c;

            return new Vector3(x, y) * grid.edge;
        }
    }
}
