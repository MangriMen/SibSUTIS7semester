using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using lab1.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using System;

namespace lab1.Views;

public sealed partial class MainPage : Page, INotifyPropertyChanged
{
    private string RawGrammar
    {
        get; set;
    } = "S: (S) | ()S | ->";

    private const string START_NON_TERMINATE_SYMBOL = "S";
    private const string EMPTY_SYMBOL = "->";
    private const int MAX_RECURSION = 10000;

    private int _sequenceLengthMin = 0;
    private int _sequenceLengthMax = 1;

    private int SequenceLengthMin
    {
        get => _sequenceLengthMin;
        set
        {
            _sequenceLengthMin = value;
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(SequenceLengthMin)));
        }
    }

    private int SequenceLengthMax
    {
        get => _sequenceLengthMax;
        set
        {
            _sequenceLengthMax = value;
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(SequenceLengthMax)));
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
            var chains = GenerateSequences(_grammar, SequenceLengthMin, SequenceLengthMax);

            var chainsOutput = new StringBuilder();
            foreach (var chain in chains)
            {
                var line = chain;
                //var line = Regex.Replace(string.Join("", sequence.Value), @"[A-Z]", "");
                if (line.EndsWith(EMPTY_SYMBOL) && chain.Length >= SequenceLengthMin && chain.Length <= SequenceLengthMax)
                {
                    chainsOutput.AppendLine(line.Replace(EMPTY_SYMBOL, ""));
                }
            }

            Output.Text = chainsOutput.ToString();
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

    public static List<string> GenerateSequences(Dictionary<string, List<string>> grammar, int sequenceLengthMin, int sequenceLengthMax)
    {
        var chains = new List<string>();
        var uncompletedChains = new List<string>();

        foreach (var ruleSequence in grammar[START_NON_TERMINATE_SYMBOL])
        {
            var isValid = Is_valid(ruleSequence, grammar, sequenceLengthMin, sequenceLengthMax);
            if (isValid == 0 && ruleSequence.Length >= sequenceLengthMin)
            {
                chains.Add(ruleSequence);
            }
            else if (isValid == -2)
            {
                uncompletedChains.Add(ruleSequence);
            }
        }

        var tempChain = new StringBuilder();
        var count = 0;
        while (count < MAX_RECURSION && uncompletedChains.Count > 0)
        {
            var newUncompletedChains = new List<string>();
            count++;
            foreach (var uncompletedChain in uncompletedChains)
            {
                tempChain.Clear();
                for (var i = 0; i < uncompletedChain.Length; i++)
                {
                    if (!grammar.ContainsKey(uncompletedChain[i].ToString()))
                    {
                        tempChain.Append(uncompletedChain[i]);
                    }
                    else
                    {
                        foreach (var ruleSequence in grammar[uncompletedChain[i].ToString()])
                        {
                            var res = string.Concat(tempChain.ToString(), ruleSequence, uncompletedChain.AsSpan(i + 1));

                            var isValid = Is_valid(res, grammar, sequenceLengthMin, sequenceLengthMax);
                            if (isValid == 0)
                            {
                                if (chains.Contains(res == "" ? "" : res) || res.Length < sequenceLengthMin)
                                {
                                    break;
                                }
                                else
                                {
                                    chains.Add(res == "" ? "" : res);
                                }
                            }
                            else if (isValid == -2)
                            {
                                newUncompletedChains.Add(res);
                            }
                        }
                        break;
                    }
                }
            }
            uncompletedChains.Clear();
            uncompletedChains = newUncompletedChains.Distinct().ToList();
        }

        return chains;
    }

    public static bool IsCompletedSequence(string sequence)
    {
        return !Regex.Match(sequence, @"[A-Z]").Success;
    }

    private static int Is_valid(string line, Dictionary<string, List<string>> grammar, int min_chain_length, int max_chain_length)
    {
        int term_sym = 0;
        int non_term_sym = 0;
        foreach (char ch in line)
        {
            if (!grammar.ContainsKey(ch.ToString()))
            {
                term_sym++;
            }
            else
            {
                non_term_sym++;
            }
        }

        if (term_sym > max_chain_length)
        {
            return -1;
        }
        if ((term_sym + non_term_sym - 5) > max_chain_length)
        {
            return -1;
        }
        return (non_term_sym > 0) ? -2 : 0;
    }

    //public static Dictionary<int, List<string>> GenerateSequences(Dictionary<string, List<string>> grammar, int sequenceLength)
    //{
    //    var root = new TreeNode<string>(START_NON_TERMINATE_SYMBOL);
    //    root.AddChildren(grammar[root.Value].ToArray());
    //    FillTree(root, grammar, sequenceLength);

    //    Dictionary<int, List<string>> sequences = new();
    //    var prevSequence = new List<string>();
    //    var prevLevel = -1;
    //    var prevIndex = 0;
    //    var index = 0;
    //    root.Traverse(new Action<string, int>((nodeValue, level) =>
    //    {
    //        sequences.TryAdd(index, new());

    //        if (level <= prevLevel)
    //        {
    //            prevIndex = index;
    //            index++;
    //            prevSequence = sequences[prevIndex].ToList();
    //            prevSequence.RemoveAt(sequences[prevIndex].Count - (prevLevel - level + 1));
    //            sequences.TryAdd(index, prevSequence);
    //        }

    //        sequences[index].Add(nodeValue);

    //        prevLevel = level;
    //    }));

    //    return sequences;
    //}

    //public static void FillTree(TreeNode<string> root, Dictionary<string, List<string>> grammar, int sequenceLength, int count = 0)
    //{
    //    if (count >= sequenceLength)
    //    {
    //        return;
    //    }

    //    foreach (var node in root.Children)
    //    {
    //        if (node.Value.Contains(EMPTY_SYMBOL))
    //        {
    //            return;
    //        }
    //        var rules = Array.FindAll(Regex.Split(node.Value, @"[^A-Z]"), rl => !string.IsNullOrEmpty(rl));
    //        foreach (var rule in rules)
    //        {
    //            node.AddChildren(grammar[rule].ToArray());
    //            FillTree(node, grammar, sequenceLength, ++count);
    //        }
    //    }
    //}

    private void ManualInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        RawGrammar = ((TextBox)sender).Text;
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