using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Logica di interazione per UpdateDLL.xaml
    /// </summary>
    public partial class UpdateDLL : Window
    {
        enum Status { NoUpdate, Success, Error };
        private Status? _status;
        public UpdateDLL()
        {
            InitializeComponent();
        }
        public string status
        {
            get
            {
                switch (_status)
                {
                    case Status.NoUpdate: return "Mediainfo.dll is update."; 
                    case Status.Error: return "Error in update."; 
                    case Status.Success: return "Mediainfo.dll it was successfully updated.";
                    default: return "Generic error.";
                }
            }
        }
    }
}
