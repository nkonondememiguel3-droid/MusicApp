using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using musicApp.Models;
using musicApp.Services;
using ReactiveUI;

namespace musicApp.ViewModels;

public class AlbumViewModel : ViewModelBase, IEquatable<AlbumViewModel>
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

    public bool Equals(AlbumViewModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _album.Equals(other._album);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((AlbumViewModel)obj);
    }

    public override int GetHashCode()
    {
        return _album.GetHashCode();
    }
}