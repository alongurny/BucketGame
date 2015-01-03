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
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;

namespace BucketGame
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : Window
    {

        MainWindow main;

        public ControlPanel(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            this.main.Closed += (sender, e) => this.Close();
        }

        public string ScoredShapes
        {
            get { return Settings.ShapesScored + ""; }
        }
    }
}
