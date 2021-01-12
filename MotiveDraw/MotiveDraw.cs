using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotiveDraw.Controls;

namespace MotiveDraw
{
    public partial class MotiveDraw : Form
    {
	    private ToolsControl _toolControl;
	    private ProjectControl _projectControl;

        public MotiveDraw()
        {
            InitializeComponent();

            _toolControl = new ToolsControl
            {
                Dock = DockStyle.Fill
            };
            this.splitContainer.Panel1.Controls.Add(_toolControl);
            _toolControl.Show();

            _projectControl = new ProjectControl
            {
	            Dock = DockStyle.Fill
            };
            this.splitContainer.Panel2.Controls.Add(_projectControl);
            _projectControl.Show();
        }
    }
}
