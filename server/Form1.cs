using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace server
{
    public partial class Form1 : Form
    {
        private string nick = "";
        private int port = 9000;
        private string mode = INIT;

        private const string INIT = "INIT";
        private const string SERVER_STARTED = "SERVER_STARTED";
        private const string CLIENT_STARTED = "CLIENT_STARTED";
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


        private ComunicationManager comunicationManager = new ComunicationManager();

        public Form1()
        {
            InitializeComponent();
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            mode = SERVER_STARTED;
            Thread threadTcp = new Thread(startServer);
            threadTcp.Start();

            Thread threadUdp = new Thread(startLitenerBroadcast);
            threadUdp.Start();

            btnStartServer.Enabled = false;
            btnStopServer.Enabled = true;

        }

        private void startLitenerBroadcast()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 20162);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Bind(iep);

            EndPoint ep = (EndPoint)iep;

            while (mode == SERVER_STARTED)
            {
                try
                {
                    byte[] data = new byte[1024];
                    int recv = socket.ReceiveFrom(data, ref ep);
                    string stringData = Encoding.ASCII.GetString(data, 0, recv);

                    Request request = JsonConvert.DeserializeObject<Request>(stringData);

                    string ipClient = ep.ToString().Split(':')[0];
                    int port = request.port;

                    Request requestServer = new Request();
                    requestServer.nick = "SERVER";
                    requestServer.port = 9000;

                    TcpClient client = new TcpClient(ipClient, port);
                    NetworkStream stream = client.GetStream();
                    byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(requestServer) + "###");
                    stream.Write(bytes, 0, bytes.Length);
                    client.Close();
                }catch(Exception e)
                {

                }
            }
        }

        private void stopServer()
        {
            socket.Shutdown(SocketShutdown.Both);
            //socket.Disconnect(true);
            socket.Dispose();
            socket.Close();
            mode = INIT;
            btnStartServer.Enabled = true;
            btnStopServer.Enabled = false;

        }

        private void startServer()
        {

            TcpListener listener = null;
            Console.WriteLine("Server started.");

            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                while (mode == SERVER_STARTED)
                {
                    TcpClient client = client = listener.AcceptTcpClient();
                    NetworkStream streamServer = client.GetStream();
                    ThreadReaderMessage trm = new ThreadReaderMessage(comunicationManager, nick, streamServer);
                    comunicationManager.addReader(trm);
                    Thread thread = new Thread(trm.read);
                    thread.Start();
                }
                listener.Stop();
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error on open socket. " + e.Message);
            }
        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            stopServer();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            stopServer();
        }
    }
}
