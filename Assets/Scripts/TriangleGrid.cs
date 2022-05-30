using System;
using System.Collections.Generic;
using UnityEngine;
using CrabStuff;

namespace ATrinity
{
    public class TriangleGrid<TCellObject>
    {
        private int range; // Grid range, half of width
        public float edge { get; private set; } // Side of triangle cell
        public Vector3 startPosition { get; private set; }

        public int cellAmount { get; private set; }
        public TriangleCell<TCellObject>[] cellArray { get; private set; }
        private (int, int, int, TriangleCell<TCellObject>)[] gridArray;

        private bool debug = true;

        public event EventHandler<OnValueChangedEventArgs> OnValueChanged;
        public class OnValueChangedEventArgs : EventArgs
        {
            public int index;
            public int a, b, c;
        }


        public TriangleGrid(int range, float edge, Vector3 startPosition, Func<int,int,int, TriangleCell<TCellObject>> createCellObject)
        {
            this.range = range;
            this.edge = edge;
            this.startPosition = startPosition;

            cellAmount = CellCount(range);
            gridArray = new (int, int, int, TriangleCell<TCellObject>)[cellAmount + 1];
            cellArray = new TriangleCell<TCellObject>[cellAmount + 1];

            int index = 0;
            for (int a = -range; a < range; a++)
            {
                for (int b = -range; b < range; b++)
                {
                    for (int c = -range; c < range; c++)
                    {
                        if (a + b + c == 1 || a + b + c == 2)
                        {
                            TriangleCell<TCellObject> cell = new TriangleCell<TCellObject>(a, b, c, edge);
                            cell.value = default(TCellObject);

                            cellArray[index] = cell;
                            gridArray[index++] = (a, b, c, cell);

                        }
                    }
                }
            }
            if(debug)
            GridCycle();
           
        }

        private void GridCycle()
        {
            foreach(var cell in cellArray)
            {
                cell.valueText = TextHelper.WorldText(cell.value.ToString(), Color.black, 20, cell.center + startPosition);
                Debug.DrawLine(cell.peaks[1] + startPosition, cell.peaks[0] + startPosition, Color.white, 100f);
                Debug.DrawLine(cell.peaks[1] + startPosition, cell.peaks[2] + startPosition, Color.white, 100f);
                Debug.DrawLine(cell.peaks[2] + startPosition, cell.peaks[0] + startPosition, Color.white, 100f);
            }
        }

        private int CellCount(int range)
        {
            int count = 0;
            for (int a = -range; a < range; a++)
            {
                for (int b = -range; b < range; b++)
                {
                    for (int c = -range; c < range; c++)
                    {
                        if (a + b + c == 1 || a + b + c == 2) count++;
                    }
                }
            }
            return count;
        }

        public void SetValue(int a, int b, int c, TCellObject value)
        {
            int index = 0;

            foreach(var cell in gridArray)
            {
                if (cell.Item1 == a)
                    if (cell.Item2 == b)
                        if (cell.Item3 == c)
                        {
                            cell.Item4.value = value;
                            cell.Item4.valueText.text = value.ToString();
                            OnValueChanged(this, new OnValueChangedEventArgs { index = index, a = a, b = b, c = c });
                        }
                index++;
            }
        }

        public void SetValue(Vector3 position, TCellObject value)
        {
            int a, b, c;
            GetTriangleCell(position - startPosition, out a, out b, out c);
            SetValue(a, b, c, value);
        }

        public TCellObject GetValue(int a, int b, int c)
        {
            foreach (var cell in gridArray)
            {
                if (cell.Item1 == a)
                    if (cell.Item2 == b)
                        if (cell.Item3 == c)
                            return cell.Item4.value;
            }
                        
            return default(TCellObject);
        }

        public TCellObject GetValue(Vector3 position)
        {
            int a, b, c;
            GetTriangleCell(position - startPosition, out a, out b, out c);
            return GetValue(a, b, c);
        }

        public TCellObject GetValue(int index) => cellArray[index].value;

        public void GetTriangleCell(float x, float y, out int a, out int b, out int c)
        {
            double aD = (1 * x - Mathf.Sqrt(3) / 3 * y) / edge;
            double bD = ((Mathf.Sqrt(3) * 2 / 3 * y) / edge) + 1;
            double cD = (-1 * x - Mathf.Sqrt(3) / 3 * y) / edge; ;

            a = (int)Math.Ceiling(aD);
            b = (int)Math.Floor(bD);
            c = (int)Math.Ceiling(cD);
        }

        public void GetTriangleCell(Vector3 position, out int a, out int b, out int c)
        {
            GetTriangleCell(position.x, position.y, out a, out b, out c);
        }
    }

    public class TriangleCell<TCellObject>
    {
        private int a;
        private int b;
        private int c;

        public int A => a;
        public int B => b;
        public int C => c;

        public TCellObject value;
        public TextMesh valueText;

        private float edge;
        private float hight;
        public Vector3 center { get; private set; }
        public bool topUp { get; private set; }

        public Vector3[] peaks { get; private set; }

        public TriangleCell(int a, int b, int c, float edge = 1)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.edge = edge;

            this.hight = edge * Mathf.Sqrt(3) / 2;

            topUp = a + b + c == 2;
            center = GetCellCenter();
            peaks = GetCellPeaks();
        }

        private Vector3[] GetCellPeaks()
        {
            Vector3[] peaks = new Vector3[3];

            float hPeace = hight / 3;

            if (topUp)
            {
                peaks[0] = center + new Vector3(-edge / 2, -hPeace);
                peaks[1] = center + new Vector3(0, hPeace * 2);
                peaks[2] = center + new Vector3(edge / 2, -hPeace);
            }
            else
            {
                peaks[0] = center + new Vector3(-edge / 2, hPeace);
                peaks[1] = center + new Vector3(0, -hPeace * 2);
                peaks[2] = center + new Vector3(edge / 2, hPeace);
            }

            return peaks;
        }

        private Vector3 GetCellCenter()
        {
            float x = (a - c) * 0.5f;
            float y = -Mathf.Sqrt(3) / 6 * a + Mathf.Sqrt(3) / 3 * b - Mathf.Sqrt(3) / 6 * c;

            return new Vector3(x, y) * edge;
        }
    }

}
