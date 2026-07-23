# MusicApp

MusicApp is an application made in C# with the Avalonia framework using the MVVM pattern.
This application was made in order to learn how to use asynchronous operation in Avalonia.mvvm using the ReactiveUI.

## Usage

In the application, we are able to click on the album icon to open a `dialog` window, in that window you are able to type in the name of an application in order to search
for that song. When the search is finish you will see a list of related songs in a list below the search field. Then you can click on the icon of the album to want to
buy, when click, the dialog with close and the main window will list all the album.

## Implementation details

In this current version, we are using the [iTunes](https://performance-partners.apple.com/search-api) API for the album and songs.
