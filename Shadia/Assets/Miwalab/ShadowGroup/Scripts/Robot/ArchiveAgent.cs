using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using System;

public class ArchiveAgent : MonoBehaviour {

    public GameObject robot;
    BinaryReader reader;
    

    bool Isreader = true;
    Thread thread;

    FPSAdjuster.FPSAdjuster FpsAd;

    public bool OpenFileChoose = false;

    string FilePath;

    public bool ReadStart = false;

    public bool ReadStop = false;

    bool IsStart = false;

    public bool PausePlay = false;

    public Vector3 robotpos = Vector3.zero;
    int frame;
    public bool IsRead;


    // Use this for initialization
    void Start () {

        (ShadowMediaUIHost.GetUI("ChooseRobotFile") as ParameterButton).Clicked += ChooseRobotFile_Clicked;
        (ShadowMediaUIHost.GetUI("PlayRobotStart") as ParameterButton).Clicked += PlayRobotStart_Clicked;

        this.FpsAd = new FPSAdjuster.FPSAdjuster();
        this.FpsAd.Fps = 23;
        this.FpsAd.Start();
    }

    private void PlayRobotStart_Clicked(object sender, EventArgs e)
    {
        if (FilePath != null)
        {
            this.reader = new BinaryReader(File.OpenRead(FilePath));
            this.thread = new Thread(new ThreadStart(this.ReadDepth));
            this.thread.Start();
            this.Isreader = true;
        }
    }

    private void ChooseRobotFile_Clicked(object sender, EventArgs e)
    {
        OpenFileDialog.OpenFileDialog.Read(ref FilePath);

    }

    // Update is called once per frame
    void Update () {

        if (Isreader)
        {
            this.robot.transform.position = this.robotpos;

        }
    }

    void ReadDepth()
    {
        //unsafe
        //{
        try
        {

            while (true)
            {
                FpsAd.Adjust();
                
                    if (Isreader)
                    {
                        frame = this.reader.ReadInt32();
                        robotpos.x = this.reader.ReadSingle();
                        robotpos.z = this.reader.ReadSingle();
                        robotpos.y = this.reader.ReadSingle();

                        robotpos.y = 0;

                        if (reader.PeekChar() == -1)
                        {
                            Debug.Log("end");
                            reader.Close();
                            Isreader = false;
                        }

                        if (ReadStop)
                        {
                            Isreader = false;
                            ReadStop = false;
                        }

                    }
                    else
                    {
                        if (reader != null)
                        {
                            reader.Close();

                        }

                        break;
                    }

                }
            
        }
        catch
        {

        }
    


    }

    void OnDestroy()
    {
        if (thread != null)
        {
            thread.Abort();

        }
        if (FilePath != null)
        {
            FilePath = null;
        }
        if (reader != null)
        {
            reader.Close();
        }
       
    }


}
