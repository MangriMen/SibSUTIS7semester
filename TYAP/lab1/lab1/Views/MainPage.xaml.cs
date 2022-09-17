using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using lab1.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace lab1.Views;

public sealed partial class MainPage : Page, INotifyPropertyChanged
{
    private string RawGrammar
    {
        get; set;
    } = "S: aA | bS | ->\r\nA: bS";

    private const string START_TERMINATE_SYMBOL = "S";
    private const string EMPTY_SYMBOL = "->";

    private int _sequenceLength = 2;

    private int SequenceLength
    {
        get => _sequenceLength;
        set
        {
            _sequenceLength = value;
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(SequenceLength)));
        }
    }

    private Dictionary<string, List<string>> _grammar = new();

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

    private void StartClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            _grammar = ParseGrammar(RawGrammar);
            var sequences = GenerateSequences(_grammar, SequenceLength);

            var sequencesOut = new StringBuilder();
            foreach (var sequence in sequences)
            {
                var line = Regex.Replace(sequence.Value, "[A-Z]", "");
                if (line.EndsWith(EMPTY_SYMBOL) && line.Length == SequenceLength + 2)
                {
                    sequencesOut.AppendLine(line.Replace(EMPTY_SYMBOL, "λ"));
                }
            }

            Output.Text = sequencesOut.ToString();
        }
        catch (GrammarException)
        {
            var dlg = new ContentDialog()
            {
                Title = "Ошибка",
                Content = "Грамматика введена неверно, проверьте правильность ввода.",
                CloseButtonText = "Закрыть",
                XamlRoot = XamlRoot
            };

            _ = dlg.ShowAsync();
        }
    }

    public static Dictionary<string, List<string>> ParseGrammar(string rawGrammar)
    {
        var lines = rawGrammar.Replace(" ", "").Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var newGrammar = new Dictionary<string, List<string>>();

        try
        {
            foreach (var line in lines)
            {
                var tokens = line.Split(':');
                var rule = tokens[0];
                var variants = tokens[1].Split('|');

                newGrammar.TryAdd(rule, new());
                foreach (var variant in variants)
                {
                    newGrammar[rule].Add(variant);
                }
            }
        }
        catch (IndexOutOfRangeException)
        {
            throw new GrammarException("Failed to parse");
        }

        return newGrammar;
    }

    public static Dictionary<int, string> GenerateSequences(Dictionary<string, List<string>> grammar, int sequenceLength)
    {
        var root = new TreeNode<string>(START_TERMINATE_SYMBOL);
        root.AddChildren(grammar[root.Value].ToArray());
        FillTree(root, grammar, sequenceLength);

        Dictionary<int, string> sequences = new();
        var prevSequence = "";
        var prevLevel = -1;
        var prevIndex = 0;
        var index = 0;
        root.Traverse(new Action<string, int>((nodeValue, level) =>
        {
            sequences.TryAdd(index, "");

            if (level <= prevLevel)
            {
                prevIndex = index;
                index++;
                prevSequence = sequences[prevIndex].Remove(sequences[prevIndex].Length - (prevLevel - level + 1) * 2);
                sequences.TryAdd(index, prevSequence);
            }

            sequences[index] += nodeValue;

            prevLevel = level;
        }));

        return sequences;
    }

    public static void FillTree(TreeNode<string> root, Dictionary<string, List<string>> grammar, int sequenceLength, int count = 0)
    {
        if (count >= sequenceLength)
        {
            return;
        }

        foreach (var node in root.Children)
        {
            if (node.Value.Contains(EMPTY_SYMBOL))
            {
                return;
            }
            node.AddChildren(grammar[Regex.Replace(node.Value, @"[a-z]", "")].ToArray());
            FillTree(node, grammar, sequenceLength, count + 1);
        }
    }

    private void ManualInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        RawGrammar = ((TextBox)sender).Text;
    }

    private void SequenceLength_TextChanged(object sender, TextChangedEventArgs e)
    {
        var prevValue = SequenceLength;

        try
        {
            SequenceLength = int.Parse(((TextBox)sender).Text);
        }
        catch
        {
            SequenceLength = prevValue;
        }
    }

    private void SequenceLength_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        var pos = sender.SelectionStart;
        sender.Text = new string(sender.Text.Where(char.IsDigit).ToArray());
        sender.SelectionStart = pos;
    }

    private void SequenceLength_LostFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var prevValue = SequenceLength;

        try
        {
            SequenceLength = int.Parse(((TextBox)sender).Text);
        }
        catch
        {
            SequenceLength = prevValue;
        }
    }
}

public class GrammarException : Exception
{
    public GrammarException()
    {
    }

    public GrammarException(string message)
        : base(message)
    {
    }

    public GrammarException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class TreeNode<T>
{
    private readonly T _value;
    private readonly List<TreeNode<T>> _children = new();

    public TreeNode(T value)
    {
        _value = value;
    }

    public TreeNode<T> this[int i]
    {
        get
        {
            return _children[i];
        }
    }

    public TreeNode<T>? Parent
    {
        get; private set;
    }

    public T Value => _value;

    public ReadOnlyCollection<TreeNode<T>> Children => _children.AsReadOnly();

    public TreeNode<T> AddChild(T value)
    {
        var node = new TreeNode<T>(value) { Parent = this };
        _children.Add(node);
        return node;
    }

    public TreeNode<T>[] AddChildren(params T[] values)
    {
        return values.Select(AddChild).ToArray();
    }

    public bool RemoveChild(TreeNode<T> node)
    {
        return _children.Remove(node);
    }

    public void Traverse(Action<T> action)
    {
        action(Value);
        foreach (var child in _children)
        {
            child.Traverse(action);
        }
    }

    public void Traverse(Action<T, int> action, int level = 0)
    {
        action(Value, level);
        foreach (var child in _children)
        {
            child.Traverse(action, level + 1);
        }
    }

    public IEnumerable<T> Flatten()
    {
        return new[] { Value }.Concat(_children.SelectMany(x => x.Flatten()));
    }
}