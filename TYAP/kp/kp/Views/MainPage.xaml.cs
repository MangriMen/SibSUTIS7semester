using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Text;
using kp.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using System.Runtime.InteropServices;
using WinRT;

namespace kp.Views;

public sealed partial class MainPage : Page, INotifyPropertyChanged
{
    private const char RULE_SEPARATOR = ':';
    private const char SEQUENCE_SEPARATOR = '|';
    private const string START_NON_TERMINATE_SYMBOL = "S";
    private const int MAX_RECURSION = 10000;

    private readonly List<char> ruleSymbols = new() {
        START_NON_TERMINATE_SYMBOL[0],
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
        'I', 'J', 'K', 'L', 'M', 'O', 'P', 'Q',
        'R', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
    };

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

    public string SubChain
    {
        get; set;
    } = "";

    private int _chainMultiplicity = 1;
    public string ChainMultiplicity
    {
        get => _chainMultiplicity.ToString();
        set => _chainMultiplicity = int.Parse(value != string.Empty ? value : "1");
    }

    public int SelectedDirectionIndex
    {
        get; set;
    } = 1;

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

    private void StartClick(object sender, RoutedEventArgs e)
    {
        try
        {
            _grammar = ParseGrammar(RawGrammar);

            var chains = GenerateSequences(_grammar, SequenceLengthMin, SequenceLengthMax);

            var chainsOutput = chains.Where(chain => chain.Length >= SequenceLengthMin && chain.Length <= SequenceLengthMax);

            Chains = string.Join('\n', chainsOutput);
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

    private static string BuildSequence(int direction, string symbol, char rule)
    {
        if (direction == 0)
        {
            return $"{rule}{symbol}";
        }
        else
        {
            return $"{symbol}{rule}";
        }
    }

    private void GenerateClick(object sender, RoutedEventArgs e)
    {
        _grammar.Clear();
        if (Alphabet.Length == 0)
        {
            return;
        }

        RawGrammar = "";

        if (SubChain.Length + 2 > ruleSymbols.Count)
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

        var currentRule = 0;

        /* Chain begin rules */

        if (SubChain.Length == 0)
        {
            if (_chainMultiplicity == 1)
            {
                _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                foreach (var symbol in _alphabet)
                {
                    _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SelectedDirectionIndex, symbol, ruleSymbols[currentRule]));
                }
            }
            else
            {
                for (var i = 0; i < _chainMultiplicity - 1; i++)
                {
                    _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                    foreach (var symbol in _alphabet)
                    {
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SelectedDirectionIndex, symbol, ruleSymbols[currentRule + 1]));
                    }
                    currentRule++;
                }

                _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                foreach (var symbol in _alphabet)
                {
                    _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SelectedDirectionIndex, symbol, ruleSymbols[0]));
                }
            }

            _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
            foreach (var symbol in _alphabet)
            {
                _grammar[ruleSymbols[currentRule].ToString()].Add($"{symbol}");
            }
        }
        else
        {
            if (_chainMultiplicity == 1)
            {
                _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                foreach (var symbol in _alphabet)
                {
                    _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SelectedDirectionIndex, symbol, ruleSymbols[currentRule]));
                }

                /* Jump to sub-chain rules */
                foreach (var symbol in _alphabet)
                {
                    _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SelectedDirectionIndex, symbol, ruleSymbols[currentRule + 1]));
                }
                currentRule++;

                /* Obligatory sub-chain rules */
                foreach (var symbol in SubChain)
                {
                    _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                    _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SelectedDirectionIndex, symbol.ToString(), ruleSymbols[++currentRule]));
                }

                /* Sequences for only sub-chain chain */
                if (SubChain.Length > 0)
                {
                    _grammar[ruleSymbols[0].ToString()].Add(BuildSequence(SelectedDirectionIndex, SubChain[0].ToString(), ruleSymbols[2]));
                    _grammar[ruleSymbols[currentRule - 1].ToString()].Add($"{SubChain[^1]}");
                }

                /* Chain end rules */
                _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                foreach (var symbol in _alphabet)
                {
                    _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SelectedDirectionIndex, symbol, ruleSymbols[currentRule]));
                }
                foreach (var symbol in _alphabet)
                {
                    _grammar[ruleSymbols[currentRule].ToString()].Add($"{symbol}");
                }
            }
            else
            {
                var isSubChainMultiple = (SubChain.Length % _chainMultiplicity) == 0;
                var diff = _chainMultiplicity - SubChain.Length;
                var startChainLength = _chainMultiplicity - 1;
                var endChainLength = _chainMultiplicity - 1;

                /* Chain begin rules - 1 */
                for (var i = 0; i < startChainLength; i++)
                {
                    _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                    foreach (var symbol in _alphabet)
                    {
                        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SelectedDirectionIndex, symbol, ruleSymbols[currentRule + 1]));
                    }
                    currentRule++;
                }

                /* Jump to start rule when begin chain is multiple */
                _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                foreach (var symbol in _alphabet)
                {
                    _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SelectedDirectionIndex, symbol, ruleSymbols[0]));
                }
                currentRule++;

                if (!isSubChainMultiple && diff == 0)
                {
                    _grammar.TryAdd(ruleSymbols[0].ToString(), new());
                    foreach (var symbol in _alphabet)
                    {
                        _grammar[ruleSymbols[0].ToString()].Add(BuildSequence(SelectedDirectionIndex, $"ФФ{symbol}", ruleSymbols[currentRule]));
                    }
                }

                /* Jump to sub-chain rules */
                var jumpRule = ((_chainMultiplicity - SubChain.Length - 1) < 0 && isSubChainMultiple) ? 0 : Convert.ToInt32(MathF.Abs(_chainMultiplicity - SubChain.Length));
                _grammar.TryAdd(ruleSymbols[jumpRule].ToString(), new());
                if (SubChain.Length > 1)
                {
                    _grammar[ruleSymbols[jumpRule].ToString()].Add(BuildSequence(SelectedDirectionIndex, $"{SubChain[0]}", ruleSymbols[_chainMultiplicity]));
                }
                else
                {
                    //_grammar[ruleSymbols[0].ToString()].Add(BuildSequence(SelectedDirectionIndex, $"{SubChain[0]}", ruleSymbols[_chainMultiplicity]));
                    _grammar[ruleSymbols[jumpRule].ToString()].Add($"{SubChain[0]}");
                }

                /* Obligatory sub-chain rules */
                if (SubChain.Length > 1)
                {
                    foreach (var symbol in SubChain[1..^1])
                    {
                        _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                        _grammar[ruleSymbols[currentRule++].ToString()].Add(BuildSequence(SelectedDirectionIndex, $"{symbol}", ruleSymbols[currentRule]));
                    }
                    _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                    _grammar[ruleSymbols[currentRule].ToString()].Add($"{SubChain[^1]}");
                }

                ///* Chain end rules */
                //_grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                //_grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SelectedDirectionIndex, SubChain[^1].ToString(), ruleSymbols[++currentRule]));

                //for (var i = 0; i < endChainLength; i++)
                //{
                //    _grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                //    foreach (var symbol in _alphabet)
                //    {
                //        _grammar[ruleSymbols[currentRule].ToString()].Add(BuildSequence(SelectedDirectionIndex, symbol, ruleSymbols[currentRule + 1]));
                //    }
                //    currentRule++;
                //}
                //_grammar.TryAdd(ruleSymbols[currentRule].ToString(), new());
                //foreach (var symbol in _alphabet)
                //{
                //    _grammar[ruleSymbols[currentRule].ToString()].Add($"{symbol}");
                //}
            }
        }

        //foreach (var rule in _grammar)
        //{
        //    _grammar[rule.Key] = rule.Value.Distinct().ToList();
        //}

        /* Build grammar */
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
        var prevSelectionPos = sender.SelectionStart;

        if (sender.Text.Length == 0)
        {
            return;
        }

        if (!Regex.IsMatch(sender.Text[sender.SelectionStart - 1].ToString(), @"[a-z]|,|\s")
            || Regex.IsMatch(sender.Text, @"[a-z]{2,}|[a-z]\s|^,+|^\s+|,,|,\s+,|\s{2,}")
            || _alphabet.Contains(sender.Text[sender.SelectionStart - 1].ToString()))
        {
            sender.Text = sender.Text.Remove(sender.SelectionStart - 1, 1);
            sender.SelectionStart = prevSelectionPos - 1;
            return;
        }
    }

    private void SubChainChanging(TextBox sender, TextBoxTextChangingEventArgs args)
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

    [ComImport]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithWindow
    {
        void Initialize(IntPtr hwnd);
    }
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
    internal interface IWindowNative
    {
        IntPtr WindowHandle
        {
            get;
        }
    }

    public static async void SaveGrammarToFile()
    {
        var savePicker = new FileSavePicker();

        var hwnd = App.MainWindow.As<IWindowNative>().WindowHandle;

        var initializeWithWindow = savePicker.As<IInitializeWithWindow>();
        initializeWithWindow.Initialize(hwnd);

        savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
        savePicker.SuggestedFileName = "OutputGrammar";

        var path = await savePicker.PickSaveFileAsync();

        await File.WriteAllTextAsync(path.Path, Grammar);
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
