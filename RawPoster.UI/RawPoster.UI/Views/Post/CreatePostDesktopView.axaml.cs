using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RawPoster.UI.Views.Post
{
    public partial class CreatePostDesktopView : UserControl
    {
        public CreatePostDesktopView()
        {
            InitializeComponent();
        }

        private void PostTextChanged(object? sender, TextChangedEventArgs e)
        {
            // Логику хэштегов подключим позже.
        }
    }
}