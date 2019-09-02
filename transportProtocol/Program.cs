using System;
using System.Text;
using System.Net; 
using System.Net.Sockets;
using System.IO;

namespace transportProtocol{
    class Program{
        static void Main(string[] args){
            Boolean isServer = true;
            int choice = 3;
            Console.WriteLine("Pick your type connection: (1) TCP - (2) UDP - (3) EXIT");
            choice = Int32.Parse(Console.ReadLine());
            if(choice == 1){
                TCP tcp = new TCP(isServer);
                tcp.registerLog("Intialize TCP Process");
                tcp.registerLog("exit TCP process");
            }else if(choice == 2){  
                Console.WriteLine("UDP");
                UDP udp = new UDP(isServer);
                udp.registerLog("Intialize UDP Process");
                udp.registerLog("exit UDP process");
            }else{
                Console.WriteLine("exit process...");
                return;
            }
        }
    }

    class TCP{
        public TCP(Boolean isServer){
            if(isServer) receive();
            else send();
        }

        public Boolean receive(){
           
            try {
                int port = 11111;
                byte[] bytes = new Byte[1024];
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
                socket.Bind(new IPEndPoint(IPAddress.Any, port));
                socket.Listen(10);    //coloca o servidor em estado de recebimento de conexoes - 10 tamanho da fila

                while (true) {                     
                    Console.WriteLine("Waiting connection ... ");
                    Socket clientSocket = socket.Accept();              //recebe a conexao do cliente
                    int numByte = clientSocket.Receive(bytes); 
                    String data = Encoding.ASCII.GetString(bytes, 0, numByte); //show PDU
                    Console.WriteLine("Data receive from: "+clientSocket.RemoteEndPoint.ToString());
                    System.Console.WriteLine(data);
                    byte[] msgAnswer = Encoding.ASCII.GetBytes("Data receive"); 
                    clientSocket.Send(msgAnswer);
                } 
                return true;
            }catch (ArgumentNullException e){
                Console.WriteLine("Argument Exception: {0}", e);
            }catch (SocketException e){
                Console.WriteLine("SocketException: {0}", e);
            }   

            return false;
        }

        public Boolean send(){
            try{
                //StreamReader rd = new StreamReader(@'data');
                byte[] msgSent = Encoding.ASCII.GetBytes("Test Client<EOF>"); 
                byte[] msgReceiv = new byte[1024];
                int port = 11111;
                String ipAddress = "192.168.0.5";
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipAddress, port);

                if (!socket.Connected){
                    Console.WriteLine("unable to connect");
                    return false;
                }else{
                    registerLog("Socket connected to -> {0} "+socket.RemoteEndPoint.ToString());                
                }
                
                socket.Send(msgSent); 
                registerLog("Send message to "+socket.RemoteEndPoint.ToString());

                int byteRecv = socket.Receive(msgReceiv); 
                Console.WriteLine(Encoding.ASCII.GetString(msgReceiv, 0, byteRecv)); 
                registerLog("Receive server answer");

                socket.Close();

                return true;
            }catch (ArgumentNullException e){
                Console.WriteLine("Argument Exception: {0}", e);
            }catch (SocketException e){
                Console.WriteLine("SocketException: {0}", e);
            }    
            return false;
        }

        public Boolean validate(){
            return false;
        }

        public void separateBuffer(){
        }
        
        public void registerLog(String msg){
            DateTime now = DateTime.Now;
            String nowDate = ""+now.Date.Year+"_"+now.Date.Month+"_"+now.Date.Day;
            String fileName = "log_"+nowDate+".txt";
            StreamWriter wr = new StreamWriter(@fileName, true);
            wr.WriteLine(msg);
            wr.Close();
        }
    }

    class UDP{
        public UDP(Boolean isServer){
            if(isServer) receive();
            else send();
        }

        public Boolean receive(){
            int port = 11111;
            byte[] bytes = new Byte[1024];
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            EndPoint endPoint = (EndPoint)localEndPoint;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            Object o = new Object();

            try{
                socket.Bind(localEndPoint);
                
                while(true){
                    socket.BeginReceiveFrom(bytes, 0, bytes.Length, SocketFlags.None, ref endPoint, (ar) =>
                    {
                        socket = (Socket)ar.AsyncState;
                        int byteRecv = socket.EndSend(ar);
                        System.Console.WriteLine();
                    }, o);
                }
            }catch (ArgumentNullException e){
                Console.WriteLine("Argument Exception: {0}", e);
            }catch (SocketException e){
                Console.WriteLine("SocketException: {0}", e);
            }   
            return false;
        }

        public Boolean send(){
            try{
                //StreamReader rd = new StreamReader(@'data');
                byte[] msgSent = Encoding.ASCII.GetBytes("Test Client<EOF>"); 
                byte[] msgReceiv = new byte[1024];
                int port = 11111;
                String ipAddress = "192.168.0.5";
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                
                //conecta com o servidor
                socket.Connect(ipAddress, port);

                if (!socket.Connected){
                    Console.WriteLine("unable to connect");
                    return false;
                }else{
                    registerLog("Socket connected to -> {0} "+socket.RemoteEndPoint.ToString());                
                }
                
                socket.Send(msgSent); 
                registerLog("Send message to "+socket.RemoteEndPoint.ToString());

                //resposta do servidor
                socket.Receive(msgReceiv);
                registerLog("Receive server answer");

                int byteRecv = socket.Receive(msgReceiv); 
                Console.WriteLine(Encoding.ASCII.GetString(msgReceiv, 0, byteRecv)); 

                socket.Close();

                return true;
            }catch (ArgumentNullException e){
                Console.WriteLine("Argument Exception: {0}", e);
            }catch (SocketException e){
                Console.WriteLine("SocketException: {0}", e);
            }    
            return false;
        }

        public void separateBuffer(){

        }

        public void registerLog(String msg){
            DateTime now = DateTime.Now;
            String nowDate = ""+now.Date.Year+"_"+now.Date.Month+"_"+now.Date.Day;
            String fileName = "log_"+nowDate+".txt";
            StreamWriter wr = new StreamWriter(@fileName, true);
            wr.WriteLine(msg);
            wr.Close();
        }
    }
}
