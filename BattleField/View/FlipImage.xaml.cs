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
    /// Interaction logic for FlipImage.xaml
    /// </summary>
    public partial class FlipImage : UserControl
    {
        //public FlipImage()
        //{
        //    InitializeComponent();
        //}

        public bool Reversed = false;
        public Storyboard sbReverse;
        public Storyboard sbFlip;
        public Binding ImageSource;

        public string FrontImage
        {
            set
            {
                imgFront.Source = new BitmapImage(new Uri(value, UriKind.Relative));
            }
            get
            {
                return imgFront.Source.ToString();
            }
        }

        public string BackImage
        {
            set
            {
                imgBack.Source = new BitmapImage(new Uri(value, UriKind.Relative));
            }
            get
            {
                return imgBack.Source.ToString();
            }
        }

        public FlipImage()
        {
            InitializeComponent();
            sbFlip = this.Resources["sbFlip"] as Storyboard;
            sbReverse = this.Resources["sbReverse"] as Storyboard;
            sbFlip.Completed += new EventHandler(sbFlip_Completed);
            sbReverse.Completed += new EventHandler(sbReverse_Completed);
        }

        void sbReverse_Completed(object sender, EventArgs e)
        {
            Reversed = false;
        }

        void sbFlip_Completed(object sender, EventArgs e)
        {
            Reversed = true;
        }

        public void Flip()
        {
            if (!Reversed)
            {
                sbFlip.Begin();
            }
        }

        public void Reverse()
        {
            if (Reversed)
            {
                sbReverse.Begin();
            }
        }
    }
}
