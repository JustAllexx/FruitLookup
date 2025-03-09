using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FruityLookup.Entities;
using FruityLookup.Exceptions;
using System.Linq.Expressions;

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
        Fruit? fruit;
        try { fruit = await fruityLookup.getFruitInformationAsync(FruitInput); }
        catch (FruitNotFound) { return; }

        if (fruit == null) return;
        FruitNameOutput = fruit.name;
        FruitIDOutput = fruit.id.ToString();
        FruitFamilyOutput = fruit.family;
        FruitSugarOutput = fruit.nutritions.sugar.ToString() + "g";
        FruitCarbohydratesOutput = fruit.nutritions.carbohydrates.ToString() + "g";
        
    }
}
