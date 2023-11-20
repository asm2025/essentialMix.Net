using System;
using System.Windows;
using JetBrains.Annotations;
using Test.WPF.ViewModels;

namespace Test.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	/// <inheritdoc />
	public MainWindow([NotNull] MainViewModel viewModel)
	{
		DataContext = viewModel;
		InitializeComponent();
	}

	/// <inheritdoc />
	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		Dispatcher.InvokeShutdown();
	}
}