using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class HttpServer : MonoBehaviour
{
    System.Net.HttpListener m_listener;

    // Use this for initialization
    void Start()
    {
        m_listener = new System.Net.HttpListener();
        m_listener.Prefixes.Add("http://*/");
        m_listener.Start();
        m_listener.BeginGetContext(this.receiveCallback, m_listener);
    }

    // Update is called once per frame
    void Update()
    {
        var context = m_listener.GetContext();
        var req = context.Request;
        var res = context.Response;
        var root = @"res";

        Console.WriteLine(req.RawUrl);

        // リクエストされたURLからファイルのパスを求める
        string path = root + req.RawUrl.Replace("/", "\\");

        // ファイルが存在すればレスポンス・ストリームに書き出す
        if (File.Exists(path))
        {
            byte[] content = File.ReadAllBytes(path);
            res.OutputStream.Write(content, 0, content.Length);
        }
        else
        {
            var sw = new StreamWriter(res.OutputStream);
            sw.WriteLine(path);
        }
        Debug.Log(path);
        res.Close();
    }


    private void receiveCallback(IAsyncResult ar)
    {
        var context = m_listener.EndGetContext(ar);
        m_listener.BeginGetContext(receiveCallback, m_listener);
        var req = context.Request;
        var res = context.Response;
        var root = @"res";

        Console.WriteLine(req.RawUrl);

        // リクエストされたURLからファイルのパスを求める
        string path = root + req.RawUrl.Replace("/", "\\");

        // ファイルが存在すればレスポンス・ストリームに書き出す
        if (File.Exists(path))
        {
            byte[] content = File.ReadAllBytes(path);
            res.OutputStream.Write(content, 0, content.Length);
        }
        else
        {
            var sw = new StreamWriter(res.OutputStream);
            sw.WriteLine(path);
        }
        Debug.Log(path);
        res.Close();
    }

}
