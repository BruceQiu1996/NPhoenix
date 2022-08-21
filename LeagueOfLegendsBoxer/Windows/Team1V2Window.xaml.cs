using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeagueOfLegendsBoxer.Windows
{
    /// <summary>
    /// Interaction logic for Team1V2Window.xaml
    /// </summary>
    public partial class Team1V2Window : Window
    {
        private readonly Team1V2WindowViewModel _viewModel;
        public Team1V2Window(Team1V2WindowViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Opacity = 0;
        }

        private void ListBoxItem1_Drop(object sender, DragEventArgs e)
        {
            Account droppedData = e.Data.GetData(typeof(Account)) as Account;
            Account target = ((ListBoxItem)sender).DataContext as Account;

            int removedIdx = listbox1.Items.IndexOf(droppedData);
            int targetIdx = listbox1.Items.IndexOf(target);

            if (_viewModel.Team1Accounts.Contains(droppedData) && _viewModel.Team1Accounts.Contains(target))
            {
                if (removedIdx < targetIdx)
                {
                    _viewModel.Team1Accounts.RemoveAt(targetIdx);
                    _viewModel.Team1Accounts.RemoveAt(removedIdx);
                    _viewModel.Team1Accounts.Insert(removedIdx, target);
                    _viewModel.Team1Accounts.Insert(targetIdx, droppedData);
                }
                else
                {
                    _viewModel.Team1Accounts.RemoveAt(removedIdx);
                    _viewModel.Team1Accounts.RemoveAt(targetIdx);
                    _viewModel.Team1Accounts.Insert(targetIdx, droppedData);
                    _viewModel.Team1Accounts.Insert(removedIdx, target);
                }
            }
        }

        private void ListBoxItem2_Drop(object sender, DragEventArgs e)
        {
            Account droppedData = e.Data.GetData(typeof(Account)) as Account;
            Account target = ((ListBoxItem)sender).DataContext as Account;

            int removedIdx = listbox2.Items.IndexOf(droppedData);
            int targetIdx = listbox2.Items.IndexOf(target);

            if (_viewModel.Team2Accounts.Contains(droppedData) && _viewModel.Team2Accounts.Contains(target))
            {
                if (removedIdx < targetIdx)
                {
                    _viewModel.Team2Accounts.RemoveAt(targetIdx);
                    _viewModel.Team2Accounts.RemoveAt(removedIdx);
                    _viewModel.Team2Accounts.Insert(removedIdx, target);
                    _viewModel.Team2Accounts.Insert(targetIdx, droppedData);
                }
                else
                {
                    _viewModel.Team2Accounts.RemoveAt(removedIdx);
                    _viewModel.Team2Accounts.RemoveAt(targetIdx);
                    _viewModel.Team2Accounts.Insert(targetIdx, droppedData);
                    _viewModel.Team2Accounts.Insert(removedIdx, target);
                }
            }
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem && e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
            }
        }
    }
}
