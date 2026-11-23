using Avalonia;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace SharpMediaInfo.GUI.Avalonia.Controls;

public partial class OpenRecentMenu : UserControl
{
    public static readonly StyledProperty<ObservableCollection<string>> ItemsProperty =
            AvaloniaProperty.Register<OpenRecentMenu, ObservableCollection<string>>(nameof(Items));

    public ObservableCollection<string> Items
    {
        get => GetValue(ItemsProperty);
        set
        {
            var oldValue = GetValue(ItemsProperty);
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= OnUpdateMenuItems;
            }
            SetValue(ItemsProperty, value);
            if (value != null)
            {
                value.CollectionChanged += OnUpdateMenuItems;
            }
        }
    }

    [GeneratedStyledProperty]
    public partial ICommand Command { get; set; }

    public OpenRecentMenu()
    {
        InitializeComponent();

        

    }

    private void OnUpdateMenuItems(object? sender, NotifyCollectionChangedEventArgs e)
    {
        MenuRoot.Items.Clear();

        foreach (var file in Items)
        {
            var mi = new MenuItem
            {
                Header = file,
                Command = Command,
                CommandParameter = file
            };
            MenuRoot.Items.Add(mi);
        }
    }
}