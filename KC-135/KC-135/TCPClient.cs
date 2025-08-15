using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KC_135
{
    public class TCPClient
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private Thread receiveThread;
        private bool isConnected;
        private string serverIP;
        private int serverPort;
        
        public event Action<string> MessageReceived;
        public event Action Connected;
        public event Action Disconnected;
        
        public bool IsConnected => isConnected && tcpClient?.Connected == true;
        
        public TCPClient(string serverIP = "127.0.0.1", int serverPort = 8080)
        {
            this.serverIP = serverIP;
            this.serverPort = serverPort;
        }
        
        public async Task<bool> ConnectAsync()
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIP, serverPort);
                
                if (tcpClient.Connected)
                {
                    stream = tcpClient.GetStream();
                    isConnected = true;
                    
                    // Start receiving messages
                    receiveThread = new Thread(ReceiveMessages)
                    {
                        IsBackground = true
                    };
                    receiveThread.Start();
                    
                    Connected?.Invoke();
                    Console.WriteLine($"Connected to TCP server at {serverIP}:{serverPort}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to TCP server: {ex.Message}");
            }
            
            return false;
        }
        
        public void Disconnect()
        {
            isConnected = false;
            
            try
            {
                stream?.Close();
                tcpClient?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during disconnect: {ex.Message}");
            }
            
            receiveThread?.Join(1000); // Wait up to 1 second for thread to finish
            
            Disconnected?.Invoke();
            Console.WriteLine("Disconnected from TCP server");
        }
        
        private void ReceiveMessages()
        {
            byte[] buffer = new byte[4096];
            
            while (isConnected && tcpClient?.Connected == true)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    
                    if (bytesRead == 0)
                    {
                        // Server closed connection
                        break;
                    }
                    
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageReceived?.Invoke(message.Trim());
                }
                catch (Exception ex)
                {
                    if (isConnected)
                    {
                        Console.WriteLine($"Error receiving message: {ex.Message}");
                    }
                    break;
                }
            }
            
            if (isConnected)
            {
                isConnected = false;
                Disconnected?.Invoke();
            }
        }
        
        public bool SendMessage(string message)
        {
            if (!IsConnected) return false;
            
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                return false;
            }
        }
        
        public void Dispose()
        {
            Disconnect();
        }
    }
}