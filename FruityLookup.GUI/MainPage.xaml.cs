using FruityLookup.GUI.ModelView;

namespace FruityLookup.GUI;

public partial class MainPage : ContentPage {
    public MainPage(MainViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }
}
