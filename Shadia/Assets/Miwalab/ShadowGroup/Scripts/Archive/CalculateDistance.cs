using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;

public class CalculateDistance : MonoBehaviour {


    public bool ChooseReadData1 = false;
    public bool ChooseReadData2 = false;
    public bool ChooseSaveData = false;
    public bool CalculateStart = false;

    string ReadDataPath1;
    string ReadDataPath2;
    string SaveDataPath;

    BinaryReader reader;
    BinaryReader reader2;

    Thread thread;


    // Use this for initialization
    void Start () {
	
        

	}
	
	// Update is called once per frame
	void Update () {

        if (ChooseReadData1)
        {
            OpenFileDialog.OpenFileDialog.Read(ref ReadDataPath1);

            ChooseReadData1 = false;
        }

        if (ChooseReadData2)
        {
            OpenFileDialog.OpenFileDialog.Read(ref ReadDataPath2);

            ChooseReadData2 = false;
        }

        if (ChooseSaveData)
        {
            SaveDataPath = "";
            OpenFileDialog.OpenFileDialog.Save(ref SaveDataPath);

            ChooseSaveData = false;
        }

        if (CalculateStart)
        {
            if (ReadDataPath1 != null)
            {
                this.reader = new BinaryReader(File.OpenRead(ReadDataPath1));

            }
            if (ReadDataPath2 != null)
            {
                this.reader2 = new BinaryReader(File.OpenRead(ReadDataPath2));

            }

            this.thread = new Thread(new ThreadStart(this.Calculate));
            this.thread.Start();

            CalculateStart = false;

        }

    }

    void Calculate()
    {

        try
        {
            while (true)
            {
                

            }
        }
        catch
        {

        }

    }

}
