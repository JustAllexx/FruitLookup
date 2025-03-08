using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FruityLookup.Entities;

namespace FruityLookup.GUI.ModelView;

public partial class MainViewModel : ObservableObject
{
    readonly FruityLookup fruityLookup = new();

    [ObservableProperty]
    string fruitInput;

    [ObservableProperty]
    string fruitNameOutput;
    [ObservableProperty]
    string fruitIDOutput;
    [ObservableProperty]
    string fruitFamilyOutput;
    [ObservableProperty]
    string fruitSugarOutput;
    [ObservableProperty]
    string fruitCarbohydratesOutput;

    [RelayCommand]
    async Task GetFruitDetails() {
        Fruit? fruit = await fruityLookup.getFruitInformationAsync(FruitInput);
        if (fruit != null) {
            FruitNameOutput = fruit.name;
            FruitIDOutput = fruit.id.ToString();
            FruitFamilyOutput = fruit.family;
            FruitSugarOutput = fruit.nutritions.sugar.ToString() + "g";
            FruitCarbohydratesOutput = fruit.nutritions.carbohydrates.ToString() + "g";
        }
    }
}
