using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Text;
using kp.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace kp.Views;

public sealed partial class MainPage : Page, INotifyPropertyChanged
{
    private string _rawGrammar = "S: (S) | ()S |";
    private string RawGrammar
    {
        get => _rawGrammar;
        set
        {
            _rawGrammar = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RawGrammar)));
        }
    }

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

    private List<string> _alphabet = new();

    public string _alphabetStr = "";
    public string Alphabet
    {
        get => _alphabetStr;
        set
        {
            _alphabetStr = value;
            _alphabet = value.Replace(" ", "").Split(",").Distinct().ToList().FindAll(item => item.Length == 1);
            _alphabet.Sort();
        }
    }

    public string StartChain
    {
        get; set;
    } = "";

    public string EndChain
    {
        get; set;
    } = "";

    public string ChainMultiplicity
    {
        get; set;
    } = "1";

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
            var chainMultiplicityInt = int.Parse(ChainMultiplicity != "" ? ChainMultiplicity : "1");
            chains = chains.FindAll(chain => (chain.Length % chainMultiplicityInt) == 0);

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

    private void GenerateClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        _grammar.Clear();
        if (Alphabet.Length == 0 || Alphabet[0] == ',')
        {
            return;
        }

        RawGrammar = "";

        var ruleSymbols = new List<char>() { START_NON_TERMINATE_SYMBOL[0], 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'O', 'P', 'Q', 'R', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        if (StartChain.Length + 1 + EndChain.Length > ruleSymbols.Count)
        {
            _ = new ContentDialog()
            {
                Title = "Ошибка",
                Content = new TextBlock() { TextWrapping = TextWrapping.Wrap, Text = "Общая длина начальной и конечной цепочек должна быть меньше 26" },
                CloseButtonText = "Закрыть",
                XamlRoot = XamlRoot,
            }.ShowAsync();
            return;
        }

        var startChainCompareIndex = -1;
        for (var i = EndChain.Length; i >= 0; i--)
        {
            if (StartChain.EndsWith(EndChain[..i]))
            {
                startChainCompareIndex = i;
                break;
            }
        }

        var currentRule = 0;
        if (StartChain.Length > 0)
        {
            foreach (var symbol in StartChain)
            {
                _grammar[ruleSymbols[currentRule].ToString()] = new() { $"{symbol}{ruleSymbols[++currentRule]}" };
            }
        }

        var startChainEndRule = currentRule - 1;

        _grammar[ruleSymbols[currentRule].ToString()] = new();
        foreach (var symbol in _alphabet)
        {
            _grammar[ruleSymbols[currentRule].ToString()].Add($"{symbol}{ruleSymbols[currentRule]}");
        }

        if (EndChain.Length > 0)
        {
            foreach (var symbol in _alphabet)
            {
                _grammar[ruleSymbols[currentRule].ToString()].Add($"{symbol}{ruleSymbols[currentRule + 1]}");
            }
        }
        else
        {
            _grammar[ruleSymbols[startChainEndRule].ToString()].Add($"{StartChain[^1]}");
            foreach (var symbol in _alphabet)
            {
                _grammar[ruleSymbols[currentRule].ToString()].Add($"{symbol}");
            }
        }
        currentRule++;

        if (StartChain.Length > 0)
        {
            if (EndChain.Length > 0)
            {
                _grammar[ruleSymbols[startChainEndRule].ToString()].Add($"{StartChain[^1]}{ruleSymbols[currentRule]}");
            }
        }
        else
        {
            _grammar[ruleSymbols[0].ToString()].Add($"{EndChain[0]}{(EndChain.Length > 1 ? ruleSymbols[currentRule + 1] : "")}");
        }

        if (EndChain.Length > 0)
        {
            for (var i = 0; i < EndChain.Length; i++)
            {
                var symbol = EndChain[i];
                _grammar[ruleSymbols[currentRule].ToString()] = new() { $"{symbol}{ruleSymbols[++currentRule]}" };
                if (i < startChainCompareIndex)
                {
                    var startChainIndex = StartChain.Length - startChainCompareIndex + i;
                    if (i != EndChain.Length - 1)
                    {
                        _grammar[ruleSymbols[startChainIndex].ToString()].Add($"{symbol}{ruleSymbols[currentRule]}");
                    }
                }
            }
        }

        foreach (var rule in _grammar)
        {
            RawGrammar += $"{rule.Key}: ";
            foreach (var sequence in rule.Value)
            {
                RawGrammar += $"{sequence} | ";
            }
            RawGrammar = RawGrammar.Remove(RawGrammar.Length - 2, 2);
            RawGrammar += "\n";
        }
        if (RawGrammar.Length >= 3 && EndChain.Length > 0)
        {
            RawGrammar = RawGrammar.Remove(RawGrammar.Length - 3, 3);
        }
        else if (RawGrammar.Length >= 2)
        {
            RawGrammar = RawGrammar.Remove(RawGrammar.Length - 2, 2);
        }
    }

    private void StartOrEndChainChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        var prevSelectionPos = sender.SelectionStart;

        if (sender.Text.Length == 0)
        {
            return;
        }

        if (!_alphabet.Contains(sender.Text[sender.SelectionStart - 1].ToString()))
        {
            sender.Text = sender.Text.Remove(sender.SelectionStart - 1, 1);
            sender.SelectionStart = prevSelectionPos - 1;
            return;
        }
    }

    private void AlphabetChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        var prevSelectionPos = sender.SelectionStart;

        if (sender.Text.Length == 0)
        {
            return;
        }

        if (!Regex.IsMatch(sender.Text[sender.SelectionStart - 1].ToString(), "[a-z]|,|\\s")
            || Regex.IsMatch(sender.Text, "[a-z]{2,}")
            || Regex.IsMatch(sender.Text, "[a-z]\\s")
            || Regex.IsMatch(sender.Text, ",,|,\\s+,|\\s{2,}")
            || _alphabet.Contains(sender.Text[sender.SelectionStart - 1].ToString()))
        {
            sender.Text = sender.Text.Remove(sender.SelectionStart - 1, 1);
            sender.SelectionStart = prevSelectionPos - 1;
            return;
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
