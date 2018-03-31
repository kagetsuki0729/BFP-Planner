using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PolygonTester : MonoBehaviour {
    public Color color;
    public Vector2[] vertice;
    float arc=0;
    void setColor(Color c)
    {
        color = new Color(c.r,c.g,c.b);
        //Debug.Log("1: "+color);
    }
    void TheStart(Vector2[] vertices2D)
    {
        vertice = vertices2D;
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
    void transformation(float[] trans) {
        //Debug.Log("Rotate " + trans[2] * Mathf.Rad2Deg + " degrees");
        for(int i=0;i< vertice.Length;i++) {
            Vector2 newV;
            Vector2 vec = new Vector2(vertice[i].x - transform.position.x, vertice[i].y - transform.position.y);
            if (trans.Length == 3)
            {
                newV.x = transform.position.x + vec.x * (float)Math.Cos(trans[2] - arc) - vec.y * (float)Math.Sin(trans[2] - arc) + trans[0];
                newV.y = transform.position.y + vec.x * (float)Math.Sin(trans[2] - arc) + vec.y * (float)Math.Cos(trans[2] - arc) + trans[1];
            }
            else {
                newV.x =transform.position.x+ vec.x + trans[0];
                newV.y =transform.position.y+ vec.y + trans[1];
            }
            vertice[i] = newV;
            //Debug.Log("transform (" + vertice[i].x + "," + vertice[i].y + ")    "+this.transform.position+" "+trans[2]*Mathf.Rad2Deg);
        }
        if(trans.Length==3)
            arc = trans[2];
    }
  
}
