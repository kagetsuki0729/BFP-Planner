using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleControl : MonoBehaviour {
    public ObstacleControl Instance;
    public int nPolygons;
    //public List<GameObject> polygonList = new List<GameObject>();
    // Use this for initialization
    void Start () {
        Instance = this;
	}

    // Update is called once per frame
    bool isPicked;
    bool isRotated;
    //public float RotationSpeed = 500;
    void Update()
    {


        if (isPicked && Input.GetMouseButtonUp(0))
            isPicked = false;
        if (isRotated && Input.GetMouseButtonUp(1))
            isRotated = false;


        if (isPicked)
        {

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position = mousePos;

            // if you want to smooth movement then lerp it

        }
        if (isRotated)
        {
            //Debug.Log("~ "+ Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime + " "+ Input.GetAxis("Mouse Y") * RotationSpeed * Time.deltaTime);
            transform.Rotate(0, 0, 5, Space.World);
        }

    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
            isPicked = true;
        if (Input.GetMouseButtonDown(1))
            isRotated = true;

    }
    public void setNPolygons(int n)
    {
        nPolygons = n;
    }
}
