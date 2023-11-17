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

namespace UDPCommiu
{
    public partial class Server : DevExpress.XtraEditors.XtraForm
    {

        
        UDPSocket s = new UDPSocket();
        public Server()
        {
            InitializeComponent();
            s.Server("127.0.0.1", 27000);

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string msgsend = txtMessSend.Text;
            s.Send(msgsend);
            txtStatus.Text = msgsend + "\n";
        }
    }
}