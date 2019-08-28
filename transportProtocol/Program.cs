using System;
using System.IO;

namespace transportProtocol{
    class Program{
        static void Main(string[] args){
            int choice = 3;
            Console.WriteLine("Pick your type connection: (1) TCP - (2) UDP - (3) EXIT");
            choice = Int32.Parse(Console.ReadLine());
            if(choice == 1){
                Console.WriteLine("Show PDU [S/N]");
                if(string.Equals(Console.ReadLine(), "s", StringComparison.CurrentCultureIgnoreCase)){
                    showPDU();
                }
                TCP tcp = new TCP();
                tcp.registerLog("---------Intialize TCP Process-------");
                udp.registerLog("exit TCP process");
            }else if(choice == 2){
                Console.WriteLine("--------UDP---------");
                UDP udp = new UDP();
                udp.registerLog("---------Intialize UDP Process-------");
                udp.registerLog("exit UDP process");
            }else{
                Console.WriteLine("exit process...");
                udp.registerLog("exit UDP process");
                return;
            }
        }

        static void showPDU(){
            
        }
    }

    class TCP{
        public TCP(){

        }

        public Boolean receive(){
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
