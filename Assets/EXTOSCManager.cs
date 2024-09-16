using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using System.IO;


public class EXTOSCManager : MonoBehaviour
{

    private OSCTransmitter transmitter;

    private static EXTOSCManager _instance;
    public static EXTOSCManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("EXTOSCManager is NUll!");
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        Debug.Log("hi from the OSC Manager");
    }

    // Start is called before the first frame update
    void Start()
    {
        transmitter = gameObject.AddComponent<OSCTransmitter>();
        transmitter.RemoteHost = "192.168.100.125";
        transmitter.RemotePort = 8001;

        var receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = 8001;
        receiver.Bind("/test", TestMessageReceived);

        receiver.Bind("/volume/master", VolumeMaster);
        receiver.Bind("/volume/player", VolumePlayer);
        receiver.Bind("/volume/backing", VolumeBacking);
        receiver.Bind("/volume/tmt", VolumeTmt);

        receiver.Bind("/game/deleteall", GameDeleteAll);
        receiver.Bind("/game/mode", GameMode);
        receiver.Bind("/game/hint", GameHint);
        receiver.Bind("/game/hands", GameHandsMode);
        receiver.Bind("/game/colour", GameColour);



        receiver.Bind("/tmt/layout", TMTlayout);
        receiver.Bind("/tmt/AorB", TMTAorB);

        receiver.Bind("/tmt/override", TMTOverride);

        receiver.Bind("/log/dump", MakeLogMessages);
        receiver.Bind("/log/timer", LogTimers);
        receiver.Bind("/log/critical", LogCritical);
        receiver.Bind("/log/headset", LogHeadset);

        receiver.Bind("/log/cg/focus", LogCaregriverFocus);
        receiver.Bind("/log/cg/usability", LogCaregiverUsability);
        receiver.Bind("/log/cg/vocal", LogCaregiverVocal);
        receiver.Bind("/log/cg/physical", LogCaregiverPhysical);

        receiver.Bind("game/armlength", ArmLength);

        setupOSCLogs();
    }

    protected void TMTOverride(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].IntValue;
       
        TMTManager.Instance.PractitionerOverride(value + 1);
    }

    protected void ArmLength(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        InteractionManager.Instance.armLength = value;
    }

    protected void LogCaregriverFocus(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        if (value == 1.0f)
        {
            LogManager.Instance.Caregiver(0);
        }
    }
    protected void LogCaregiverUsability(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        if (value == 1.0f)
        {
            LogManager.Instance.Caregiver(1);
        }
    }
    protected void LogCaregiverVocal(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        if (value == 1.0f)
        {
            LogManager.Instance.Caregiver(2);
        }
    }
    protected void LogCaregiverPhysical(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        if (value == 1.0f)
        {
            LogManager.Instance.Caregiver(3);
        }
    }


    protected void LogHeadset(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        if (value == 1.0f)
        {
            LogManager.Instance.Headset(value);
        }
        else if (value == 0.0f)
        {
            LogManager.Instance.Headset(value);
        }
    }

    protected void LogCritical(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        if (value == 1.0f)
        {
            LogManager.Instance.Critical();
        }
    }


    protected void LogTimers(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        if (value == 0.0f)
        {
            //stop
            if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
            {
                LogManager.Instance.oeStartStop = false;
            }
            if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
            {
                LogManager.Instance.tmtStartStop = false;
            }
        }
        else if (value == 1.0f)
        {
            //start
            if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
            {
                LogManager.Instance.oeStartStop = true;
            }
            if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
            {
                LogManager.Instance.tmtStartStop = true;
            }
        }
    }

/*    protected void OeStartStop(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        if (value == 0.0f) {
            //stop
            LogManager.Instance.oeStartStop = false;
        }
        else if( value == 1.0f)
        {
            //start
            LogManager.Instance.oeStartStop = true;
        }
    }
    protected void TmtStartStop(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        if (value == 0.0f)
        {
            //stop
            LogManager.Instance.tmtStartStop = false;
        }
        else if (value == 1.0f)
        {
            //start
            LogManager.Instance.tmtStartStop = true;
        }
    }*/


    protected void TMTAorB(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].IntValue;
        if (value < 2)
        {
            TMTManager.Instance.testAorTestB = value;
        }
    }
    protected void TMTlayout(OSCMessage message)
    {
        Debug.Log("osc tmt called?");
        appendText(message.ToString());
        var value = message.Values[0].IntValue;
        if (value < 4)
        {
            TMTManager.Instance.layout = value;
        }
    }

    // GAME MANAGER //
    protected void GameDeleteAll(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        if(value == 1.0f)
        {
            GameManager.Instance.deleteAllLines();
            LogManager.Instance.DltAll();
        }
    }
    protected void GameMode(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].IntValue;
        if (value == 1)
        {
            //GameManager.Instance.gameMode = GameManager.GameMode.OpenEnded;
            appendText("game mode = " + GameManager.Instance.gameMode.ToString());


            GameManager.Instance.SetGameMode(GameManager.GameMode.OpenEnded);

        }
        if (value == 2)
        {
            //GameManager.Instance.gameMode = GameManager.GameMode.TMT;
            appendText("game mode = " + GameManager.Instance.gameMode.ToString());

            GameManager.Instance.SetGameMode(GameManager.GameMode.TMT);

        }
    }

    protected void GameColour(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].IntValue;

        GameObject rh = GameObject.Find("RightDraw");
        rh.GetComponent<Draw_Line>().setColourInt(value);
        GameObject lh = GameObject.Find("LeftDraw");
        lh.GetComponent<Draw_Line>().setColourInt(value);
    }

    protected void GameHint(OSCMessage message)
    {
        // todo
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
    }
    protected void GameHandsMode(OSCMessage message)
    {
        // todo
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
    }

    // VOLUME //
    protected void VolumeMaster(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        float newVolume = MusicManager.Instance.setMasterVolume(value);
        appendText("master volume = " + newVolume.ToString());
    }
    protected void VolumePlayer(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        MusicManager.Instance.setDrawnNodesVolume(value);
    }
    protected void VolumeBacking(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        MusicManager.Instance.setBackingTrackVolume(value);
    }
    protected void VolumeTmt(OSCMessage message)
    {
        appendText(message.ToString());
        var value = message.Values[0].FloatValue;
        MusicManager.Instance.setTMTVolume(value);
    }

    protected void MakeLogMessages(OSCMessage message)
    {
        var value = message.Values[0].FloatValue;
        if (value == 1.0f) {
            Debug.Log("YO OSC saying DUMP THEM LOGS");
            LogManager.Instance.MakeLogMessages();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //SendMessage();
        
    }

    

    protected void TestMessageReceived(OSCMessage message)
    {
        Debug.Log("/test");
        Debug.Log(message);
    }



    void SendMessage()
    {
        var message = new OSCMessage("/music");
        message.AddValue(OSCValue.Float(0.5f));
        message.AddValue(OSCValue.Bool(false));
        transmitter.Send(message);
    }

    public void setupOSCLogs()
    {
        string path = Path.Combine(Application.persistentDataPath, "OSC_logs.txt");
        using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            using (var writer = new StreamWriter(file))
            {
                writer.Write("OSC Tracking Data");
            }
        }
    }

    public void appendText(string newText)
    {
/*        Debug.Log("osc logger append");
        string path = Path.Combine(Application.persistentDataPath, "OSC_logs.txt");
        using (StreamWriter outputFile = new StreamWriter(path, true))
        {
            outputFile.WriteLine(newText);
        }*/
    }
}
