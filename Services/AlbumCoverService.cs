using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using musicApp.Models;

namespace musicApp.Services;

public static class AlbumCoverService
{
    public static async Task<Bitmap?> LoadAlbumCoverAsync(Uri url)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            return new Bitmap(new MemoryStream(data));
        }
        catch (HttpRequestException ex)
        {
            await Console.Error.WriteLineAsync($"An error occurred while downloading image '{url}' : {ex.Message}");
            return null;
        }
    }
}