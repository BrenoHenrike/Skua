﻿using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;
using System.Windows.Controls;

namespace Skua.WPF.Views;

/// <summary>
/// Interaction logic for ColorToolUserControl.xaml
/// </summary>
public partial class ApplicationThemesView : UserControl
{
    public ApplicationThemesView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<ApplicationThemesViewModel>();
    }
}