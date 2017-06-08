using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;

using OxyPlot.WindowsForms;
using OxyPlot;
using OxyPlot.Series;
using System.IO;

using OMathParser.Syntax;

namespace OMathPlotter.Forms
{
    public partial class PlotForm : Form
    {
        public class OpenXMLExpression
        {
            public string sourceXml;
            public SyntaxTree expressionTree;
        }

        private PlotView plotView;

        private List<OpenXMLExpression> parsedExpressions;

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

            String myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openOXMLFileDialog.InitialDirectory = myDocumentsPath;
            openOXMLFileDialog.Filter = "Office Open XML Wordprocessing document (*.docx)|*.docx";
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

        private void extractMathExpressions(WordprocessingDocument doc)
        {
            
        }

        private void openFileMenuItem_Click(object sender, EventArgs e)
        {
            if (openOXMLFileDialog.ShowDialog() == DialogResult.OK
                && !String.IsNullOrEmpty(openOXMLFileDialog.FileName))
            {
                try
                {
                    using (var doc = WordprocessingDocument.Open(openOXMLFileDialog.FileName, false))
                    {
                        extractMathExpressions(doc);
                    }
                }
                catch (Exception ex)
                {
                    String fileName = Path.GetFileName(openOXMLFileDialog.FileName);
                    string title = "Error opening file";
                    string message = "Couldn't open \"" + fileName + "\"";
                    if (ex is OpenXmlPackageException || ex is FileFormatException)
                    {
                        message += "\nThe file isn't a valid Open XML WordprocessingDocument";
                    }

                    MessageBoxButtons button = MessageBoxButtons.OK;
                    MessageBoxIcon icon = MessageBoxIcon.Error;

                    MessageBox.Show(this, message, title, button, icon);
                }
            }
        }


    }
}
