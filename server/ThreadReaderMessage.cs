using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server
{

    public class ThreadReaderMessage
    {

        private NetworkStream stream;

        /**
         * Tamanho do buffer de leitura
         */
        private const int BUFFER_SIZE = 1024;

        private const string SEND_MESSAGE = "SEND_MESSAGE";

        private List<Request> mensagens = new List<Request>();

        private bool active = true;

        private string nick;


        public ThreadReaderMessage(string nick, NetworkStream stream)
        {
            this.nick = nick;
            this.stream = stream;
        }

        public void deactive()
        {
            active = false;
        }

        private void proccessRequest(string json)
        {
            Request request = JsonConvert.DeserializeObject<Request>(json);
            Console.WriteLine(json);
            if (request.type == SEND_MESSAGE)
            {
                mensagens.Add(request);

            }

        }

        public void sendMessage(string message)
        {
            Request request = new Request();
            request.body = message;
            request.type = SEND_MESSAGE;
            request.nick = nick;
            string json = JsonConvert.SerializeObject(request) + "###";
            byte[] bytesToSend = Encoding.ASCII.GetBytes(json);
            stream.Write(bytesToSend, 0, bytesToSend.Length);
            stream.Flush();
        }

        public void read()
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            string message = "";
            while (active)
            {
                int bytesReceived = stream.Read(buffer, 0, BUFFER_SIZE);
                message += Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                if (message.Contains("###"))
                {

                    int splitpoint = message.IndexOf("###");
                    string msg = message.Substring(0, splitpoint);
                    message = message.Substring(splitpoint + 3);

                    proccessRequest(msg);
                }
            }
        }

        public List<Request> getMensagens()
        {
            return mensagens;
        }
    }

}
