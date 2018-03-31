using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ObstacleControl : MonoBehaviour
{
    public ObstacleControl Instance;
    public int nPolygons;
    //public List<GameObject> polygonList = new List<GameObject>();
    // Use this for initialization
    void Start()
    {
        Instance = this;
        //Debug.Log(this.transform.childCount);
    }

    // Update is called once per frame
    bool isPicked;
    bool isRotated;
    //public float RotationSpeed = 500;
    float prevArc = 0;
    void Update()
    {


        if (isPicked && Input.GetMouseButtonUp(0))
            isPicked = false;
        if (isRotated && Input.GetMouseButtonUp(1))
            isRotated = false;


        if (isPicked)
        {

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 delta = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
            transform.position = mousePos;
            // if you want to smooth movement then lerp it
            //transform.position = Vector2.Lerp(transform.position, mousePos, 0.5f);


            Debug.Log(this.name + " " + transform.position);

            for (int i = 0; i < this.transform.childCount; i++)
            {
                GameObject poly = this.transform.GetChild(i).gameObject;
                poly.SendMessage("transformation", new float[] { delta.x, delta.y});
            }


        }
        if (isRotated)
        {

            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 dir = Input.mousePosition - pos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //Debug.Log(transform.rotation.eulerAngles);

            if (angle != prevArc)
            {
                prevArc = angle;
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    GameObject poly = this.transform.GetChild(i).gameObject;
                    Debug.Log(Mathf.Atan2(dir.y, dir.x) + " " + angle + " " + Quaternion.AngleAxis(angle, Vector3.forward) * transform.position);
                    poly.SendMessage("transformation", new float[] { 0, 0, Mathf.Atan2(dir.y, dir.x) });

                }
            }



        }

    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(this.name + " is clicked");
            isPicked = true;
        }
        if (Input.GetMouseButtonDown(1))
            isRotated = true;


    }
    public void setNPolygons(int n)
    {
        nPolygons = n;
    }
}
