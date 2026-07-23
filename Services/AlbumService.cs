using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using musicApp.Models;

namespace musicApp.Services;

public class AlbumService
{
    private static readonly HttpClient s_httpClient = new();
    private static readonly JsonSerializerOptions s_jsonOptions = new() { PropertyNameCaseInsensitive = true };

    private record ItunesAlbum(string ArtistName, string CollectionName, string ArtworkUrl100);

    private record ItunesSearchResult(ItunesAlbum[] Results);

    public async Task<IEnumerable<Album>> SearchAsync(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Enumerable.Empty<Album>();
        }

        var url = $"https://itunes.apple.com/search?term={Uri.EscapeDataString(searchTerm)}&entity=album&limit=25";
        Debug.WriteLine($"[DEBUG] HTTP Fetch URL: {url}");
        
        try
        {
            var json = await s_httpClient.GetStringAsync(url).ConfigureAwait(false);
            Debug.WriteLine($"[DEBUG] HTTP Raw JSON Sample: {json.Substring(0, Math.Min(json.Length, 200))}...");

            var result = JsonSerializer.Deserialize<ItunesSearchResult>(json, s_jsonOptions);
        
            if (result?.Results == null)
            {
                Debug.WriteLine("[DEBUG] JSON Deserialization failed or results array is empty!");
                return Enumerable.Empty<Album>();
            }

            Debug.WriteLine($"[DEBUG] JSON Deserialized successfully. Found {result.Results.Length} raw results.");
            return result.Results.Select(x =>
                new Album(x.ArtistName, x.CollectionName, x.ArtworkUrl100.Replace("100x100bb", "600x600bb")));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[DEBUG] Service Layer Error: {ex.Message}");
            throw;
        }
    }
    private static string CachePath(Album album) =>
        $"./Cache/{SanitizeFileName(album.Artist)} - {SanitizeFileName(album.Title)}";

    public async Task<Stream> LoadCoverBitmapAsync(Album album)
    {
        var cachePath = CachePath(album);
        if (File.Exists(cachePath + ".bmp"))
        {
            return File.Open(cachePath + ".bmp", FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        else
        {
            var data = await s_httpClient.GetByteArrayAsync(album.CoverUrl);
            return new MemoryStream(data);
        }
    }

    private static string SanitizeFileName(string input)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            input = input.Replace(c, '_');
        }
        return input;
    }
}