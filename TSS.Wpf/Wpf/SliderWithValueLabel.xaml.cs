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

namespace TSS.Wpf
{
    public partial class SliderWithValueLabel : UserControl
    {
        public long SliderValue
        {
            get { return (long)slider.Value; }
        }
        public SliderWithValueLabel()
        {
            InitializeComponent();
        }
    }
}
