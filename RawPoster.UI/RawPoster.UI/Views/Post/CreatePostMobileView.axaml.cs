using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;

namespace RawPoster.UI.Views.Post
{
    public partial class CreatePostMobileView : UserControl
    {
        public CreatePostMobileView()
        {
            InitializeComponent();

            PostTextBox.TemplateApplied += PostTextBoxOnTemplateApplied;
        }

        private void PostTextBoxOnTemplateApplied(object sender, TemplateAppliedEventArgs e)
        {
            var presenter =
                e.NameScope.Find<TextPresenter>("PART_TextPresenter");

            if (presenter is not null)
            {
                presenter.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
                presenter.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            }

            var watermark =
                e.NameScope.Find<TextBlock>("PART_Watermark");

            if (watermark is not null)
            {
                watermark.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
            }
        }
    }

 }
