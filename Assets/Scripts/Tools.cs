using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Some custom tools for easy work

namespace CrabStuff {

    // Class for work with mouse  
    public class Pointer
    {
        public static Vector3 GetPointerWorldPosition2D()
        {
            return GetPointerWorldPosition2D(Camera.main);
        }

        public static Vector3 GetPointerWorldPosition2D(Camera camera)
        {
            Vector3 v3 = GetPointerWorldPosition3D(Input.mousePosition, camera);
            v3.z = 0f;
            return v3;
        }

        public static Vector3 GetPointerWorldPosition3D()
        {
            return GetPointerWorldPosition3D(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetPointerWorldPosition3D(Camera camera)
        {
            return GetPointerWorldPosition3D(Input.mousePosition, camera);
        }

        public static Vector3 GetPointerWorldPosition3D(Vector3 screenPos, Camera camera)
        {
            Vector3 worldPos = camera.ScreenToWorldPoint(screenPos);
            return worldPos;
        }
    }

    // Class for work with text
    public class TextHelper
    {
        public static TextMesh WorldText(string text, Color color, int fontSize = 30, Vector3 localPosition = default,  Transform parent = null,  TextAnchor textAnchor = TextAnchor.MiddleCenter)
        {
            GameObject gameObject = new GameObject("WorldText", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;

            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            return textMesh;
        }

    }

    // Class for handle vertices, uv, triangles arrays
    public class MeshArrays
    {
        public Vector3[] vertices;
        public Vector2[] uv;
        public int[] triangles;

        public MeshArrays(Vector3[] vertices, Vector2[] uv, int[] triangles)
        {
            this.vertices = vertices;
            this.uv = uv;
            this.triangles = triangles;
        }

        public void SetToMesh(Mesh mesh)
        {
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
    }
}

