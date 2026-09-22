using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RawrPoster.Application.Interfaces;
using RawrPoster.Application.Services;
using RawrPoster.Core.Entites;
using Serilog;

namespace RawPoster.UI.ViewModels;

public partial class MainViewModel(ITemplateService templates, IHashtagService hashtags, IFileStorage files, IImageSourceSearch search, PostService posts) : ViewModelBase
{
    private Guid? _templateId;
    private int _hashtagStart;
    public ObservableCollection<PostTemplate> Templates { get; } = [];
    public ObservableCollection<Hashtag> SavedHashtags { get; } = [];
    public ObservableCollection<Hashtag> HashtagSuggestions { get; } = [];
    public ObservableCollection<ImageSearchResult> SearchResults { get; } = [];
    [ObservableProperty] private PostTemplate? selectedTemplate;
    [ObservableProperty] private string templateName = string.Empty;
    [ObservableProperty] private string channel = string.Empty;
    [ObservableProperty] private string postText = string.Empty;
    [ObservableProperty] private bool hasSpoiler;
    [ObservableProperty] private MediaAttachment? media;
    [ObservableProperty] private Bitmap? imagePreview;
    [ObservableProperty] private bool isSearching;
    [ObservableProperty] private string statusMessage = "Ready";
    [ObservableProperty] private bool isSuggestionOpen;
    public bool HasMedia => Media is not null;

    public async Task InitializeAsync() { await RefreshTemplatesAsync(); await RefreshHashtagsAsync(); }
    partial void OnSelectedTemplateChanged(PostTemplate? value) { if (value is not null) LoadTemplate(value); }
    partial void OnMediaChanged(MediaAttachment? value) => OnPropertyChanged(nameof(HasMedia));
    private void LoadTemplate(PostTemplate x) { _templateId = x.Id; TemplateName = x.Name; PostText = x.Text; HasSpoiler = x.HasSpoiler; Media = x.Media; LoadPreview(); StatusMessage = $"Template ‘{x.Name}’ loaded"; }
    [RelayCommand] private void NewPost() { _templateId = null; SelectedTemplate = null; TemplateName = PostText = string.Empty; HasSpoiler = false; Media = null; ImagePreview = null; SearchResults.Clear(); StatusMessage = "New post"; }
    [RelayCommand] private async Task SaveTemplateAsync()
    {
        if (string.IsNullOrWhiteSpace(TemplateName)) { StatusMessage = "Enter a template name before saving."; return; }
        try { var x = new PostTemplate { Id = _templateId ?? Guid.NewGuid(), Name = TemplateName.Trim(), Text = PostText, Media = Media, HasSpoiler = HasSpoiler, Hashtags = (await SaveTagsFromTextAsync()).ToList() }; var saved = await templates.SaveAsync(x); _templateId = saved.Id; await RefreshTemplatesAsync(); StatusMessage = "Template saved."; Log.Information("Template {TemplateId} saved.", saved.Id); }
        catch (Exception ex) { Log.Error(ex, "Unable to save template."); StatusMessage = "Unable to save template."; }
    }
    public async Task AttachImageAsync(Stream input, string fileName, string? mimeType)
    {
        try { Media = await files.SaveImageAsync(input, fileName, mimeType); LoadPreview(); StatusMessage = "Image attached."; }
        catch (Exception ex) { Log.Error(ex, "Unable to attach image."); StatusMessage = "Unable to attach image."; }
    }
    [RelayCommand] private void RemoveImage() { Media = null; ImagePreview = null; SearchResults.Clear(); }
    [RelayCommand] private async Task FindSourceAsync()
    {
        if (Media is null || !File.Exists(Media.LocalPath)) { StatusMessage = "Attach an image before searching for its source."; return; }
        IsSearching = true; SearchResults.Clear();
        try { await using var image = File.OpenRead(Media.LocalPath); foreach (var result in await search.SearchAsync(image, Media.FileName)) SearchResults.Add(result); StatusMessage = SearchResults.Count == 0 ? "Image source was not found." : $"Found {SearchResults.Count} result(s)."; }
        catch (Exception ex) { Log.Error(ex, "FuzzySearch failed."); StatusMessage = ex.Message; } finally { IsSearching = false; }
    }
    [RelayCommand] private void UseSearchResult(ImageSearchResult? x) { if (x is null || Media is null) return; Media.SourceUrl = x.Sources.FirstOrDefault() ?? x.PostUrl; OnPropertyChanged(nameof(Media)); StatusMessage = "Source metadata added to the attached image."; }
    [RelayCommand] private async Task PublishAsync() { try { await posts.PublishAsync(Channel, PostText, Media, HasSpoiler); StatusMessage = "Post sent to Telegram."; } catch (Exception ex) { Log.Error(ex, "Telegram publication failed."); StatusMessage = "Telegram publication failed. Check the channel and connection."; } }
    public async Task UpdateHashtagSuggestionsAsync(string text, int caret)
    {
        var prefix = CurrentHashtagPrefix(text, caret, out _hashtagStart); HashtagSuggestions.Clear(); if (prefix is null) { IsSuggestionOpen = false; return; }
        foreach (var tag in await hashtags.SearchAsync(prefix)) HashtagSuggestions.Add(tag); IsSuggestionOpen = HashtagSuggestions.Count > 0;
    }
    public string ApplyHashtag(Hashtag tag, string text, int caret, out int newCaret) { var end = caret; while (end < text.Length && (char.IsLetterOrDigit(text[end]) || text[end] == '_')) end++; newCaret = _hashtagStart + tag.Value.Length; IsSuggestionOpen = false; return text[.._hashtagStart] + tag.Value + text[end..]; }
    private async Task<IReadOnlyList<Hashtag>> SaveTagsFromTextAsync() { var result = new List<Hashtag>(); foreach (var v in Regex.Matches(PostText, @"(?<!\w)#[\p{L}\p{N}_]+").Select(x => x.Value).Distinct(StringComparer.OrdinalIgnoreCase)) result.Add(await hashtags.SaveAsync(v)); await RefreshHashtagsAsync(); return result; }
    private async Task RefreshTemplatesAsync() { Templates.Clear(); foreach (var x in await templates.GetAllAsync()) Templates.Add(x); }
    private async Task RefreshHashtagsAsync() { SavedHashtags.Clear(); foreach (var x in await hashtags.SearchAsync(string.Empty)) SavedHashtags.Add(x); }
    private void LoadPreview() { ImagePreview = null; if (Media is { LocalPath: { } path } && File.Exists(path)) try { ImagePreview = new Bitmap(path); } catch { StatusMessage = "The attached image cannot be displayed."; } }
    private static string? CurrentHashtagPrefix(string text, int caret, out int start) { start = caret; while (start > 0 && (char.IsLetterOrDigit(text[start - 1]) || text[start - 1] == '_')) start--; if (start == 0 || text[start - 1] != '#') return null; start--; return text[start..caret]; }
}
