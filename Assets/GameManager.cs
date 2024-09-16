using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Error: No GameManager");
            return _instance;
        }
    }

    private void Awake() => _instance = this;

    public GameObject userDrawnGroup;
    private float duration = 15.0f;// 2.2857f * 2.0f;
    private float timer = 0f;



    public enum GameMode
    {
        OpenEnded,
        TMT
    }


    public GameMode gameMode;

    public int numHandsSculpting = 0;

    public GameObject brushOE;
    public GameObject brushTMT;
    public GameObject currentBrushType;

    public GameObject rightDrawObj;
    public GameObject leftDrawObj;

    public int noteNodeID = 0;
    public int smallNodeID = 10000;


    public GameObject usrDrawn;

    public string drawnLayer = "Drawn";
    public bool drawnVisible = true;

    // Start is called before the first frame update
    void Start()
    {

        gameMode = GameMode.OpenEnded;
        currentBrushType = brushOE;


        /*        gameMode = GameMode.TMT;
                TMTManager.Instance.StartTMTA();
                LogManager.Instance.tmtStartStop = true;*/
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            timer = 0f;

/*            if (gameMode == GameMode.OpenEnded)
            {
                SetGameMode(GameMode.TMT);
                Debug.Log("gm now is " + gameMode + "drawn visible is " + drawnVisible);
            }

            else if (gameMode == GameMode.TMT)
            {
                SetGameMode(GameMode.OpenEnded);
                Debug.Log("gm now is " + gameMode + "drawn visible is " + drawnVisible);


            }*/


            /*            if (GameManager.Instance.gameMode == GameMode.OpenEnded)
                        {
                            if (LogManager.Instance.oeStartStop == false) { LogManager.Instance.oeStartStop = true; }
                            else if (LogManager.Instance.oeStartStop == true) { LogManager.Instance.oeStartStop = false; }
                        }

                        if (GameManager.Instance.gameMode == GameMode.TMT)
                        {
                            if (LogManager.Instance.tmtStartStop == false) { LogManager.Instance.tmtStartStop = true; }
                            else if (LogManager.Instance.tmtStartStop == true) { LogManager.Instance.tmtStartStop = false; }
                        }*/



            /*            if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
                        {
                            GameManager.Instance.gameMode = GameManager.GameMode.OpenEnded;
                            TMTManager.Instance.tearDownTMT();
                        }

                        if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
                        {
                            GameManager.Instance.gameMode = GameManager.GameMode.TMT;
                            TMTManager.Instance.StartTMTA();
                        }*/
        }
    }

    public void deleteAllLines()
    {
        if(userDrawnGroup != null)
        {
            foreach(Transform child in userDrawnGroup.transform)
            {
                Destroy(child.gameObject);
                EXTOSCManager.Instance.appendText(child.gameObject.name + " deleted");
            }
        }

    }

    public void deleteAllTmtLines()
    {
        int layer = LayerMask.NameToLayer("TMT");
        Debug.Log("tmt layer number " + layer);

        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        Debug.Log("all objects = " + allObjects.Length);

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == layer)
            {
                Destroy(obj);
            }
        }
    }

    public void SetGameMode(GameMode _gameMode)
    {
        LogManager.Instance.MakeLogMessages();

        if (_gameMode == GameMode.OpenEnded)
        {
            if (gameMode == GameMode.TMT) 
            {
                LogManager.Instance.tmtStartStop = false;
                gameMode = GameMode.OpenEnded;
                ToggleDrawnLayerVisible(true);
                currentBrushType = brushOE;
                TMTManager.Instance.tearDownTMT();
                deleteAllTmtLines();
                UpdateBrushes();
            }
        }

        if (_gameMode == GameMode.TMT)
        { 
            if (gameMode == GameMode.OpenEnded)
            {
                LogManager.Instance.oeStartStop = false;
                InteractionManager.Instance.TMT_Collider_Ball.transform.position = new Vector3(0f, 0f, 0f);
                gameMode = GameMode.TMT;
                ToggleDrawnLayerVisible(false);
                currentBrushType = brushTMT;
                TMTManager.Instance.StartTMTA();
                UpdateBrushes();
            }
        }
    }

    public void UpdateBrushes() {
        rightDrawObj.GetComponent<Draw_Line>().brushBezier = currentBrushType;
        rightDrawObj.GetComponent<Draw_Line>().brushUser = currentBrushType;

        leftDrawObj.GetComponent<Draw_Line>().brushBezier = currentBrushType;
        leftDrawObj.GetComponent<Draw_Line>().brushUser = currentBrushType;

    }

    public void ToggleDrawnLayerVisible(bool _visible)
    {
        drawnVisible = _visible;
        int layer = LayerMask.NameToLayer(drawnLayer);
        Debug.Log("drawn layer number " + layer);

        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        Debug.Log("all objects = " + allObjects.Length);

        foreach(GameObject obj in allObjects)
        {
            if(obj.layer == layer)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if(renderer != null)
                {
                renderer.enabled = drawnVisible;
                }
            }
        }
    }



}
