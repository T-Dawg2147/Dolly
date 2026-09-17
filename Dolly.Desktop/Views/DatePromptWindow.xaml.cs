using System.Windows;

namespace Dolly.Desktop.Views;

public partial class DatePromptWindow : Window
{
    public DateTime? SelectedDate { get; private set; }

    public DatePromptWindow(string title, string message)
    {
        InitializeComponent();
        Title = title;
        MessageText.Text = message;
        DatePickerControl.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (DatePickerControl.SelectedDate is null)
        {
            MessageBox.Show("Please choose a date.", "Date required", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        SelectedDate = DatePickerControl.SelectedDate;
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}