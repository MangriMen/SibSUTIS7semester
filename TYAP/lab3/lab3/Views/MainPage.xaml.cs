using System.ComponentModel;
using lab3.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace lab3.Views;

public sealed partial class MainPage : Page, INotifyPropertyChanged
{
    class Edge
    {
        public string Rule { get; set; }
        public string State { get; set; }

        public Edge(string rule="", string state="")
        {
            Rule = rule;
            State = state;
        }
    }

    List<string> _rules;
    List<string> _alphabet;
    string _start_rule;
    List<string> _end_rules;
    string _grammar;

    public string Grammar
    {
        get => _grammar;
        set
        {
            try
            {
                var grm = _grammar.Replace(" ", "").Split(",");
                _rules = parseMultipleField(grm[0]);
                _alphabet = parseMultipleField(grm[1]);
                _start_rule = grm[2];
                _end_rules = parseMultipleField(grm[3]);

                Edges.Clear();
                foreach (var rule in _rules) {
                    Edges.Add(new Edge(rule, ""));
                }

                _grammar = value;
            }
            catch
            {

            }
        }
    }

    List<string> parseMultipleField(string field)
    {
        return field.Replace(" ", "").Replace("{", "").Replace("}", "").Split(",").ToList();
    }

    List<Edge> Edges = new() { new Edge("q1", "q2"), new Edge("q2", "q3")};

    Dictionary<string, List<string>> graph = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
}
