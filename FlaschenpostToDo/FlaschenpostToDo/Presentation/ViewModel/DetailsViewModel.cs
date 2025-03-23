using CommunityToolkit.Mvvm.ComponentModel;
using FlaschenpostToDo.Network;

namespace FlaschenpostToDo.Presentation.ViewModel;

[QueryProperty(nameof(Network.TodoItem), "todo")]
public partial class DetailsViewModel : BaseViewModel
{
    [ObservableProperty]
    private TodoItem _todoItem;
}