using GetMediaInfo;
using GetMediaInfoGUI.Utils;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace GetMediaInfoGUI.Windows
{
    /// <summary>
    /// Logica di interazione per AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        private readonly AboutModel aboutModel;

        public AboutWindow()
        {
            InitializeComponent();

            this.aboutModel = new AboutModel();
            this.DataContext = aboutModel;
        }

        private sealed class AboutModel : INotifyPropertyChanged
        {
            public AboutModel()
            {
                var mediainfoLibVersion = FileUtils.GetAssemblyVersion(FolderUtils.GetAbsolutePathFromBaseDir("MediaInfo.dll"));
                var getMediaInfoLibVersion = FileUtils.GetAssemblyVersion(FolderUtils.GetAbsolutePathFromBaseDir("GetMediaInfo.dll"));

                KnownCodecs = MediaInfo.KnownCodecs();
                Capacities = MediaInfo.Capacities();
                ParametersList = MediaInfo.ParametersList();
                ProgramInfo = $"{Assembly.GetExecutingAssembly().GetName()?.Name} v{Assembly.GetExecutingAssembly().GetName().Version} - Made with ❤ ";
                LibraryInfo = $"MediaInfo v{mediainfoLibVersion ?? " unavailable"} (GetMediaInfo: v{getMediaInfoLibVersion ?? " unavailable"})";
            }

            private string programInfo;
            public string ProgramInfo
            {
                get
                {
                    return programInfo;
                }

                set
                {
                    programInfo = value;
                    NotifyPropertyChanged("ProgramInfo");
                }
            }

            private string libraryInfo;
            public string LibraryInfo
            {
                get
                {
                    return libraryInfo;
                }

                set
                {
                    libraryInfo = value;
                    NotifyPropertyChanged("LibraryInfo");
                }
            }

            private string knownCodecs;
            public string KnownCodecs
            {
                get
                {
                    return knownCodecs;
                }
                set
                {
                    knownCodecs = value;
                    NotifyPropertyChanged("KnownCodecs");
                }
            }

            private string capacities;
            public string Capacities
            {
                get
                {
                    return capacities;
                }
                set
                {
                    capacities = value;
                    NotifyPropertyChanged("Capacities");
                }
            }

            private string parametersList;
            public string ParametersList
            {
                get
                {
                    return parametersList;
                }
                set
                {
                    parametersList = value;
                    NotifyPropertyChanged("ParametersList");
                }
            }

            #region INotifyPropertyChanged

            /// <summary>
            /// Generated when a property value changes.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Notifies the property changed.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            private void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion
        }
    }
}
