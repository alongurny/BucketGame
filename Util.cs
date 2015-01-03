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
    class Util
    {
        public static Point RandomPoint(Random random)
        {
            Point p = new Point(random.Next(Consts.ScreenWidth - 100), random.Next(Consts.ScreenHeight - 100));
            // StatusLabel.Content = p.ToString();
            return p;
        }
        public static Point RandomPointWithFairDistanceFrom(Random random, params Point[] points)
        {
            Point ran;
            do
            {
                ran = RandomPoint(random);
            } while (Util.IsFairDistanceFromAll(ran, points));
            return ran;
        }
        public static Point RandomPointAtTopHalfOfScreen(Random random)
        {
            return new Point(random.Next(Consts.PortalSize,Consts.ScreenWidth-Consts.PortalSize), random.Next(0,Consts.ScreenHeight / 2));
        }
        public static bool IsFairDistanceFromAll(Point p, params Point[] points)
        {
            foreach (Point other in points)
            {
                if (Distance(other, p) > Consts.FairDistance)
                {
                    return false;
                }
            }
            return true;
        }
        public static void DrawOnImage(ColorImageFrame colorFrame, Image image)
        {

            if (colorFrame == null || image == null)
            {
                throw new ArgumentNullException("Value cannot be null");
            }
            byte[] pixels = new byte[colorFrame.PixelDataLength];
            colorFrame.CopyPixelDataTo(pixels);
            int stride = colorFrame.Width * 4;
            image.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
        }

        private static Polygon[] polygons = new Polygon[8];

        public static Polygon MakePolygon(int n, double radius)
        {
            Polygon p = new Polygon();
            p.Width = 2 * radius;
            p.Height = 2 * radius;
            double deltaAngle = 2 * Math.PI / n ;
            double startAngle = Math.PI / n;
            for (int i = 0; i < n; i++)
            {
                p.Points.Add(new Point(radius + Math.Sin(i * deltaAngle + startAngle) * radius,  radius + Math.Cos(i * deltaAngle + startAngle) * radius));
            }
            return p;
        }

        public static Polygon GetPolygon(int n, double radius)
        {
            if (polygons[n] == null)
            {
                polygons[n] = new Polygon();
                polygons[n].Width = 2 * radius;
                polygons[n].Height = 2 * radius;
                double deltaAngle = 2 * Math.PI / n;
                for (int i = 0; i < n; i++)
                {
                    polygons[n].Points.Add(new Point(radius + Math.Sin(i * deltaAngle) * radius, radius - Math.Cos(i * deltaAngle) * radius));
                }
            }
            return polygons[n];
        }

        public static float Round(float num)
        {
            return ((int)(num * 1000)) / 1000.0f;
        }

        public static Point NewPoint(Skeleton skel, JointType joint, DepthImageFrame depth)
        {
           SkeletonPoint point = skel.Joints[joint].Position;
           if (depth == null)
           {
               MessageBox.Show("depth null");
           }
           if (point == null)
           {
               MessageBox.Show("point null");
           }
           DepthImagePoint dip = depth.MapFromSkeletonPoint(point);
           return new Point(dip.X, dip.Y);
        }

        public static Point ToPoint(Polygon elem)
        {
            return new Point(Canvas.GetLeft(elem), Canvas.GetTop(elem));
        }
        public static Point ToCenterPoint(Point topLeftPoint)
        {
            return new Point(topLeftPoint.X + Consts.ShapeRadius, topLeftPoint.Y + Consts.ShapeRadius);
        }
        public static Point FromCenterPoint(Point topLeftPoint)
        {
            return new Point(topLeftPoint.X - Consts.ShapeRadius, topLeftPoint.Y - Consts.ShapeRadius);
        }

        public static double Distance(Point p1, Point p2)
        {
            double dx = p2.X - p1.X, dy = p2.Y - p1.Y;
            return Math.Sqrt(dx*dx + dy*dy);
        }
        public static Point ToPoint(dynamic point)
        {
            return new Point(point.X, point.Y);
        }

        
    }
}
