using System;
using System.Text;
using System.Net; 
using System.Net.Sockets;
using System.IO;

namespace transportProtocol{
    class Program{
        static void Main(string[] args){
            int choice = 3;
            Console.WriteLine("Pick your type connection: (1) Connection Oriented - (2) Non Connection Oriented - (3) EXIT");
            choice = Int32.Parse(Console.ReadLine());
            if(choice == 1){
                receiveFromLayer(false);
                Connection con = new Connection();
                registerLog("Intialize Connection Oriented Process");
                registerLog("exit Connection Oriented process");
            }else if(choice == 2){
                receiveFromLayer(); 
                Console.WriteLine("UDP");
                Connectionless conl = new Connectionless();
                registerLog("Intialize Non Connection Oriented Process");
                registerLog("exit Non Connection Oriented process");
            }else{
                Console.WriteLine("exit process...");
                return;
            }
        }

        public static Connection receiveFromLayer(Boolean isNCO){
           
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
                    String data = Encoding.ASCII.GetString(bytes, 0, numByte);
                    Console.WriteLine("Data receive from: "+clientSocket.RemoteEndPoint.ToString());
                    System.Console.WriteLine(data);
                    byte[] msgAnswer = Encoding.ASCII.GetBytes("Data receive"); 
                    clientSocket.Send(msgAnswer);
                } 
                return new Connection();
            }catch (ArgumentNullException e){
                Console.WriteLine("Argument Exception: {0}", e);
            }catch (SocketException e){
                Console.WriteLine("SocketException: {0}", e);
            }   

            return null;
        }

        public static void sendToLayer(){
            try{
                //StreamReader rd = new StreamReader(@'data');
                byte[] msgSent = Encoding.ASCII.GetBytes("Test Client<EOF>"); 
                byte[] msgReceiv = new byte[1024];
                int port = 11111;
                String ipAddress = "192.168.0.5";
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipAddress, port);

                if (!socket.Connected){
                    Console.WriteLine("unable to connect with another layer");
                    return;
                }else{
                    registerLog("Socket connected to -> {0} "+socket.RemoteEndPoint.ToString());                
                }
                
                socket.Send(msgSent); 
                registerLog("Send message to "+socket.RemoteEndPoint.ToString());

                int byteRecv = socket.Receive(msgReceiv); 
                Console.WriteLine(Encoding.ASCII.GetString(msgReceiv, 0, byteRecv)); 
                registerLog("Receive server answer");

                socket.Close();

            }catch (ArgumentNullException e){
                Console.WriteLine("Argument Exception: {0}", e);
            }catch (SocketException e){
                Console.WriteLine("SocketException: {0}", e);
            }    
        }

        public static void registerLog(String msg){
            DateTime now = DateTime.Now;
            String nowDate = ""+now.Date.Year+"_"+now.Date.Month+"_"+now.Date.Day;
            String fileName = "log_"+nowDate+".txt";
            StreamWriter wr = new StreamWriter(@fileName, true);
            wr.WriteLine(msg);
            wr.Close();
        }
    }

    class Connection{
        public Connection(){
        }

        public Boolean validate(){
            Program.registerLog("Start Validation");
            return false;
        }

        public void separateBuffer(){
        }
    }

    class Connectionless{
        public Connectionless(){
        }

        public void separateBuffer(){

        }
    }
}
