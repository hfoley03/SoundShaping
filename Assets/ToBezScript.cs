using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToBezScript : MonoBehaviour
{
    public GameObject myOldLine;
    public LineRenderer myOldLineRender;
    public Vector3[] oldPoints;
    public List<Vector3> oldPointsList;
    public Vector3 P0;
    public Vector3 P1;
    public Vector3 P2;
    public Vector3 P3;

    public GameObject brush;
    LineRenderer currentLineRenderer;


    GameObject controlPoint0;
    GameObject controlPoint1;
    GameObject controlPoint2;
    GameObject controlPoint3;

    GameObject lineStart0;
    GameObject lineStart1;
    GameObject lineStart2;
    GameObject lineStart3;
    GameObject lineStart4;


    public GameObject controlPointMarker;
    public GameObject littlePoint;

    [SerializeField]
    private float maxERROR = 1f;

    // Start is called before the first frame update
    void Start()
    {
        controlPoint0 = Instantiate(controlPointMarker, this.transform);
        controlPoint1 = Instantiate(controlPointMarker, this.transform);
        controlPoint2 = Instantiate(controlPointMarker, this.transform);
        controlPoint3 = Instantiate(controlPointMarker, this.transform);

        Debug.Log("My Old Line Info");
        Debug.Log(myOldLine.name);
        Debug.Log("orginal line pos count: " + myOldLine.GetComponent<LineRenderer>().positionCount);
        myOldLineRender = myOldLine.GetComponent<LineRenderer>();

        for (int index = 0; index < myOldLineRender.positionCount; index++)
        {
            //Debug.Log(myOldLineRender.GetPosition(index));
            oldPointsList.Add(myOldLineRender.GetPosition(index));
        }

        Debug.Log("Old Points array length: " + oldPointsList.Count);

        P0 = myOldLineRender.GetPosition(0);
        P3 = myOldLineRender.GetPosition(myOldLineRender.positionCount - 1);


        //calculateCP();
        //BezierInterp(P0, P1, P2, P3);

        CubicBezier[] curves = CurveFit.Fit(oldPointsList, maxERROR);
        Debug.Log("Cubic Bezzzzz");
        Debug.Log(curves.Length);
        Debug.Log(curves[0]);

        for(int n = 0; n< curves.Length; n++)
        {
            BezierInterp(curves[n].p0, curves[n].p1, curves[n].p2, curves[n].p3);
        }
    }

    //Dr. Khan, approx of data using cubic Bezier curve least square fittig, ported from matlab to c#
    void calculateCP()
    {
        float[] t = Linspace(0, 1, myOldLineRender.positionCount);
        float a1 = 0f;
        float a2 = 0f;
        float a12 = 0f;
        Vector3 c1 = Vector3.zero;
        Vector3 c2 = Vector3.zero;

        for (int i = 0; i < myOldLineRender.positionCount; i++)
        {
            float b0 = Mathf.Pow((1 - t[i]), 3.0f);
            float b1 = (3f*t[i]) * Mathf.Pow((1 - t[i]), 2.0f);
            float b2 = 3f * Mathf.Pow((t[i]), 2.0f) * (1 - t[i]);
            float b3 = Mathf.Pow(t[i], 3.0f);

            a1 = a1 + Mathf.Pow(b1, 2f);
            a2 = a2 + Mathf.Pow(b2, 2f);
            a12 = a12 + b1 * b2;
            c1 = c1 + b1 * (myOldLineRender.GetPosition(i) - (b0 * P0) - (b3 * P3));
            c2 = c2 + b2 * (myOldLineRender.GetPosition(i) - (b0 * P0) - (b3 * P3));

        }

        float denom = a1 * a2 - a12 * a12;
        if (denom == 0f) {
            Debug.Log("denom equals zero");
            P1 = P0;
            P2 = P3;
        }

        else
        {
            P1 = (a2 * c1 - a12 * c2) / denom;
            P2 = (a1 * c2 - a12 * c1) / denom;

            Debug.Log("P0 " + P0 + "  P3 " + P3);
            Debug.Log("P1 " + P1 + "  P2 " + P2);

        }

        controlPoint0.transform.position = P0;
        controlPoint0.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
        controlPoint1.transform.position = P1;
        controlPoint1.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
        controlPoint2.transform.position = P2;
        controlPoint2.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
        controlPoint3.transform.position = P3;







    }

    //Port of standard Matlab function 
    public float[] Linspace(float start, float end, int n)
    {
        float[] result = new float[n];
        float step = (end - start) / (n - 1);

        for (int i = 0; i<n; i++)
        {
            result[i] = start + step * i;
        }
        return result;
    }

    //Dr. Khan, approx of data using cubic Bezier curve least square fittig, ported from matlab to c#
    // Further adapted for intended purpose in this project
    public Vector3[] BezierInterp(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
    {
        int length = myOldLineRender.positionCount;
        float[] t = Linspace(0, 1, myOldLineRender.positionCount);

        Vector3[] Q = new Vector3[myOldLineRender.positionCount];


        GameObject brushInstance = Instantiate(brush);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();

                int red = Random.Range(0, 254);
        int green = Random.Range(0, 254);
        int blue = Random.Range(0, 254);

        currentLineRenderer.startColor = new Color(255, 0, 0);



        Matrix4x4 M = new Matrix4x4(
            new Vector4(-1, 3, -3, 1),
            new Vector4(3, -6, 3, 0),
            new Vector4(-3, 3, 0, 0),
            new Vector4(1, 0, 0, 0));

        for (int k = 0; k < length; k++)
        {
            Vector4 tVector = new Vector4(Mathf.Pow(t[k], 3), Mathf.Pow(t[k], 2), t[k], 1);
            Vector4 PVector = new Vector4(P0.x, P1.x, P2.x, P3.x);
            float x = Vector4.Dot(tVector, M * PVector);

            PVector = new Vector4(P0.y, P1.y, P2.y, P3.y);
            float y = Vector4.Dot(tVector, M * PVector);

            PVector = new Vector4(P0.z, P1.z, P2.z, P3.z);
            float z = Vector4.Dot(tVector, M * PVector);
            Q[k] = new Vector3(x, y, z);

            if (k == 0)
            {
                Instantiate(controlPointMarker);
                controlPointMarker.transform.position = Q[k];
                currentLineRenderer.SetPosition(k, Q[k]);
            }
            else if (k == 1)
            {
                currentLineRenderer.SetPosition(k, Q[k]);
            }
            else
            {
                currentLineRenderer.positionCount++;
                currentLineRenderer.SetPosition(k, Q[k]);
            }

        }
        currentLineRenderer = null;
        return Q;
    }


    public bool EqualsOrClose(Vector3 v1, Vector3 v2)
    {
        return DistanceSquared(v1, v2) < 1.2e-12f;
    }
    public float DistanceSquared(Vector3 a, Vector3 b) { float dx = a.x - b.x; float dy = a.y - b.y; float dz = a.z - b.z; return dx * dx + dy * dy + dz * dz; }


}
