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
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json.Linq;
using Windows.UI;

namespace lab3.Views;

public sealed partial class MainPage : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private const string GRAMMAR_REGEXP = @"\[[^]]*]|\{[^}]*}|[^,]+";

    private string[] _rules = new[] { "" };
    private string[] _alphabet = new[] { "" };
    private string _start_rule = "";
    private string[] _end_rules = new[] { "" };

    private string _current_rule = "";

    public string Chain
    {
        get; set;
    } = "";

    public static StackPanel CreateStackPanelRowHeader(string[] cells)
    {
        var r = 4;
        var fullRadius = new CornerRadius(r, r, r, r);
        var leftRadius = new CornerRadius(r, 0, 0, r);
        var rightRadius = new CornerRadius(0, r, r, 0);
        var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
        if (cells.Length > 0)
        {
            var isMoreThanOneElement = cells.Length > 1;
            stackPanel.Children.Add(CreateHeaderCell(cells[0], isMoreThanOneElement ? leftRadius : fullRadius));
            for (var i = 1; i < cells.Length - 1; i++)
            {
                stackPanel.Children.Add(CreateHeaderCell(cells[i]));
            }
            stackPanel.Children.Add(CreateHeaderCell(cells[^1], rightRadius));
        }
        return stackPanel;
    }

    public static Border CreateHeaderCell(string label, CornerRadius? cornerRadius = null)
    {
        return new Border()
        {
            Background = new SolidColorBrush(Colors.Gray),
            CornerRadius = cornerRadius ?? new CornerRadius(0),
            Padding = new Thickness(4),
            HorizontalAlignment = HorizontalAlignment.Center,
            Width = 80,
            Child = new TextBlock() { Text = label, HorizontalAlignment = HorizontalAlignment.Center }
        };
    }

    public static Border CreateRowCell(FrameworkElement frameworkElement)
    {
        return new Border()
        {
            Width = 80,
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

    private string _ouput = "";
    public string Output
    {
        get => _ouput;
        set
        {
            _ouput = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Output)));
        }
    }

    private readonly Dictionary<string, List<string>> _graph = new();

    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    private async void GrammarChanged(object sender, TextChangedEventArgs e)
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
            _rules = ParseGrammarSetPart(grm[0]).Distinct().ToArray();
            _alphabet = ParseGrammarSetPart(grm[1]).Distinct().ToArray();
            _start_rule = grm[2];
            _end_rules = ParseGrammarSetPart(grm[3]).Distinct().ToArray();

            var newItems = new List<ListViewItem>();

            var headerLabels = new List<string>() { "Правила" };
            headerLabels.AddRange(_alphabet);
            newItems.Add(new ListViewItem()
            {
                Padding = new Thickness(0),
                Content = CreateStackPanelRowHeader(headerLabels.ToArray())
            });

            foreach (var rule in _rules)
            {
                var row = new StackPanel() { Orientation = Orientation.Horizontal };
                row.Children.Add(CreateRowCell(new TextBlock()
                {
                    Text = rule,
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }));
                for (var i = 0; i < _alphabet.Length; i++)
                {
                    var cellContent = new TextBox()
                    {
                        Text = "",
                        TextAlignment = TextAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    cellContent.TextChanging += SequenceChangingAsync;
                    row.Children.Add(CreateRowCell(cellContent));
                }
                newItems.Add(new ListViewItem()
                {
                    Padding = new Thickness(0),
                    Content = row
                });
            }

            grammarRules.ItemsSource = newItems;
        }
        catch
        {

        }
    }

    private void ChainChanging(object sender, TextBoxTextChangingEventArgs e)
    {
        var textBox = (TextBox)sender;

        if (m_ignoreNextTextChanged)
        {
            m_ignoreNextTextChanged = false;
            return;
        }

        var alphabetRegex = @"[^";
        foreach (var symbol in _alphabet)
        {
            alphabetRegex += symbol;
        }
        alphabetRegex += "]+";

        var selectionStart = textBox.SelectionStart;
        var prevTextLength = textBox.Text.Length;
        textBox.Text = Regex.Replace(textBox.Text, alphabetRegex, "");
        textBox.SelectionStart = selectionStart - (prevTextLength - textBox.Text.Length);
    }

    private bool m_ignoreNextTextChanged = false;
    private void ChainKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Back
            || e.Key == Windows.System.VirtualKey.Delete)
        {
            m_ignoreNextTextChanged = true;
        }
    }

    public static List<string> ParseGrammarSetPart(string field)
    {
        return Regex.Replace(field, @"{|}|\s+", "").Split(",").ToList();
    }

    private async void SequenceChangingAsync(object sender, TextBoxTextChangingEventArgs e)
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

        var textBox = (TextBox)sender;

        if (!_rules.Contains(textBox.Text))
        {
            textBox.Text = "";
        }
    }

    public void CreateGraph()
    {
        try
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
                        var childText = ((TextBox)child).Text;
                        //if (rule != _start_rule && childText == "")
                        //{
                        //    throw new Exception($"Rule sequenc for rule {rule} not defined");
                        //}
                        _graph[rule].Add(childText);
                    }
                    rowItemNumber++;
                }
                rowNumber++;
            }
        }
        catch
        {
            _ = new ContentDialog()
            {
                Title = "Ошибка",
                Content = new TextBlock() { Text = "Одно или несколько правил для состояний не заданы" },
                CloseButtonText = "Закрыть",
                XamlRoot = XamlRoot
            }.ShowAsync();
        }
    }

    private void CheckClicked(object sender, RoutedEventArgs e)
    {
        CreateGraph();

        Output = "";
        var prev_rule = "";
        var current_symbol = "";
        _current_rule = _start_rule;
        try
        {
            foreach (var symbol in Chain)
            {
                var rule = _graph[_current_rule];
                Output += $"{_current_rule} ";
                for (var i = 0; i < rule.Count; i++)
                {
                    if (symbol == _alphabet[i][0])
                    {
                        current_symbol = symbol.ToString();
                        prev_rule = _current_rule;
                        _current_rule = rule[i];
                        Output += $"─{symbol}─> ";
                    }
                }
            }
        }
        catch
        {
            _ = new ContentDialog()
            {
                Title = "Ошибка",
                Content = new TextBlock() { Text = $"Цепочка не принадлежит ДКА. Не существует перехода из {prev_rule} по символу {current_symbol}." },
                CloseButtonText = "Закрыть",
                XamlRoot = XamlRoot
            }.ShowAsync();
            return;
        }

        if (_current_rule == "")
        {
            _ = new ContentDialog()
            {
                Title = "Ошибка",
                Content = new TextBlock() { Text = $"Цепочка не принадлежит ДКА. Не существует перехода из {prev_rule} по символу {current_symbol}." },
                CloseButtonText = "Закрыть",
                XamlRoot = XamlRoot
            }.ShowAsync();
            return;
        }

        Output += $"{_current_rule}";

        if (!_end_rules.Contains(_current_rule))
        {
            Output += "!";
            _ = new ContentDialog()
            {
                Title = "Ошибка",
                Content = new TextBlock() { Text = $"Цепочка не принадлежит ДКА. {_current_rule} не является конечным состоянием." },
                CloseButtonText = "Закрыть",
                XamlRoot = XamlRoot
            }.ShowAsync();
            return;
        }
    }
}
