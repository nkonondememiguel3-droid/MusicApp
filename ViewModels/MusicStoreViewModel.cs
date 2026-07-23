using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using musicApp.Messages;
using musicApp.Services;
using ReactiveUI;

namespace musicApp.ViewModels;

public class MusicStoreViewModel : ViewModelBase
{
    private readonly AlbumService _albumService = new();
    private string? _searchText;
    private bool _isBusy;
    private AlbumViewModel? _selectedAlbum;
    public Interaction<AlbumViewModel, Unit> CloseMusicStoreInteraction { get; }
    public Interaction<string, Unit> NotificationInteraction { get; }
    public ReactiveCommand<Unit, Unit> BuyMusicCommand { get; }

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
            Debug.WriteLine($"[DEBUG] EXCEPTION caught during search: {ex.Message}");
            Debug.WriteLine($"[DEBUG] StackTrace: {ex.StackTrace}");
        }
        finally
        {
            IsBusy = false;
            Debug.WriteLine($"[DEBUG] DoSearchAsync finished. SearchResults count: {SearchResults.Count}");
        }
    }

    public MusicStoreViewModel()
    {
        CloseMusicStoreInteraction = new Interaction<AlbumViewModel, Unit>();
        NotificationInteraction = new Interaction<string, Unit>();
        
        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(400))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(async void (text) =>
            {
                Debug.WriteLine($"[DEBUG] SearchText changed stream triggered! Text is: '{text}'");
                await DoSearchAsync(text);
            });
        
        IObservable<bool> canBuy = this.WhenAnyValue(x => x.SelectedAlbum)
            .Select(album => album != null);
        
        BuyMusicCommand = ReactiveCommand.CreateFromTask(BuyMusic, canBuy);
    }

    private async Task BuyMusic()
    {
        if (SelectedAlbum is null) return;

        bool exists = await ViewModelInteractions.AlbumAlreadyExistsInteraction.Handle(SelectedAlbum);

        if (exists is true)
        {
            await NotificationInteraction.Handle("This album already exists");
        }
        else
        {
            await CloseMusicStoreInteraction.Handle(SelectedAlbum);
        }
    }
}