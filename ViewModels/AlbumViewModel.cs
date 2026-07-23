using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using musicApp.Models;
using musicApp.Services;
using ReactiveUI;

namespace musicApp.ViewModels;

public class AlbumViewModel : ViewModelBase
{
    private readonly Album _album;

    public AlbumViewModel(Album album)
    {
        _album = album;
        Cover = LoadCoverAsync();
    }

    public string Artist => _album.Artist;
    public string Title => _album.Title;
    public Task<Bitmap?> Cover { get; }

    private async Task<Bitmap?> LoadCoverAsync()
    {
        await using Stream? stream = await AlbumCoverService.LoadAlbumCoverAsync(_album);
        if (stream is null) return null;
        return await Task.Run(() =>
        {
            try
            {
                return new Bitmap(stream);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Failed to decode bitmap: {e.Message}");
                return null;
            }
        });
    }
}