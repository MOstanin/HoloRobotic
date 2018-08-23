using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using HoloToolkit.Unity;

#if !UNITY_EDITOR
using System.Threading.Tasks;
#endif

public class MyNetworking : Singleton<MyNetworking>
{
    /*
    [SerializeField]
    String host = "10.240.20.232";

    [SerializeField]
    String port = "6666";

    //public USTrackingManager TrackingManager;
    //public USStatusTextManager StatusTextManager;
    public readonly static Queue<Action> ExecuteOnMainThread = new Queue<Action>();

#if !UNITY_EDITOR
    private bool _useUWP = true;
    private Windows.Networking.Sockets.StreamSocket socket;
    private Task exchangeTask;
#endif

#if UNITY_EDITOR
    private bool _useUWP = false;
    System.Net.Sockets.TcpClient client;
    System.Net.Sockets.NetworkStream stream;
    private Thread exchangeThread;
#endif

    private Byte[] bytes = new Byte[256];
    private StreamWriter writer;
    private StreamReader reader;

    public void Connect(string host, string port)
    {
        if (_useUWP)
        {
            ConnectUWP(host, port);
        }
        else
        {
            ConnectUnity(host, port);
        }
    }
#if !UNITY_EDITOR
    public async void Connect()
    {
        try
        {
            // Create the StreamSocket and establish a connection to the echo server.
            using (var streamSocket = new Windows.Networking.Sockets.StreamSocket())
            {
                // The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
                var hostName = new Windows.Networking.HostName(host);
                
                await streamSocket.ConnectAsync(hostName, port);
                

                // Send a request to the echo server.
                string request = GetQ();
                using (Stream outputStream = streamSocket.OutputStream.AsStreamForWrite())
                {
                    using (var streamWriter = new StreamWriter(outputStream))
                    {
                        await streamWriter.WriteLineAsync(request);
                        await streamWriter.FlushAsync();
                    }
                }
                

                // Read data from the echo server.
                string response;
                using (Stream inputStream = streamSocket.InputStream.AsStreamForRead())
                {
                    using (StreamReader streamReader = new StreamReader(inputStream))
                    {
                        response = await streamReader.ReadLineAsync();
                    }
                }
                
            }
            
        }
        catch (Exception ex)
        {
            Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
            
        }
    }
#else
    public void Connect()
    {
        ConnectUnity(host, port);
    }
#endif
#if UNITY_EDITOR
        private void ConnectUWP(string host, string port)
#else
    private async void ConnectUWP(string host, string port)
#endif
    {
#if UNITY_EDITOR
        errorStatus = "UWP TCP client used in Unity!";
#else
        try
        {
            if (exchangeTask != null) StopExchange();
        
            socket = new Windows.Networking.Sockets.StreamSocket();
            Windows.Networking.HostName serverHost = new Windows.Networking.HostName(host);
            await socket.ConnectAsync(serverHost, port);
        
            Stream streamOut = socket.OutputStream.AsStreamForWrite();
            writer = new StreamWriter(streamOut) { AutoFlush = true };
        
            Stream streamIn = socket.InputStream.AsStreamForRead();
            reader = new StreamReader(streamIn);

            RestartExchange();
            successStatus = "Connected!";
        }
        catch (Exception e)
        {
            errorStatus = e.ToString();
        }
#endif
    }

    private void ConnectUnity(string host, string port)
    {
#if !UNITY_EDITOR
        errorStatus = "Unity TCP client used in UWP!";
#else
        try
        {
            if (exchangeThread != null) StopExchange();

            client = new System.Net.Sockets.TcpClient(host, Int32.Parse(port));
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };

            RestartExchange();
            successStatus = "Connected!";
            SendMessageToRobot();

        }
        catch (Exception e)
        {
            errorStatus = e.ToString();
        }
#endif
    }

    private bool exchanging = false;
    private bool exchangeStopRequested = false;
    private string lastPacket = null;

    private string errorStatus = null;
    private string warningStatus = null;
    private string successStatus = null;
    private string unknownStatus = null;

    public void RestartExchange()
    {
#if UNITY_EDITOR
        if (exchangeThread != null) StopExchange();
        exchangeStopRequested = false;
        exchangeThread = new System.Threading.Thread(ExchangePackets2);
        exchangeThread.Start();
#else
        if (exchangeTask != null) StopExchange();
        exchangeStopRequested = false;
        exchangeTask = Task.Run(() => ExchangePackets2());
#endif
    }


    public void Update()
    {
        while (ExecuteOnMainThread.Count > 0)
        {
            ExecuteOnMainThread.Dequeue().Invoke();
        }
        

        if (errorStatus != null)
        {
            //StatusTextManager.SetError(errorStatus);
            Debug.Log(errorStatus);
            errorStatus = null;
        }
        if (warningStatus != null)
        {
            //StatusTextManager.SetWarning(warningStatus);
            Debug.Log(warningStatus);
            warningStatus = null;
        }
        if (successStatus != null)
        {
            //StatusTextManager.SetSuccess(successStatus);
            Debug.Log(successStatus);
            successStatus = null;
        }
        if (unknownStatus != null)
        {
            //StatusTextManager.SetUnknown(unknownStatus);
            Debug.Log(unknownStatus);
            unknownStatus = null;
        }
    }
    /*
    public void SendMessageToRobot()
    {
        message = GetQ();
    }
    string message;
    */
    /*
#if UNITY_EDITOR
    public void ExchangePackets()
    {
        while (!exchangeStopRequested)
        {
            if (writer == null || reader == null) continue;
            exchanging = true;

            writer.Write("Xasdas\n");
            Debug.Log("Sent data!");
            string received = null;
            byte[] bytes = new byte[client.SendBufferSize];
            int recv = 0;
            while (true)
            {
                recv = stream.Read(bytes, 0, client.SendBufferSize);
                received += Encoding.ASCII.GetString(bytes, 0, recv);
                if (received.EndsWith("\n")) break;
            }
            lastPacket = received;

            Debug.Log("Read data: " + received);

            exchanging = false;
        }
    }
#else
    public async void ExchangePackets()
    {
        while (!exchangeStopRequested)
        {
            if (writer == null || reader == null) continue;
            exchanging = true;

            writer.Write("Xasdas\n");
            Debug.Log("Sent data!");
            string received = null;
            //received = reader.ReadLine();
            received = await reader.ReadLineAsync();


            if (ExecuteOnMainThread.Count == 0)
            {
                ExecuteOnMainThread.Enqueue(() =>
                {
                    lastPacket = received;

                });
            }
            //lastPacket = received;
            Debug.Log("Read data: " + received);

            exchanging = false;
        }
    }
#endif
    public void ExchangePackets2()
    {
        while (!exchangeStopRequested)
        {
            if (writer == null || reader == null) continue;
            exchanging = true;
            if (message == null) continue;
            writer.Write(message);
            Debug.Log("Sent data!");
            message = null;
            StopExchange();
        }
    }


    public void StopExchange()
    {
        exchangeStopRequested = true;

#if UNITY_EDITOR
        if (exchangeThread != null)
        {
            exchangeThread.Abort();
            stream.Close();
            client.Close();
            writer.Close();
            reader.Close();

            stream = null;
            exchangeThread = null;
        }
#else
        if (exchangeTask != null) {
            exchangeTask.Wait();
            socket.Dispose();
            writer.Dispose();
            reader.Dispose();

            socket = null;
            exchangeTask = null;
        }
#endif
        writer = null;
        reader = null;
    }

    public void OnDestroy()
    {
        StopExchange();
    }
    /*
    private string GetQ()
    {
        return AppManager.Instance.GetQ();
    }
    */

    
}

