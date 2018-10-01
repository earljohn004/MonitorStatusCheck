using Microsoft.WindowsAPICodePack.ApplicationServices;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MonitorStatusCheck
{
    public partial class Service1 : ServiceBase
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        Timer timer = new Timer(); // name space(using System.Timers;)  
        Timer shutDownTimer = new Timer();
        private MyPowerSettings settings;

        public Service1()
        {
            InitializeComponent();
            settings = new MyPowerSettings();
           // PowerManager.IsMonitorOnChanged += new EventHandler(MonitorOnChanged);
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);

        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {

            if (settings.StateSetting == true)      //True State will check for monitor activity
            {
                WriteToFile(String.Format("settings.StateSetting[{0}]", settings.StateSetting));
                MonitorOnChanged();
            }
            else                                  //False state will run timer before shutdown
            {
                shutDownTimer.Elapsed += new ElapsedEventHandler(shutdownStateTimer);
                shutDownTimer.Interval = 60000;    //number in milisecinds  
                shutDownTimer.Enabled = true;
                timer.Stop();            //Main timer is now disabled. Shutdown timer is waiting for monitor activity for 60 seconds until shutdown

            }
        }

        private void shutdownStateTimer(object sender, ElapsedEventArgs e)
        {
            if (settings.StateSetting == true)
            {
                WriteToFile("[SHUTDOWN COMMAND] Shutting Down PC...");
            }else
            {
                WriteToFile("Changing PC State...");
                shutDownTimer.Enabled = false;
                timer.Enabled = true;
                shutDownTimer.Stop();
                shutDownTimer.Dispose();
            }
            
        }

        private void changeState(bool monitorStatus)
        {
            //Toggle StateSetting
            settings.StateSetting = monitorStatus;
            WriteToFile(string.Format("State setting changed [STATE: {0}]", monitorStatus ? "true" : "false"));
        }

        void MonitorOnChanged()
        {
            bool monitorStatus;
            settings.MonitorOn = PowerManager.IsMonitorOn;
            monitorStatus = PowerManager.IsMonitorOn ? true : false;
            changeState(monitorStatus);
            WriteToFile(string.Format("Monitor status changed (new status: {0})", monitorStatus ? "On" : "Off"));
        }

        //Helper Function for Debugging
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

    }
}
