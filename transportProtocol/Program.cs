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
                tcp.registerLog("---------Intialize TCP Process-------");
                tcp.registerLog("exit TCP process");
            }else if(choice == 2){  
                Console.WriteLine("--------UDP---------");
                UDP udp = new UDP(isServer);
                udp.registerLog("---------Intialize UDP Process-------");
                udp.registerLog("exit UDP process");
            }else{
                Console.WriteLine("exit process...");
                udp.registerLog("exit UDP process");
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
            int port = 0666;
            String ipAddress = "172.16.253.72";
            byte[] bytes = new Byte[1024];2
            
            try { 
                Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); 
                socket.Bind(new IPEndPoint(ipAddress, port)); //associa um endereco a network do server
                socket.Listen();    //coloca o servidor em estado de recebimento de conexoes

                while (true) { 
                    
                    Console.WriteLine("Waiting connection ... "); 
                    Socket clientSocket = socket.Accept(); //recebe a conexao do cliente
                    int numByte = clientSocket.Receive(bytes); 
                    String data = Encoding.ASCII.GetString(bytes, 0, numByte); //show PDU
                
                    Console.WriteLine("Data receive from"); 
                    byte[] msgAnswer = Encoding.ASCII.GetBytes("Data receive"); 

                    //responde o client 
                    clientSocket.Send(msgAnswer);

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close(); //fecha a conexao com o cliente
                } 
                return true;
            } 
            
            catch (Exception e) { 
                Console.WriteLine(e.ToString()); 
            } 

            return false;
        }

        public Boolean send(){
            try{
                //StreamReader rd = new StreamReader(@'data');
                byte[] msgSent = Encoding.ASCII.GetBytes("Test Client<EOF>"); 
                byte[] msgReceiv = new byte[1024];
                int port = 0666;
                String ipAddress = "172.16.253.72";
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                //conecta com o servidor
                socket.Connect(new IPEndPoint(ipAddress, port));

                if (!socket.Connected){
                    Console.WriteLine("unable to connect");
                    return false;
                }else{
                    registerLog("Socket connected to -> {0} "+socket.RemoteEndPoint.ToString());                
                }
                
                socket.Send(msgSent); 
                registerLog("Send message to "+sender.RemoteEndPoint.ToString());

                //resposta do servidor
                socket.Receive(msgReceiv);
                registerLog("Receive server answer");

                int byteRecv = socket.Receive(msgReceived); 
                Console.WriteLine(Encoding.ASCII.GetString(msgReceived, 0, byteRecv)); 

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
        public UDP(){

        }

        public Boolean receive(){
            return false;
        }

        public Boolean send(){
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
