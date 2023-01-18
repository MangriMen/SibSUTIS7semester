using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Text;
using kp.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using kp.Models;
using kp.Helpers;
using CourseWork.Models.RegularExpression;
using CourseWork.Helpers;

namespace kp.Views;

public sealed partial class MainPage : Page, INotifyPropertyChanged
{
    public static string StaticChains
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

    private List<string> _chains = new();
    private List<string> Chains
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

    private string _symbol = "";
    private string Symbol
    {
        get => _symbol;
        set
        {
            _symbol = value;
        }
    }

    private int _symbolMultiplicity = 1;
    private string SymbolMultiplicity
    {
        get => _symbolMultiplicity.ToString();
        set => _symbolMultiplicity = int.Parse(value != string.Empty ? value : "1");
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
    }

    private void StartClick(object sender, RoutedEventArgs e)
    {
    }

    private void GenerateAndStartClick(object sender, RoutedEventArgs e)
    {
    }


    private string _rawRegularExpression = "";
    private string RawRegularExpression
    {
        get => _rawRegularExpression;
        set
        {
            _rawRegularExpression = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RawRegularExpression)));
        }
    }

    private RegularExpression _regularExpression = new();


    private void RawRegularExpressionTextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        try
        {
            var condition = !Regex.IsMatch(sender.Text[sender.SelectionStart - 1].ToString(), @"[a-z0-9]|\(|\)|\*|\+|\.");
            Utils.RevertTextBoxEnteredSymbol(sender, condition);
        }
        catch { }
    }

    private RegularExpression GetParsedRegularExpression(string regularExpression)
    {
        var counter = 0;
        var scope = new RegularExpression();
        var lastScope = scope;
        var stack = new Stack<int>();
        var prevScopeStack = new Stack<RegularExpression>();

        prevScopeStack.Push(scope);

        var symbolRegex = new Regex(@"\(|\)|\*|\+|\.");

        while (counter < regularExpression.Length)
        {
            switch (regularExpression[counter])
            {
                case '(':
                    {
                        stack.Push(counter);
                        prevScopeStack.Push(scope);
                        scope = new RegularExpression();
                        lastScope = scope;
                        break;
                    }
                case '.':
                    {
                        scope.type = RegularExpression.Type.Multiply;
                        break;
                    }
                case '+':
                    {
                        scope.type = RegularExpression.Type.Select;
                        break;
                    }
                case ')':
                    {
                        var startIndex = stack.Pop();
                        scope.raw = regularExpression[startIndex..(counter + 1)];
                        var prevScope = prevScopeStack.Pop();
                        prevScope.tokens.Add(scope);
                        lastScope = scope;
                        scope = prevScope;
                        break;
                    }
                case '*':
                    {
                        lastScope.raw += '*';
                        lastScope.isLoop = true;
                        break;
                    }
                case var symbol when !symbolRegex.IsMatch(symbol.ToString()):
                    {
                        scope.tokens.Add(new RegularExpressionToken(regularExpression[counter]));
                        break;
                    }
            }
            counter++;
        }
        scope.raw = regularExpression;
        return scope;
    }

    private List<string> VisitRegularExpression(RegularExpressionToken regularExpression, int _)
    {
        return new() { regularExpression.value.ToString() };
    }

    private List<string> VisitRegularExpression(RegularExpression regularExpression, int currentSequenceLength = 0)
    {
        var uncompleted = new List<string>();
        var completed = new List<string>();

        if (regularExpression.isLoop)
        {
            completed.Add("");
        }

        var length = regularExpression.isLoop ? (SequenceLengthMax + 1 - currentSequenceLength) : 1;

        switch (regularExpression.type)
        {
            case RegularExpression.Type.Multiply:
                for (var i = 0; i < length; i++)
                {
                    foreach (dynamic token in regularExpression.tokens)
                    {
                        var temp = (List<string>)VisitRegularExpression(token, uncompleted.Count);

                        uncompleted = uncompleted.EachWithEach(temp).ToList();
                    }
                    completed = completed.Union(uncompleted).ToList();
                }
                break;
            case RegularExpression.Type.Select:

                var arr = new List<string>();
                foreach (dynamic token in regularExpression.tokens)
                {
                    var temp = (List<string>)VisitRegularExpression(token, uncompleted.Count);

                    arr = arr.Union(temp).ToList();
                }

                for (var i = 0; i < length; i++)
                {
                    uncompleted = uncompleted.EachWithEach(arr).ToList();
                    completed = completed.Union(uncompleted).ToList();
                }
                break;
        }
        completed = completed.Union(uncompleted).ToList();
        return completed;
    }

    private List<string> GenerateSequences(RegularExpression regularExpression)
    {
        var chains = VisitRegularExpression(regularExpression);
        var matchingChains = chains.Where(chain => chain.Length >= SequenceLengthMin && chain.Length <= SequenceLengthMax).ToList();
        matchingChains = matchingChains.OrderBy(x => x.Length).ThenBy(x => x).ToList();
        return matchingChains;
    }


    public void ParseRegularExpression()
    {
        _regularExpression = GetParsedRegularExpression(RawRegularExpression);
        var generatedChains = GenerateSequences(_regularExpression).ToArray();
        Chains = generatedChains.ToList();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Chains)));
    }

    private void StartButtonClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ParseRegularExpression();
    }

    private void AlphabetChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        try
        {
            var condition = !Regex.IsMatch(sender.Text[sender.SelectionStart - 1].ToString(), @"[a-z0-9]|,|\s")
                || Regex.IsMatch(sender.Text, @"[a-z0-9]{2,}|[a-z0-9]\s|^,+|^\s+|,,|,\s+,|\s{2,}")
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

    private void SymbolChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        try
        {
            var condition = !_alphabet.Contains(sender.Text[sender.SelectionStart - 1].ToString()) && sender.SelectionLength < 1;
            Utils.RevertTextBoxEnteredSymbol(sender, condition);
        }
        catch { }
    }

    public static async void SaveChainsToFile()
    {
        var savePicker = new FileSavePicker();
        savePicker.InitializeWithWindow(App.MainWindow);

        savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
        savePicker.SuggestedFileName = "output";

        var path = await savePicker.PickSaveFileAsync();
        await File.WriteAllTextAsync(path.Path, StaticChains);
    }
}
