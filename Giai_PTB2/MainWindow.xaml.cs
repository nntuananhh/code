using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Giai_PTB2;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void SolveButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryParseCoefficient(InputA, "a", out double a) ||
            !TryParseCoefficient(InputB, "b", out double b) ||
            !TryParseCoefficient(InputC, "c", out double c))
        {
            return;
        }

        if (Math.Abs(a) < 1e-12)
        {
            DisplayResult("-", "Hệ số a phải khác 0 để là phương trình bậc 2.");
            return;
        }

        double delta = b * b - 4 * a * c;
        DeltaTextBlock.Text = delta.ToString("F3", CultureInfo.InvariantCulture);

        if (delta < 0)
        {
            SolutionTextBlock.Text = "Phương trình vô nghiệm trong số thực.";
        }
        else if (Math.Abs(delta) < 1e-9)
        {
            double x = -b / (2 * a);
            SolutionTextBlock.Text = $"Phương trình có nghiệm kép: x = {x:F3}.";
        }
        else
        {
            double sqrtDelta = Math.Sqrt(delta);
            double x1 = (-b + sqrtDelta) / (2 * a);
            double x2 = (-b - sqrtDelta) / (2 * a);
            SolutionTextBlock.Text = $"Phương trình có hai nghiệm phân biệt:\n" +
                                      $"x₁ = {x1:F3}\n" +
                                      $"x₂ = {x2:F3}";
        }
    }

    private bool TryParseCoefficient(TextBox textBox, string name, out double value)
    {
        value = 0;
        string text = textBox.Text.Trim();

        if (string.IsNullOrEmpty(text))
        {
            DisplayResult("-", $"Vui lòng nhập hệ số {name}.");
            return false;
        }

        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            DisplayResult("-", $"Hệ số {name} phải là số hợp lệ.");
            return false;
        }

        return true;
    }

    private void DisplayResult(string deltaText, string solutionMessage)
    {
        DeltaTextBlock.Text = deltaText;
        SolutionTextBlock.Text = solutionMessage;
    }

    private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        int selectionStart = textBox.SelectionStart;
        int selectionLength = textBox.SelectionLength;
        string currentText = textBox.Text;
        string proposedText = currentText.Remove(selectionStart, selectionLength).Insert(selectionStart, e.Text);

        e.Handled = !IsValidNumericInput(proposedText);
    }

    private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (!e.DataObject.GetDataPresent(typeof(string)))
        {
            e.CancelCommand();
            return;
        }

        string pastedText = (string)e.DataObject.GetData(typeof(string))!;

        if (sender is not TextBox textBox)
        {
            e.CancelCommand();
            return;
        }

        int selectionStart = textBox.SelectionStart;
        int selectionLength = textBox.SelectionLength;
        string currentText = textBox.Text;
        string proposedText = currentText.Remove(selectionStart, selectionLength).Insert(selectionStart, pastedText);

        if (!IsValidNumericInput(proposedText))
        {
            e.CancelCommand();
        }
    }

    private static bool IsValidNumericInput(string text)
    {
        if (string.IsNullOrEmpty(text) || text == "-" || text == "." || text == "-.")
        {
            return true;
        }

        return Regex.IsMatch(text, "^(-)?\\d*(\\.\\d*)?$");
    }
}
