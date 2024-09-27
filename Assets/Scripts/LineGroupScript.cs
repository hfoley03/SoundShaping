using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGroupScript : MonoBehaviour
{

    private CubicBezier[] curves;
    public int lineNumber;
    private GameObject lineToUpdate;

    void Start()
    {
       Debug.Log("Hi I am the LineGroupScript");
    }

    public void PrintControlPoints(CubicBezier[] items)
    {
        Debug.Log(items);
    }

    public void setCurves(CubicBezier[] items)
    {
        curves = items;
    }

    public CubicBezier[] getCurves()
    {
        return curves;
    }

    private void printCurvesInfo()
    {
        for(int i = 0; i < curves.Length; i++)
        {
            Debug.Log(curves[i]);
        }
    }

    public void updateControlPointsStartNode(LineRenderer lineRenderer, Vector3 newPosition, int segmentNumber, int nodeNumber, bool updateMesh)
    {
        curves[0].p0 = newPosition;
        updateLine(lineRenderer, segmentNumber, updateMesh);
    }

    public void updateControlPointsEndNode(LineRenderer lineRenderer, Vector3 newPosition, int segmentNumber, int nodeNumber, bool updateMesh)
    {
        curves[segmentNumber].p3 = newPosition;
        updateLine(lineRenderer, segmentNumber, updateMesh);
    }

    public void updateControlPointsSmallNode(LineRenderer lineRenderer1, LineRenderer lineRenderer2, Vector3 newPosition, int segmentNumber, int nodeNumber, bool updateMesh)
    {
        curves[segmentNumber].p0 = newPosition;
        curves[segmentNumber - 1].p3 = newPosition;
        updateLine(lineRenderer1, segmentNumber, updateMesh);
        updateLine(lineRenderer2, segmentNumber - 1, updateMesh);
    }

    private void updateLine(LineRenderer lineRenderer, int segmentNumber, bool updateMesh)
    {
        Vector3[] bezPoints = BezierLineFunctions.BezierInterp(curves[segmentNumber].p0, curves[segmentNumber].p1, curves[segmentNumber].p2, curves[segmentNumber].p3, lineRenderer, 2);

        lineRenderer.SetPositions(bezPoints);

        float dist = Vector3.Distance(curves[segmentNumber].p0, curves[segmentNumber].p3);

        if (updateMesh)
        {
            MeshCollider oldMeshCollider = lineRenderer.gameObject.GetComponent<MeshCollider>();

            if (oldMeshCollider != null)
            {
                destroyOldMeshMakeNewMesh(lineRenderer, oldMeshCollider, lineRenderer.gameObject);
            }
            else
            {
                Debug.LogWarning("line mesh collider was null");
            }
        }
        
    }

    public void destroyOldMeshMakeNewMesh(LineRenderer newLine, MeshCollider lineMeshCollider, GameObject lineToUpdate)
    {
        Destroy(lineMeshCollider);

        newLine.useWorldSpace = false;

        MeshCollider meshCollider = lineToUpdate.AddComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        newLine.startWidth = 0.05f;
        newLine.endWidth = 0.05f;

        newLine.BakeMesh(mesh);
        meshCollider.sharedMesh = mesh;
        newLine.startWidth = 0.01f;
        newLine.endWidth = 0.01f;
    }
}
