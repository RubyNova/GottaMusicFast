using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AccelerometerMusicConverter
{
    class Client
    {
        public static string ConnectIP = "172.17.0.1";
        public static List<SendData> DataBuffer = new List<SendData>();
        public static bool Recieving = false;

        public static void StartClient()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {
                Console.WriteLine("Trying to connect to: " + ConnectIP);
                IPAddress ipAddress = IPAddress.Parse(ConnectIP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5555);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);

                    while (true)
                    {
                        // Receive the response from the remote device
                        Recieving = true;
                        int bytesRec = sender.Receive(bytes);
                        string result = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        result = result.Split("<EOF>")[0];
                        try
                        {
                            DataBuffer.Add(JsonConvert.DeserializeObject<SendData>(result));
                        }
                        catch (JsonReaderException e)
                        {
                            Console.WriteLine("Deserialization exception! " + e.Message);
                        }
                        Recieving = false;
                    }
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    if(se.SocketErrorCode == SocketError.HostNotFound || se.SocketErrorCode == SocketError.HostUnreachable)
                    {
                        
                    }
                    Console.WriteLine("SocketException : Code: {0}, {1}", se.SocketErrorCode, se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
