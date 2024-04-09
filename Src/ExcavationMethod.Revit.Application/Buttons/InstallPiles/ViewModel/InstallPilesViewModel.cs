using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

using ExcavationMethod.Revit.Application.Buttons.InstallPiles.Model;

namespace ExcavationMethod.Revit.Application.Buttons.InstallPiles.ViewModel
{
    public partial class InstallPilesViewModel : ObservableObject
    {
        private InstallPilesModel m;

        [ObservableProperty]
        private string? _documentTitle;

        public InstallPilesViewModel(InstallPilesModel m)
        {
            this.m = m;
            DocumentTitle = m.UiApp.ActiveUIDocument.Document.Title;
        }

        [RelayCommand]
        private void InstallPiles()
        {
            m.InstallPiles();
        }

        [RelayCommand]
        private void Close(Window win)
        {
            win.Close();
        }
    }
}
