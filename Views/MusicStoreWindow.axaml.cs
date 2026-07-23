using System;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using musicApp.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace musicApp.Views;

public partial class MusicStoreWindow : ReactiveWindow<MusicStoreViewModel>
{
    public MusicStoreWindow()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            this.BindInteraction(ViewModel, vm => vm.CloseMusicStoreInteraction,
                context => HandleCloseMusicStoreAsync(context).DisposeWith(disposables));
        });
        this.WhenActivated(disposables =>
        {
            this.BindInteraction(ViewModel, vm => vm.NotificationInteraction,
                context => HandleNotificationAsync(context).DisposeWith(disposables));
        });
    }

    private Task HandleNotificationAsync(IInteractionContext<string, Unit> context)
    {
        try
        {
            NotificationManager.CloseAll();
            NotificationManager.Show(context.Input, NotificationType.Warning, TimeSpan.FromSeconds(3));
            context.SetOutput(Unit.Default);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    private Task HandleCloseMusicStoreAsync(IInteractionContext<AlbumViewModel, Unit> context)
    {
        try
        {
            Close(context.Input);
            context.SetOutput(Unit.Default);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }
}