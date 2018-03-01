using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonTester : MonoBehaviour {
    public Color color;
    void setColor(Color c)
    {
        color = new Color(c.r,c.g,c.b);
        //Debug.Log("1: "+color);
    }
    void TheStart(Vector2[] vertices2D)
    {
        // Create Vector2 vertices
       /* Vector2[] vertices2D = new Vector2[] {
            new Vector2(0,0),
            new Vector2(0,50),
            new Vector2(50,50),
       
        };*/

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();
       

         // Set up game object with mesh;
        gameObject.AddComponent(typeof(MeshRenderer));

        Renderer rend = GetComponent<Renderer>();
        //rend.material.shader = Shader.Find("Specular");
        //rend.material.SetColor("_EmissionColor", Color.blue);
        rend.material.shader = Shader.Find("UI/Default");
        //Debug.Log("2: "+color);
        rend.material.color = color;


        GetComponent<Renderer>().sortingLayerName = "Polygon";
        //GetComponent<Renderer>().sortingOrder = SortingLayer.GetLayerValueFromName("Polygon");
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
    }
    private void Start()
    {
        
    }
}
