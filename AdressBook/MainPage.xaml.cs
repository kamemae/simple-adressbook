using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
namespace AdressBook {
    using MVVM.Models;
    public partial class MainPage : ContentPage {
        ObservableCollection<personData> census { get; set; } = new ObservableCollection<personData>();
        ObservableCollection<personData> searchResults { get; set; } = new ObservableCollection<personData>();

        public MainPage() {
            InitializeComponent();

            AdressBook.ItemsSource = census;
            AdressBook.BindingContext = this;

            AdressBookSearch.ItemsSource = searchResults;

            SearchList.BindingContext = this;

            editForm.IsVisible = false;
            SearchList.IsVisible = false;
        }

        private bool Validate(string tel) {
            Regex intvalidator = new Regex(@"[0-9]{9}");
            return !intvalidator.IsMatch(tel);
        }

        private void addNewEntry(object sender, EventArgs e) {
            if(string.IsNullOrEmpty(entryName.Text) || string.IsNullOrEmpty(entrySurname.Text) || string.IsNullOrEmpty(entryTel.Text)) {
                DisplayAlert("Error", "Check if values are correct", "Ok");
                return;
            }
            if(Validate($"{entryTel.Text}")) return;
                
            census.Add(new personData {
                FirstName = entryName.Text,
                LastName = entrySurname.Text,
                Tel = entryTel.Text
            });

            entryName.Text = ""; entrySurname.Text = ""; entryTel.Text = "";
            entryName.Focus();

            SearchList.IsVisible = false;
            AdressBook.IsVisible = true;
        }

        private int index = -1;
        private async void AdressBook_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(AdressBook.SelectedItem == null) return;

            var selectedItem = (personData)AdressBook.SelectedItem;
            string message = $"You selected {selectedItem.FirstName}, {selectedItem.LastName} {selectedItem.Tel}";
            index = census.IndexOf(selectedItem);

            bool alertAnswer = await DisplayAlert("Selected Entry", message, $"Remove", "Edit");
            if(alertAnswer) census.RemoveAt(index);
            else {
                editForm.IsVisible = true;
                AdressBook.IsVisible = false;
                addForm.IsVisible = false;
                searchBar.IsVisible = false;

                editName.Text = selectedItem.FirstName;
                editSurname.Text = selectedItem.LastName;
                editTel.Text = selectedItem.Tel;
            }

            AdressBook.SelectedItem = null;
        }

        private void editUser(object sender, EventArgs e) {
            if(index >= 0 && index < census.Count) {
                dynamic editedPerson = census[index];
                editedPerson.FirstName = editName.Text;
                editedPerson.LastName = editSurname.Text;
                editedPerson.Tel = editTel.Text;

                census[index] = editedPerson;

                AdressBook.ItemsSource = null;
                AdressBook.BindingContext = null;

                AdressBook.ItemsSource = census;
                AdressBook.BindingContext = this;

                editName.Text = "";
                editSurname.Text = "";
                editTel.Text = "";

                editForm.IsVisible = false;
                AdressBook.IsVisible = true;
                addForm.IsVisible = true;
                searchBar.IsVisible = true;
                SearchList.IsVisible = false;
            }
        }

        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e) {
            dynamic filteredResults;
            string searchText = e.NewTextValue.ToLower();
            searchResults.Clear();

            if(string.IsNullOrEmpty(searchText)) {
                SearchList.IsVisible = false;
                AdressBook.IsVisible = true;
            } else {
                filteredResults = census.Where(person => person.FirstName.ToLower().Contains(searchText) ||
                                                             person.LastName.ToLower().Contains(searchText) ||
                                                             person.Tel.ToLower().Contains(searchText));

                foreach(var person in filteredResults) searchResults.Add(person);

                SearchList.IsVisible = true;
                AdressBook.IsVisible = false;
            }
        }
    }
}