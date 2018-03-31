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
    //public Vector2 goalPosition;
    //public float goalArc;
    public GameObject Goal=null;
    // Use this for initialization
    void Start()
    {
        Instance = this;

    }

    // Update is called once per frame
    bool isPicked;
    bool isRotated;
    //public float RotationSpeed = 500;
    Vector2 prevVec = new Vector2(0, 0);
    Vector2 clickVec = new Vector2(0, 0);
    void Update()
    {
       /* if(Goal!=null)
            Debug.Log("Goal " + Goal.transform.position);*/

        if (isPicked && Input.GetMouseButtonUp(0))
            isPicked = false;
        if (isRotated && Input.GetMouseButtonUp(1))
            isRotated = false;


        if (isPicked)
        {

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position = mousePos;

            // if you want to smooth movement then lerp it

            Debug.Log(this.name + " " + transform.position);

        }
        if (isRotated)
        {
            //Debug.Log("~ "+ Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime + " "+ Input.GetAxis("Mouse Y") * RotationSpeed * Time.deltaTime);
            //transform.Rotate(0, 0,5 , Space.World);
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 vec = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
            //Debug.Log("isRotated " + vec + ",  " + prevVec+ ",  " +clickVec);
            if (vec.x != prevVec.x || vec.y != prevVec.y)
            {
                double theta = Math.Atan2(vec.y,vec.x)-Math.Atan2(prevVec.y, prevVec.x);
                prevVec.x = vec.x;
                prevVec.y = vec.y;
                theta = theta * Mathf.Rad2Deg;
               // Debug.Log(theta+" "+ Math.Atan2(vec.y, vec.x)+" "+ Math.Atan2(prevVec.y, prevVec.x));
                transform.Rotate(0, 0, (float)theta,Space.World);
            }

        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
            isPicked = true;
        if (Input.GetMouseButtonDown(1))
        {
            isRotated = true;
            clickVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickVec.x = clickVec.x - transform.position.x;
            clickVec.y = clickVec.y - transform.position.y;
        }

    }

    public void setControlPoints(Vector2[] v)
    {
        controlPoints = (Vector2[])v.Clone();
    }
    public void setNPolygons(int n)
    {
        nPolygons = n;
    }
    /*public void setGoalPosition(Vector2 v)
    {
        goalPosition = v;
    }
    public void setGoalArc(float arc)
    {
        goalArc = arc;
    }*/
    public void setGoal(GameObject goal)
    {
        Goal = goal;
    }
}
