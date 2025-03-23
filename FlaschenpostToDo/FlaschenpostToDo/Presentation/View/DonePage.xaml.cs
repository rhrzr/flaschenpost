using FlaschenpostToDo.Presentation.ViewModel;

namespace FlaschenpostToDo.Presentation.View;

public partial class DonePage : ContentPage
{
    public DonePage(DoneViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        ((DoneViewModel)BindingContext).OnAppearing();
    }

    protected override void OnDisappearing()
    {
        ((BaseViewModel)BindingContext).OnDisappearing();
    }
}