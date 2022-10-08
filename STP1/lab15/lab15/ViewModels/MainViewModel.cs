using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI.Controls;
using lab15.Models;
using lab15.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace lab15.ViewModels;

public class MainViewModel : ObservableRecipient
{
    private readonly AbonentList _abonents;

    public struct Abonent
    {
        public string Имя
        {
            get;set;
        }

        public int Номер
        {
            get;set;
        }
    }

    public string FindQuery
    {
        get;set;
    }

    public ReadOnlyObservableCollection<Abonent> Abonents
    {
        get
        {
            ILookup<string, List<int>> abonents;
            if (string.IsNullOrWhiteSpace(FindQuery)) {
                abonents = _abonents.Abonents;
            }
            else
            {
                abonents = _abonents.Find(FindQuery);
            }

            var listAbonents = new ObservableCollection<Abonent>();
            foreach (var abonent in abonents)
            {
                foreach (var phone in abonent.Single())
                {
                    listAbonents.Add(new Abonent() { Имя = abonent.Key, Номер = phone });
                }
            }
            return new ReadOnlyObservableCollection<Abonent>(listAbonents);
        }
    }

    public object? SelectedItem
    {
        get; set;
    }

    public bool IsEditing
    {
        get; set;
    }
    public string AddEditButtonText => IsEditing ? "Сохранить" : "Добавить";

    private string _abonentName = "";
    private string _abonentPhone = "";

    public string AbonentName
    {
        get => _abonentName;
        set
        {
            _abonentName = value;
            OnPropertyChanged(nameof(AbonentName));
        }
    }

    public string AbonentPhone
    {
        get => _abonentPhone;
        set
        {
            _abonentPhone = value;
            OnPropertyChanged(nameof(AbonentPhone));
        }
    }

    public MainViewModel()
    {
        _abonents = new(Windows.Storage.ApplicationData.Current.LocalFolder.Path + "abonents.list");
        _abonents.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "Abonents")
            {
                OnPropertyChanged(nameof(Abonents));
            }
        };
    }

    public void AddHandler(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(AbonentName) || string.IsNullOrWhiteSpace(AbonentPhone))
        {
            return;
        }

        int parsedInt;
        if (!int.TryParse(AbonentPhone, out parsedInt))
        {
            return;
        }

        _abonents.Add(AbonentName, parsedInt);
    }

    public void EditHandler(object sender, DataGridCellEditEndingEventArgs e)
    {
        var columnName = e.Column.Header;
        var textBox = (TextBox)e.EditingElement;

        var abonent = (Abonent)e.Row.DataContext;

        switch (columnName)
        {
            case "Номер":
                int parsedAbonentNumber;
                if (!int.TryParse(textBox.Text, out parsedAbonentNumber))
                {
                    return;
                }
                _abonents.Edit(abonent.Имя, abonent.Номер, abonent.Имя, parsedAbonentNumber);
                break;
            case "Имя":
                _abonents.Edit(abonent.Имя, abonent.Номер, textBox.Text, abonent.Номер);
                break;
        }
    }

    public void SaveHandler(object sender, RoutedEventArgs e)
    {
        _abonents.Save();
    }

    public void RemoveHandler(object sender, RoutedEventArgs e)
    {
        if (SelectedItem == null)
        {
            return;
        }

        var abonent = (Abonent)SelectedItem;

        _abonents.Remove(abonent.Имя, abonent.Номер);
    }

    public void ClearHandler(object sender, RoutedEventArgs e)
    {
        _abonents.Clear();
    }

    public async void FindHandler(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        async Task<bool> UserKeepsTyping()
        {
            var txt = ((TextBox)sender).Text;
            await Task.Delay(500);
            return txt != ((TextBox)sender).Text;
        }

        if (await UserKeepsTyping())
        {
            return;
        }

        FindQuery = sender.Text;
        OnPropertyChanged(nameof(Abonents));
    }
}
