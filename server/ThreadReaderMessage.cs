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
        private const string CREATE_GAME = "CREATE_GAME";
        private const string LIST_GAMES = "LIST_GAMES";
        private const string JOIN_GAME = "JOIN_GAME";

        private ComunicationManager comunicationManager;

        private bool active = true;

        private string nick;


        public ThreadReaderMessage(ComunicationManager cm, string nick, NetworkStream stream)
        {
            this.nick = nick;
            this.stream = stream;
            this.comunicationManager = cm;
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
                comunicationManager.sendMessage(this, request);
            }
            if (request.type == CREATE_GAME)
            {
                comunicationManager.createGame(this, request);
            }
            if (request.type == LIST_GAMES)
            {
                comunicationManager.listGames(this);
            }
            if (request.type == JOIN_GAME)
            {
                comunicationManager.joinGame(this, request);
            }
        }

        public void sendMessage(Request request)
        {
            string json = JsonConvert.SerializeObject(request) + "###";
            byte[] bytesToSend = Encoding.ASCII.GetBytes(json);
            stream.Write(bytesToSend, 0, bytesToSend.Length);
            stream.Flush();
        }

        public void sendMessage(string str)
        {
            byte[] bytesToSend = Encoding.ASCII.GetBytes(str);
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
    }

}
