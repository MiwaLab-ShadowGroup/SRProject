using UnityEngine;
using System.Collections;
using System.IO;

public class CheckData : MonoBehaviour {


    public bool ChooseFile;
    public bool CheckStart;
    public bool Finish;
    private string OpenFilePath;
    private string SaveFilePath;
    StreamWriter streamwriter;

    BinaryReader reader;

    bool IsOpenFile;
    bool IsSaveFile;

    string time;
    int datalength;
    public ushort[] ReadDepthData;
    public bool finish = false;
    public bool SaveFileChoose;


    // Use this for initialization
    void Start () {
	    


	}
	
	// Update is called once per frame
	void Update () {

        if (ChooseFile)
        {
            OpenFileDialog.OpenFileDialog.Read(ref OpenFilePath);
            IsOpenFile = true;
            ChooseFile = false;
        }

        if (SaveFileChoose)
        {

            OpenFileDialog.OpenFileDialog.Save(ref SaveFilePath);

            SaveFileChoose = false;
            IsSaveFile = true;
        }

        if (CheckStart)
        {
            if (IsOpenFile && IsSaveFile)
            {

                while (true)
                {

                    this.time = reader.ReadString();
                    this.datalength = this.reader.ReadInt32();

                    for (int i = 0; i < datalength; ++i)
                    {
                        this.ReadDepthData[i] = this.reader.ReadUInt16();

                    }

                    streamwriter.Write(time);
                    streamwriter.Write(",");

                    if (finish)
                    {
                        Debug.Log("finish");
                        break;
                    }

                    if (reader.PeekChar() == -1)
                    {
                        Debug.Log("end");
                        reader.Close();
                        break;
                    }

                }
            }
        }

    }
}
