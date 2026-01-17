using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using UniGetUI.Core.Data;
using UniGetUI.Core.Logging;
using UniGetUI.Core.Tools;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UniGetUI.Interface.Widgets
{
    public partial class Announcer : UserControl
    {
        // TODO: Avalonia - Missing XAML control field declaration
        private RichTextBlock _textblock = null!;

        public static readonly StyledProperty<Uri?> UrlProperty =
            AvaloniaProperty.Register<Announcer, Uri?>(nameof(Url));

        public Uri? Url
        {
            get => GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }

        private static readonly HttpClient NetClient = new(CoreTools.GenericHttpClientParameters);
        
        public Announcer()
        {
            NetClient.DefaultRequestHeaders.UserAgent.ParseAdd(CoreData.UserAgentString);
            
            InitializeComponent();
            
            UrlProperty.Changed.AddClassHandler<Announcer>((x, e) => x.LoadAnnouncements());

            int i = 0;
            PointerPressed += (_, _) => { if (i++ % 3 != 0) { _ = LoadAnnouncements(); } };

            SetText(CoreTools.Translate("Fetching latest announcements, please wait..."));
            // TODO: Avalonia - TextBlock.TextWrapping might not apply to RichTextBlock
            // _textblock.TextWrapping = Avalonia.Media.TextWrapping.Wrap;
        }

        public async Task LoadAnnouncements(bool retry = false)
        {
            try
            {
                Uri announcement_url = Url;
                if (retry)
                {
                    announcement_url = new Uri(Url.ToString().Replace("https://", "http://"));
                }

                HttpResponseMessage response = await NetClient.GetAsync(announcement_url);
                if (response.IsSuccessStatusCode)
                {
                    string[] response_body = (await response.Content.ReadAsStringAsync()).Split("////");
                    string title = response_body[0].Trim().Trim('\n').Trim();
                    string body = response_body[1].Trim().Trim('\n').Trim();
                    string linkId = response_body[2].Trim().Trim('\n').Trim();
                    string linkName = response_body[3].Trim().Trim('\n').Trim();
                    Uri imageUrl = new(response_body[4].Trim().Trim('\n').Trim());
                    SetText(title, body, linkId, linkName);
                    SetImage(imageUrl);
                }
                else
                {
                    SetText(CoreTools.Translate("Could not load announcements - HTTP status code is $CODE").Replace("$CODE", response.StatusCode.ToString()));
                    SetImage(new Uri("ms-appx:///Assets/Images/warn.png"));
                    if (!retry)
                    {
                        _ = LoadAnnouncements(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Could not load announcements");
                Logger.Warn(ex);
                SetText(CoreTools.Translate("Could not load announcements - ") + ex.ToString());
                SetImage(new Uri("ms-appx:///Assets/Images/warn.png"));
            }
        }

        public void SetText_Safe(string title, string body, string linkId, string linkName)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                SetText(title, body, linkId, linkName);
            });
        }

        public void SetText(string title, string body, string linkId, string linkName)
        {
            Avalonia.Controls.Documents.Paragraph paragraph = new();
            // TODO: Avalonia - Run.FontSize, Run.FontWeight, Run.FontFamily not supported, properties removed
            paragraph.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = title });
            // TODO: Avalonia - RichTextBlock.Blocks API may not be available, commenting out
            // _textblock.Blocks.Clear();
            // _textblock.Blocks.Add(paragraph);

            paragraph = new();
            // TODO: Avalonia - LineBreak is not an Inline in Avalonia, Hyperlink doesn't exist, commenting out
            // foreach (string line in body.Split("\n"))
            // {
            //     paragraph.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = line + " " });
            //     paragraph.Inlines.Add(new LineBreak());
            // }
            // var link = new Avalonia.Controls.Documents.Hyperlink
            // {
            //     NavigateUri = new Uri("https://marticliment.com/redirect?" + linkId)
            // };
            // link.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = linkName });
            // paragraph.Inlines[^1] = link;
            // paragraph.Inlines.Add(new Avalonia.Controls.Documents.Run() { Text= "" });

            // TODO: Avalonia - RichTextBlock.Blocks API may not be available, commenting out
            // _textblock.Blocks.Add(paragraph);
        }

        public void SetText(string body)
        {
            Avalonia.Controls.Documents.Paragraph paragraph = new();
            // TODO: Avalonia - LineBreak is not an Inline in Avalonia, commenting out
            // foreach (string line in body.Split("\n"))
            // {
            //     paragraph.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = line });
            //     paragraph.Inlines.Add(new LineBreak());
            // }

            // TODO: Avalonia - RichTextBlock.Blocks API may not be available, commenting out
            // _textblock.Blocks.Clear();
            // _textblock.Blocks.Add(paragraph);
        }

        public void SetImage(Uri url)
        {
            // TODO: Avalonia - BitmapImage with UriSource needs investigation
            // BitmapImage bitmapImage = new() { UriSource = url };
            // _image.Source = bitmapImage;

        }
    }
}
