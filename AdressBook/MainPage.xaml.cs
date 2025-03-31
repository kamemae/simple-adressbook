using System.Collections.ObjectModel;
namespace AdressBook {
    using MVVM.Models;
    public partial class MainPage : ContentPage {
        ObservableCollection<personData> census { get; set; } = new ObservableCollection<personData>();
        ObservableCollection<personData> searchResults { get; set; } = new ObservableCollection<personData>();
        public MainPage() {
            InitializeComponent();
            AdressBook.ItemsSource = census;
            AdressBook.BindingContext = census;
            AdressBookSearch.ItemsSource = searchResults;
            AdressBookSearch.BindingContext = searchResults;
            editForm.IsVisible = false;
        }
        private void addNewEntry(object sender, EventArgs e) {
            census.Add(
                new personData {
                    FirstName = entryName.Text,
                    LastName = entrySurname.Text,
                    Tel = entryTel.Text
                }
            );
            entryName.Text = ""; entrySurname.Text = ""; entryTel.Text = ""; 
            entryName.Focus();
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
            census[index].FirstName = editName.Text;
            census[index].LastName = editSurname.Text;
            census[index].Tel = editTel.Text;

            editName.Text = "";
            editSurname.Text = "";
            editTel.Text = "";

            census.Add(census[index]);
            census.RemoveAt(index);

            editForm.IsVisible = false;
            AdressBook.IsVisible = true;
            addForm.IsVisible = true;
            searchBar.IsVisible = true;
        }

        private void Search(object sender, EventArgs e) {
            string result = searchBar.Text.ToLower();

            searchResults.Clear();
            foreach(personData person in census) {
                searchResults.Add(person);
            }

        }
    }
}