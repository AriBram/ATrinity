using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrabStuff;
using ATrinity;

public class GridMaker : MonoBehaviour
{
    public int gridRange = 5;
    public float cellEdge = 10f;

    public int walls = 10;

    private Pathfinding pathfinding;
    private TriangleGrid<PathNode> grid;
    private GridVisual visual;
   

    private bool firstTap = true;
    private int a, b, c;

    private void Start()
    {
        pathfinding = new Pathfinding(gridRange, cellEdge);
        grid = pathfinding.GetGrid();
        visual = GetComponent<GridVisual>();
        SetRandomWalls();
        visual.SetGrid(grid);

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Pointer.GetPointerWorldPosition2D();

            if (firstTap)
            {
                grid.GetTriangleCell(pos, out a, out b, out c);
                PathNode start = grid.GetValue(a, b, c);

                visual.CreateGridMesh();
                visual.SetMesh(start, new Vector2(0.5f, 0.5f));

                firstTap = false;
            }
            else
            {
                grid.GetTriangleCell(pos, out int a2, out int b2, out int c2);
                List<PathNode> path = pathfinding.FindPath(a, b, c, a2, b2, c2);

                if (path != null)
                {
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        //Debug.DrawLine(path[i].center, path[i + 1].center, Color.red, 20f);
                        visual.SetMesh(path[i + 1], new Vector2(0.4f, 0.4f));
                    }
                    visual.SetMesh(path[path.Count-1], new Vector2(0.5f, 0.5f));
                }
                firstTap = true;
            }
        }
    }

    private void SetRandomWalls()
    {
        for(int r = 0; r < walls; r++)
        {
            int index = Random.Range(0, grid.cellAmount);
            grid.cellArray[index].value.isWall = true;
        }
    }

}
