using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Interactivity;
using UniGetUI.Core.SettingsEngine;
using UniGetUI.Core.Tools;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UniGetUI.Interface.Widgets
{
    public partial class CheckboxButtonCard : CommunityToolkit.WinUI.Controls.SettingsCard
    {
        public ToggleSwitch _checkbox;
        public TextBlock _textblock;
        public Button Button;
        private bool IS_INVERTED;

        private Settings.K setting_name = Settings.K.Unset;
        public Settings.K SettingName
        {
            set {
                setting_name = value;
                IS_INVERTED = Settings.ResolveKey(value).StartsWith("Disable");
                (_checkbox.IsChecked ?? false) = Settings.Get(setting_name) ^ IS_INVERTED ^ ForceInversion;
                _textblock.Opacity = (_checkbox.IsChecked ?? false) ? 1 : 0.7;
                Button.IsEnabled = ((_checkbox.IsChecked ?? false)) || _buttonAlwaysOn ;
            }
        }

        public bool ForceInversion { get; set; }

        public bool Checked
        {
            get => (_checkbox.IsChecked ?? false);
        }
        public event EventHandler<EventArgs>? StateChanged;
        public new event EventHandler<Avalonia.Interactivity.RoutedEventArgs>? Click;

        public string CheckboxText
        {
            set => _textblock.Text = CoreTools.Translate(value);
        }

        public string ButtonText
        {
            set => Button.Content = CoreTools.Translate(value);
        }

        private bool _buttonAlwaysOn;
        public bool ButtonAlwaysOn
        {
            set
            {
                _buttonAlwaysOn = value;
                Button.IsEnabled = ((_checkbox.IsChecked ?? false)) || _buttonAlwaysOn ;
            }
        }

        public CheckboxButtonCard()
        {
            Button = new Button()
            {
                Margin = new Thickness(0, 8, 0, 0)

            };
            _checkbox = new ToggleSwitch()
            {
                 Margin = new Thickness(0, 0, 8, 0)
            };
            _textblock = new TextBlock()
            {
                Margin = new Thickness(2,0,0,0),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Avalonia.Styling.Style = (Avalonia.Styling.Style)Application.Current.Resources["BaseTextBlockStyle"],
                FontWeight = FontWeight.Normal,
                Foreground = (SolidColorBrush)Application.Current.Resources["ButtonForeground"]
            };
            IS_INVERTED = false;

            Content = _checkbox;
            Header = _textblock;
            Description = Button;
            _checkbox.IsCheckedChanged += (_, _) =>
            {
                Settings.Set(setting_name, (_checkbox.IsChecked ?? false) ^ IS_INVERTED ^ ForceInversion);
                StateChanged?.Invoke(this, EventArgs.Empty);
                Button.IsEnabled = (_checkbox.IsChecked ?? false) ? true : _buttonAlwaysOn;
                _textblock.Opacity = (_checkbox.IsChecked ?? false) ? 1 : 0.7;
            };

            Button.Click += (s, e) => Click?.Invoke(s, e);
        }
    }
}
