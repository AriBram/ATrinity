using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrabStuff;
using ATrinity;

public class GridMaker : MonoBehaviour
{
    public int gridRange = 5;
    public float cellEdge = 7f;

    private TriangleGrid<int> grid;
    private GridVisual visual;
    private Pathfinding pathfinding;

    private void Start()
    {
        //grid = new TriangleGrid<int>(gridRange, cellEdge, Vector3.zero, (int a, int b , int c) => new TriangleCell<int>(a,b,c));

        //visual = GetComponent<GridVisual>();
        //visual.SetGrid(grid);

        pathfinding = new Pathfinding(gridRange, cellEdge);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Pointer.GetPointerWorldPosition2D();

            //int value = Mathf.Clamp(grid.GetValue(pos) + 20, 0, 100);
            //grid.SetValue(Pointer.GetPointerWorldPosition2D(), value);

            pathfinding.GetGrid().GetTriangleCell(Vector3.up, out int a1, out int b1, out int c1);
            pathfinding.GetGrid().GetTriangleCell(pos, out int a2, out int b2, out int c2);

            List<PathNode> path = pathfinding.FindPath(a1,b1,c1, a2, b2, c2);

            if(path != null)
            {
                for(int i = 0; i < path.Count-1; i++)
                {
                    Debug.DrawLine(path[i].center, path[i + 1].center, Color.green);
                }
            }
        }
    }
}
