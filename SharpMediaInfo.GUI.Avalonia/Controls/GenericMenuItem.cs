using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;

namespace SharpMediaInfo.GUI.Avalonia.Controls;

public class GenericMenuItem : MenuItem
{
    protected override Type StyleKeyOverride => typeof(MenuItem);


    public static readonly StyledProperty<IEnumerable?> ChildrensProperty =
        AvaloniaProperty.Register<GenericMenuItem, IEnumerable?>(nameof(Childrens));

    public static readonly StyledProperty<ICommand?> ChildrenCommandProperty =
        AvaloniaProperty.Register<GenericMenuItem, ICommand?>(nameof(ChildrenCommand));

    bool IsAttachedToVisualTree { get; set; } = false;

    public IEnumerable? Childrens
    {
        get => GetValue(ChildrensProperty);
        set
        {
            if (GetValue(ChildrensProperty) is INotifyCollectionChanged oldNcc)
                oldNcc.CollectionChanged -= OnCollectionChanged;

            SetValue(ChildrensProperty, value);

            if (value is INotifyCollectionChanged newNcc)
                newNcc.CollectionChanged += OnCollectionChanged;

            UpdateItems();
        }
    }

    public ICommand? ChildrenCommand
    {
        get => GetValue(ChildrenCommandProperty);
        set
        {
            SetValue(ChildrenCommandProperty, value);
            UpdateItems();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        IsAttachedToVisualTree = true;
        UpdateItems(); 
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        IsAttachedToVisualTree = false;
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateItems();
    }

    private void UpdateItems()
    {
        if (!IsAttachedToVisualTree)
            return;

        Items.Clear();

        if (Childrens == null || ChildrenCommand == null)
            return;

        foreach (var item in Childrens)
        {
            Items.Add(new MenuItem
            {
                Header = item,
                Command = ChildrenCommand,
                CommandParameter = item
            });
        }
    }
}

