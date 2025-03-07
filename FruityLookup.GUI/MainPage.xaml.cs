using FruityLookup.Entities;
using System.Threading.Tasks;

namespace FruityLookup.GUI {
    public partial class MainPage : ContentPage {
        int count = 0;
        FruityLookup fruityLookup = new();

        public MainPage() {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e) {
            count++;
            Fruit? fruit = await fruityLookup.getFruitInformationAsync("apple");
            if (fruit != null) {
                CounterBtn.Text = fruit.genus;
            }
            /*
            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";
            */
        }
    }

}
