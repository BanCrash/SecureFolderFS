﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SecureFolderFS.Backend.ViewModels.Dashboard;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SecureFolderFS.WinUI.UserControls
{
    public sealed partial class VaultHealthControl : UserControl
    {
        public VaultHealthControl()
        {
            this.InitializeComponent();
        }

        public VaultHealthViewModel ViewModel
        {
            get => (VaultHealthViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(VaultHealthViewModel), typeof(VaultHealthControl), new PropertyMetadata(null));
    }
}
