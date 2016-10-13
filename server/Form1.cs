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

        private List<ThreadReaderMessage> readers = new List<ThreadReaderMessage>();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            mode = SERVER_STARTED;
            Thread thread = new Thread(startServer);
            thread.Start();

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
                    ThreadReaderMessage trm = new ThreadReaderMessage(nick, streamServer);
                    readers.Add(trm);
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
    }
}
