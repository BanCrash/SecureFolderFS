﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using SecureFolderFS.Sdk.Enums;
using SecureFolderFS.Sdk.Services;
using SecureFolderFS.Shared.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SecureFolderFS.Sdk.ViewModels.Controls.Banners
{
    public sealed partial class UpdateBannerViewModel : ObservableObject, IAsyncInitialize
    {
        private ISettingsService SettingsService { get; } = Ioc.Default.GetRequiredService<ISettingsService>();

        private IUpdateService UpdateService { get; } = Ioc.Default.GetRequiredService<IUpdateService>();

        public InfoBarViewModel UpdateInfoBar { get; }

        [ObservableProperty] private string? _UpdateText;
        [ObservableProperty] private bool _AreUpdatesSupported;

        public DateTime LastChecked
        {
            get => SettingsService.AppSettings.UpdateLastChecked;
            set => SettingsService.AppSettings.UpdateLastChecked = value;
        }

        public UpdateBannerViewModel()
        {
            UpdateInfoBar = new();
            UpdateText = "Latest version installed";
        }

        /// <inheritdoc/>
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            var updatesSupported = await UpdateService.IsSupportedAsync();
            var isInitialized = updatesSupported && await UpdateService.InitializeAsync();

            return; // TODO: Implement updates
            if (!updatesSupported || !isInitialized)
            {
                await Task.Delay(800);

                AreUpdatesSupported = false;
                UpdateInfoBar.IsOpen = true;
                UpdateInfoBar.Message = "Updates are not supported for the sideloaded version.";
                UpdateInfoBar.InfoBarSeverity = InfoBarSeverityType.Warning;
                UpdateInfoBar.CanBeClosed = false;
            }
        }

        [RelayCommand]
        private async Task UpdateAppAsync(CancellationToken cancellationToken)
        {
            LastChecked = DateTime.Now;
            OnPropertyChanged(nameof(LastChecked));

            var isUpdateAvailable = await UpdateService.IsUpdateAvailableAsync();
            if (isUpdateAvailable)
                _ = UpdateService.UpdateAsync(null, cancellationToken);
        }
    }
}
