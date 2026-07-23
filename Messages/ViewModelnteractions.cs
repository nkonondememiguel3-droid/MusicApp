using musicApp.ViewModels;
using ReactiveUI;

namespace musicApp.Messages;

public static class ViewModelInteractions
{
    public static Interaction<AlbumViewModel, bool> AlbumAlreadyExistsInteraction { get; } = new Interaction<AlbumViewModel, bool>();
}