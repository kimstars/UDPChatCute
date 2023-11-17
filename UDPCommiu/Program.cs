using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPCommiu
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        class MyApplicationContext : ApplicationContext
        {
            private void onFormClosed(object sender, EventArgs e)
            {
                if (Application.OpenForms.Count == 0)
                {
                    ExitThread();
                }
            }

            public MyApplicationContext()
            {
             
                var forms = new List<Form>() {
                    new Server(),
                    new Client(),
                 };
                foreach (var form in forms)
                {
                    form.FormClosed += onFormClosed;
                }

                foreach (var form in forms)
                {
                    form.Show();
                }

            }
        }
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MyApplicationContext());


        }
    }
}
