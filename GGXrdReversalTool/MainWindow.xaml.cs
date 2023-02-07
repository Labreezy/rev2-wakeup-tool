using System;
using System.Windows;
using GGXrdReversalTool.ViewModels;

namespace GGXrdReversalTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ScenarioWindowViewModel;

            var command = viewModel?.WindowLoadedCommand;
            
            if (command != null && command.CanExecute())
            {
                command.Execute();
            }
        }

        private void MainWindow_OnClosed(object? sender, EventArgs e)
        {
            var viewModel = DataContext as ScenarioWindowViewModel;

            var command = viewModel?.WindowClosedCommand;

            if (command != null && command.CanExecute())
            {
                command.Execute();
            }
        }
    }
}