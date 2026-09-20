
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RawPoster.UI.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void PublishButton_OnClick(
       object? sender,
       RoutedEventArgs e)
    {
        // Логику публикации подключим следующим шагом.
    }
}