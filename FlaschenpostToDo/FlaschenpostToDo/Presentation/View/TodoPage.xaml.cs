using FlaschenpostToDo.Presentation.ViewModel;

namespace FlaschenpostToDo.Presentation.View;

public partial class TodoPage : ContentPage
{
    public TodoPage(TodoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        ((BaseViewModel)BindingContext).OnAppearing();
    }

    protected override void OnDisappearing()
    {
        ((BaseViewModel)BindingContext).OnDisappearing();
    }
}