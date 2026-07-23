using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Threading.Tasks;
using Avalonia.Controls;
using musicApp.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace musicApp.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        if (Design.IsDesignMode) return;
        this.WhenActivated(disposables =>
        {
            this.BindInteraction(ViewModel, vm => vm.ShowMusicStoreInteraction,
                context => HandleShowMusicStoreAsync(context).DisposeWith(disposables));
        });
    }

    private async Task HandleShowMusicStoreAsync(IInteractionContext<Unit, AlbumViewModel?> context)
    {
        var dialog = new MusicStoreWindow() { DataContext = new MusicStoreViewModel() };
        AlbumViewModel? result = await dialog.ShowDialog<AlbumViewModel?>(this);
        context.SetOutput(result);
    }
}