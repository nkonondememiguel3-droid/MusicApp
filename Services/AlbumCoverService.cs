using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using musicApp.Models;

namespace musicApp.Services;

public static class AlbumCoverService
{
    private static readonly HttpClient HttpClient = new();

    private static string CachePath(Album album) =>
        $"./Cache/{SanitizeFileName(album.Artist)} - {SanitizeFileName(album.Title)}";

    // LoadAlbumCoverAsync will load a bitmap album cover from a file or url.
    public static async Task<Stream?> LoadAlbumCoverAsync(Album album)
    {
        string cachePath = CachePath(album);
        try
        {
            // if the album cover image already exists in the cache folder.
            if (File.Exists(cachePath + ".bmp"))
            {
                return File.Open(cachePath + ".bmp", FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            // order-wise, download it on the internet.
            var response = await HttpClient.GetAsync(album.CoverUrl);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            return new MemoryStream(data);
        }
        catch (IOException ioException)
        {
            await Console.Error.WriteLineAsync(
                $"An error occurred while loading the image '{cachePath}' : {ioException.Message}");
            return null;
        }
        catch (HttpRequestException ex)
        {
            await Console.Error.WriteLineAsync(
                $"An error occurred while downloading image '{album.CoverUrl}' : {ex.Message}");
            return null;
        }
    }

    // SanitizeFileName will replace all the invalid characters in the album name with an underscore: '_'
    private static string SanitizeFileName(string input)
    {
        // foreach (var c in Path.GetInvalidFileNameChars())
        // {
        //     input = input.Replace(c,
        //         '_'); // INEFFICIENT: strings are immutable, so we are recreating new strings replace the invalid characters.
        // }
        return new string(input.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c).ToArray());
    }
}