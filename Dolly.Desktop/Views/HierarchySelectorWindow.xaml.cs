using System.Windows;

namespace Dolly.Desktop.Views;

public partial class HierarchySelectorWindow : Window
{
    public string? SelectedLeafId { get; private set; }

    public HierarchySelectorWindow()
    {
        InitializeComponent();
    }

    private void OkButton_OnClick(object sender, RoutedEventArgs e)
    {
        SelectedLeafId = string.IsNullOrWhiteSpace(LeafIdTextBox.Text)
            ? null
            : LeafIdTextBox.Text.Trim();

        if (SelectedLeafId is null)
            return;

        DialogResult = true;
    }
}