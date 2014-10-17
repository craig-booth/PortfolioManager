using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PortfolioManager.Test
{
    public partial class frmSettings : Form
    {
        public PortfolioManagerSettings Settings { get; private set; }

        public frmSettings()
        {
            InitializeComponent();

            Settings = null;
        }

        public frmSettings(PortfolioManagerSettings settings)
            : this()
        {
            Settings = settings;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }
    }
}
