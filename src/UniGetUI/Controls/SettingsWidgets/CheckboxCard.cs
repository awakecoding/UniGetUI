using Avalonia.Layout;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Interactivity;
using UniGetUI.Core.SettingsEngine;
using UniGetUI.Core.Tools;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UniGetUI.Interface.Widgets
{
    public partial class CheckboxCard : CommunityToolkit.WinUI.Controls.SettingsCard
    {
        public ToggleSwitch _checkbox;
        public TextBlock _textblock;
        public TextBlock _warningBlock;
        protected bool IS_INVERTED;

        // Property to expose _checkbox for XAML binding
        public ToggleSwitch CheckBox => _checkbox;

        private Settings.K setting_name = Settings.K.Unset;
        public Settings.K SettingName
        {
            set
            {
                setting_name = value;
                IS_INVERTED = Settings.ResolveKey(value).StartsWith("Disable");
                _checkbox.IsChecked = Settings.Get(setting_name) ^ IS_INVERTED ^ ForceInversion;
                _textblock.Opacity = (_checkbox.IsChecked ?? false) ? 1 : 0.7;
            }
        }

        public bool ForceInversion { get; set; }

        public bool Checked
        {
            get => _checkbox.IsChecked ?? false;
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

        public Brush WarningForeground
        {
            set => _warningBlock.Foreground = value;
        }

        public double WarningOpacity
        {
            set => _warningBlock.Opacity = value;
        }

        public CheckboxCard()
        {
            _checkbox = new ToggleSwitch()
            {
                Margin = new Thickness(0, 0, 8, 0)
            };
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
                FontSize = 12,
                Opacity = 0.7,
                IsVisible = false,
            };
            IS_INVERTED = false;
            Content = _checkbox;
            // TODO: Avalonia - SettingsCard.Header may not accept complex controls in Avalonia version
            // Header = new StackPanel()
            // {
            //     Spacing = 4,
            //     Orientation = Orientation.Vertical,
            //     Children = { _textblock, _warningBlock }
            // };

            _checkbox.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            _checkbox.IsCheckedChanged += _checkbox_Toggled;
        }
        protected virtual void _checkbox_Toggled(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Settings.Set(setting_name, (_checkbox.IsChecked ?? false) ^ IS_INVERTED ^ ForceInversion);
            StateChanged?.Invoke(this, EventArgs.Empty);
            _textblock.Opacity = (_checkbox.IsChecked ?? false) ? 1 : 0.7;
        }
    }

    public partial class CheckboxCard_Dict : CheckboxCard
    {
        public override event EventHandler<EventArgs>? StateChanged;

        private Settings.K _dictName = Settings.K.Unset;
        private bool _disableStateChangedEvent = false;

        private string _keyName = "";
        public string KeyName { set
        {
            _keyName = value;
            if (_dictName != Settings.K.Unset && _keyName.Any())
            {
                _disableStateChangedEvent = true;
                _checkbox.IsChecked = Settings.GetDictionaryItem<string, bool>(_dictName, _keyName) ^ IS_INVERTED ^ ForceInversion;
                _textblock.Opacity = (_checkbox.IsChecked ?? false) ? 1 : 0.7;
                _disableStateChangedEvent = false;
            }
        } }

        public Settings.K DictionaryName
        {
            set
            {
                _dictName = value;
                IS_INVERTED = Settings.ResolveKey(value).StartsWith("Disable");
                if (_dictName != Settings.K.Unset && _keyName.Any())
                {
                    _checkbox.IsChecked = Settings.GetDictionaryItem<string, bool>(_dictName, _keyName) ^ IS_INVERTED ^ ForceInversion;
                    _textblock.Opacity = (_checkbox.IsChecked ?? false) ? 1 : 0.7;
                }
            }
        }

        public CheckboxCard_Dict() : base()
        {
        }

        protected override void _checkbox_Toggled(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_disableStateChangedEvent) return;
            Settings.SetDictionaryItem(_dictName, _keyName, (_checkbox.IsChecked ?? false) ^ IS_INVERTED ^ ForceInversion);
            StateChanged?.Invoke(this, EventArgs.Empty);
            _textblock.Opacity = (_checkbox.IsChecked ?? false) ? 1 : 0.7;
        }
    }
}
