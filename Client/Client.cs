using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;


namespace Client
{
    public partial class Client : DevExpress.XtraEditors.XtraForm
    {
        #region Private Members

        // Client socket
        private Socket clientSocket;

        // Client name
        private string name;

        // Server End Point
        private EndPoint epServer;

        // Data stream
        private byte[] dataStream = new byte[1024];

        // Display message delegate
        private delegate void DisplayMessageDelegate(string message);
        private DisplayMessageDelegate displayMessageDelegate = null;

        #endregion

        #region Constructor

        public Client()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string msgsend = txtMessClient.Text;

            txtMsgAll.Text = msgsend + "\n";
        }

        private void Client_Load(object sender, EventArgs e)
        {
            this.displayMessageDelegate = new DisplayMessageDelegate(this.DisplayMessage);

        }
        #endregion


        #region Other Methods

        private void DisplayMessage(string messge)
        {
            txtMsgAll.Text += messge + Environment.NewLine;
        }

        #endregion

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {

                this.name = txtUname.Text.Trim();

                Packet sendData = new Packet();
                sendData.ChatName = this.name;
                sendData.ChatMessage = null;
                sendData.ChatDataIdentifier = DataIdentifier.LogIn;

                this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                IPAddress serverIP = IPAddress.Parse(txtServerIP.Text.Trim());

                IPEndPoint server = new IPEndPoint(serverIP, 30000);

                epServer = (EndPoint)server;

                byte[] data = sendData.GetDataStream();

                // Send data to server
                clientSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, epServer, new AsyncCallback(this.SendData), null);

                // Initialise data stream
                this.dataStream = new byte[1024];
                clientSocket.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None, ref epServer, new AsyncCallback(this.ReceiveData), null);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void SendData(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Data: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveData(IAsyncResult ar)
        {
            try
            {
                this.clientSocket.EndReceive(ar);
                Packet receivedData = new Packet(this.dataStream);

                if (receivedData.ChatMessage != null)
                {
                    this.Invoke(this.displayMessageDelegate, new object[] { receivedData.ChatMessage });

                    this.dataStream = new byte[1024];

                    clientSocket.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None, ref epServer, new AsyncCallback(this.ReceiveData), null);
                }

            }
            catch (ObjectDisposedException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show("Receive Data: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if(this.clientSocket != null)
                {
                    Packet sendData = new Packet();
                    sendData.ChatDataIdentifier = DataIdentifier.LogOut;
                    sendData.ChatName = this.name;
                    sendData.ChatMessage = null;

                    byte[] byteData = sendData.GetDataStream();
                    this.clientSocket.SendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer);

                    this.clientSocket.Close();
                }
            }catch (Exception ex)
            {
                MessageBox.Show("Closing Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                Packet sendData = new Packet();
                sendData.ChatName = this.name;
                sendData.ChatMessage = txtMessClient.Text.Trim();
                sendData.ChatDataIdentifier = DataIdentifier.Message;

                byte[] byteData = sendData.GetDataStream();

                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, new AsyncCallback(this.SendData), null);

                txtMessClient.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.clientSocket != null)
                {
                    Packet sendData = new Packet();
                    sendData.ChatDataIdentifier = DataIdentifier.LogOut;
                    sendData.ChatName = this.name;
                    sendData.ChatMessage = null;

                    byte[] byteData = sendData.GetDataStream();
                    this.clientSocket.SendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer);

                    this.clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Closing Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
    }
}