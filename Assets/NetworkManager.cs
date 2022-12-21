using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Text;

public class NetworkManager : MonoBehaviour
{
    

    public GameObject player;
    public GameObject enemy;
    string DISCONNECT_MSG = "!DISCONNECT";
    string IS_HOST_MSG = "!IS_HOST";

    public GameState gameState = new GameState();
    public int isHost = 0;
    private Socket sender;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress item in ipHost.AddressList)
            {
                Console.WriteLine(item);
            }
            IPAddress ipAddr = ipHost.AddressList[1];
            ipAddr = Dns.GetHostEntry("192.168.55.8").AddressList[1];
            Debug.Log(ipAddr.ToString());
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 5000);

            // Creation TCP/IP Socket using
            // Socket Class Constructor
            sender = new Socket(ipAddr.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Connect Socket to the remote
                sender.Connect(localEndPoint);

                // print EndPoint information that we are connected to
                Debug.LogFormat("Socket connected to -> {0} ",
                            sender.RemoteEndPoint.ToString());

                // Try to get room from server
                string message = JsonUtility.ToJson(gameState);
                byte[] messageSent = Encoding.ASCII.GetBytes(Console.ReadLine());
                int byteSent = sender.Send(messageSent);

                // Data buffer
                byte[] messageReceived = new byte[1024];

                int byteRecv = sender.Receive(messageReceived);
                message = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);
                
                Debug.LogFormat("Message from Server -> {0}", message);
            }
            catch (ArgumentNullException ane)
            {

                Debug.LogFormat("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {

                Debug.LogFormat("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Debug.LogFormat("Unexpected exception : {0}", e.ToString());
            }
        } 
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isHost == 0)
        {
            string message = IS_HOST_MSG;
            byte[] messageSent = Encoding.ASCII.GetBytes(message);
            
            sender.Send(messageSent);

            byte[] messageReceived = new byte[1024];
            int byteRecv = sender.Receive(messageReceived);
            string recivedMessage = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);
            isHost = int.Parse(recivedMessage);
            Debug.LogFormat("Is Host Int -> {0}", isHost);
        }

        //Debug.LogFormat("Game State read enemy player x -> {0}",gameState.EnemyPosX);
        enemy.GetComponent<Transform>().position = new Vector3(-gameState.EnemyPosX  , enemy.GetComponent<Transform>().position.y, 0);
        if (isHost == -1)
        {
            GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody2D>().MovePosition(new Vector2(gameState.BallPosX, gameState.BallPosY));
        }

        gameState.HostPosX = player.GetComponent<Rigidbody2D>().position.x;
        gameState.HostPoints = 0;
        if (isHost == 1)
        {
            try
            {
                gameState.BallPosX = -GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody2D>().position.x;
                gameState.BallPosY = -GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody2D>().position.y;
            }
            catch (Exception ex)
            {
                ;
            }
            
        }
        else
        {
            gameState.BallPosX = 0f;
            gameState.BallPosY = 0f;
        }
        

        SendData();
    }

    private void OnApplicationQuit()
    {
        byte[] messageSent = Encoding.ASCII.GetBytes(DISCONNECT_MSG);
        //int byteSent = sender.Send(messageSent);
        sender.Send(messageSent);
    }

    void SendData()
    {
        string message = JsonUtility.ToJson(gameState);
        byte[] messageSent = Encoding.ASCII.GetBytes(message);
        //int byteSent = sender.Send(messageSent);
        sender.Send(messageSent);
        //Debug.LogFormat("Message from send -> {0}", message);

        // Data buffer
        byte[] messageReceived = new byte[1024];


        // We receive the message
        int byteRecv = sender.Receive(messageReceived);
        string recivedMessage = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);
        gameState = JsonUtility.FromJson<GameState>(recivedMessage);
    }
}
