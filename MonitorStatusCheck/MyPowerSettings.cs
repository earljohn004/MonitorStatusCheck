//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.ComponentModel;

namespace MonitorStatusCheck
{
    internal class MyPowerSettings : INotifyPropertyChanged
    {
        bool monitorOn;
        bool monitorRequired;
        bool stateSetting = true;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        public bool StateSetting
        {
            get { return stateSetting; }
            set
            {
                    stateSetting = value;
                    OnPropertyChanged("stateSetting");
            }

        }
        
        public bool MonitorOn
        {
            get { return monitorOn; }
            set
            {
                if (monitorOn != value)
                {
                    monitorOn = value;
                    OnPropertyChanged("MonitorOn");
                }
            }
        }
        public bool MonitorRequired
        {
            get { return monitorRequired; }
            set
            {
                if (monitorRequired != value)
                {
                    monitorRequired = value;
                    OnPropertyChanged("MonitorRequired");
                }
            }
        }

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
