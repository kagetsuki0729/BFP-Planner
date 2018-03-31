using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Control : MonoBehaviour
{
    public GameObject Polygon;
    public GameObject Robot;
    public GameObject Obstacle;
    public List<GameObject> robotList = new List<GameObject>();
    public List<GameObject> goalList = new List<GameObject>();
    public List<GameObject> obstacleList = new List<GameObject>();
    public int nRobots;
    public int nObstacles;
    public float xScale;
    public float yScale;
    public GameObject[] boundary = new GameObject[4];
    int bitMapSize = 128;
    public int[,] potentialField;
    bool showPotential = false;
    List<List<Vector2>> listcfg = new List<List<Vector2>>();
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
        xScale = 1f / 10;
        yScale = 1f / 10;
        potentialField = new int[bitMapSize, bitMapSize];
        
        createRobots();
        createObstacles();
        makeBoundary();
        //calculatePF();


        Debug.Log("ScreenHeight: " + Screen.height + ", ScreenWidth: " + Screen.width);
        for (int i = 0; i < nRobots; i++)
            Debug.Log("Robot " + i + " is on " + robotList[i].transform.position + " " + robotList[i].GetComponent<RobotControl>().Goal.transform.position);
        for (int i = 0; i < nObstacles; i++)
        {
            Debug.Log("Obstacle " + i + " is on " + obstacleList[i].transform.position + " " + obstacleList[i].transform.childCount);
            /*for (int j = 0; j < obstacleList[i].transform.childCount; j++) {
                GameObject obstacle = obstacleList[i].transform.GetChild(j).gameObject;
                Vector2[] vertices = obstacle.GetComponent<PolygonTester>().vertice;
                foreach (Vector2 v in vertices) {
                    Debug.Log("(" + v.x/xScale + ","+v.y/yScale + ")");
                }
            }*/
        }

    }
    private void OnGUI()
    {
        if (showPotential)
        {
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fontSize = 15;
            guiStyle.normal.textColor = Color.green;
            //GUI.Label(new Rect(10, 10, 100, 100), "Hello World!");

            for (int i = 0; i < bitMapSize; i += 1)
            {
                for (int j = 0; j < bitMapSize; j += 1)
                {
                    float grad=0;
                    if (potentialField[i, j] == 0)guiStyle.normal.textColor = Color.white;
                    else if (potentialField[i, j] > 0 && potentialField[i, j] <= 40)
                    {
                        grad = 6f * potentialField[i, j];
                        if(grad > 255) grad = 255;
                        guiStyle.normal.textColor = new Color(1, grad / 255, 0);
                    }
                    else if (potentialField[i, j] > 40 && potentialField[i, j] <= 70)
                    {
                        grad = 4f * (potentialField[i, j]-40);
                        if (grad > 255) grad = 255;
                        guiStyle.normal.textColor = new Color(1-2*grad/255, 1-grad / 255, 0);
                    }
                    else if (potentialField[i, j] > 70 && potentialField[i, j] <= 100)
                    {
                        grad = 4f * (potentialField[i, j] - 70);
                        if (grad > 255) grad = 255;
                        guiStyle.normal.textColor = new Color(0, (128 - grad) / 255, 2 * grad / 255);
                    }
                    else 
                    {
                        grad = 4f * (potentialField[i, j] - 100);
                        if (grad > 255) grad = 255;
                        guiStyle.normal.textColor = new Color(0, 0, 1- grad / 255);
                    }
                    if (potentialField[i,j]==254) guiStyle.normal.textColor = Color.gray;
                    if (potentialField[i, j] == 255) guiStyle.normal.textColor = Color.black;
                    //if (potentialField[i, j] == 255)
                    //GUI.Label(new Rect(50 + i * 5, 55 + j * 5, 50, 50), System.Convert.ToString(potentialField[i, j]), guiStyle);
                    GUI.Label(new Rect(60 + i * 7, 15 + (bitMapSize- j) * 7, 50, 50), "*", guiStyle);
                }
            }
        }
    }
    void Update()
    {
        //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //calculatePF();
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
                            GameObject goalTemp = Instantiate(Robot, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
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
                                GameObject polyTemp2 = Instantiate(Polygon, new Vector3(0, 0, 0), Quaternion.identity, goalTemp.transform) as GameObject;
                                polyTemp2.SendMessage("setColor", Color.yellow);
                                polyTemp2.SendMessage("TheStart", vertices);
                            }
                            line = theReader.ReadLine(); line = theReader.ReadLine();
                            //"# initial configuration"
                            string[] initcfg = line.Split(' ');
                            float x = float.Parse(initcfg[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * xScale;
                            float y = float.Parse(initcfg[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * yScale;
                            float arc = float.Parse(initcfg[2], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                            //Debug.Log("init-  x: " + x + "  y: " + y);
                            //Debug.Log("Scale:" + xScale);
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
                            goalTemp.transform.position = new Vector3(x2, y2, 0);
                            goalTemp.transform.eulerAngles = new Vector3(
                                        goalTemp.transform.eulerAngles.x,
                                        goalTemp.transform.eulerAngles.y,
                                        goalTemp.transform.eulerAngles.z + arc2
                            );
                            //robotTemp.SendMessage("setGoalPosition", new Vector2(x2, y2));
                            //robotTemp.SendMessage("setGoalArc", arc2);
                            line = theReader.ReadLine(); line = theReader.ReadLine();
                            int nControlPoints;
                            System.Int32.TryParse(line, out nControlPoints);
                            Vector2[] controlPoints = new Vector2[nControlPoints];
                            for (int j = 0; j < nControlPoints; j++)
                            {
                                line = theReader.ReadLine(); line = theReader.ReadLine();
                                //Debug.Log("Cotrol " + j + " " + line);
                                string[] cp = line.Split(' ');
                                float cpX = float.Parse(cp[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * xScale;
                                float cpY = float.Parse(cp[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * yScale;
                                controlPoints[j] = new Vector2(cpX, cpY);
                            }
                            robotTemp.SendMessage("setControlPoints", controlPoints);
                            robotTemp.name = "Robot " + i;
                            robotTemp.AddComponent(typeof(Rigidbody2D));
                            robotTemp.AddComponent(typeof(BoxCollider2D));
                            robotTemp.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;

                            goalTemp.SendMessage("setControlPoints", controlPoints);
                            goalTemp.name = "Goal for Robot " + i;
                            goalTemp.AddComponent(typeof(Rigidbody2D));
                            goalTemp.AddComponent(typeof(BoxCollider2D));
                            goalTemp.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                            goalTemp.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                            robotTemp.SendMessage("setGoal", goalTemp);
                            robotList.Add(robotTemp);
                        }
                    }
                }
            }
            while (line != null);
            // Done reading, close the reader and return true to broadcast success    
            theReader.Close();
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
                        Debug.Log("nObstacles: " + nObstacles);
                        for (int i = 0; i < nObstacles; i++)
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

                            for (int j = 0; j < obstacleTemp.transform.childCount; j++)
                            {
                                GameObject obstacle = obstacleTemp.transform.GetChild(j).gameObject;
                                obstacle.SendMessage("transformation", new float[] { x, y, arc / Mathf.Rad2Deg });
                            }

                            //Debug.Log("init-  x: " + x + "  y: " + y);
                            //Debug.Log("Scale:" + xScale);
                            obstacleTemp.transform.position = new Vector3(x, y, 0);
                            obstacleTemp.transform.eulerAngles = new Vector3(
                                        obstacleTemp.transform.eulerAngles.x,
                                        obstacleTemp.transform.eulerAngles.y,
                                        obstacleTemp.transform.eulerAngles.z + arc
                            );
                            obstacleTemp.name = "Obstacle " + i;
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
            return;
        }


    }

    void makeBoundary()
    {
        Vector2[][] vertices = new Vector2[4][];
        vertices[0] = new[]{new Vector2(0 * xScale, 0 * yScale),
                        new Vector2(-2 * xScale, 0 * yScale),
                        new Vector2(-2 * xScale, bitMapSize * yScale),
                        new Vector2(0 * xScale, bitMapSize * yScale) };
        vertices[1] = new[] { new Vector2(bitMapSize * xScale, 0 * yScale),
                        new Vector2((bitMapSize + 2) * xScale, 0 * yScale),
                        new Vector2((bitMapSize + 2) * xScale, bitMapSize * yScale),
                        new Vector2(bitMapSize * xScale, bitMapSize * yScale)};
        vertices[2] = new[] { new Vector2(0 * xScale, 0 * yScale),
                        new Vector2(bitMapSize * xScale, 0 * yScale),
                        new Vector2(bitMapSize * xScale, -2 * yScale),
                        new Vector2(0 * xScale, -2 * yScale) };
        vertices[3] = new[]{ new Vector2(0 * xScale, bitMapSize * yScale),
                        new Vector2(bitMapSize * xScale, bitMapSize * yScale),
                        new Vector2(bitMapSize * xScale, (bitMapSize + 2) * yScale),
                        new Vector2(0 * xScale, (bitMapSize + 2) * yScale) };
        for (int i = 0; i < 4; i++)
        {
            GameObject polyTemp = Instantiate(Polygon, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            polyTemp.SendMessage("setColor", Color.red);
            polyTemp.SendMessage("TheStart", vertices[i]);
            polyTemp.name = "Boundary " + i;
            boundary[i] = polyTemp;
        }
    }

    int[] xn = { 1, 0, -1, 0 };
    int[] yn = { 0, 1, 0, -1 };
    public void calculatePF()
    {
        //init bitmap=254
        listcfg.Clear();
        for (int i = 0; i < bitMapSize; i++)
            for (int j = 0; j < bitMapSize; j++)
                potentialField[i, j] = 254;
        //set goal=0
        for (int i = 0; i < nRobots; i++) {
            int x=(int)(robotList[i].GetComponent<RobotControl>().Goal.transform.position.x / xScale);
            int y = (int)(robotList[i].GetComponent<RobotControl>().Goal.transform.position.y / yScale);
            potentialField[x, y] = 0;
            List<Vector2> l0 = new List<Vector2>();
            l0.Add(new Vector2((float)x, (float)y));
            listcfg.Add(l0);
        }

        //set obstacle=255
        for (int i = 0; i < nObstacles; i++)
        {
            for (int j = 0; j < obstacleList[i].transform.childCount; j++)
            {
                GameObject obstacle = obstacleList[i].transform.GetChild(j).gameObject;
                Vector2[] vertices = obstacle.GetComponent<PolygonTester>().vertice;
                for (int k = 0; k < vertices.Length; k++)
                {
                    //Debug.Log(j + " is (" + vertices[k].x / xScale + "," + vertices[k].y / yScale + ")");
                    int d;
                    float dx, dy, x1, x2, y1, y2;
                    if (k != 0)
                    {
                        x1 = vertices[k].x / xScale;
                        y1 = vertices[k].y / yScale;
                        x2 = vertices[(k + 1) % vertices.Length].x / xScale;
                        y2 = vertices[(k + 1) % vertices.Length].y / xScale;
                    }
                    else
                    {
                        x1 = vertices[k].x / xScale;
                        y1 = vertices[k].y / yScale;
                        x2 = vertices[(k + 1)].x / xScale;
                        y2 = vertices[(k + 1)].y / xScale;
                    }
                    //Debug.Log(j+" 1: ("+x1 + "," + y1+")  2: (" + x2 + "," + y2 + ")");
                    d = (int)Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));
                    dx =(x2 - x1) / d;
                    dy = (y2 - y1) / d;
                    if (dx > 1) dx = 1;
                    if (dy > 1) dy = 1;
                    if (dx < -1) dx = -1;
                    if (dy < -1) dy = -1;
                    //Debug.Log(d + " " + dx + " " + dy);
                    for (int l = 0; l <= d; l++)
                    {
                        int x = (int)(x1 + l * dx);
                        int y = (int)(y1 + l * dy);
                       //Debug.Log(l+" ("+x + "," + y+")");
                        if(x<bitMapSize&&y<bitMapSize)
                            potentialField[x, y] = 255;
                    }
                }
            }
        }
        //Debug.Log("Set Obstacle PF done");
        //NF1
       for(int i = 0; i < listcfg.Count; i++)
        {
            //Debug.Log("In " + i + "...");
            List<Vector2> li = new List<Vector2>();
            foreach (Vector2 v in listcfg[i]){
                //check 1-neighbor
                for (int j = 0; j < 4; j++) {
                    int x = (int)v.x + xn[j];
                    int y= (int)v.y + yn[j];
                    //Debug.Log(i+"## "+x + " " + y+" "+ Enumerable.Range(0, bitMapSize - 1).Contains(x)+ Enumerable.Range(0, bitMapSize - 1).Contains(x)+li.Count);
                    if (Enumerable.Range(0, bitMapSize - 1).Contains(x)&& Enumerable.Range(0, bitMapSize - 1).Contains(y)) {
                        if (potentialField[x, y] == 254) {
                            potentialField[x, y] = i + 1;
                            li.Add(new Vector2((float)x, (float)y));
                        }
                    }
                }                
            }
            if (li.Count != 0)
                listcfg.Add(li);
        }
        Debug.Log("PF Complete");
    }

    public void togglePotential()
    {
        showPotential = !showPotential;
    }
    public void ResetGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        //Application.LoadLevel(Application.loadedLevel); //讀取關卡(已讀取的關卡)

    }

}