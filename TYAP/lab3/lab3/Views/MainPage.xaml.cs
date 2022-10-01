using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CommunityToolkit.WinUI.UI.Controls;
using CommunityToolkit.WinUI.UI.Controls.Primitives;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using lab3.Models;
using lab3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json.Linq;
using Windows.UI;

namespace lab3.Views;

public sealed partial class MainPage : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private const string GRAMMAR_REGEXP = @"\[[^]]*]|\{[^}]*}|[^,]+";

    private List<string> _rules = new();
    private List<string> _alphabet = new();
    private string _start_rule = "";
    private List<string> _end_rules = new();

    private string _current_rule = "";

    public string Chain
    {
        get; set;
    } = "";

    public static Border CreateHeaderCell(string label)
    {
        return new Border()
        {
            BorderBrush = new SolidColorBrush(Color.FromArgb(110, 0, 0, 0)),
            BorderThickness = new Thickness(2),
            Padding = new Thickness(4),
            HorizontalAlignment = HorizontalAlignment.Center,
            Width = 100,
            Child = new TextBlock() { Text = label, HorizontalAlignment = HorizontalAlignment.Center }
        };
    }

    public static Border CreateRowCell(FrameworkElement frameworkElement)
    {
        return new Border()
        {
            Width = 100,
            Child = frameworkElement
        };
    }

    private string _grammar = "";
    public string Grammar
    {
        get => _grammar;
        set
        {
            _grammar = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Grammar)));
        }
    }

    Dictionary<string, List<string>> _graph = new();

    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    public static List<string> ParseGrammarSetPart(string field)
    {
        return Regex.Replace(field, @"{|}|\s+", "").Split(",").ToList();
    }

    private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
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

        try
        {
            var grm = (from Match m in Regex.Matches(_grammar.Replace(" ", ""), GRAMMAR_REGEXP) select m.Value).ToArray();
            _rules = ParseGrammarSetPart(grm[0]).Distinct().ToList();
            _alphabet = ParseGrammarSetPart(grm[1]).Distinct().ToList();
            _start_rule = grm[2];
            _end_rules = ParseGrammarSetPart(grm[3]).Distinct().ToList();

            var newItems = new List<ListViewItem>();

            var header = new StackPanel() { Orientation = Orientation.Horizontal };
            header.Children.Add(CreateHeaderCell("Правила"));
            foreach (var symbol in _alphabet)
            {
                header.Children.Add(CreateHeaderCell(symbol));
            }
            newItems.Add(new ListViewItem() { Content = header });

            foreach (var rule in _rules)
            {
                var row = new StackPanel() { Orientation = Orientation.Horizontal };
                row.Children.Add(CreateRowCell(new TextBlock() { Text = rule, HorizontalAlignment = HorizontalAlignment.Center }));
                for (var i = 0; i < _alphabet.Count; i++)
                {
                    row.Children.Add(CreateRowCell(new TextBox() { Text = "", HorizontalAlignment = HorizontalAlignment.Center }));
                }
                newItems.Add(new ListViewItem() { Content = row });
            }

            grammarRules.ItemsSource = newItems;
        }
        catch
        {

        }
    }

    public void CreateGraph()
    {
        _graph.Clear();

        var rowNumber = 1;
        foreach (var row in grammarRules.Items)
        {
            if (rowNumber == 1)
            {
                rowNumber++;
                continue;
            }

            var rowItemNumber = 1;
            var stackPanelRow = (StackPanel)((ListViewItem)row).Content;
            var rule = ((TextBlock)(((Border)stackPanelRow.Children[0]).Child)).Text;
            _graph.Add(rule, new());
            foreach (var item in stackPanelRow.Children)
            {
                var child = ((Border)item).Child;
                if (rowItemNumber != 1)
                {
                    _graph[rule].Add(((TextBox)child).Text);
                }
                rowItemNumber++;
            }
            rowNumber++;
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        CreateGraph();

        int current_symbol = 0;


        _current_rule = _start_rule;
        foreach (var symbol in Chain)
        {
            var rule = _graph[_current_rule];
            for (var i = 0; i < rule.Count; i++)
            {
                if (symbol == _alphabet[i][0])
                {
                    _current_rule = rule[i];
                    Debug.WriteLine(rule[i][0]);
                    if (_end_rules.Contains(_current_rule))
                    {
                        break;
                    }
                }
                //else
                //{
                //    throw new Exception("Symbol not in alphabet or correct rule not found");
                //}
            }
        }
    }
}
