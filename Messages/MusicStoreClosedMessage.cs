using musicApp.ViewModels;

namespace musicApp.Messages;

// The album message has an albumViewModel.
public class MusicStoreClosedMessage(AlbumViewModel selectedAlbum)
{
    // public AlbumViewModel SelectedAlbum { get; } = selectedAlbum;
}