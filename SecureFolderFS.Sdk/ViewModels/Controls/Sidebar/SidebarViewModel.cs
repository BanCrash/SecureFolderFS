﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using SecureFolderFS.Sdk.AppModels;
using SecureFolderFS.Sdk.Messages;
using SecureFolderFS.Sdk.Models;
using SecureFolderFS.Sdk.Services;
using SecureFolderFS.Sdk.ViewModels.Vault;
using SecureFolderFS.Shared.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SecureFolderFS.Sdk.ViewModels.Controls.Sidebar
{
    public sealed partial class SidebarViewModel : ObservableObject, IAsyncInitialize, IRecipient<AddVaultMessage>, IRecipient<RemoveVaultMessage>
    {
        private readonly IVaultCollectionModel _vaultCollectionModel;

        [ObservableProperty] private SidebarItemViewModel? _SelectedItem;
        [ObservableProperty] private SidebarSearchViewModel _SearchViewModel;
        [ObservableProperty] private SidebarFooterViewModel _FooterViewModel;
        [ObservableProperty] private ObservableCollection<SidebarItemViewModel> _SidebarItems;

        private ISettingsService SettingsService { get; } = Ioc.Default.GetRequiredService<ISettingsService>();

        public SidebarViewModel(IVaultCollectionModel vaultCollectionModel)
        {
            _vaultCollectionModel = vaultCollectionModel;
            _SidebarItems = new();
            _SearchViewModel = new(SidebarItems);
            _FooterViewModel = new(_vaultCollectionModel);

            WeakReferenceMessenger.Default.Register<AddVaultMessage>(this);
            WeakReferenceMessenger.Default.Register<RemoveVaultMessage>(this);
        }

        /// <inheritdoc/>
        public Task InitAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in _vaultCollectionModel.GetVaults())
                AddVaultToSidebar(item);

            if (SettingsService.UserSettings.ContinueOnLastVault)
                SelectedItem = SidebarItems.FirstOrDefault(x => x.VaultViewModel.VaultModel.Folder.Id.Equals(SettingsService.AppSettings.LastVaultFolderId));

            SelectedItem ??= SidebarItems.FirstOrDefault();

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Receive(AddVaultMessage message)
        {
            AddVaultToSidebar(message.VaultModel);
        }

        /// <inheritdoc/>
        public void Receive(RemoveVaultMessage message)
        {
            var itemToRemove = SidebarItems.FirstOrDefault(x => x.VaultViewModel.VaultModel.Equals(message.VaultModel));
            if (itemToRemove is null)
                return;

            try
            {
                SidebarItems.Remove(itemToRemove);
            }
            catch (NullReferenceException)
            {
                // TODO: This happens rarely but the vault is actually removed
            }

            SelectedItem = SidebarItems.FirstOrDefault();
        }

        private void AddVaultToSidebar(IVaultModel vaultModel)
        {
            var widgetsCollection = new WidgetsCollectionModel(vaultModel.Folder);
            var vaultViewModel = new VaultViewModel(vaultModel, widgetsCollection);

            var sidebarItem = new SidebarItemViewModel(vaultViewModel, _vaultCollectionModel);

            sidebarItem.LastAccessDate = vaultModel.LastAccessDate;
            SidebarItems.Add(sidebarItem);
        }

        partial void OnSelectedItemChanged(SidebarItemViewModel? value)
        {
            if (SettingsService.UserSettings.ContinueOnLastVault)
                SettingsService.AppSettings.LastVaultFolderId = value?.VaultViewModel.VaultModel.Folder.Id;
        }
    }
}
