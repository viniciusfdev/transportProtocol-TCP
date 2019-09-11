using System;
using System.Text;
using System.Net; 
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace transportProtocol{
    class Program{
        static void Main(string[] args){
            int choice = 3;
            Console.WriteLine("Pick your type connection: (1) Connection Oriented - (2) Non Connection Oriented - (3) EXIT");
            choice = Int32.Parse(Console.ReadLine());
            if(choice == 1){
                TransportEngine te = new TransportEngine(choice);
                te.registerLog("Intialize Connection Oriented Process");
                te.listening();
                te.registerLog("exit Connection Oriented process");
            }else if(choice == 2){
                TransportEngine te = new TransportEngine(choice);
                Console.WriteLine("UDP");
                te.registerLog("Intialize Non Connection Oriented Process");
                te.listening();
                te.registerLog("exit Non Connection Oriented process");
            }else{
                Console.WriteLine("exit process...");
                return;
            }
        }
    }

    class TransportEngine{
        private Boolean sending = false;
        private String ipDest;
        private String ipOri;
        private int typeConn = 1;
        private String ackSyn = "00";
        private Byte [] bytes;
        //00 = SYN client
        //01 = SYN-ACK server
        //10 = ACK client
        //11 = ESTABLISHED CONNECTION

        public TransportEngine(int typeConn){
            this.typeConn = typeConn;
        }
        
        public void listening(){
            try{
                while(true){
                    if(new System.IO.FileInfo(@"../transTop.txt").Length > 0 && !this.sending){
                        this.sending = true;
                        StreamReader transTop = new StreamReader(@"../transTop.txt");
                        if(this.typeConn == 1){
                            //three hand shake control
                            switch(ackSyn){
                                default: //00 - SYN
                                    StreamWriter wr = new StreamWriter(@"../redeTop.txt");
                                    wr.WriteLine("SYN");
                                    wr.Close();
                                break;
                            }
                        }else{
                            StreamWriter wr = new StreamWriter(@"../redeTop.txt");
                            wr.WriteLine("DATA FINALLY ARRIVE - UDP");
                        }
                        transTop.Close();
                        
                        //limpa o arquivo
                        StreamWriter transTopClean = new StreamWriter(@"../transTop.txt");
                        transTopClean.Flush();
                        transTopClean.Close();
                    }
                    if(new System.IO.FileInfo(@"../transDown.txt").Length > 0){
                        StreamWriter wr;
                        StreamReader transDown = new StreamReader(@"../transDown.txt");
                        
                        if(this.typeConn == 1){
                            //three hand shake control
                            switch(ackSyn){
                                case "01"://SYN-ACK
                                    wr = new StreamWriter(@"../redeTop.txt");
                                    wr.WriteLine("ACK");
                                    wr.Close();
                                break;
                                case "10"://ACK
                                    wr = new StreamWriter(@"../redeTop.txt");
                                    wr.WriteLine("ESTABLISHED");
                                    wr.Close();
                                    this.sending = false;
                                break;
                                case "11"://ESTABLISHED
                                    wr = new StreamWriter(@"../appDown.txt");
                                    wr.WriteLine("DATA FINALLY ARRIVE - TCP");
                                    wr.Close();
                                break;
                                default: //00 - SYN
                                    wr = new StreamWriter(@"../redeTop.txt");
                                    wr.WriteLine("SYN-ACK");
                                    wr.Close();
                                break;
                            }
                        }else{
                            wr = new StreamWriter(@"../appDown.txt");
                            wr.WriteLine("DATA FINALLY ARRIVE - UDP");
                        }
                        transDown.Close();
                        
                        //limpa o arquivo
                        StreamWriter transTopClean = new StreamWriter(@"../transDown.txt", true);
                        transTopClean.Flush();
                        transTopClean.Close();
                    }
                    Thread.Sleep(1000);
                } 
            }
            catch (System.Exception){
                
            }
        }
        public string binaryToString(string data){
            List<Byte> byteList = new List<Byte>();
        
            for (int i = 0; i < data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(byteList.ToArray());
        }

        public string stringToBinary(string data){
            StringBuilder sb = new StringBuilder();
        
            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        public void registerLog(String msg){
            DateTime now = DateTime.Now;
            String nowDate = ""+now.Date.Year+"_"+now.Date.Month+"_"+now.Date.Day;
            String fileName = "log_"+nowDate+".txt";
            StreamWriter wr = new StreamWriter(@fileName, true);
            wr.WriteLine(msg);
            wr.Close();
        }

        public void separateBytes(){
        }
    }
}
