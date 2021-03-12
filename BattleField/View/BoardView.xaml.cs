using BattleField.ViewModel;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace BattleField.View
{
    /// <summary>
    /// Interaction logic for BoardView.xaml
    /// </summary>
    public partial class BoardView : UserControl
    {
        public BoardView()
        {
            InitializeComponent();
            //string test = image1.Source.ToString();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtLog.ScrollToEnd();
        }

        private void Grid_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            
        }

        private void image1_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            Image i = (Image)sender;
            if (i.Source.ToString() == "/BattleField;component/Images/back.png")
                return;
            else
            {
                Storyboard sb = FindResource("FlipOpen") as Storyboard;
                Storyboard.SetTarget(sb, i);
                sb.Begin();
            }
        }

        private void image1_TargetUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void image1_LayoutUpdated(object sender, EventArgs e)
        {
            Image i = (Image)sender;
            if (i == null || i.Source.ToString() == "/BattleField;component/Images/back.png")
                return;
            else
            {
                Storyboard sb = FindResource("FlipOpen") as Storyboard;
                Storyboard.SetTarget(sb, i);
                sb.Begin();
            }
        }
    }
}
