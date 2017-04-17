using System.Windows;
using System.Windows.Input;
using TSS.Another;
using TSS.UniverseLogic;

namespace TSS.Wpf
{
    /// <summary>
    /// The window used to display the fields of the class editor.
    /// <para></para>
    /// Окно используется для отображения редактора полей класса.
    /// </summary>
    public partial class WindowValuesRedactor : Window
    {
        internal static ConstsUniverse ShowModal(ConstsUniverse constsUniverse)
        {
            ValuesRedactor vr = new ValuesRedactor(constsUniverse);
            var wd = new WindowValuesRedactor(vr);
            wd.ShowDialog();
            return null;
        }

        ValuesRedactor valuesRedactor;

        WindowValuesRedactor(ValuesRedactor valuesRedactor)
        {
            InitializeComponent();
            applyButton.Content = LanguageHandler.GetInstance().ApplyButtonText;
            Title = LanguageHandler.GetInstance().WindowTitleOfUniverseConstsRedactor;
            this.valuesRedactor = valuesRedactor;
            valuesRedactor.CreateControlsForObject(stackPanel);
            stackPanel.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                    TryToConfirm();
            };
        }
        void TryToConfirm()
        {
            if (valuesRedactor.ConfirmChanges())
            {
                (valuesRedactor.ObjectWithValues as ConstsUniverse).SaveToFile();
                Close();
            }
        }

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            TryToConfirm();
        }

        
    }



    

}
