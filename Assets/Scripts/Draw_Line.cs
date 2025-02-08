using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Draw_Line : MonoBehaviour
{
    private bool meshColliderEnabled = true;

    private GameObject brushInstance;
    public GameObject brushUser, brushBezier;
    public GameObject noteNode, smallNode, smallNodeNonInteractable;
    public GameObject wristRotationCube;
    public GameObject lineGroupParent, lineGrandparent, userDrawnGroup;
    private GameObject groupParent, grandParent;
    
    public Material blue, red, green, yellow, purple, pink, orange;
    private Material currentColour;
    private Dictionary<string, Material> colourDict;

    public GameObject theWrist;

    public LineRenderer currentLineRenderer;
    private LineRenderer bezierLineRenderer;

    [SerializeField]
    private float minDistance = 0.01f;

    public int newVal;

    private Vector3 lastPos, indexPos;

    float lastAngle;

    int segmentCounter = 0;

  

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
        setColourInt(1);
    }

    public void setColourInt(int col)
    {
        Material[] colorArray = { blue, red, green, yellow, purple, pink, orange };
        
        if (col >= 0 && col < colorArray.Length)
        {
            currentColour = colorArray[col];
        }
        else
        {
            Debug.LogWarning($"Invalid color index {col}. Using default color.");
            currentColour = blue;
        }
    }

    public void SetPosition(Vector3 pos) { indexPos = pos;}

    private void SetupBrushHierarchy()
    {
        CreateGrandparent();
        CreateParent();
        CreateFirstNode();
    }


    public void CreateBrush()
    {
        brushInstance = Instantiate(brushUser);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();
        currentLineRenderer.material = currentColour;
        
        currentLineRenderer.SetPosition(0, indexPos);
        currentLineRenderer.SetPosition(1, indexPos);
        
        lastPos = indexPos;

        SetupBrushHierarchy();
    }

    void CreateFirstNode()
    {
        GameObject sphereNodeInstanceLineStart = null;
        if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
        {
            sphereNodeInstanceLineStart = Instantiate(noteNode);
            Debug.LogError("oe note node init");
        }
        else if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
        {
            sphereNodeInstanceLineStart = Instantiate(smallNodeNonInteractable);
            Debug.LogError("tmt small node init");
        }

        if (sphereNodeInstanceLineStart != null){
            //NODE AT START OF EACH LINE
            sphereNodeInstanceLineStart.transform.position = indexPos;
            sphereNodeInstanceLineStart.name = "NodeP0_" + "Line_" + InteractionManager.Instance.getGlobalLineCounter() + "_Segment_" + 0;
            sphereNodeInstanceLineStart.transform.SetParent(groupParent.transform);
            sphereNodeInstanceLineStart.GetComponent<MeshRenderer>().material = currentColour;
            sphereNodeInstanceLineStart.GetComponent<NodeSphereInteractable>().SetSegmentNumber(0);
            sphereNodeInstanceLineStart.GetComponent<NodeSphereInteractable>().SetNodeNumber(0);
            sphereNodeInstanceLineStart.GetComponent<NodeSphereInteractable>().SetLineNumber(InteractionManager.Instance.getGlobalLineCounter());
            sphereNodeInstanceLineStart.GetComponent<NodeSphereInteractable>().SetNotdeType(NodeSphereInteractable.NodeType.StartNoteNode);
        }
    }

    public void AddPoint()
    {
        if(Vector3.Distance(indexPos, lastPos) > minDistance && currentLineRenderer != null)
        {
            currentLineRenderer.positionCount++;
            currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, indexPos);
            lastPos = indexPos;
        }
        else if (currentLineRenderer == null)
        {
            Debug.LogError("current line renderer null");
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
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();
        currentLineRenderer.material = currentColour;
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
        groupParent.transform.name = "Line_" + InteractionManager.Instance.getGlobalLineCounter() + "_parent"; 
        groupParent.GetComponent<LineGroupScript>().lineNumber = InteractionManager.Instance.getGlobalLineCounter();
        currentLineRenderer.transform.SetParent(groupParent.transform);
        groupParent.transform.SetParent(grandParent.transform);
    }

    void DrawnLineToBeziers(bool fromWrist)
    {
        // get current line's points, conv to list
        Vector3[] currentPoints = new Vector3[currentLineRenderer.positionCount];
        currentLineRenderer.GetPositions(currentPoints);
        List<Vector3> processedPointsList = CurvePreprocess.RemoveDuplicates(currentPoints.ToList());

        // get the bez curve(s)
        CubicBezier[] curves = CurveFit.Fit(processedPointsList, 0.02f);
        groupParent.GetComponent<LineGroupScript>().setCurves(curves);

        for (int n = 0; n < curves.Length; n++)
        {
            Vector3[] bezPoints = BezierLineFunctions.BezierInterp(curves[n].p0, curves[n].p1, curves[n].p2, curves[n].p3, 
                                                                    currentLineRenderer, curves.Length);
            float dist = Vector3.Distance(curves[n].p0, curves[n].p3);

            DrawBezier(bezPoints, groupParent);
            
            if (n > 0 && n < curves.Length) 
            {
                GameObject sphereNodeInstanceAlongLine;
                
                if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded){
                    sphereNodeInstanceAlongLine = Instantiate(smallNode);
                }else{
                    sphereNodeInstanceAlongLine = Instantiate(smallNodeNonInteractable);
                }

                //NODE AT START OF EACH SEGEMENT
                sphereNodeInstanceAlongLine.transform.position = curves[n].p0;
                sphereNodeInstanceAlongLine.name = "NodeP0_" + "Line_" + InteractionManager.Instance.getGlobalLineCounter() + "_Segment_" + segmentCounter;
                sphereNodeInstanceAlongLine.transform.SetParent(groupParent.transform);
                sphereNodeInstanceAlongLine.GetComponent<MeshRenderer>().material = currentColour;
                sphereNodeInstanceAlongLine.GetComponent<NodeSphereInteractable>().SetSegmentNumber(segmentCounter);
                sphereNodeInstanceAlongLine.GetComponent<NodeSphereInteractable>().SetNodeNumber(0);
                sphereNodeInstanceAlongLine.GetComponent<NodeSphereInteractable>().SetLineNumber(InteractionManager.Instance.getGlobalLineCounter());
                sphereNodeInstanceAlongLine.GetComponent<NodeSphereInteractable>().SetNotdeType(NodeSphereInteractable.NodeType.SmallNode);
            }

            //ADD NODE TO END
            if (n == curves.Length - 1)
            {
                GameObject sphereNodeInstanceLineEnd = null;

                if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded){
                    sphereNodeInstanceLineEnd = Instantiate(noteNode);
                } else{
                    sphereNodeInstanceLineEnd = Instantiate(smallNodeNonInteractable);
                }

                if (sphereNodeInstanceLineEnd != null)
                {
                    sphereNodeInstanceLineEnd.transform.position = curves[n].p3;
                    sphereNodeInstanceLineEnd.name = "NodeP3_" + "Line_" + InteractionManager.Instance.getGlobalLineCounter() + "_Segment_" + segmentCounter;
                    sphereNodeInstanceLineEnd.transform.SetParent(groupParent.transform);
                    sphereNodeInstanceLineEnd.GetComponent<MeshRenderer>().material = currentColour;
                    sphereNodeInstanceLineEnd.GetComponent<NodeSphereInteractable>().SetSegmentNumber(segmentCounter);
                    sphereNodeInstanceLineEnd.GetComponent<NodeSphereInteractable>().SetNodeNumber(3);
                    sphereNodeInstanceLineEnd.GetComponent<NodeSphereInteractable>().SetLineNumber(InteractionManager.Instance.getGlobalLineCounter());

                    if (fromWrist){sphereNodeInstanceLineEnd.GetComponent<NodeSphereInteractable>().SetNotdeType(NodeSphereInteractable.NodeType.MiddleNoteNode);}
                    else{sphereNodeInstanceLineEnd.GetComponent<NodeSphereInteractable>().SetNotdeType(NodeSphereInteractable.NodeType.EndNoteNode);}
                }
            }
            segmentCounter++;
        }
        segmentCounter = 0;
        InteractionManager.Instance.incrementGlobalLineCounter();
        Destroy(currentLineRenderer);
        Destroy(brushInstance);

        if (!fromWrist){ grandParent.GetComponent<LineGrandparent>().setup = true;}
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
            if (k > 1){ bezierLineRenderer.positionCount++; }
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
            bezierLineRenderer.startWidth = 0.008f; 
            bezierLineRenderer.endWidth = 0.008f;
            meshCollider.sharedMesh.name = "Mesh_" + bezierLineRenderer.name;
        }
        bezierLineRenderer = null;
    }


}
