using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

using ExcavationMethod.Revit.Application.Buttons.DummyWpfModeless.Model;

namespace ExcavationMethod.Revit.Application.Buttons.DummyWpfModeless.ViewModel
{
    public partial class DummyWpfModelessViewModel : ObservableObject
    {
        private DummyWpfModelessModel m;

        [ObservableProperty]
        private string? _documentTitle;

        public DummyWpfModelessViewModel(DummyWpfModelessModel m)
        {
            this.m = m;
            DocumentTitle = m.UiApp.ActiveUIDocument.Document.Title;
        }

        [RelayCommand]
        private void DoSomething()
        {
            m.DoSomething();
        }

        [RelayCommand]
        private void Close(Window win)
        {
            win.Close();
        }
    }
}
