using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Draw_Line : MonoBehaviour
{
    private bool meshColliderEnabled = true;

    GameObject brushInstance;
    public GameObject brushUser;
    public GameObject brushBezier;
    public GameObject noteNode;
    public GameObject smallNode;
    public GameObject smallNodeNonInteractable;
    public GameObject wristRotationCube;
    public GameObject lineGroupParent;
    public GameObject lineGrandparent;
    public GameObject userDrawnGroup;

    public Material blue, red, green, yellow, purple, pink, orange;
    private Material currentColour;
    private Dictionary<string, Material> colourDict;

    public GameObject theWrist;

    public LineRenderer currentLineRenderer;
    LineRenderer bezierLineRenderer;

    [SerializeField]
    private float minDistance = 0.01f;

    public int newVal;

    Vector3 lastPos;
    Vector3 indexPos;

    float lastAngle;

    //int lineCounter = 0;
    int segmentCounter = 0;

    GameObject groupParent;
    GameObject grandParent;

    void Start()
    {
       colourDict = new Dictionary<string, Material>()
       {
            {"blue", blue },
            {"red", red },
            { "green", green },
            {"yellow", yellow},
            {"purple", purple },
            {"pink", pink },
           {"orange", orange }
       };
        setColour("orange");
    }

    public void setColour(string col)
    {
        Material material;
        if(colourDict.TryGetValue(col, out material))
        {
            currentColour = material;
        }
        else
        {
            Debug.Log("we dont have that colour");
        }
    }

    public void setColourInt(int col)
    {
        if(col == 0)
        {
            currentColour = blue;
        }
        if (col == 1)
        {
            currentColour = red;
        }
        if (col == 2)
        {
            currentColour = green;
        }
        if (col == 3)
        {
            currentColour = yellow;
        }
        if (col == 4)
        {
            currentColour = purple;
        }
        if (col == 5)
        {
            currentColour = pink;
        }
        if (col == 6)
        {
            currentColour = orange;
        }
    }

    public void SetPosition(Vector3 pos) {
        indexPos = pos;
    }

    public void CreateBrush()
    {
        brushInstance = Instantiate(brushUser);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();
        currentLineRenderer.material = currentColour;
        lastPos = indexPos;
        currentLineRenderer.SetPosition(0, indexPos);
        currentLineRenderer.SetPosition(1, indexPos);
        lastPos = indexPos;

        CreateGrandparent();
        CreateParent();
        CreateFirstNode();
    }

    void CreateFirstNode()
    {
        GameObject sphereNodeInstanceP0 = null;
        if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
        {
            sphereNodeInstanceP0 = Instantiate(noteNode);
            Debug.LogError("oe note node init");
        }
        else if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
        {
            sphereNodeInstanceP0 = Instantiate(smallNodeNonInteractable);
            Debug.LogError("tmt small node init");
        }

        if (sphereNodeInstanceP0 != null){
            //NODE AT START OF EACH LINE
            sphereNodeInstanceP0.transform.position = indexPos;
            sphereNodeInstanceP0.name = "NodeP0_" + "Line_" + InteractionManager.Instance.getGlobalLineCounter() + "_Segment_" + 0;
            sphereNodeInstanceP0.transform.SetParent(groupParent.transform);
            sphereNodeInstanceP0.GetComponent<MeshRenderer>().material = currentColour;
            sphereNodeInstanceP0.GetComponent<NodeSphereInteractable>().SetSegmentNumber(0);
            sphereNodeInstanceP0.GetComponent<NodeSphereInteractable>().SetNodeNumber(0);
            sphereNodeInstanceP0.GetComponent<NodeSphereInteractable>().SetLineNumber(InteractionManager.Instance.getGlobalLineCounter());
            sphereNodeInstanceP0.GetComponent<NodeSphereInteractable>().SetNotdeType(NodeSphereInteractable.NodeType.StartNoteNode);
        }
    }

    public void AddPoint()
    {
        if(Vector3.Distance(indexPos, lastPos) > minDistance)
        {
            if (currentLineRenderer != null)
            {
                currentLineRenderer.positionCount++;
                int posIndex = currentLineRenderer.positionCount - 1;
                currentLineRenderer.SetPosition(posIndex, indexPos);
                lastPos = indexPos;
            }
            else
            {
                Debug.LogError("hmmmm");
            }
        }
    }

    public void StopDrawing(bool fromWrist)
    {
        DrawnLineToBeziers(fromWrist);
        currentLineRenderer = null;
    }

    public void WristSplit()
    {
        StopDrawing(true);
        GameObject brushInstance = Instantiate(brushUser);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>(); //<LineRenderer>
        currentLineRenderer.material = currentColour;
        lastPos = indexPos;
        currentLineRenderer.SetPosition(0, indexPos);
        currentLineRenderer.SetPosition(1, indexPos);
        lastPos = indexPos;
        CreateParent();
    }

    void CreateGrandparent() 
    {     
        grandParent = Instantiate(lineGrandparent);
        grandParent.transform.name = "GrandParent_" + InteractionManager.Instance.getGlobalGrandParentCounter();
        grandParent.GetComponent<LineGrandparent>().grandParentNumber = InteractionManager.Instance.getGlobalGrandParentCounter();
        InteractionManager.Instance.incrementGlobalGrandParentCounter();
        grandParent.transform.SetParent(userDrawnGroup.transform);
        LogManager.Instance.IncrementNumLines();
    }

    void CreateParent()
    {
        groupParent = Instantiate(lineGroupParent);
        groupParent.transform.name = "Line_" + InteractionManager.Instance.getGlobalLineCounter() + "_parent";  // lineCounter
        groupParent.GetComponent<LineGroupScript>().lineNumber = InteractionManager.Instance.getGlobalLineCounter();
        currentLineRenderer.transform.SetParent(groupParent.transform);
        groupParent.transform.SetParent(grandParent.transform);
    }

    void DrawnLineToBeziers(bool fromWrist)
    {
        // get current line's points, conv to list
        Vector3[] currentPoints = new Vector3[currentLineRenderer.positionCount];
        currentLineRenderer.GetPositions(currentPoints);
        List<Vector3> currentPointsList = currentPoints.ToList();
        List<Vector3> processedPointsList = CurvePreprocess.RemoveDuplicates(currentPointsList);

        // get the bez curve(s)
        CubicBezier[] curves = CurveFit.Fit(processedPointsList, 0.02f); //currentPointsList
        groupParent.GetComponent<LineGroupScript>().setCurves(curves);

        for (int n = 0; n < curves.Length; n++)
        {
            Vector3[] bezPoints = BezierLineFunctions.BezierInterp(curves[n].p0, curves[n].p1, curves[n].p2, curves[n].p3, currentLineRenderer, curves.Length);

            float dist = Vector3.Distance(curves[n].p0, curves[n].p3);

            DrawBezier(bezPoints, groupParent);
            
                if (n > 0 && n < curves.Length) //c.l -1
                {
                    GameObject sphereNodeInstanceP0;

                    if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
                    {
                        sphereNodeInstanceP0 = Instantiate(smallNodeNonInteractable);
                    }
                    else
                    {
                        sphereNodeInstanceP0 = Instantiate(smallNode); //small
                    }

                    //NODE AT START OF EACH SEGEMENT
                    sphereNodeInstanceP0.transform.position = curves[n].p0;
                    sphereNodeInstanceP0.name = "NodeP0_" + "Line_" + InteractionManager.Instance.getGlobalLineCounter() + "_Segment_" + segmentCounter;
                    sphereNodeInstanceP0.transform.SetParent(groupParent.transform);
                    sphereNodeInstanceP0.GetComponent<MeshRenderer>().material = currentColour;
                    sphereNodeInstanceP0.GetComponent<NodeSphereInteractable>().SetSegmentNumber(segmentCounter);
                    sphereNodeInstanceP0.GetComponent<NodeSphereInteractable>().SetNodeNumber(0);
                    sphereNodeInstanceP0.GetComponent<NodeSphereInteractable>().SetLineNumber(InteractionManager.Instance.getGlobalLineCounter());
                    sphereNodeInstanceP0.GetComponent<NodeSphereInteractable>().SetNotdeType(NodeSphereInteractable.NodeType.SmallNode);
                }

            //IF IF THE SEGMENT, ADD NODE TO END
            if (n == curves.Length - 1)
            {
                GameObject sphereNodeInstanceP3 = null;

                if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
                {
                    sphereNodeInstanceP3 = Instantiate(noteNode);
                }
                if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
                {
                    sphereNodeInstanceP3 = Instantiate(smallNodeNonInteractable);
                }

                if (sphereNodeInstanceP3 != null)
                {
                    sphereNodeInstanceP3.transform.position = curves[n].p3;
                    sphereNodeInstanceP3.name = "NodeP3_" + "Line_" + InteractionManager.Instance.getGlobalLineCounter() + "_Segment_" + segmentCounter;
                    sphereNodeInstanceP3.transform.SetParent(groupParent.transform);
                    sphereNodeInstanceP3.GetComponent<MeshRenderer>().material = currentColour;
                    sphereNodeInstanceP3.GetComponent<NodeSphereInteractable>().SetSegmentNumber(segmentCounter);
                    sphereNodeInstanceP3.GetComponent<NodeSphereInteractable>().SetNodeNumber(3);
                    sphereNodeInstanceP3.GetComponent<NodeSphereInteractable>().SetLineNumber(InteractionManager.Instance.getGlobalLineCounter());

                    if (fromWrist)
                    {
                        sphereNodeInstanceP3.GetComponent<NodeSphereInteractable>().SetNotdeType(NodeSphereInteractable.NodeType.MiddleNoteNode);
                    }
                    else
                    {
                        sphereNodeInstanceP3.GetComponent<NodeSphereInteractable>().SetNotdeType(NodeSphereInteractable.NodeType.EndNoteNode);
                    }
                }
            }
            
            segmentCounter++;
            
        }
        segmentCounter = 0;
        InteractionManager.Instance.incrementGlobalLineCounter(); //lineCounter++
        Destroy(currentLineRenderer);
        Destroy(brushInstance);

        if (!fromWrist)
        {
            grandParent.GetComponent<LineGrandparent>().setup = true;
        }

    }

    //CREATE A NEW LINE RENDERER FOR THE SEGMENT, SET POINTS OF BEZ CURVE AND GROUP TO PARENT
    void DrawBezier(Vector3[] bezSegmentPoints, GameObject groupParent)
    {
        GameObject brushInstance = Instantiate(brushBezier);
        bezierLineRenderer = brushInstance.GetComponent<LineRenderer>();
        bezierLineRenderer.material = currentColour;
        bezierLineRenderer.name = "Line_" + InteractionManager.Instance.getGlobalLineCounter() + "_Segment_" + segmentCounter;

        for (int k = 0; k < bezSegmentPoints.Length; k++)
        {
            if (k > 1)
            {
                bezierLineRenderer.positionCount++;
            }
            bezierLineRenderer.SetPosition(k, bezSegmentPoints[k]);
        }

        bezierLineRenderer.transform.SetParent(groupParent.transform);

        if (meshColliderEnabled)
        {
            bezierLineRenderer.useWorldSpace = false;

            MeshCollider meshCollider = brushInstance.AddComponent<MeshCollider>();
            Mesh mesh = new Mesh();
            bezierLineRenderer.startWidth = 0.05f;
            bezierLineRenderer.endWidth = 0.05f;

            bezierLineRenderer.BakeMesh(mesh);
            meshCollider.sharedMesh = mesh;
            bezierLineRenderer.startWidth = 0.008f;  //0.01
            bezierLineRenderer.endWidth = 0.008f;

            meshCollider.sharedMesh.name = "Mesh_" + bezierLineRenderer.name;

        }
        bezierLineRenderer = null;
    }


}
