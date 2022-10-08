using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using lab15.Models;
using lab15.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace lab15.ViewModels;

public class MainViewModel : ObservableRecipient
{
    private readonly AbonentList _abonents;

    ReadOnlyObservableCollection<string> a;
    public ReadOnlyObservableCollection<string> Abonents
    {
        get
        {
            var abonents = _abonents.Abonents;
            var listAbonents = new ObservableCollection<string>();
            foreach (var abonent in abonents)
            {
                foreach (var phone in abonent.Single())
                {
                    listAbonents.Add($"Имя: {abonent.Key}    т-ф: {phone}");
                }
            }
            return new ReadOnlyObservableCollection<string>(listAbonents);
        }
        set
        {
            a = value;
        }
    }

    private string _abonentName = "";
    private string _abonentPhone = 0;

    public string AbonentName
    {
        get => _abonentName;
        set => _abonentName = value;
    }

    public string AbonentPhone
    {
        get => _abonentPhone;
        set => _abonentPhone = value;
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

        var parsedInt = 0;
        if (!int.TryParse(AbonentPhone, out parsedInt))
        {
            return;
        }

        _abonents.Add(AbonentName, parsedInt);
    }

    public void ClearHandler(object sender, RoutedEventArgs e)
    {
        _abonents.Clear();
    }
}
