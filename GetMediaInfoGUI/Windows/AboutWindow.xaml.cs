using GetMediaInfo;
using GetMediaInfoGUI.Configuration;
using GetMediaInfoGUI.Utils;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

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
                ProgramInfo = $"{Assembly.GetExecutingAssembly().GetName()?.Name} v{Assembly.GetExecutingAssembly().GetName().Version} - Made with ❤";
                LibraryInfo = $"MediaInfo v{mediainfoLibVersion ?? " unavailable"} (GetMediaInfo: v{getMediaInfoLibVersion ?? " unavailable"})";

                try
                {
                    this.FontFamily = new FontFamily(ConfigHelper.GetConfig().ReportFontFamily);
                }
                catch (Exception)
                {
                    this.FontFamily = new FontFamily(Constants.DefaultFontFamily);
                    ConfigHelper.GetConfig().ReportFontFamily = Constants.DefaultFontFamily;
                }

                this.FontSize = ConfigHelper.GetConfig().ReportFontSize;
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

            private double? fontSize;
            /// <summary>
            /// Gets or sets the font size.
            /// </summary>
            /// <value>
            /// The font size.
            /// </value>
            public double? FontSize
            {
                get
                {
                    return fontSize;
                }

                set
                {
                    fontSize = value ?? 12D;
                    NotifyPropertyChanged("FontSize");
                }
            }

            private FontFamily fontFamily;
            /// <summary>
            /// Gets or sets the font family.
            /// </summary>
            /// <value>
            /// The font family.
            /// </value>
            public FontFamily FontFamily
            {
                get
                {
                    return fontFamily;
                }

                set
                {
                    fontFamily = value ?? new FontFamily("Segoe UI");
                    NotifyPropertyChanged("FontFamily");
                }
            }

            private FontStyle fontStyle;
            /// <summary>
            /// Gets or sets the font style.
            /// </summary>
            /// <value>
            /// The font style.
            /// </value>
            public FontStyle FontStyle
            {
                get
                {
                    return fontStyle;
                }

                set
                {
                    if (value == null) { value = FontStyles.Normal; }
                    fontStyle = value;
                    NotifyPropertyChanged("FontStyle");
                }
            }

            private FontWeight fontWeight;
            /// <summary>
            /// Gets or sets the font weight.
            /// </summary>
            /// <value>
            /// The font weight.
            /// </value>
            public FontWeight FontWeight
            {
                get
                {
                    return fontWeight;
                }

                set
                {
                    if (value == null) { value = FontWeights.Normal; }
                    fontWeight = value;
                    NotifyPropertyChanged("FontWeight");
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
