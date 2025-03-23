using FlaschenpostToDo.Presentation.ViewModel;

namespace FlaschenpostToDo.Presentation.View;

public partial class DetailsPage : ContentPage
{
    public DetailsPage(DetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        Shell.SetTabBarIsVisible(this, false);
        ((BaseViewModel)BindingContext).OnAppearing();
    }

    protected override void OnDisappearing()
    {
        ((BaseViewModel)BindingContext).OnDisappearing();
    }
}