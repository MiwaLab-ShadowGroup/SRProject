using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
//using UnityEditor;
using System;
using UnityEngine.UI;
using OpenCvSharp.CPlusPlus;

public class ReadData : MonoBehaviour
{

    BinaryReader reader;

    Thread thread;

    string FilePath;

    FPSAdjuster.FPSAdjuster FpsAd;

    public ushort[] ReadDepthData;
    string time;
    int datalength;

    public Mat playmat;
    bool PausePlay;

    public bool IsRead;

    void Start()
    {
        (UIHost.GetUI("ChooseFile") as ParameterButton).Clicked += ChooseFile_Clicked;
        (UIHost.GetUI("PlayStart") as ParameterButton).Clicked += PlayStart_Clicked;
        (UIHost.GetUI("Pause") as ParameterCheckbox).ValueChanged += Pause_ValueChanged;
        (UIHost.GetUI("Pause") as ParameterCheckbox).ValueUpdate();


        this.FpsAd = new FPSAdjuster.FPSAdjuster();
        this.FpsAd.Fps = 30;
        this.FpsAd.Start();

        playmat = new Mat(new Size(512, 424), MatType.CV_16U);
    }

    private void Pause_ValueChanged(object sender, EventArgs e)
    {
        this.PausePlay = (e as ParameterCheckbox.ChangedValue).Value;
    }

    private void PlayStart_Clicked(object sender, EventArgs e)
    {
        if (FilePath != null)
        {
            this.reader = new BinaryReader(File.OpenRead(FilePath));
            this.thread = new Thread(new ThreadStart(this.ReadDepth));
            this.thread.Start();
            this.IsRead = true;
        }

    }

    private void ChooseFile_Clicked(object sender, EventArgs e)
    {
        OpenFileDialog.OpenFileDialog.Read(ref FilePath);
    }

    void Update()
    {


    }

    void ReadDepth()
    {
        //unsafe
        //{
        try
        {

            while (true)
            {
                if (!PausePlay)
                {

                    this.FpsAd.Adjust();

                    //ushort* ptr = (ushort*)playmat.Data;

                    this.time = reader.ReadString();
                    this.datalength = this.reader.ReadInt32();



                    for (int i = 0; i < datalength; ++i)
                    {
                        this.ReadDepthData[i] = this.reader.ReadUInt16();

                    }



                    if (reader.PeekChar() == -1)
                    {
                        Debug.Log("end");
                        reader.Close();
                        this.IsRead = false;
                    }
                }
            }
        }
        catch
        {

        }


        // }


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
        if(this.IsRead == true)
        {
            IsRead = false;
        }
    }

}
