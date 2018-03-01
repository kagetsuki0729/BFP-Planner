using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class Control : MonoBehaviour
{
    public GameObject Polygon;
    public GameObject Robot;
    public GameObject Obstacle;
    public List<GameObject> robotList = new List<GameObject>();
    public List<GameObject> obstacleList = new List<GameObject>();
    public int nRobots;
    public int nObstacles;
    public float xScale;
    public float yScale;

   

    void Start()
    {
        /*GameObject robot1 = new GameObject("Robot");
        GameObject poly1=Instantiate(Polygon, new Vector3(0, 0, 0), Quaternion.identity,robot1.transform) as GameObject;
        GameObject poly2 = Instantiate(Polygon, new Vector3(0, 0, 0), Quaternion.identity, robot1.transform) as GameObject;
        poly1.SendMessage("setColor", Color.blue);
        poly1.SendMessage("TheStart", vertices2D);        
        poly2.SendMessage("TheStart", vertices2D_2);
        robot1.transform.eulerAngles = new Vector3(
            robot1.transform.eulerAngles.x,
            robot1.transform.eulerAngles.y,
            robot1.transform.eulerAngles.z+10
            );
        robot1.transform.position = new Vector3(20, 20, 0);
        */
        xScale = 1f / 20;
        yScale = 1f / 20;
        createRobots();
        createObstacles();

    }
    void Update()
    {
       
    }

    void createRobots()
    {

        string line;
        // Create a new StreamReader, tell it which file to read and what encoding the file
        // was saved as
        StreamReader theReader = new StreamReader("./Assets/robot.dat", Encoding.Default);
        // Immediately clean up the reader after this block of code is done.
        // You generally use the "using" statement for potentially memory-intensive objects
        // instead of relying on garbage collection.
        // (Do not confuse this with the using directive for namespace at the 
        // beginning of a class!)
        using (theReader)
        {
            // While there's lines left in the text file, do this:
            do
            {
                line = theReader.ReadLine();

                if (line != null)
                {
                    //Debug.Log(line);
                    if (line.Equals("# number of robots"))
                    {
                        line = theReader.ReadLine();
                        System.Int32.TryParse(line, out nRobots);
                        for (int i = 0; i < nRobots; i++)
                        {
                            int nPolygons;
                            GameObject robotTemp = Instantiate(Robot, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                            line = theReader.ReadLine(); line = theReader.ReadLine(); line = theReader.ReadLine();
                            System.Int32.TryParse(line, out nPolygons);
                            robotTemp.SendMessage("setNPolygons", nPolygons);
                            Debug.Log("nPolygons: " + nPolygons);
                            //List<GameObject> polygonList = new List<GameObject>();
                            for (int j = 0; j < nPolygons; j++)
                            {
                                int nVertices;
                                line = theReader.ReadLine(); line = theReader.ReadLine(); line = theReader.ReadLine();
                                System.Int32.TryParse(line, out nVertices);
                                Vector2[] vertices = new Vector2[nVertices];
                                line = theReader.ReadLine();
                                for (int k = 0; k < nVertices; k++)
                                {
                                    line = theReader.ReadLine();
                                    string[] coor = line.Split(' ');
                                    vertices[k] = new Vector2(float.Parse(coor[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * xScale, float.Parse(coor[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * yScale);

                                }

                                GameObject polyTemp = Instantiate(Polygon, new Vector3(0, 0, 0), Quaternion.identity, robotTemp.transform) as GameObject;
                                polyTemp.SendMessage("setColor", Color.blue);
                                polyTemp.SendMessage("TheStart", vertices);
                            }
                            line = theReader.ReadLine(); line = theReader.ReadLine();
                            //"# initial configuration"
                            string[] initcfg = line.Split(' ');
                            float x = float.Parse(initcfg[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * xScale;
                            float y = float.Parse(initcfg[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * yScale;
                            float arc = float.Parse(initcfg[2], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                            Debug.Log("init-  x: " + x + "  y: " + y);
                            Debug.Log("Scale:" + xScale);
                            robotTemp.transform.position = new Vector3(x, y, 0);
                            robotTemp.transform.eulerAngles = new Vector3(
                                        robotTemp.transform.eulerAngles.x,
                                        robotTemp.transform.eulerAngles.y,
                                        robotTemp.transform.eulerAngles.z + arc
                            );
                            line = theReader.ReadLine(); line = theReader.ReadLine();
                            //# goal configuration
                            string[] goalcfg = line.Split(' ');
                            float x2 = float.Parse(goalcfg[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * xScale;
                            float y2 = float.Parse(goalcfg[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * yScale;
                            float arc2 = float.Parse(goalcfg[2], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                            robotTemp.SendMessage("setGoalPosition", new Vector2(x2, y2));
                            robotTemp.SendMessage("setGoalArc", arc2);
                            line = theReader.ReadLine(); line = theReader.ReadLine();
                            int nControlPoints;
                            System.Int32.TryParse(line, out nControlPoints);
                            Vector2[] controlPoints = new Vector2[nControlPoints];
                            for (int j = 0; j < nControlPoints; j++)
                            {
                                line = theReader.ReadLine(); line = theReader.ReadLine();
                                Debug.Log("Cotrol " + j + " " + line);
                                string[] cp = line.Split(' ');
                                float cpX = float.Parse(cp[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * xScale;
                                float cpY = float.Parse(cp[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * yScale;
                                controlPoints[j] = new Vector2(cpX, cpY);
                            }
                            robotTemp.SendMessage("setControlPoints", controlPoints);
                            robotTemp.AddComponent(typeof(Rigidbody2D));
                            robotTemp.AddComponent(typeof(BoxCollider2D));
                            robotTemp.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                            robotList.Add(robotTemp);
                        }
                    }
                }
            }
            while (line != null);
            // Done reading, close the reader and return true to broadcast success    
            theReader.Close();
            Debug.Log(robotList.Count);
            for (int i = 0; i < robotList.Count; i++)
            {
                Debug.Log(i + " " + robotList[i]);
            }
            return;
        }


    }

    void createObstacles()
    {

        string line;
        // Create a new StreamReader, tell it which file to read and what encoding the file
        // was saved as
        StreamReader theReader = new StreamReader("./Assets/obstacle.dat", Encoding.Default);
        // Immediately clean up the reader after this block of code is done.
        // You generally use the "using" statement for potentially memory-intensive objects
        // instead of relying on garbage collection.
        // (Do not confuse this with the using directive for namespace at the 
        // beginning of a class!)
        using (theReader)
        {
            // While there's lines left in the text file, do this:
            do
            {
                line = theReader.ReadLine();

                if (line != null)
                {
                    //Debug.Log(line);
                    if (line.Equals("#number of obstacles"))
                    {
                        line = theReader.ReadLine();
                        System.Int32.TryParse(line, out nObstacles);
                        for (int i = 0; i < nRobots; i++)
                        {
                            int nPolygons;
                            GameObject obstacleTemp = Instantiate(Obstacle, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                            line = theReader.ReadLine(); line = theReader.ReadLine(); line = theReader.ReadLine();
                            System.Int32.TryParse(line, out nPolygons);
                            obstacleTemp.SendMessage("setNPolygons", nPolygons);
                            Debug.Log("nPolygons: " + nPolygons);
                            //List<GameObject> polygonList = new List<GameObject>();
                            for (int j = 0; j < nPolygons; j++)
                            {
                                int nVertices;
                                line = theReader.ReadLine(); line = theReader.ReadLine(); line = theReader.ReadLine();
                                System.Int32.TryParse(line, out nVertices);
                                Vector2[] vertices = new Vector2[nVertices];
                                line = theReader.ReadLine();
                                for (int k = 0; k < nVertices; k++)
                                {
                                    line = theReader.ReadLine();
                                    string[] coor = line.Split(' ');
                                    vertices[k] = new Vector2(float.Parse(coor[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * xScale, float.Parse(coor[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * yScale);

                                }

                                GameObject polyTemp = Instantiate(Polygon, new Vector3(0, 0, 0), Quaternion.identity, obstacleTemp.transform) as GameObject;
                                polyTemp.SendMessage("setColor", Color.black);
                                polyTemp.SendMessage("TheStart", vertices);
                            }
                            line = theReader.ReadLine(); line = theReader.ReadLine();
                            //"# initial configuration"
                            string[] initcfg = line.Split(' ');
                            float x = float.Parse(initcfg[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * xScale;
                            float y = float.Parse(initcfg[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * yScale;
                            float arc = float.Parse(initcfg[2], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                            Debug.Log("init-  x: " + x + "  y: " + y);
                            Debug.Log("Scale:" + xScale);
                            obstacleTemp.transform.position = new Vector3(x, y, 0);
                            obstacleTemp.transform.eulerAngles = new Vector3(
                                        obstacleTemp.transform.eulerAngles.x,
                                        obstacleTemp.transform.eulerAngles.y,
                                        obstacleTemp.transform.eulerAngles.z + arc
                            );
                            obstacleTemp.AddComponent(typeof(Rigidbody2D));
                            obstacleTemp.AddComponent(typeof(BoxCollider2D));
                            obstacleTemp.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                            obstacleTemp.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                            obstacleList.Add(obstacleTemp);
                        }
                    }
                }
            }
            while (line != null);
            // Done reading, close the reader and return true to broadcast success    
            theReader.Close();
            Debug.Log(obstacleList.Count);
            for (int i = 0; i < obstacleList.Count; i++)
            {
                Debug.Log(i + " " + obstacleList[i]);
            }
            return;
        }


    }

}