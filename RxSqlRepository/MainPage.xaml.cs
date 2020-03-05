using System;
using System.Collections.Generic;
using System.Windows.Input;
using RxSqlRepository.Data;
using RxSqlRepository.Model;
using Xamarin.Forms;

namespace RxSqlRepository
{
    public partial class MainPage : ContentPage
    {
        private static IList<string> beersToAdd = new List<string>
        {
            "Indian Pale Ale",
            "Steam Beer",
            "Berliner Weisse",
            "Porter",
            "Stout",
            "Lager",
            "American Pale Ale",
            "Pilsener",
            "Saison",
            "Amber Ale",
            "Gose",
            "Wheat beer"
        };

        private readonly IDataSource _dataSource;
        private IDisposable? _subscription;

        public IReadOnlyList<BeerType> BeerItems { get; set; } = new List<BeerType>();
        public ICommand AddBeerCommand { get; set; }

        private void AddBeer()
        {
            // Add a random beer
            var rand = new Random();
            _dataSource.Add(new BeerType {Name = beersToAdd[rand.Next(0, beersToAdd.Count - 1)]});
        }

        public MainPage()
        {
            InitializeComponent();
            AddBeerCommand = new Command(AddBeer);
            // you can change the implementation here
            // _dataSource = new BeerInMemoryDataSource();
            _dataSource = new BeerRequeryDataSource();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _subscription = _dataSource.FetchAll().Subscribe(list =>
                {
                    // every time we get a new list, we can rebind
                    BeerItems = list;
                    OnPropertyChanged(nameof(BeerItems));
                }
            );
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Don't forget to dispose of the subscription when it's no longer needed.
            _subscription?.Dispose();
        }
    }
}