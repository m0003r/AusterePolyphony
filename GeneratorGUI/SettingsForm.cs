using System.Windows.Forms;
using Compositor.Rules.Base;

namespace GeneratorGUI
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            
            var rules = RuleAttribute.GetAllRules();
        }
    }
}
