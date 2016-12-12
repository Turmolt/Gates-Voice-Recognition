using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;


/****************************************************************************************
 *    Sam Gates's Voice Recognition Server Variant                                      *
 *    Must use RecoServer created by ZJP at the link below                              *
 *    https://forum.unity3d.com/threads/windows-udp-voice-recognition-server.172758/    *
 *                                                                                      *
 *    Read through the comments to gain a better understanding of what is happening!    *
 *                                                                                      *
 ****************************************************************************************/
public class RecoServer : MonoBehaviour
{
    
    Thread receiveThread;
    UdpClient client;

    //Pick a port to recieve data on, must match the port running on reco server
    public int port = 9090;
    string strReceiveUDP = "";
    string LocalIP = String.Empty;
    string hostname;


    public GameManager gm;
    

    bool newData = false;

    public void Start()
    {
        Application.runInBackground = true;
        init();
    }

    // init creates a thread which runs in the background
    // this is used to recieve voice data from the voice server with little down time
    private void init()
    {
        //start the thread, pass it the function that you wish it to run
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        //aquire the host name from your local dns
        hostname = Dns.GetHostName();
        IPAddress[] ips = Dns.GetHostAddresses(hostname);
        
        //ips will have a length greater than 0 if it actually was able to connect to the recoserver
        if (ips.Length > 0)
        {
            LocalIP = ips[0].ToString();
        }
    }

    void Update()
    {
        //newData is flagged to true whenever the spawned thread recieves data from the voice server
        if (newData)
        {
            //send data to game manager to be parsed however you see fit
            gm.UseVoiceData(strReceiveUDP);

            //flag newData as false so that we dont send the same data more than once
            newData = false;
        }

    }

    //This is the function that is passed to the thread,
    //it handles all transactions between the recoserver and Unity.

    //When data is recieved, newData is flagged to true, and then in the update function
    //the new data is passed to our game manager
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                //try to recieve data
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Broadcast, port);
                byte[] data = client.Receive(ref anyIP);

                //decode the data that was recieved
                strReceiveUDP = Encoding.UTF8.GetString(data);

                //flag newData to true so that we can use it!
                newData = true;
            }
            catch (Exception err)
            {
                //catch those pesky errors
                print(err.ToString());
            }
        }
    }
    
    //When we close the program, we have to kill the thread and close the UdpClient
    void OnDisable()
    {
        if (receiveThread != null) receiveThread.Abort();
        client.Close();
    }
}