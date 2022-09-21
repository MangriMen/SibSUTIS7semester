using System.ComponentModel;
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
    } = "S: (S) | ()S |";

    private string _chains = "";
    private string Chains
    {
        get => _chains;
        set
        {
            _chains = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Chains)));
        }
    }

    private const string START_NON_TERMINATE_SYMBOL = "S";
    private const int MAX_RECURSION = 10000;

    private int _sequenceLengthMin = 0;
    private int _sequenceLengthMax = 2;

    private int SequenceLengthMin
    {
        get => _sequenceLengthMin;
        set
        {
            _sequenceLengthMin = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SequenceLengthMin)));
        }
    }

    private int SequenceLengthMax
    {
        get => _sequenceLengthMax;
        set
        {
            _sequenceLengthMax = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SequenceLengthMax)));
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
                if (chain.Length >= SequenceLengthMin && chain.Length <= SequenceLengthMax)
                {
                    chainsOutput.AppendLine(chain);
                }
            }

            Chains = chainsOutput.ToString();
        }
        catch (GrammarException)
        {
            _ = new ContentDialog()
            {
                Title = "Ошибка",
                Content = "Грамматика введена неверно, проверьте правильность ввода.",
                CloseButtonText = "Закрыть",
                XamlRoot = XamlRoot
            }.ShowAsync();
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
                                if (chains.Contains(res) || res.Length < sequenceLengthMin)
                                {
                                    break;
                                }
                                else
                                {
                                    chains.Add(res);
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

    private static int Is_valid(string line, Dictionary<string, List<string>> grammar, int min_chain_length, int max_chain_length)
    {
        var term_sym = 0;
        var non_term_sym = 0;
        foreach (var ch in line)
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