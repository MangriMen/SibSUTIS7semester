using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using lab4.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace lab4.Views;

public sealed partial class MainPage : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private const string GRAMMAR_REGEXP = @"\[[^]]*]|\{[^}]*}|[^,]+";

    private string _rules_count = "1";
    public string RulesCount
    {
        get => _rules_count;
        set
        {
            _rules_count = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RulesCount)));
        }
    }

    private static readonly string DELETE_SYMBOL = "!";
    private static readonly string EXIT_SYMBOL = "@";
    private static readonly string EMPTY_SEQUENCE_SYMBOL = "^";
    private readonly string[] _default_alphabet = { DELETE_SYMBOL, EXIT_SYMBOL, EMPTY_SEQUENCE_SYMBOL };

    private string[] _rules = new[] { "" };
    private string[] _alphabet = new[] { "" };
    private string[] _stack_alphabet = new[] { "" };
    private string _start_rule = "";
    private string _start_stack_symbol = "";
    private string[] _end_rules = new[] { "" };

    private readonly Stack<char> stack = new();

    private string _current_rule = "";

    public string Chain
    {
        get; set;
    } = "";

    public static StackPanel CreateStackPanelRowHeader(string[] cells)
    {
        var r = 4;
        var stackPanel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Background = new SolidColorBrush(Colors.Gray),
            CornerRadius = new CornerRadius(r),
        };
        if (cells.Length > 0)
        {
            stackPanel.Children.Add(CreateHeaderCell(cells[0]));
            for (var i = 1; i < cells.Length - 1; i++)
            {
                stackPanel.Children.Add(CreateHeaderCell(cells[i]));
            }
            stackPanel.Children.Add(CreateHeaderCell(cells[^1]));
        }
        return stackPanel;
    }

    public static Border CreateHeaderCell(string label)
    {
        return new Border()
        {
            Padding = new Thickness(4),
            HorizontalAlignment = HorizontalAlignment.Center,
            MinWidth = 80,
            Child = new TextBlock() { Text = label, HorizontalAlignment = HorizontalAlignment.Center }
        };
    }

    public static Border CreateRowCell(FrameworkElement frameworkElement)
    {
        return new Border()
        {
            MinWidth = 80,
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

    private readonly Dictionary<string, string> _graph = new();

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
            Output = "";
            var grm = (from Match m in Regex.Matches(_grammar.Replace(" ", ""), GRAMMAR_REGEXP) select m.Value).ToArray();
            _rules = ParseGrammarSetPart(grm[0]).Distinct().ToArray();
            _alphabet = ParseGrammarSetPart(grm[1]).Distinct().ToArray();
            _stack_alphabet = ParseGrammarSetPart(grm[2]).Distinct().ToArray();
            _start_rule = grm[3];
            _start_stack_symbol = grm[4];
            _end_rules = ParseGrammarSetPart(grm[5]).Distinct().ToArray();

            var extendedAlphabet = new List<string>();
            extendedAlphabet.AddRange(_default_alphabet);
            extendedAlphabet.AddRange(_alphabet);
            _alphabet = extendedAlphabet.ToArray();

            stack.Clear();
            stack.Push(_start_stack_symbol[0]);

            var newItems = new List<ListViewItem>();

            var headerLabels = new List<string>() { "Номер", "Текущее состояние", "Символ цепочки", "Верхний символ стека", "Следующее состояние", "Стек" };
            newItems.Add(new ListViewItem()
            {
                Padding = new Thickness(0),
                Content = CreateStackPanelRowHeader(headerLabels.ToArray())
            });

            var handlers = new List<TypedEventHandler<TextBox, TextBoxTextChangingEventArgs>?>() { null, RuleChangingAsync, SequenceChangingAsync, StackChangingAsync, RuleChangingAsync, StackMultipleChangingAsync };

            var intRulesCount = int.Parse(RulesCount);

            for (var rc = 0; rc < intRulesCount; rc++)
            {
                var row = new StackPanel() { Orientation = Orientation.Horizontal, Spacing = 8 };
                for (var i = 0; i < headerLabels.Count; i++)
                {
                    var header_ = (Border)((StackPanel)newItems[0].Content).Children[i];
                    header_.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    FrameworkElement cellContent;
                    if (i == 0)
                    {
                        cellContent = new TextBlock()
                        {
                            Width = header_.DesiredSize.Width,
                            Text = (rc + 1).ToString(),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                    }
                    else
                    {
                        cellContent = new TextBox()
                        {
                            Width = header_.DesiredSize.Width,
                            Text = "",
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        if (handlers[i] != null)
                        {
                            ((TextBox)cellContent).TextChanging += handlers[i];
                        }
                    }

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

        try
        {
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
        catch
        {
            textBox.Text = "";

            try
            {
                _ = new ContentDialog()
                {
                    Title = "Ошибка",
                    Content = new TextBlock() { TextWrapping = TextWrapping.Wrap, Text = $"Грамматика не задана" },
                    CloseButtonText = "Закрыть",
                    XamlRoot = XamlRoot
                }.ShowAsync();
                return;
            }
            catch
            {

            }
        }
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

    private async void RuleChangingAsync(object sender, TextBoxTextChangingEventArgs e)
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

        if (!_alphabet.Contains(textBox.Text))
        {
            textBox.Text = "";
        }
    }

    private async void StackChangingAsync(object sender, TextBoxTextChangingEventArgs e)
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

        if (!_stack_alphabet.Contains(textBox.Text))
        {
            textBox.Text = "";
        }
    }

    private async void StackMultipleChangingAsync(object sender, TextBoxTextChangingEventArgs e)
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

        foreach (var symbol in textBox.Text)
        {
            if (!_stack_alphabet.Contains(symbol.ToString()))
            {
                textBox.Text = "";
                break;
            }
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

                var stackPanelRow = (StackPanel)((ListViewItem)row).Content;
                var children = new List<TextBox>();

                for (var i = 1; i < stackPanelRow.Children.Count; i++)
                {
                    children.Add(((TextBox)((Border)stackPanelRow.Children[i]).Child));
                }

                if (_end_rules.Contains(children[3].Text) && children[4].Text != EXIT_SYMBOL)
                {
                    try
                    {
                        _graph.Clear();

                        _ = new ContentDialog()
                        {
                            Title = "Ошибка",
                            Content = new TextBlock() { Text = "Найдено конечное правило с символом отличным от символа выхода." },
                            CloseButtonText = "Закрыть",
                            XamlRoot = XamlRoot
                        }.ShowAsync();

                        return;
                    }
                    catch
                    {

                    }
                }

                if (!_end_rules.Contains(children[3].Text) && children[4].Text == EXIT_SYMBOL)
                {
                    try
                    {
                        _graph.Clear();

                        _ = new ContentDialog()
                        {
                            Title = "Ошибка",
                            Content = new TextBlock() { Text = "Найдено правило с символом выхода, но состояние не является конечным." },
                            CloseButtonText = "Закрыть",
                            XamlRoot = XamlRoot
                        }.ShowAsync();

                        return;
                    }
                    catch
                    {

                    }
                }

                var ruleString = $"{children[0].Text},{children[1].Text},{children[2].Text}";
                var transitionString = $"{children[3].Text} {children[4].Text}";

                _graph.Add(ruleString, transitionString);
            }
        }
        catch
        {
            try
            {
                _ = new ContentDialog()
                {
                    Title = "Ошибка",
                    Content = new TextBlock() { Text = "Одно или несколько правил для состояний не заданы" },
                    CloseButtonText = "Закрыть",
                    XamlRoot = XamlRoot
                }.ShowAsync();
            }
            catch
            {

            }
        }
    }

    private void CheckClicked(object sender, RoutedEventArgs e)
    {
        CreateGraph();

        Output = "";
        stack.Clear();
        stack.Push(_start_stack_symbol[0]);
        _current_rule = _start_rule;
        var prev_rule = "";
        var current_symbol = "";
        var processChain = $"{Chain}^";
        try
        {
            for (var i = 0; i < processChain.Length; i++)
            {
                var symbol = processChain[i];

                prev_rule = _current_rule;
                current_symbol = symbol.ToString();
                var assembledRule = $"{_current_rule},{symbol},{stack.Peek()}";
                var sequence = _graph[assembledRule].Split(" ");

                _current_rule = sequence[0];
                if (sequence[1].Length > 1)
                {
                    stack.Push(sequence[1][0]);
                }
                else if (sequence[1] == DELETE_SYMBOL)
                {
                    stack.Pop();
                }

                Output += $"({assembledRule}) ├─ ({string.Join(",", sequence)});\n";

                if (current_symbol == EMPTY_SEQUENCE_SYMBOL && !_end_rules.Contains(sequence[0]) && sequence[1] != EXIT_SYMBOL)
                {
                    i--;
                    continue;
                }
            }
        }
        catch
        {
            try
            {
                if (current_symbol == "^")
                {
                    _ = new ContentDialog()
                    {
                        Title = "Ошибка",
                        Content = new TextBlock() { TextWrapping = TextWrapping.Wrap, Text = $"Цепочка не принадлежит ДМПА. Не найден выход." },
                        CloseButtonText = "Закрыть",
                        XamlRoot = XamlRoot
                    }.ShowAsync();
                    return;
                }
                else
                {
                    _ = new ContentDialog()
                    {
                        Title = "Ошибка",
                        Content = new TextBlock()
                        {
                            TextWrapping = TextWrapping.Wrap,
                            Text = $"Цепочка не принадлежит ДМПА. Не существует перехода из {(prev_rule == "" ? "Ничего" : prev_rule)} по символу {current_symbol} при верхнем символе стека {stack.Peek()}."
                        },
                        CloseButtonText = "Закрыть",
                        XamlRoot = XamlRoot
                    }.ShowAsync();
                    return;
                }
            }
            catch
            {

            }
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            GrammarTextBox.TextChanged -= GrammarChanged;
            RulesCountTextBox.TextChanged -= GrammarChanged;
            Output = "";

            var rules = new List<string[]>();
            var counter = 0;
            foreach (var line in File.ReadLines(@"D:\Projects\GithubProjects\SibSUTIS7semester\TYAP\lab4\DMPA.txt"))
            {
                if (counter == 0)
                {
                    Grammar = line;
                    counter++;
                }
                else if (counter > 0)
                {
                    rules.Add(line.Trim().Split(","));
                }
            }

            RulesCount = rules.Count.ToString();

            var grm = (from Match m in Regex.Matches(Grammar.Replace(" ", ""), GRAMMAR_REGEXP) select m.Value).ToArray();
            _rules = ParseGrammarSetPart(grm[0]).Distinct().ToArray();
            _alphabet = ParseGrammarSetPart(grm[1]).Distinct().ToArray();
            _stack_alphabet = ParseGrammarSetPart(grm[2]).Distinct().ToArray();
            _start_rule = grm[3];
            _start_stack_symbol = grm[4];
            _end_rules = ParseGrammarSetPart(grm[5]).Distinct().ToArray();

            var extendedAlphabet = new List<string>();
            extendedAlphabet.AddRange(_default_alphabet);
            extendedAlphabet.AddRange(_alphabet);
            _alphabet = extendedAlphabet.ToArray();

            extendedAlphabet = new List<string>();
            extendedAlphabet.AddRange(_default_alphabet);
            extendedAlphabet.AddRange(_stack_alphabet);
            _stack_alphabet = extendedAlphabet.ToArray();

            stack.Clear();
            stack.Push(_start_stack_symbol[0]);

            var newItems = new List<ListViewItem>();

            var headerLabels = new List<string>() { "Номер", "Текущее состояние", "Символ цепочки", "Верхний символ стека", "Следующее состояние", "Стек" };
            newItems.Add(new ListViewItem()
            {
                Padding = new Thickness(0),
                Content = CreateStackPanelRowHeader(headerLabels.ToArray())
            });

            var handlers = new List<TypedEventHandler<TextBox, TextBoxTextChangingEventArgs>?>() { null, RuleChangingAsync, SequenceChangingAsync, StackChangingAsync, RuleChangingAsync, StackMultipleChangingAsync };

            for (var rc = 0; rc < rules.Count; rc++)
            {
                var row = new StackPanel() { Orientation = Orientation.Horizontal, Spacing = 8 };
                for (var i = 0; i < headerLabels.Count; i++)
                {
                    var header_ = (Border)((StackPanel)newItems[0].Content).Children[i];
                    header_.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    FrameworkElement cellContent;
                    if (i == 0)
                    {
                        cellContent = new TextBlock()
                        {
                            Width = header_.DesiredSize.Width,
                            Text = (rc + 1).ToString(),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                    }
                    else
                    {
                        cellContent = new TextBox()
                        {
                            Width = header_.DesiredSize.Width,
                            Text = rules[rc][i - 1],
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        if (handlers[i] != null)
                        {
                            ((TextBox)cellContent).TextChanging += handlers[i];
                        }
                    }

                    row.Children.Add(CreateRowCell(cellContent));
                }
                newItems.Add(new ListViewItem()
                {
                    Padding = new Thickness(0),
                    Content = row
                });
            }

            grammarRules.ItemsSource = newItems;

            RulesCountTextBox.TextChanged += GrammarChanged;
            GrammarTextBox.TextChanged += GrammarChanged;
        }
        catch
        {
            Grammar = "";
            RulesCount = "1";
            _rules = new[] { "" };
            _alphabet = new[] { "" };
            _stack_alphabet = new[] { "" };
            _start_rule = "";
            _start_stack_symbol = "";
            _end_rules = new[] { "" };

            stack.Clear();

            grammarRules.ItemsSource = new List<ListViewItem>();

            try
            {
                _ = new ContentDialog()
                {
                    Title = "Ошибка",
                    Content = new TextBlock() { TextWrapping = TextWrapping.Wrap, Text = $"Ошибка чтения файла." },
                    CloseButtonText = "Закрыть",
                    XamlRoot = XamlRoot
                }.ShowAsync();
                return;
            }
            catch
            {

            }
        }
    }
}
