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

namespace BucketGame
{
    /// <summary>
    /// Interaction logic for Bucket.xaml
    /// </summary>
    public partial class Bucket : UserControl
    {
        public Bucket()
        {
            InitializeComponent();
        }
        public Brush Fill
        {
            get { return ellipse.Fill; }
            set { ellipse.Fill = value; }
        }
    }

}
