using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace musicApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    public Interaction<Unit, AlbumViewModel?> ShowMusicStoreInteraction { get; }

    // command to add an album to the list of albums.
    public ReactiveCommand<Unit, Unit> AddAlbumCommand { get; }

    public MainViewModel()
    {
        ShowMusicStoreInteraction = new Interaction<Unit, AlbumViewModel?>();
        
        AddAlbumCommand = ReactiveCommand.CreateFromTask(AddAlbumAsync);
    }

    private async Task AddAlbumAsync()
    {
        AlbumViewModel? album = await ShowMusicStoreInteraction.Handle(Unit.Default);

        if (album is not null)
        {
            
        }
    }
}