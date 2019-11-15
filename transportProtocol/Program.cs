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
        private Boolean working = false;
        private String dstIP;
        private String srcIP;
        private String srcPort;
        private String dstPort;
        private String data = "";
        private int segSize = 0;
        private int seq = 1;

        private int typeConn = 1;
        private String ackSyn = "00";
        private Byte [] bytes;
        //000 = SYN client
        //001 = SYN-ACK server
        //010 = ACK client
        //011 = FIM SYN
        //100 = FIN ACK+ SYN
        //101 = FIN ACK
        
        //**TCP PDU EXAMPLE TEST*/
        //192.162.1.5 128.50.13.10 500 200 1 00 | DADO

        //**UDP PDU EXAMPLE TEST*/
        //192.162.1.5 128.50.13.10 500 200 10 00 | DADO
        public TransportEngine(int typeConn){
            this.typeConn = typeConn;
        }
        
        public void listening(){
            try{
                while(true){
                    Console.WriteLine("Listening");
                    if(new System.IO.FileInfo(@"../logout.txt").Length > 0){
                        Console.WriteLine("Initiate fin conn");
                        StreamWriter wr = new StreamWriter(@"../redeTop.txt");
                        String PDU = "";
                        PDU = (srcIP+" "+dstIP+" "
                                +srcPort+" "+dstPort+" "
                                +seq+" "+"011");
                        wr.WriteLine(PDU);
                        wr.Close();
                        
                        StreamWriter logoutClean = new StreamWriter(@"../logout.txt");
                        logoutClean.Flush();
                        logoutClean.Close();
                    }
                    if(new System.IO.FileInfo(@"../transTop.txt").Length > 0 && !this.working){
                        Console.WriteLine("Receive from top layer");
                        StreamReader transTop = new StreamReader(@"../transTop.txt");
                        StreamWriter wr = new StreamWriter(@"../redeTop.txt");
                        String PDU = transTop.ReadToEnd();
                        List<String> labels = new List<String>(PDU.Split(" "));
                        
                        srcIP = labels[0];
                        dstIP= labels[1];
                        srcPort = labels[2];
                        dstPort = labels[3];
                        
                        if(this.typeConn == 1){
                        
                            if(ackSyn == "000"){
                                labels.Insert(4, seq.ToString());
                                labels.Insert(5, ackSyn);
                            }
                            
                            else{
                                seq = Convert.ToInt32(labels[4])+1;
                                ackSyn = labels[5];
                            } 
                            if(labels.Count > 6){
                                foreach (String s in (labels.GetRange(6, labels.Count-6))){
                                    data += s+" ";
                                }
                            }

                            //three hand shake control
                            switch(ackSyn){
                                case "010":
                                    Console.WriteLine("send to bottom layer");
                                    PDU = (srcIP+" "+dstIP+" "
                                          +srcPort+" "+dstPort+" "
                                          +seq+" "+ackSyn);
                                    wr.WriteLine(PDU);
                                    wr.Close();
                                break;
                                default: //000 - SYN
                                    Console.WriteLine("send to bottom layer");
                                    PDU = (srcIP+" "+dstIP+" "
                                          +srcPort+" "+dstPort+" "
                                          +seq+" "+"000");
                                    wr.WriteLine(PDU);
                                    wr.Close();
                                break;
                            }
                        }else{
                            labels.Insert(4, ((labels.GetRange(4, labels.Count-4)).Count).ToString());
                            if(labels.Count > 5){
                                foreach (String s in (labels.GetRange(5, labels.Count-5))){
                                    data += s+" ";
                                }
                            }
                            
                            //bypass
                            Console.WriteLine("send to bottom layer");
                            PDU = (srcIP+" "+dstIP+" "
                                  +srcPort+" "+dstPort+" "
                                  +segSize+" "+data);
                            wr.WriteLine(PDU);
                            wr.Close();
                        }
                        transTop.Close();
                        
                        //limpa o arquivo
                        StreamWriter transTopClean = new StreamWriter(@"../transTop.txt");
                        transTopClean.Flush();
                        transTopClean.Close();
                    }
                    if(new System.IO.FileInfo(@"../transDown.txt").Length > 0){
                        Console.WriteLine("Receive from bottom layer");
                        StreamWriter wr;
                        StreamReader transDown = new StreamReader(@"../transDown.txt");
                        String PDU = transDown.ReadToEnd();
                        List<String> labels = new List<String>(PDU.Split(" "));
                        srcIP = labels[0];
                        dstIP= labels[1];
                        srcPort = labels[2];
                        dstPort = labels[3];

                        if(this.typeConn == 1){                          
                            seq = Convert.ToInt32(labels[4])+1;
                            ackSyn = labels[5];
                            if(labels.Count > 6){
                                foreach (String s in (labels.GetRange(6, labels.Count-6))){
                                    data += s+" ";
                                }
                            }

                            //three hand shake control
                            switch(ackSyn){
                                case "001"://SYN-ACK
                                    PDU = (srcIP+" "+dstIP+" "
                                          +srcPort+" "+dstPort+" "
                                          +seq+" "+"010");
                                    Console.WriteLine("send to bottom layer");
                                    wr = new StreamWriter(@"../redeTop.txt");
                                    wr.WriteLine(PDU);
                                    wr.Close();
                                break;
                                case "010"://ACK
                                    PDU = (srcIP+" "+dstIP+" "
                                          +srcPort+" "+dstPort+" "
                                          +seq+" "+"010"+" "+data);
                                    Console.WriteLine("send to bottom layer");
                                    wr = new StreamWriter(@"../appDown.txt");
                                    wr.WriteLine(PDU);
                                    wr.Close();
                                break;
                                case "011":
                                    PDU = (srcIP+" "+dstIP+" "
                                          +srcPort+" "+dstPort+" "
                                          +seq+" "+"100");
                                    Console.WriteLine("send to bottom layer");
                                    wr = new StreamWriter(@"../redeTop.txt");
                                    wr.WriteLine(PDU);
                                    wr.Close();
                                break;
                                case "100":
                                    PDU = (srcIP+" "+dstIP+" "
                                          +srcPort+" "+dstPort+" "
                                          +seq+" "+"101");
                                    Console.WriteLine("Fim Connection layer");
                                    wr = new StreamWriter(@"../finLogout.txt");
                                    wr.WriteLine(PDU);
                                    wr.Close();

                                    StreamWriter transDownClean = new StreamWriter(@"../transDown.txt");
                                    transDownClean.Flush();
                                    transDownClean.Close();

                                    System.Environment.Exit(0);
                                break;
                                default: //00 - SYN
                                    PDU = (srcIP+" "+dstIP+" "
                                          +srcPort+" "+dstPort+" "
                                          +seq+" "+"001");
                                    Console.WriteLine("send to bottom layer");
                                    wr = new StreamWriter(@"../redeTop.txt");
                                    wr.WriteLine(PDU);
                                    wr.Close();
                                break;
                            }
                        }else{                           
                            
                            labels.Insert(4, ((labels.GetRange(4, labels.Count-4)).Count).ToString());
                            if(labels.Count > 5){
                                foreach (String s in (labels.GetRange(5, labels.Count-5))){
                                    data += s+" ";
                                }
                            }

                            PDU = (srcIP+" "+dstIP+" "
                                  +srcPort+" "+dstPort+" "
                                  +segSize+" "+data);
                            Console.WriteLine("send to bottom layer");
                            wr = new StreamWriter(@"../appDown.txt");
                            wr.WriteLine(PDU);
                            wr.Close();
                        }
                        transDown.Close();
                        
                        //limpa o arquivo
                        StreamWriter transDownClean = new StreamWriter(@"../transDown.txt");
                        transDownClean.Flush();
                        transDownClean.Close();
                    }
                    Thread.Sleep(2000);
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
    }
}
