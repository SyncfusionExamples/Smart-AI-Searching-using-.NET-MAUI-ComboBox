using SmartAIComboBox.SmartAIComboBox;

namespace SmartAIComboBox
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Add the Syncfusion License in here
            MainPage = new SmartAIComboBoxPage();
        }
    }
}
