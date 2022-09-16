using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using lab1.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using Windows.Perception.Spatial;
using Windows.Storage.Pickers;

namespace lab1.Views;

public sealed partial class MainPage : Page
{
    private string RawGrammar
    {
        get; set;
    } = "S: aA | bS | ->\r\nA: bS";

    private int SequenceLenght
    {

        get; set;
    } = 1;

    private Dictionary<string, List<string>> _grammar = new();

    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    private async void OpenFileClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void StartClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            _grammar = ParseGrammar(RawGrammar);
            var sequences = GenerateSequences(_grammar, SequenceLenght);

            var sequencesOut = new StringBuilder();
            foreach (var sequence in sequences)
            {
                var line = Regex.Replace(sequence.Value, "[A-Z]", "");
                if (line.EndsWith("->"))
                {
                    sequencesOut.AppendLine(line.Replace("->", "λ"));
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

        //PrintDebug();
    }

    public static Dictionary<string, List<string>> ParseGrammar(string rawGrammar)
    {
        var lines = rawGrammar.Replace(" ", "").Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var newGrammar = new Dictionary<string, List<string>>();

        try
        {
            foreach (var line in lines)
            {
                var tokens = line.Split(":");
                var rule = tokens[0];
                var variants = tokens[1].Split("|");

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
        var root = new TreeNode<string>("S");
        root.AddChildren(grammar[root.Value].ToArray());

        FillTree(root, grammar, sequenceLength);

        Dictionary<int, string> sequences = new();

        var prevLevel = -1;
        var prevIndex = 0;
        var index = 0;
        root.Traverse(new Action<string, int>((nodeValue, level) =>
        {
            sequences.TryAdd(index, "");

            if (level < prevLevel)
            {
                prevIndex = index;
                index++;
                sequences.TryAdd(index, "");
                sequences[index] = sequences[prevIndex].Remove(sequences[prevIndex].Length - (prevLevel - level + 1) * 2);
            }
            else if (level == prevLevel)
            {
                prevIndex = index;
                index++;
                sequences.TryAdd(index, "");
                sequences[index] = sequences[prevIndex].Remove(sequences[prevIndex].Length - 2);
            }

            sequences[index] += nodeValue;

            prevLevel = level;

            Debug.WriteLine($"{new string('\t', level)} {nodeValue}");
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
            var rule = node.Value;
            if (rule.Contains("->"))
            {
                return;
            }
            node.AddChildren(grammar[Regex.Replace(rule, @"[a-z]", "")].ToArray());
            FillTree(node, grammar, sequenceLength, count + 1);
        }
    }

    public void PrintDebug()
    {
        var grammarOut = new StringBuilder();
        foreach (var item in _grammar)
        {
            _ = grammarOut.Append($"{item.Key}: ");
            foreach (var variant in item.Value)
            {
                _ = grammarOut.Append($"{variant.Replace("->", "λ")} | ");
            }
            grammarOut.Remove(grammarOut.Length - 2, 2);
            _ = grammarOut.Append("\n");
        }

        var dlg = new ContentDialog()
        {
            Title = "Грамматика",
            Content = grammarOut.ToString(),
            CloseButtonText = "Закрыть",
            XamlRoot = XamlRoot
        };

        _ = dlg.ShowAsync();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var prevValue = SequenceLenght;
        try
        {
            SequenceLenght = int.Parse((sender as TextBox).Text);
        }
        catch
        {
            var dlg = new ContentDialog()
            {
                Title = "Ошибка",
                Content = "Неверно задана длина последовательности",
                CloseButtonText = "Закрыть",
                XamlRoot = XamlRoot
            };

            _ = dlg.ShowAsync();

            SequenceLenght = prevValue;
        }
    }

    private void ManualInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        RawGrammar = (sender as TextBox).Text;
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
    private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

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

    public TreeNode<T> Parent
    {
        get; private set;
    }

    public T Value
    {
        get
        {
            return _value;
        }
    }

    public ReadOnlyCollection<TreeNode<T>> Children
    {
        get
        {
            return _children.AsReadOnly();
        }
    }

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
            child.Traverse(action);
    }

    public void Traverse(Action<T, int> action, int level = 0)
    {
        action(Value, level);
        foreach (var child in _children)
            child.Traverse(action, level + 1);
    }

    public IEnumerable<T> Flatten()
    {
        return new[] { Value }.Concat(_children.SelectMany(x => x.Flatten()));
    }
}