using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Resources;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostgreSQLControlApp
{
    public partial class fmMain : Form
    {
        public fmMain()
        {
            if (CultureInfo.CurrentCulture.Name == "ru-RU")
            {
                rs = new ResourceManager("PostgreSQLControlApp.ruResource", typeof(fmMain).Assembly);
            }
            else
            {
                rs = new ResourceManager("PostgreSQLControlApp.enResource", typeof(fmMain).Assembly);
            }
            InitializeComponent();
        }

        ResourceManager rs;
        const string ServiceName = "postgresql-x64-13";
        ServiceController postgresService;
        string serverStopped;
        string serverIsRunning;

        private void fmMain_Load(object sender, EventArgs e)
        {
            serverStopped = rs.GetString("serverStopped");
            serverIsRunning = rs.GetString("serverRunning");
            ServiceController[] services = ServiceController.GetServices();
            postgresService = services.Where(x => x.ServiceName == ServiceName).Single();
            if (postgresService.Status.Equals(ServiceControllerStatus.Stopped) ||
                postgresService.Status.Equals(ServiceControllerStatus.StopPending))
            {
                //if current status is "stopped"
                lbStatus.Text = serverStopped;
                lbStatus.ForeColor = Color.Red;
                btnStop.Enabled = false;
                btnRestart.Enabled = false;
            }
            else
            {
                //if current status is not "stopped"
                lbStatus.Text = serverIsRunning;
                lbStatus.ForeColor = Color.Green;
                btnStart.Enabled = false;
            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            btnRestart.Enabled = false;
            postgresService.Start();
            while (postgresService.Status != ServiceControllerStatus.Running)
            {
                Thread.Sleep(1000);
                postgresService.Refresh();
            }
            lbStatus.Text = serverIsRunning;
            lbStatus.ForeColor = Color.Green;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnRestart.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            btnRestart.Enabled = false;
            postgresService.Stop();
            while (postgresService.Status != ServiceControllerStatus.Stopped)
            {
                Thread.Sleep(1000);
                postgresService.Refresh();
            }
            lbStatus.Text = serverStopped;
            lbStatus.ForeColor = Color.Red;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnRestart.Enabled = false;
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            btnRestart.Enabled = false;
            postgresService.Stop();
            while (postgresService.Status != ServiceControllerStatus.Stopped)
            {
                Thread.Sleep(1000);
                postgresService.Refresh();
            }
            lbStatus.Text = serverStopped;
            lbStatus.ForeColor = Color.Red;

            postgresService.Start();
            while (postgresService.Status != ServiceControllerStatus.Running)
            {
                Thread.Sleep(1000);
                postgresService.Refresh();
            }
            lbStatus.Text = serverIsRunning;
            lbStatus.ForeColor = Color.Green;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnRestart.Enabled = true;
        }
    }
}
