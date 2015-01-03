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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor sensor;
        Skeleton skeleton;
        static Random random = new Random();
        bool HasTouchedPolygon = false;
        Bucket[] Portals = new Bucket[Consts.Colors.Length];
        Bucket Portal;
        int ActiveColor;
        public MainWindow()
        {
            
            InitializeComponent();

            sensor = KinectSensor.KinectSensors.FirstOrDefault(s => s != null);
            if (sensor == default(KinectSensor))
            {
                MessageBox.Show("Kinect sensor is null");
                return;
            }
            int x = -Consts.PortalSize, y = Consts.ScreenHeight - Consts.PortalSize * 3/ 2;
            for (int i = 0; i < Consts.Colors.Length; i++)
            {
                //MessageBox.Show(i.ToString()); 
                x += Consts.DistanceBetweenPortals;
                Portals[i] = new Bucket();
                Bucket it = Portals[i];
                it.HorizontalAlignment = HorizontalAlignment.Left;
                it.VerticalAlignment = VerticalAlignment.Top;
                it.Width = Consts.PortalSize;
                it.Height = Consts.PortalSize;
                it.Fill = Consts.Colors[i];
                PortalGrid.Children.Add(it);
                Canvas.SetLeft(it,x);
                Canvas.SetTop(it, y);
                
            }
            Portal = Portals[2];
            sensor.ColorStream.Enable();
            sensor.DepthStream.Enable();
            sensor.SkeletonStream.Enable();
            sensor.AllFramesReady += sensor_AllFramesReady;
            sensor.Start();
            //sensor.ElevationAngle = 27;
            CreateNextShape(new Point(0,0));
            
        }

        public static void MoveTo(UIElement elem, Point point)
        {
            Canvas.SetLeft(elem, point.X);
            Canvas.SetTop(elem, point.Y);   
        }
        

        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame == null)
                {
                    return; //TODO add "hold on, lag is occuring..."
                }
                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                {
                    if (colorFrame != null)
                    {
                        Util.DrawOnImage(colorFrame, frame);
                    }
                }

                using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                {
                    if (skeletonFrame != null)
                    {
                        if (!sensor.IsRunning)
                        {
                            StatusLabel.Content = "Kinect isn't running (check is plugged in)";
                            return;
                        }
                        Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                        skeletonFrame.CopySkeletonDataTo(skeletons);
                        skeleton = (from s in skeletons where s != null && s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                        if (skeleton == default(Skeleton))
                        {
                            StatusLabel.Content = "No skeleton tracked";
                            return;
                        }
                        else
                        {
                            StatusLabel.Content = "";
                        }
                       
                        //StatusLabel.Content = "";
                        SkeletonPoint sp = skeleton.Joints[JointType.HandRight].Position;
                        Point rightHand = Util.NewPoint(skeleton, JointType.HandRight, depthImageFrame);
                  
                        Point paulPoint = Util.ToCenterPoint(Util.ToPoint(pol));
                        double distance = Util.Distance(Util.FromCenterPoint(rightHand), Util.ToPoint(pol));
                        //StatusLabel.Content = String.Format("Pol: {0} | Hand: {1}", paulPoint, rightHand);
                        if (HasTouchedPolygon)
                        {
                            MoveTo(pol, Util.FromCenterPoint(rightHand));
                            Point goal = new Point(Canvas.GetLeft(Portal), Canvas.GetTop(Portal));
                            double distanceFromTarget = Util.Distance(goal, Util.FromCenterPoint(rightHand));
                            if (distanceFromTarget <= Consts.TouchingDistance)
                            {
                                HasTouchedPolygon = false;
                                CreateNextShape(rightHand);
                            }
                            
                        }
                        //StatusLabel.Content = String.Format("{0}", distance);
                        else if (distance <= Consts.ShapeRadius)
                        {
                            HasTouchedPolygon = true;
                        }
                        
                    }
                }
            }
        }

      
        public void CreateNextShape(Point rightHand)
        {
            ActiveColor = random.Next(0, Consts.NumberOfPortals);
            pol.Fill = Consts.Colors[ActiveColor];
            Portal = Portals[ActiveColor];
            pol.Points = Util.MakePolygon(random.Next(3, 10), Consts.ShapeRadius).Points;
            MoveTo(pol, Util.RandomPointAtTopHalfOfScreen(random));
            //MoveTo(Portal,RandomPointWithFairDistanceFrom(Util.ToPoint(pol)));
            HasTouchedPolygon = false;
        }

        private Point RandomPoint()
        {
            Point p= new Point(random.Next((int)frame.Width - 100), random.Next((int)frame.Height - 100));
           // StatusLabel.Content = p.ToString();
            return p;
        }

        private Point RandomPointWithFairDistanceFrom(Point p, double fairDistance)
        {
            Point ran;
            do
            {
                ran = RandomPoint();
            } while (Util.Distance(ran, p) <= fairDistance);
            return ran;
        }
        private Point RandomPointWithFairDistanceFrom(Point p)
        {
            return RandomPointWithFairDistanceFrom(p, Consts.FairDistance);
        }



      
    }
}
