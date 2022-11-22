using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Text;
using kp.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using kp.Models;
using kp.Helpers;

namespace kp.Views;

public sealed partial class MainPage : Page, INotifyPropertyChanged
{
    private static readonly char RULE_SEPARATOR = ':';
    private static readonly char SEQUENCE_SEPARATOR = '|';
    private static readonly string START_NON_TERMINATE_SYMBOL = "S";
    private static readonly int MAX_RECURSION = 10000;
    private static readonly List<char> ruleSymbols = new() {
        START_NON_TERMINATE_SYMBOL[0],
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
        'I', 'J', 'K', 'L', 'M', 'O', 'P', 'Q',
        'R', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
    };

    private Dictionary<string, List<string>> _grammar = new();

    public static string Grammar
    {
        get; private set;
    } = "";

    private string _rawGrammar = "";
    private string RawGrammar
    {
        get => _rawGrammar;
        set
        {
            _rawGrammar = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RawGrammar)));
        }
    }
    public static string Grammar
    {
        get; private set;
    } = "";

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

    private int SequenceLengthMin { get; set; } = 0;
    private int SequenceLengthMax { get; set; } = 2;

    private List<string> _alphabet = new();
    private string _alphabetStr = "";
    private string Alphabet
    {
        get => _alphabetStr;
        set
        {
            _alphabetStr = value;
            _alphabet = Regex.Replace(value, @"\s+", "").Split(",").Distinct().ToList().FindAll(item => item.Length == 1);
            _alphabet.Sort();
            foreach (var symbol in SubChain)
    {
                if (!_alphabet.Contains(symbol.ToString()))
        {
                    SubChain = "";
                    break;
        }
    }
        }
    }

    private string _subChain = "";
    private string SubChain
    {
        get => _subChain;
        set
        {
            _subChain = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubChain)));
        }
    }

    public string SubChain
    {
        get; set;
    } = "";

    private int _chainMultiplicity = 1;
    private string ChainMultiplicity
    {
        get => _chainMultiplicity.ToString();
        set => _chainMultiplicity = int.Parse(value != string.Empty ? value : "1");
    }

    private int SelectedDirectionIndex { get; set; } = 1;

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

    private void GenerateClick(object sender, RoutedEventArgs e)
    {
        GenerateRegularGrammar();
    }

    private void StartClick(object sender, RoutedEventArgs e)
    {
        BuildChainsByRegularGrammar();
    }

    private void GenerateAndStartClick(object sender, RoutedEventArgs e)
    {
        GenerateRegularGrammar();
        BuildChainsByRegularGrammar();
    }

    public static Dictionary<string, List<string>> ParseGrammar(string rawGrammar)
    {
        var parsedGrammar = new Dictionary<string, List<string>>();

        var rules = rawGrammar.Replace(" ", "").Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        try
        {
            foreach (var line in rules)
            {
                var tokens = line.Split(RULE_SEPARATOR);
                var rule = tokens[0];
                var sequences = tokens[1].Split(SEQUENCE_SEPARATOR);

                parsedGrammar.TryAdd(rule, new());
                parsedGrammar[rule].AddRange(sequences);
            }
        }
        catch (IndexOutOfRangeException)
        {
            throw new GrammarException("Failed to parse");
        }

        return parsedGrammar;
    }

    private void BuildChainsByRegularGrammar()
    {
        try
        {
            _grammar = ParseGrammar(RawGrammar);

            var chains = GenerateSequences(_grammar, SequenceLengthMin, SequenceLengthMax);
            var chainsOutput = chains.Where(chain => chain.raw.Length >= SequenceLengthMin && chain.raw.Length <= SequenceLengthMax);
            var output = chainsOutput.Select(chain => chain.GetFormattedOutput());

            Chains = string.Join('\n', output);
        }
        catch (GrammarException)
        {
            try
            {
            _ = new ContentDialog()
            {
                Title = "Ошибка",
                Content = "Грамматика введена неверно, проверьте правильность ввода.",
                CloseButtonText = "Закрыть",
                XamlRoot = XamlRoot
            }.ShowAsync();
        }
            catch
            {

    }
        }
    }

    public static List<Chain> GenerateSequences(Dictionary<string, List<string>> grammar, int sequenceLengthMin, int sequenceLengthMax)
    {
        var chains = new List<Chain>();
        var uncompletedChains = new List<Chain>();
        var newUncompletedChains = new List<Chain>();

        foreach (var ruleSequence in grammar[START_NON_TERMINATE_SYMBOL])
        {
            var isValid = Is_valid(ruleSequence, grammar, sequenceLengthMin, sequenceLengthMax);
            if (isValid == 0 && ruleSequence.Length >= sequenceLengthMin)
            {
                chains.Add(new Chain(ruleSequence, new string[] { START_NON_TERMINATE_SYMBOL, ruleSequence }));
            }
            else if (isValid == -2)
            {
                uncompletedChains.Add(new Chain(ruleSequence, new string[] { START_NON_TERMINATE_SYMBOL, ruleSequence }));
            }
        }

        var tempChain = new StringBuilder();
        for (var count = 0; count < MAX_RECURSION && uncompletedChains.Count > 0; count++)
        {
            newUncompletedChains.Clear();
            foreach (var uncompletedChain in uncompletedChains)
            {
                tempChain.Clear();
                for (var i = 0; i < uncompletedChain.raw.Length; i++)
                {
                    if (!grammar.ContainsKey(uncompletedChain.raw[i].ToString()))
                    {
                        tempChain.Append(uncompletedChain.raw[i]);
                    }
                    else
                    {
                        foreach (var ruleSequence in grammar[uncompletedChain.raw[i].ToString()])
                        {
                            var res = new Chain
                            {
                                output = uncompletedChain.output.ToList(),
                                raw = $"{tempChain}{ruleSequence}{uncompletedChain.raw.AsSpan(i + 1)}"
                            };
                            res.output.Add(uncompletedChain.output[^1].Replace(uncompletedChain.raw[i].ToString(), ruleSequence));

                            var isValid = Is_valid(res.raw, grammar, sequenceLengthMin, sequenceLengthMax);
                            if (isValid == 0)
                            {
                                if (chains.Contains(res) || res.raw.Length < sequenceLengthMin)
                                {
                                    break;
                                }

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

    private static string BuildSequence(string symbol, char? rule = null, int direction = -1)
    {
        if (rule == null)
        {
            return $"{symbol}";
        }

        return direction == 0 ? $"{rule}{symbol}" : $"{symbol}{rule}";
    }

    private static string BuildSequence(char symbol, char? rule = null, int direction = -1)
        {
        return BuildSequence(symbol.ToString(), rule, direction);
        }
    }

    private void GenerateRegularGrammar()
    {
        _grammar.Clear();
        if (Alphabet.Length == 0)
        {
            return;
        }

        _grammar.Clear();
        RawGrammar = "";

        if (SubChain.Length + (_chainMultiplicity - 1) * 2 + 1 > ruleSymbols.Count)
        {
            _ = new ContentDialog()
            {
                Title = "Ошибка",
                Content = new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = $"Длина подцепочки при кратности {_chainMultiplicity} должна не больше {ruleSymbols.Count - (_chainMultiplicity - 1) * 2}"
                },
                CloseButtonText = "Закрыть",
                XamlRoot = XamlRoot,
            }.ShowAsync();
            return;
        }

        try
        {

        var currentRule = 0;

        /* Chain begin rules */
            if (SubChain.Length == 0)
            {
                if (_chainMultiplicity == 1)
                {
                    _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
        foreach (var symbol in _alphabet)
        {
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol, ruleSymbols[currentRule], SelectedDirectionIndex));
                    }
                }
                else
                {
                    for (var i = 0; i < _chainMultiplicity - 1; i++)
                    {
                        _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                        foreach (var symbol in _alphabet)
                        {
                            _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol, ruleSymbols[currentRule + 1], SelectedDirectionIndex));
                        }
                        currentRule++;
        }

                    _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                    foreach (var symbol in _alphabet)
        {
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol, ruleSymbols[0], SelectedDirectionIndex));
                    }
                }

                _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
            foreach (var symbol in _alphabet)
            {
                    _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol));
            }
        }
        else
        {
                if (_chainMultiplicity == 1)
                {
                    _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                    foreach (var symbol in _alphabet)
                    {
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol, ruleSymbols[currentRule], SelectedDirectionIndex));
                    }

            /* Jump to sub-chain rules */
            foreach (var symbol in _alphabet)
            {
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol, ruleSymbols[currentRule + 1], SelectedDirectionIndex));
            }
            currentRule++;

            /* Obligatory sub-chain rules */
            foreach (var symbol in SubChain)
            {
                        _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol, ruleSymbols[++currentRule], SelectedDirectionIndex));
            }

            /* Sequences for only sub-chain chain */
            if (SubChain.Length > 0)
            {
                        _grammar[ruleSymbols[0].ToString()].Add(BuildSequence(SubChain[0], ruleSymbols[2], SelectedDirectionIndex));
                        _grammar[ruleSymbols[currentRule - 1].ToString()].Add(BuildSequence(SubChain[^1]));
            }

            /* Chain end rules */
                    _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
            foreach (var symbol in _alphabet)
            {
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol, ruleSymbols[currentRule], SelectedDirectionIndex));
            }
            foreach (var symbol in _alphabet)
            {
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol));
            }
        }
                else
                {
                    var isSubChainMultiple = (SubChain.Length % _chainMultiplicity) == 0;
                    var diff = _chainMultiplicity - SubChain.Length;
                    var startChainLength = _chainMultiplicity - 1;
                    var endChainLength = _chainMultiplicity;

                    /* Chain begin rules - 1 */
                    for (var i = 0; i < startChainLength; i++)
        {
                        _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                        foreach (var symbol in _alphabet)
            {
                            _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol, ruleSymbols[currentRule + 1], SelectedDirectionIndex));
        }
                        currentRule++;
    }
    private void AlphabetChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        var prevSelectionPos = sender.SelectionStart;

                    /* Jump to start rule when begin chain is multiple */
                    _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                    foreach (var symbol in _alphabet)
        {
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol, ruleSymbols[0], SelectedDirectionIndex));
        }
                    currentRule++;

                    if (!isSubChainMultiple && diff == 0)
                    {
                        _grammar.TryAdd(ruleSymbols[0].ToString(), new());
                        foreach (var symbol in _alphabet)
        {
                            _grammar[ruleSymbols[0].ToString()].Add(BuildSequence($"ФФ{symbol}", ruleSymbols[currentRule], SelectedDirectionIndex));
        }
    }

                    /* Jump to sub-chain rules */
                    var jumpRule = ((_chainMultiplicity - SubChain.Length - 1) < 0 && isSubChainMultiple) ? 0 : Convert.ToInt32(MathF.Abs(_chainMultiplicity - SubChain.Length));
                    _grammar.TryAdd(ruleSymbols[jumpRule].ToString(), new());
                    if (SubChain.Length > 1)
    {
                        _grammar[ruleSymbols[jumpRule].ToString()].Add(BuildSequence(SubChain[0], ruleSymbols[_chainMultiplicity], SelectedDirectionIndex));
                    }
                    else
        {
                        _grammar[ruleSymbols[jumpRule].ToString()].Add(BuildSequence(SubChain[0]));
        }

                    /* Obligatory sub-chain rules */
                    if (SubChain.Length > 1)
                    {
                        foreach (var symbol in SubChain[1..^1])
        {
                            _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                            _grammar[ruleSymbols[currentRule++].ToString()].Add(BuildSequence(symbol, ruleSymbols[currentRule], SelectedDirectionIndex));
        }
                        _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SubChain[^1]));
    }

                    /* Chain end rules */
                    if (SubChain.Length > 1)
    {
                        _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SubChain[^1], ruleSymbols[++currentRule], SelectedDirectionIndex));
    }

                    var endLoopRule = currentRule;

                    for (var i = 0; i < endChainLength - 1; i++)
    {
                        _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                        foreach (var symbol in _alphabet)
        {
                            _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol, ruleSymbols[currentRule + 1], SelectedDirectionIndex));
        }
                        currentRule++;
    }

                    _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                    foreach (var symbol in _alphabet)
    {
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(symbol, ruleSymbols[endLoopRule], SelectedDirectionIndex));
                    }
                    currentRule++;

                    foreach (var symbol in _alphabet)
                    {
                        _grammar[ruleSymbols[currentRule - 1].ToString()].Add(BuildSequence(symbol));
                    }
                }
            }
        }
        catch
        {
            _grammar.Clear();
        }

        foreach (var rule in _grammar)
        {
            RawGrammar += $"{rule.Key}: ";
            foreach (var sequence in rule.Value)
            {
                RawGrammar += $"{sequence} | ";
            }
            RawGrammar = $"{RawGrammar[0..^3]}\n";
        }
        if (RawGrammar.Length > 0)
        {
            RawGrammar = RawGrammar[0..^1];
    }
        Grammar = RawGrammar;
}

    private void AlphabetChanging(TextBox sender, TextBoxTextChangingEventArgs args)
{
        try
    {
            var condition = !Regex.IsMatch(sender.Text[sender.SelectionStart - 1].ToString(), @"[a-z]|,|\s")
                || Regex.IsMatch(sender.Text, @"[a-z]{2,}|[a-z]\s|^,+|^\s+|,,|,\s+,|\s{2,}")
                || _alphabet.Contains(sender.Text[sender.SelectionStart - 1].ToString());
            Utils.RevertTextBoxEnteredSymbol(sender, condition);
        }
        catch { }
    }

    private void SubChainChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        try
    {
            var condition = !_alphabet.Contains(sender.Text[sender.SelectionStart - 1].ToString());
            Utils.RevertTextBoxEnteredSymbol(sender, condition);
        }
        catch { }
    }

    public static async void SaveGrammarToFile()
    {
        var savePicker = new FileSavePicker();
        savePicker.InitializeWithWindow(App.MainWindow);

        savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
        savePicker.SuggestedFileName = "OutputGrammar";

        var path = await savePicker.PickSaveFileAsync();
        await File.WriteAllTextAsync(path.Path, Grammar);
    }
}
