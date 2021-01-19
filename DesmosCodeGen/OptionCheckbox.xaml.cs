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

namespace DesmosCodeGen
{
    /// <summary>
    /// Interaction logic for OptionCheckbox.xaml
    /// </summary>
    public partial class OptionCheckbox : UserControl
    {
        public OptionCheckbox(int i)
        {
            InitializeComponent();
            optionLabel.Content = IntToLetter(i) + ")";
        }

        /*
        * Returns the letter associated with each integer, like those you'd see
        * on a multiple choice question. Starts at 0.
        * So 0 returns 'A', 1 returns 'B', etc.
        */
        private char IntToLetter(int i)
        {
            return (char)(i + 65);
        }

        /*
         * Returns whether or not this UserControl's checkbox has been checked
         */
        public bool GetCheck()
        {
            bool? result = optionCheck.IsChecked;
            if (result == null)
                return false;
            return (bool) result;
        }
    }
}
