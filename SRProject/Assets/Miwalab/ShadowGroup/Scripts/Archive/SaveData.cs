using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEditor;
using System;
using UnityEngine.UI;
using OpenCvSharp.CPlusPlus;
[RequireComponent(typeof(KinectImporter))]
public class SaveData : MonoBehaviour {

    BinaryWriter writer;

    Thread thread;

    KinectImporter kinect;

    //CameraImporter camera;


    string FolderPath;

    FPSAdjuster.FPSAdjuster FpsAd;

    bool OpenFileChoose = false;

    bool Savestart = false;

    bool IsSaveStop = false;

    string filename;

    DateTime datetime;
    TimeSpan timestump;
    private Text m_SaveNameTextUI;

    ushort[] _savedDepthBuffer;

    private void SaveStart_Clicked(object sender, EventArgs e)
    {
        if(FolderPath != null)
        {
            this.writer = new BinaryWriter(File.OpenWrite(FolderPath + @"\" + this.m_SaveNameTextUI.text));
            thread = new Thread(new ThreadStart(SaveDepth));
            thread.Start();
        }
       
    }

    

    // Use this for initialization
    void Start () {
        kinect = gameObject.GetComponent<KinectImporter>();
        //this.pointcloud = pointCloudShadow.GetComponent<PointCloud>();
        (UIHost.GetUI("ChooseFolder") as ParameterButton).Clicked += ChooseFolder_Clicked;
        this.m_SaveNameTextUI = (UIHost.GetUI("SaveFileName") as ParameterText).m_valueText;

        (UIHost.GetUI("SaveStart") as ParameterButton).Clicked += SaveStart_Clicked;
        (UIHost.GetUI("SaveStop") as ParameterButton).Clicked += SaveStop_Clicked;

        this.FpsAd = new FPSAdjuster.FPSAdjuster();
        this.FpsAd.Fps = 30;
        this.FpsAd.Start();
        //Debug.Log("start1");

    }

    private void ChooseFolder_Clicked(object sender, EventArgs e)
    {
        FolderPath = EditorUtility.SaveFolderPanel("フォルダ選択", " ", " ");

    }

    private void OpenFileChoose_ValueChanged(object sender, System.EventArgs e)
    {
        this.OpenFileChoose = (e as ParameterCheckbox.ChangedValue).Value;
    }

    private void SaveStop_Clicked(object sender, EventArgs e)
    {
        this.IsSaveStop = true;
    }

    // Update is called once per frame
    void Update () {

        
    }

    void SaveDepth()
    {
        unsafe {
            try
            {
                this._savedDepthBuffer = this.kinect.SaveDepth;
                Mat mat = new Mat(new Size(512, 424), MatType.CV_16U);
                while (true)
                {
                    this.FpsAd.Adjust();
                    ushort* ptr = (ushort*)mat.Data;
                    for (int i=0; i < 512*424; ++i)
                    {
                        ptr[i] = _savedDepthBuffer[i];
                    }

                    byte[] data = mat.ToBytes(".png");
                    
                    datetime = DateTime.Now;
                    timestump = datetime.TimeOfDay;
                    //Debug.Log(framecount);
                    writer.Write(timestump.ToString());
                    writer.Write(data.Length);

                    writer.Write(data);
                    //framecount++;
                    if (this.IsSaveStop)
                    {
                        this.IsSaveStop = false;
                        break;
                    }

                }
                writer.Close();

            }
            catch
            {

            }
        }
    }

    void SaveCamera()
    {

    }

    void OnDestroy()
    {
        if (thread != null)
        {
            thread.Abort();

        }
        if (FolderPath != null)
        {
            FolderPath = null;
        }
    }
}
