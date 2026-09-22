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

    private void MainViewSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        UpdateResponsiveLayout(e.NewSize.Width);
    }

    private void UpdateResponsiveLayout(double width)
    {
        var mobile = width < 900;

        MenuButton.IsVisible = mobile;
        BotStatus.IsVisible = true;
        
          if (mobile)
            {
                Sidebar.IsVisible = false;

                BodyGrid.ColumnDefinitions.Clear();
                BodyGrid.ColumnDefinitions.Add(
                    new ColumnDefinition(1, GridUnitType.Star));

                CreatePostView.SetValue(Grid.ColumnProperty, 0);
                CreatePostView.Margin = new Thickness(16);
            }
            else
            {
                Sidebar.IsVisible = true;

                BodyGrid.ColumnDefinitions.Clear();

                BodyGrid.ColumnDefinitions.Add(
                    new ColumnDefinition(246, GridUnitType.Pixel));

                BodyGrid.ColumnDefinitions.Add(
                    new ColumnDefinition(1, GridUnitType.Star));

                CreatePostView.SetValue(Grid.ColumnProperty, 1);
                CreatePostView.Margin = new Thickness(0, 16, 16, 16);

            }
    }

    private void MenuButtonClick(object? sender, RoutedEventArgs e)
    {
        Sidebar.IsVisible = !Sidebar.IsVisible;
    }


}
