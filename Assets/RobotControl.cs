using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class RobotControl : MonoBehaviour
{
    public static RobotControl Instance;
    public Vector2[] controlPoints;
    public int nPolygons;
    //public List<GameObject> polygonList = new List<GameObject>();
    public Vector2 goalPosition;
    public float goalArc;
    
    // Use this for initialization
    void Start()
    {
        Instance = this;

    }

    // Update is called once per frame
    bool isPicked;
    bool isRotated;
    //public float RotationSpeed = 500;
    void Update()
    {


        if (isPicked&&Input.GetMouseButtonUp(0))       
            isPicked = false;
        if (isRotated&&Input.GetMouseButtonUp(1))
            isRotated = false;
      

        if (isPicked)
        {

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position = mousePos;

            // if you want to smooth movement then lerp it

        }
        if (isRotated) {
            //Debug.Log("~ "+ Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime + " "+ Input.GetAxis("Mouse Y") * RotationSpeed * Time.deltaTime);
            transform.Rotate(0, 0,5 , Space.World);
        }

    }

    void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0))
            isPicked = true;
        if (Input.GetMouseButtonDown(1))
            isRotated = true;

    }
    
    public void setControlPoints(Vector2[] v)
    {
        controlPoints = (Vector2[])v.Clone();
    }
    public void setNPolygons(int n)
    {
        nPolygons = n;
    }
    public void setGoalPosition(Vector2 v)
    {
        goalPosition = v;
    }
    public void setGoalArc(float arc)
    {
        goalArc = arc;
    }

}
