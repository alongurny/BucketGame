using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
namespace BucketGame
{
    class Consts
    {
        public static readonly Brush[] Colors = { Brushes.Blue, Brushes.Red, Brushes.Yellow, Brushes.Green };
        public static readonly int ShapeRadius = 30, TouchingDistance = 35, NumberOfPortals = Colors.Length,
            ScreenWidth = 640, ScreenHeight = 480, PortalSize = 100,
            DistanceBetweenPortals = (ScreenWidth - NumberOfPortals * PortalSize) / (NumberOfPortals + 1) + PortalSize; //dont question my maths
        public static readonly double FairDistance = 130;
        
       
    }
}
