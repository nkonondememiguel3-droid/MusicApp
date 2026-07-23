using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using musicApp.Models;
using musicApp.Services;

namespace musicApp.ViewModels;

public class AlbumViewModel : ViewModelBase
{
    private readonly Album _album;

    public AlbumViewModel(Album album)
    {
        _album = album;
    }

    public string Artist => _album.Artist;
    public string Title => _album.Title;

    public Task<Bitmap?> Cover => AlbumCoverService.LoadAlbumCoverAsync(new Uri(_album.CoverUrl));
    // public Task<Bitmap?> Cover => LoadCoverAsync();
    // 
    // private async Task<Bitmap?> LoadCoverAsync()
    // {
    //     return null;
    // }

}