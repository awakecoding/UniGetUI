using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System;

namespace CommunityToolkit.WinUI.Controls
{
    // Stub implementation of SettingsCard for migration
    // This is a temporary replacement for the WinUI SettingsCard control
    public class SettingsCard : ContentControl
    {
        public static readonly StyledProperty<string> HeaderProperty =
            AvaloniaProperty.Register<SettingsCard, string>(nameof(Header));

        public static readonly StyledProperty<string> DescriptionProperty =
            AvaloniaProperty.Register<SettingsCard, string>(nameof(Description));

        public static readonly StyledProperty<object> HeaderIconProperty =
            AvaloniaProperty.Register<SettingsCard, object>(nameof(HeaderIcon));

        public static readonly StyledProperty<bool> IsClickEnabledProperty =
            AvaloniaProperty.Register<SettingsCard, bool>(nameof(IsClickEnabled));

        // Click event for Avalonia
        public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
            RoutedEvent.Register<SettingsCard, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

        public event EventHandler<RoutedEventArgs>? Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        public string Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public string Description
        {
            get => GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public object HeaderIcon
        {
            get => GetValue(HeaderIconProperty);
            set => SetValue(HeaderIconProperty, value);
        }

        public bool IsClickEnabled
        {
            get => GetValue(IsClickEnabledProperty);
            set => SetValue(IsClickEnabledProperty, value);
        }

        protected override void OnPointerReleased(Avalonia.Input.PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (IsClickEnabled)
            {
                RaiseEvent(new RoutedEventArgs(ClickEvent));
            }
        }
    }
}
