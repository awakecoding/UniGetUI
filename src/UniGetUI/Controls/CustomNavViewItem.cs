using Avalonia;
using Avalonia.Controls;
using UniGetUI.Core.Tools;
using UniGetUI.Interface;
using UniGetUI.Interface.Enums;
using UniGetUI.Interface.Widgets;

namespace UniGetUI.Controls;
internal partial class CustomNavViewItem : ListBoxItem
{
    int _iconSize = 28;
    
    // Icon properties as StyledProperties for Avalonia
    public static readonly StyledProperty<string?> GlyphIconProperty =
        AvaloniaProperty.Register<CustomNavViewItem, string?>(nameof(GlyphIcon));

    public string? GlyphIcon
    {
        get => GetValue(GlyphIconProperty);
        set => SetValue(GlyphIconProperty, value);
    }

    public static readonly StyledProperty<Control?> IconProperty =
        AvaloniaProperty.Register<CustomNavViewItem, Control?>(nameof(Icon));

    public Control? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<Control?> InfoBadgeProperty =
        AvaloniaProperty.Register<CustomNavViewItem, Control?>(nameof(InfoBadge));

    public Control? InfoBadge
    {
        get => GetValue(InfoBadgeProperty);
        set => SetValue(InfoBadgeProperty, value);
    }

    public IconType LocalIcon
    {
        set => Icon = new LocalIcon(value);
    }

    public bool IsLoading
    {
        set
        {
            // TODO: Avalonia - Animation disabled until Icon property is reimplemented
            // if (value) _ = increaseMargins();
            // else _ = decreaseMargins();
            _progressRing.IsVisible = value;
        }
    }

    public int IconSize
    {
        set => Resources["NavigationViewItemOnLeftIconBoxHeight"] = _iconSize = value;
    }

    public string Text
    {
        set
        {
            string text = CoreTools.Translate(value);
            _textBlock.Text = text;
            // TODO: Avalonia - ToolTipService may have different accessibility in Avalonia
            ToolTip.SetTip(this, text);
        }

    }

    private readonly TextBlock _textBlock;
    private readonly ProgressBar _progressRing;

    private PageType _pageType;
    public new PageType Tag
    {
        set => _pageType = value;
        get => _pageType;
    }

    public CustomNavViewItem()
    {
        Height = 60;
        Resources["NavigationViewItemOnLeftIconBoxHeight"] = _iconSize;
        Resources["NavigationViewItemContentPresenterMargin"] = new Thickness(0);

        var grid = new Grid { Height = 50 };

        _progressRing = new ProgressBar
        {
            Margin = new Thickness(-46, 0, 0, 0),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            IsIndeterminate = true,
            IsVisible = false,
        };

        _textBlock = new TextBlock
        {
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        };

        grid.Children.Add(_progressRing);
        grid.Children.Add(_textBlock);
        base.Content = grid;
    }

    // TODO: Avalonia - increaseMargins/decreaseMargins depend on Icon property which doesn't exist in ListBoxItem
    // These methods need to be reimplemented when Icon support is added
    /*
    public async Task increaseMargins()
    {
        for(int i = (int)base.Icon.Margin.Left; i < 6; i += 2)
        {
            base.Icon.Margin = new Thickness(i);
            await Task.Delay(15);
        }
        base.Icon.Margin = new Thickness(6);
    }

    public async Task decreaseMargins()
    {
        for (int i = (int)base.Icon.Margin.Left; i > 0; i -= 2)
        {
            base.Icon.Margin = new Thickness(i);
            await Task.Delay(15);
        }
        base.Icon.Margin = new Thickness(0);
    }
    */
}
