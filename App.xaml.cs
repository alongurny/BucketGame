using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Kinect;

namespace BucketGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainWindow mainWindow;

        public static int ShapeScored { get; set; }

        public App()
        {
            mainWindow = new MainWindow();
        }

        public static KinectSensor Sensor { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            mainWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (Sensor != null)
            {
                Sensor.SkeletonStream.Disable();
                Sensor.DepthStream.Disable();
                Sensor.ColorStream.Disable();
            }
        }
    }
}
