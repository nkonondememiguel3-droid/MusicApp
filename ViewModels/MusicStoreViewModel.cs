using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using musicApp.Services;
using ReactiveUI;
using ReactiveUI.Builder;

namespace musicApp.ViewModels;

public class MusicStoreViewModel : ViewModelBase
{
    private readonly AlbumService _albumService = new();
    private string? _searchText;
    private bool _isBusy;
    private AlbumViewModel? _selectedAlbum;

    public MusicStoreViewModel()
    {
        this.WhenAnyValue(x => x.SearchText).Throttle(TimeSpan.FromMilliseconds(400))
            .ObserveOn(ReactiveUI.RxSchedulers.MainThreadScheduler).Subscribe(async text =>
            {
                Debug.WriteLine($"[DEBUG] SearchText changed stream triggered! Text is: '{text}'");
                await DoSearchAsync(text);
            });
    }

    public string? SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public AlbumViewModel? SelectedAlbum
    {
        get => _selectedAlbum;
        set => this.RaiseAndSetIfChanged(ref _selectedAlbum, value);
    }
    
    public bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }
    
    public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();
    
    private async Task DoSearchAsync(string? searchText)
    {
        Debug.WriteLine($"[DEBUG] DoSearchAsync started with parameter: '{searchText}'");
        IsBusy = true;
        SearchResults.Clear();
        if (string.IsNullOrWhiteSpace(searchText))
        {
            Debug.WriteLine("[DEBUG] Search text is empty or null. Aborting search processing.");
            IsBusy = false;
            return;
        }

        try
        {
            Debug.WriteLine("[DEBUG] Calling AlbumService.SearchAsync...");
            var albums = await _albumService.SearchAsync(searchText);

            if (albums == null)
            {
                Debug.WriteLine("[DEBUG] CRITICAL: AlbumService returned a NULL collection!");
            }
            else
            {
                var albumList = System.Linq.Enumerable.ToList(albums);
                Debug.WriteLine($"[DEBUG] Success! Network returned {albumList.Count} albums.");
                foreach (var album in albumList)
                {
                    Debug.WriteLine($"[DEBUG] Adding album to list: {album.Title} by {album.Artist}");
                    var vm = new AlbumViewModel(album);
                    SearchResults.Add(vm);
                }
            }
        }
        catch (Exception ex)
        {
            // DEBUG PRINT 3: Capture any silent network or serialization crashes
            Debug.WriteLine($"[DEBUG] EXCEPTION caught during search: {ex.Message}");
            Debug.WriteLine($"[DEBUG] StackTrace: {ex.StackTrace}");
        }
        finally
        {
            IsBusy = false;
            Debug.WriteLine($"[DEBUG] DoSearchAsync finished. SearchResults count: {SearchResults.Count}");
        }
    }
}