using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OxyPlot.WindowsForms;
using OxyPlot;
using OxyPlot.Series;

namespace OMathPlotter.Forms
{
    public partial class PlotForm : Form
    {
        private PlotView plotView;

        public PlotForm()
        {
            InitializeComponent();
            InitializePlotView();
            plot();
        }

        private void InitializePlotView()
        {
            this.plotView = new PlotView();
            SuspendLayout();

            this.plotView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotView.Location = new System.Drawing.Point(0, 0);
            this.plotView.Margin = new System.Windows.Forms.Padding(0);
            this.plotView.TabIndex = 0;

            plotTab.Controls.Add(this.plotView);

            ResumeLayout(true);
        }

        private void plot()
        {
            var pm = new PlotModel
            {
                Title = "Trigonometric functions",
                Subtitle = "Example using the FunctionSeries",
                PlotType = PlotType.Cartesian,
                Background = OxyColors.White
            };

            pm.Series.Add(new FunctionSeries(Math.Sin, -10, 10, 0.1, "sin(x)"));
            pm.Series.Add(new FunctionSeries(Math.Cos, -10, 10, 0.1, "cos(x)"));
            pm.Series.Add(new FunctionSeries(t => 5 * Math.Cos(t), t => 5 * Math.Sin(t), 0, 2 * Math.PI, 0.1, "cos(t),sin(t)"));
            this.plotView.Model = pm;
        }
    }
}
