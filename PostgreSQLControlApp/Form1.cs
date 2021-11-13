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
        const string ServiceName = "postgresql";
        ServiceController postgresService;
        ServiceController[] postgresServices;
        string serverStopped, serverIsRunning, serverNotFound;

        private void fmMain_Load(object sender, EventArgs e)
        {
            serverStopped = rs.GetString("serverStopped");
            serverIsRunning = rs.GetString("serverRunning");
            serverNotFound = rs.GetString("serverNotFound");
            ServiceController[] services = ServiceController.GetServices();
            postgresServices = services.Where(x => x.ServiceName.Contains(ServiceName)).ToArray();
            int lastUsedServerVersionIndex;
            try
            {
                lastUsedServerVersionIndex = Array.IndexOf(postgresServices, Properties.Settings.Default.LastUsedServerVersion);
            }
            catch (ArgumentNullException ex)
            {
                lbStatus.Text = serverNotFound;
                lbStatus.ForeColor = Color.Red;
                btnStart.Enabled = false;
                btnRestart.Enabled = false;
                btnStop.Enabled = false;
                return;
            }
            if (lastUsedServerVersionIndex == -1)
            {
                //Properties.Settings.Default.LastUsedServerVersion = postgresServices[0].ServiceName;
                lastUsedServerVersionIndex = 0;
            }
            for (int i = 0; i < postgresServices.Length; i++)
            {
                bool state = lastUsedServerVersionIndex == i;
                ToolStripMenuItem menuItem = new ToolStripMenuItem(postgresServices[i].ServiceName)
                {
                    Checked = state,
                    CheckOnClick = true
                };
                menuItem.CheckedChanged += menuItem_CheckedChanged;
                tsmiVersion.DropDownItems.Add(menuItem);
            }
            postgresService = postgresServices[lastUsedServerVersionIndex];
            SetControlsForCurrentStatus();
        }

        private void SetControlsForCurrentStatus()
        {
            if (postgresService.Status.Equals(ServiceControllerStatus.Stopped) ||
    postgresService.Status.Equals(ServiceControllerStatus.StopPending))
            {
                //if current status is "stopped"
                lbStatus.Text = serverStopped;
                lbStatus.ForeColor = Color.Red;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                btnRestart.Enabled = false;
            }
            else
            {
                //if current status is not "stopped"
                lbStatus.Text = serverIsRunning;
                lbStatus.ForeColor = Color.Green;
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                btnRestart.Enabled = true;
            }
        }

        private void menuItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem.CheckState == CheckState.Checked)
            {
                int index = tsmiVersion.DropDownItems.IndexOf(menuItem);
                for (int i = 0; i < tsmiVersion.DropDownItems.Count; i++)
                {
                    ToolStripMenuItem menuItem1 = (ToolStripMenuItem)tsmiVersion.DropDownItems[i];
                    if (menuItem1.CheckState == CheckState.Checked && i != index)
                    {
                        menuItem1.CheckState = CheckState.Unchecked;
                    }
                }
                postgresService = postgresServices[index];
                SetControlsForCurrentStatus();
            }
            else
            {
                bool flag = false;
                int index = tsmiVersion.DropDownItems.IndexOf(menuItem);
                for (int i = 0; i < tsmiVersion.DropDownItems.Count; i++)
                {
                    ToolStripMenuItem menuItem1 = (ToolStripMenuItem)tsmiVersion.DropDownItems[i];
                    if(menuItem1.CheckState == CheckState.Checked && i != index)
                    {
                        flag = true;
                    }
                }
                if (menuItem.CheckState == CheckState.Unchecked && !flag)
                {
                    menuItem.CheckState = CheckState.Checked;
                }
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
