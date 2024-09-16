using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using MixedReality.Toolkit.Input;

/*string path = Path.Combine(Application.persistentDataPath, "MyFile.txt");
string myText = "this is some text";

using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write)) 
{
    using (var writer = new StreamWriter(file))
    {
        writer.Write("come on you");
    }
}
*/

public class LogManager : MonoBehaviour
{
    private static LogManager _instance;
    public static LogManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("LogManager is NUll!");
            return _instance;
        }
    }

    List<string> logsToDump;

    public int oe_numLinesDrawn = 0;
    public int oe_numNoteNodes = 0;
    public int oe_numTimesNodesPlayed = 0;
    public int oe_numScupltInteraction = 0;
    public int oe_numDeleteInteractionsByUser = 0;
    public float oe_attentionTime = 0.0f;

    public int tmt_numLinesDrawn = 0;
    public float tmt_timeTakenToComplete = 0.0f;
    public int tmt_numIncorrectHits = 0;
    public int tmt_numCorrectHitsStreak = 0;
    public float tmt_attentionTime = 0.0f;
    public float tmt_numScupltInteraction = 0;
    public int tmt_numDeleteInteractionsByUser = 0;

    public int osc_critical_moment = 0;
    public int osc_dlt_all = 0;

    private int osc_cg_focus = 0;
    private int osc_cg_usability = 0;
    private int osc_cg_vocal = 0;
    private int osc_cg_physical = 0;



    float oeTotalTime = 0f;
    float oeLastUpdateTime = 0f;
    float oeAttentionLastUpdateTime = 0f;
    public bool oeStartStop = false;

    float tmtTotalTime = 0f;
    float tmtLastUpdateTime = 0f;
    float tmtAttentionLastUpdateTime = 0f;
    public bool tmtStartStop = false;





    private void Awake()
    {
        _instance = this;
        Debug.Log("hi from the LogManager");
    }

    void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, "MTAR_Logs.txt");
        using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            using (var writer = new StreamWriter(file))
            {
                writer.Write("MTAR - Logs - Date: " + DateTime.Now.ToString("MM-dd HH:mm:ss"));
            }
        }

        logsToDump = new List<string>();
    }

    private void Update()
    {
        if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
        {
            //Debug.LogWarning("oe time " + oeTotalTime + "   oe attention time " + oe_attentionTime);

            float currentTime = Time.time;
            float deltaTime = currentTime - oeLastUpdateTime;
            float deltaTimeattention = currentTime - oeAttentionLastUpdateTime;

            if (oeStartStop)
            {
                oeTotalTime += deltaTime;
            }
            if (oeStartStop && AttentionAreaScript.Instance.ObjectDetectionforAttentionOE() > 0) {
                oe_attentionTime += deltaTimeattention;
            }
            oeLastUpdateTime = currentTime;
            oeAttentionLastUpdateTime = currentTime;
        }

        if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
        {
           // Debug.LogWarning("tmt time " + tmtTotalTime + "   tmt attention time " + tmt_attentionTime);

            float currentTime = Time.time;
            float deltaTime = currentTime - tmtLastUpdateTime;
            float deltaTimeattention = currentTime - tmtAttentionLastUpdateTime;

            if (tmtStartStop)
            {
                tmtTotalTime += deltaTime;
            }
            if (tmtStartStop && AttentionAreaScript.Instance.ObjectDetectionforAttentionTMT() > 0)
            {
                tmt_attentionTime += deltaTimeattention;
            }
            tmtLastUpdateTime = currentTime;
            tmtAttentionLastUpdateTime = currentTime;
        }
    }

    public void AppendText(string newText)
    {
        string path = Path.Combine(Application.persistentDataPath, "MTAR_Logs.txt");
        string time = DateTime.Now.ToString("HH:mm:ss");
        using (StreamWriter outputFile = new StreamWriter(path, true))
        {
            outputFile.WriteLine(time + "--" + newText);
        }
    }

    public void DumpLogsToFile() {
        foreach (string log in logsToDump) {
            AppendText(log);
        }
    }


    public void MakeLogMessages()
    {
        logsToDump.Clear();
        logsToDump.Add("------------");
        logsToDump.Add("---- " + DateTime.Now.ToString("MM - dd HH:mm:ss") + " ----");
        logsToDump.Add("------------");


        logsToDump.Add("OE_Num_Lines_                " + oe_numLinesDrawn);
        logsToDump.Add("OE_Num_Note_Nodes_           " + oe_numNoteNodes);
        logsToDump.Add("OE_Num_Note_Nodes_Played_    " + oe_numTimesNodesPlayed);
        logsToDump.Add("OE_Num_Sculpt_Interactions_  " + oe_numScupltInteraction);
        logsToDump.Add("OE_Num_Delete_Interactions_  " + oe_numDeleteInteractionsByUser);
        logsToDump.Add("OE_Attention_Time_           " + oe_attentionTime);
        logsToDump.Add("OE_Total_Time_               " + oeTotalTime);

        logsToDump.Add("TMT_Num_Lines_               " + tmt_numLinesDrawn);
        logsToDump.Add("TMT_Comp_Time_               " + tmt_timeTakenToComplete);
        logsToDump.Add("TMT_Num_Incorrect_           " + tmt_numIncorrectHits);
        logsToDump.Add("TMT_Num_Correct_Streak_      " + tmt_numCorrectHitsStreak);
        logsToDump.Add("TMT_Attention_Time_          " + tmt_attentionTime);
        logsToDump.Add("TMT_Total_Time_              " + tmtTotalTime);
        logsToDump.Add("TMT_Num_Sculpt_Interactions_ " + tmt_numScupltInteraction);
        logsToDump.Add("TMT_Num_Delete_Interactions_ " + tmt_numDeleteInteractionsByUser);

        DumpLogsToFile();

    }

    public void Critical()
    {
        osc_critical_moment++;
        AppendText("OSC_M_S_    " + osc_critical_moment);
    }

    public void DltAll()
    {
        osc_dlt_all++;
        AppendText("OSC_Dlt_All_    " + osc_dlt_all);
    }

    public void Caregiver(int n)
    {
        if (n == 0) {
            osc_cg_focus++;
            AppendText("OSC_Caregiver_Focus_     " + osc_cg_focus);
        }
        else if (n == 1) {
            osc_cg_usability++;
            AppendText("OSC_Caregiver_Usability_     " + osc_cg_usability);
        }
        else if (n == 2) {
            osc_cg_vocal++;
            AppendText("OSC_Caregiver_Vocal_     " + osc_cg_vocal);
        }
        else if (n == 3) {
            osc_cg_physical++;
            AppendText("OSC_Caregiver_Physical_    " + osc_cg_physical);
        }

    }

    public void Headset(float n)
    {
        if(n == 0.0f)
        {
            AppendText("OSC_Headset_Off");
        }
        if(n == 1.0f)
        {
            AppendText("OSC_Headset_On");
        }
    }

    public void IncrementNumLines()
    {
        if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
        {
            oe_numLinesDrawn++;
        }

        else if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
        {
            tmt_numLinesDrawn++;
        }
    }

    public void IncrementNumSculpt()
    {
        if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
        {
            oe_numScupltInteraction++;
        }

        else if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
        {
            tmt_numScupltInteraction++;
        }
    }

    public void IncrementNumDelete()
    {
        if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
        {
            oe_numDeleteInteractionsByUser++;
        }

        else if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
        {
            tmt_numDeleteInteractionsByUser++;
        }
    }

    public void UpdateAttentionTime(float t)
    {
        if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
        {
            oe_attentionTime += t;
        }

        else if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
        {
            tmt_attentionTime += t;
        }
    }

    public void ResetForTmt()
    {
        tmt_numLinesDrawn = 0;
        tmt_timeTakenToComplete = 0.0f;
        tmt_numIncorrectHits = 0;
        tmt_numCorrectHitsStreak = 0;
        tmt_attentionTime = 0.0f;
        tmt_numScupltInteraction = 0;
        tmt_numDeleteInteractionsByUser = 0;
        tmtTotalTime = 0f;
        tmtLastUpdateTime = 0f;
        tmtAttentionLastUpdateTime = 0f;
    }

}
