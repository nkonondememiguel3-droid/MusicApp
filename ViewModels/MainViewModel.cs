using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using musicApp.Messages;
using musicApp.Models;
using ReactiveUI;

namespace musicApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    public Interaction<Unit, AlbumViewModel?> ShowMusicStoreInteraction { get; }

    // command to add an album to the list of albums.
    public ReactiveCommand<Unit, Unit> AddAlbumCommand { get; }
    public ObservableCollection<AlbumViewModel> Albums { get; } = new();

    public MainViewModel()
    {
        ShowMusicStoreInteraction = new Interaction<Unit, AlbumViewModel?>();
        AddAlbumCommand = ReactiveCommand.CreateFromTask(AddAlbumAsync);

        ViewModelInteractions.AlbumAlreadyExistsInteraction.RegisterHandler(context =>
        {
            AlbumViewModel albumToCheck = context.Input;
            bool exists = Albums.Any(a => a.Artist == albumToCheck.Artist && a.Title == albumToCheck.Title);
            context.SetOutput(exists);
        });
    }

    private async Task AddAlbumAsync()
    {
        AlbumViewModel? album = await ShowMusicStoreInteraction.Handle(Unit.Default);
        if (album is not null)
        {
            Albums.Add(album);
        }
    }
}