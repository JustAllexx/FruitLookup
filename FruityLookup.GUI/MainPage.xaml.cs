using FruityLookup.Entities;
using FruityLookup.GUI.ModelView;
using System.Threading.Tasks;

namespace FruityLookup.GUI;

public partial class MainPage : ContentPage {
    int count = 0;
    FruityLookup fruityLookup = new();

    public MainPage(MainViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }
}
