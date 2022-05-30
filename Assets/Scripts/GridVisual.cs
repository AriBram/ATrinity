using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrabStuff;

// GridVisual class creates and updates Mesh what based on TriangleGrid
namespace ATrinity
{
    public class GridVisual : MonoBehaviour
    {
        private TriangleGrid<int> grid;
        private Mesh mesh;
        private LineRenderer line;

        private void Awake()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            line = GetComponent<LineRenderer>();
        }

        public void SetGrid(TriangleGrid<int> grid)
        {
            this.grid = grid;
            CreateGridMesh();

            grid.OnValueChanged += Grid_OnValueChanged;
        }

        private void Grid_OnValueChanged(object sender, TriangleGrid<int>.OnValueChangedEventArgs e)
        {
            UpdateGridMesh(e.index);
        }

        public void CreateGridMesh()
        {
            Vector3[] vertices = new Vector3[grid.cellAmount * 4];
            Vector2[] uv = new Vector2[grid.cellAmount * 4];
            int[] triangles = new int[grid.cellAmount * 9];

            MeshArrays arrays = new MeshArrays(vertices, uv, triangles);

            Vector2[] uvs = new Vector2[] { Vec2r(0), Vec2r(0), Vec2r(0), Vec2r(1) };

            for (int index = 0; index < grid.cellAmount; index++)
            {
                arrays = CellMesh(index, arrays, uvs);
            }

            arrays.SetToMesh(mesh);
        }

        private void UpdateGridMesh(int index)
        {
            MeshArrays arrays = new MeshArrays(mesh.vertices, mesh.uv, mesh.triangles);

            float value = grid.cellArray[index].value * 0.005f;
            arrays = CellMesh(index, arrays, SameUv(VecHalf(value)));
            arrays.SetToMesh(mesh);
        }

        // UV array with same values
        private Vector2[] SameUv(Vector2 uv) => new Vector2[] { uv, uv, uv, uv };
        // Vector2 with same x and y values
        private Vector2 Vec2r(float value) => new Vector2(value, value);

        // Vector2 with 0.5 on x
        private Vector2 VecHalf(float value) => new Vector2(0.5f, value);

        // Create MeshArrays with new UV values for specific cell
        private MeshArrays CellMesh(int index, MeshArrays arrays, Vector2[] uvs)
        {
            Vector3[] vertices = arrays.vertices;
            Vector2[] uv = arrays.uv;
            int[] triangles = arrays.triangles;
            TriangleCell<int> cell = grid.cellArray[index];

            //Debug.Log(index);
            int i = index * 4;
            int j = index * 9;
            

            vertices[i + 0] = cell.peaks[0] + grid.startPosition;
            vertices[i + 1] = cell.peaks[1] + grid.startPosition;
            vertices[i + 2] = cell.peaks[2] + grid.startPosition;
            vertices[i + 3] = cell.center + grid.startPosition;

            uv[i + 0] = uvs[0];
            uv[i + 1] = uvs[1];
            uv[i + 2] = uvs[2];
            uv[i + 3] = uvs[3];

            if (cell.topUp)
            {
                triangles[j + 0] = i + 0;
                triangles[j + 1] = i + 1;
                triangles[j + 2] = i + 3;

                triangles[j + 3] = i + 1;
                triangles[j + 4] = i + 2;
                triangles[j + 5] = i + 3;

                triangles[j + 6] = i + 2;
                triangles[j + 7] = i + 0;
                triangles[j + 8] = i + 3;
            }
            else
            {
                triangles[j + 0] = i + 0;
                triangles[j + 1] = i + 2;
                triangles[j + 2] = i + 3;

                triangles[j + 3] = i + 2;
                triangles[j + 4] = i + 1;
                triangles[j + 5] = i + 3;

                triangles[j + 6] = i + 1;
                triangles[j + 7] = i + 0;
                triangles[j + 8] = i + 3;
            }

            return new MeshArrays(vertices, uv, triangles);
        }
    }
   
}

