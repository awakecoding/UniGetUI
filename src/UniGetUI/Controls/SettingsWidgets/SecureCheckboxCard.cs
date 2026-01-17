using Avalonia.Layout;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using UniGetUI.Core.Logging;
using UniGetUI.Core.SettingsEngine.SecureSettings;
using UniGetUI.Core.Tools;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UniGetUI.Interface.Widgets
{
    public partial class SecureCheckboxCard : CommunityToolkit.WinUI.Controls.SettingsCard
    {
        public ToggleSwitch _checkbox;
        public TextBlock _textblock;
        public TextBlock _warningBlock;
        public ProgressBar _loading;
        private bool IS_INVERTED;

        private SecureSettings.K setting_name = SecureSettings.K.Unset;
        public SecureSettings.K SettingName
        {
            set
            {
                _checkbox.IsEnabled = false;
                setting_name = value;
                IS_INVERTED = SecureSettings.ResolveKey(value).StartsWith("Disable");
                (_checkbox.IsChecked ?? false) = SecureSettings.Get(setting_name) ^ IS_INVERTED ^ ForceInversion;
                _textblock.Opacity = (_checkbox.IsChecked ?? false) ? 1 : 0.7;
                _checkbox.IsEnabled = true;
            }
        }

        public new bool IsEnabled
        {
            set
            {
                base.IsEnabled = value;
                _warningBlock.Opacity = value ? 1 : 0.2;
            }
            get => base.IsEnabled;
        }

        public bool ForceInversion { get; set; }

        public bool Checked
        {
            get => (_checkbox.IsChecked ?? false);
        }
        public virtual event EventHandler<EventArgs>? StateChanged;

        public string Text
        {
            set => _textblock.Text = CoreTools.Translate(value);
        }

        public string WarningText
        {
            set
            {
                _warningBlock.Text = CoreTools.Translate(value);
                _warningBlock.IsVisible = value.Any();
            }
        }

        public SecureCheckboxCard()
        {
            _checkbox = new ToggleSwitch()
            {
                Margin = new Thickness(0, 0, 8, 0)
            };

            _loading = new ProgressBar() { IsIndeterminate = true, IsVisible = false};
            _textblock = new TextBlock()
            {
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0),
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            };
            _warningBlock = new TextBlock()
            {
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0),
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Foreground = (SolidColorBrush)Application.Current.Resources["SystemControlErrorTextForegroundBrush"],
                FontSize = 12,
                IsVisible = false,
            };
            IS_INVERTED = false;
            Content = new StackPanel()
            {
                Spacing = 4,
                Orientation = Orientation.Horizontal,
                Children = { _loading, _checkbox },
            };
            //Header = _textblock;
            Header = new StackPanel()
            {
                Spacing = 4,
                Orientation = Orientation.Vertical,
                Children = { _textblock, _warningBlock }
            };

            _checkbox.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            _checkbox.IsCheckedChanged += (s, e) => _ = _checkbox_Toggled();
        }
        protected virtual async Task _checkbox_Toggled()
        {
            try
            {
                if (_checkbox.IsEnabled is false)
                    return;

                _loading.IsVisible = true;
                _checkbox.IsEnabled = false;
                await SecureSettings.TrySet(setting_name, (_checkbox.IsChecked ?? false) ^ IS_INVERTED ^ ForceInversion);
                StateChanged?.Invoke(this, EventArgs.Empty);
                _textblock.Opacity = (_checkbox.IsChecked ?? false) ? 1 : 0.7;
                (_checkbox.IsChecked ?? false) = SecureSettings.Get(setting_name) ^ IS_INVERTED ^ ForceInversion;
                _loading.IsVisible = false;
                _checkbox.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
                (_checkbox.IsChecked ?? false) = SecureSettings.Get(setting_name) ^ IS_INVERTED ^ ForceInversion;
                _loading.IsVisible = false;
                _checkbox.IsEnabled = true;
            }
        }
    }
}
