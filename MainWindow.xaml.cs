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
        /// <summary>
        /// Format we will use for the depth stream
        /// </summary>
        private const DepthImageFormat DepthFormat = DepthImageFormat.Resolution320x240Fps30;

        /// <summary>
        /// Format we will use for the color stream
        /// </summary>
        private const ColorImageFormat ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Bitmap that will hold opacity mask information
        /// </summary>
        private WriteableBitmap playerOpacityMaskImage = null;

        /// <summary>
        /// Intermediate storage for the depth data received from the sensor
        /// </summary>
        private DepthImagePixel[] depthPixels;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        /// <summary>
        /// Intermediate storage for the player opacity mask
        /// </summary>
        private int[] playerPixelData;

        /// <summary>
        /// Intermediate storage for the depth to color mapping
        /// </summary>
        private ColorImagePoint[] colorCoordinates;

        /// <summary>
        /// Inverse scaling factor between color and depth
        /// </summary>
        private int colorToDepthDivisor;

        /// <summary>
        /// Width of the depth image
        /// </summary>
        private int depthWidth;

        /// <summary>
        /// Height of the depth image
        /// </summary>
        private int depthHeight;

        /// <summary>
        /// Indicates opaque in an opacity mask
        /// </summary>
        private int opaquePixelValue = -1;


        KinectSensor sensor;
        Skeleton skeleton;

        /// <summary>
        /// Whether the player is holding a polygon right now or not
        /// </summary>
        bool isHoldingPolygon = false;
        Bucket[] portals = new Bucket[Consts.Colors.Length];
        Bucket portal;
        int activeColor;
        public MainWindow()
        {

            InitializeComponent();

            sensor = KinectSensor.KinectSensors.FirstOrDefault(s => s != null);
            if (sensor == null)
            {
                MessageBox.Show("KinectSensor is not found. Please check if the kinect is " +
                "currently connected to your computer and try again.");
                return;
            }

        }

        public static void MoveTo(UIElement elem, Point point)
        {
            Canvas.SetLeft(elem, point.X);
            Canvas.SetTop(elem, point.Y);
        }


        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
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
                        if (skeleton == null)
                        {
                            StatusLabel.Content = "No skeleton tracked";
                            return;
                        }
                        else
                        {
                            StatusLabel.Content = "";
                        }
                        Point rightHand = Util.NewPoint(skeleton, JointType.HandRight, depthFrame);

                        Point paulPoint = Util.ToCenterPoint(Util.ToPoint(pol));
                        double distance = Util.Distance(Util.FromCenterPoint(rightHand), Util.ToPoint(pol));

                        if (isHoldingPolygon)
                        {
                            MoveTo(pol, Util.FromCenterPoint(rightHand));
                            Point goal = new Point(Canvas.GetLeft(portal), Canvas.GetTop(portal));
                            double distanceFromTarget = Util.Distance(goal, Util.FromCenterPoint(rightHand));
                            if (distanceFromTarget <= Consts.TouchingDistance)
                            {
                                isHoldingPolygon = false;
                                CreateNextShape(rightHand);
                            }

                        }
                        else if (distance <= Consts.ShapeRadius)
                        {
                            isHoldingPolygon = true;
                        }

                    }
                }
            }
        }


        public void CreateNextShape(Point rightHand)
        {
            activeColor = Util.Random.Next(0, Consts.NumberOfPortals);
            pol.Fill = Consts.Colors[activeColor];
            portal = portals[activeColor];
            pol.Points = Util.MakePolygon(Util.Random.Next(3, 10), Consts.ShapeRadius).Points;
            MoveTo(pol, Util.RandomPointAtTopHalfOfScreen());
            //MoveTo(Portal,RandomPointWithFairDistanceFrom(Util.ToPoint(pol)));
            isHoldingPolygon = false;
        }

        private Point RandomPoint()
        {
            return new Point(Util.Random.Next((int)frame.Width - 100), Util.Random.Next((int)frame.Height - 100));
        }

        /// <summary>
        /// Return a random point, created in a fair (given) distance from a given point (the "center")
        /// </summary>
        /// <param name="p">Point that the distance is measured from</param>
        /// <param name="fairDistance">fair distance from the point</param>
        /// <returns></returns>
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int x = -Consts.PortalSize, y = Consts.ScreenHeight - Consts.PortalSize * 3 / 2;
            for (int i = 0; i < Consts.Colors.Length; i++)
            {
                //MessageBox.Show(i.ToString()); 
                x += Consts.DistanceBetweenPortals;
                portals[i] = new Bucket();
                Bucket it = portals[i];
                it.HorizontalAlignment = HorizontalAlignment.Left;
                it.VerticalAlignment = VerticalAlignment.Top;
                it.Width = Consts.PortalSize;
                it.Height = Consts.PortalSize;
                it.Fill = Consts.Colors[i];
                PortalGrid.Children.Add(it);
                Canvas.SetLeft(it, x);
                Canvas.SetTop(it, y);

            }
            portal = portals[2];
            sensor.ColorStream.Enable();
            sensor.DepthStream.Enable();
            sensor.SkeletonStream.Enable();
            sensor.AllFramesReady += sensor_AllFramesReady;
            sensor.Start();
            //sensor.ElevationAngle = 27;
            CreateNextShape(new Point(0, 0));
            // new ControlPanel(this).Show();

        }




    }
}
