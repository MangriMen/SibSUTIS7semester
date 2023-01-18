using Microsoft.UI.Xaml.Controls;

namespace kp.Helpers;
public static class Utils
{
    public static void RevertTextBoxEnteredSymbol(TextBox sender, bool condition)
    {
        var prevSelectionPos = sender.SelectionStart;

        if (sender.Text.Length == 0)
        {
            return;
        }

        if (condition)
        {
            sender.Text = sender.Text.Remove(sender.SelectionStart - 1, 1);
            sender.SelectionStart = prevSelectionPos - 1;
        }
    }
}
