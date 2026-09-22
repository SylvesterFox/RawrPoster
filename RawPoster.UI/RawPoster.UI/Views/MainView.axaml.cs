
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using RawPoster.UI.ViewModels;
using RawrPoster.Core.Entites;
using RawrPoster.Application.Interfaces;

namespace RawPoster.UI.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private async void AttachImageClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider is null || DataContext is not MainViewModel viewModel) return;
        var selected = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { AllowMultiple = false, FileTypeFilter = [FilePickerFileTypes.ImageAll] });
        var file = selected.FirstOrDefault();
        if (file is null) return;
        await using var stream = await file.OpenReadAsync();
        await viewModel.AttachImageAsync(stream, file.Name, null);
    }

    private void PostTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { Text: { } text, CaretIndex: var caret } && DataContext is MainViewModel viewModel)
            _ = viewModel.UpdateHashtagSuggestionsAsync(text, caret);
    }

    private void HashtagClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: Hashtag tag } || DataContext is not MainViewModel viewModel) return;
        var editor = this.FindControl<TextBox>("EditorTextBox");
        if (editor?.Text is not { } text) return;
        editor.Text = viewModel.ApplyHashtag(tag, text, editor.CaretIndex, out var caret);
        editor.CaretIndex = caret;
    }

    private void UseSearchResultClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: ImageSearchResult result } && DataContext is MainViewModel viewModel)
            viewModel.UseSearchResultCommand.Execute(result);
    }
}
