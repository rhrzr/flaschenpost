using CommunityToolkit.Mvvm.ComponentModel;

namespace FlaschenpostToDo.Presentation.ViewModel;

public class BaseViewModel : ObservableObject
{
    public virtual void OnAppearing()
    {
    }

    public virtual void OnDisappearing()
    {
    }
}