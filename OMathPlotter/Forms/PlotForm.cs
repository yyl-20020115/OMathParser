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
using OfficeMath = DocumentFormat.OpenXml.Math.OfficeMath;

using OxyPlot.WindowsForms;
using OxyPlot;
using OxyPlot.Series;
using System.IO;

using OMathParser.Tokens;
using OMathParser.Syntax;
using OMathParser.Utils;
using OxyPlot.Wpf;

namespace OMathPlotter.Forms
{
    public partial class PlotForm : Form
    {
        public class OpenXMLExpression
        {
            public OpenXMLExpression(OfficeMath source, SyntaxTree builtUpExpression)
            {
                this.sourceElement = source;
                this.expressionTree = builtUpExpression;
            }

            public OfficeMath sourceElement;
            public SyntaxTree expressionTree;
        }

        private PlotView plotView;
        private ParseProperties parseProperties;
        private TokenTreeBuilder tokenTreeBuilder;
        private SyntaxTreeBuilder syntaxTreeBuilder;
        private List<OpenXMLExpression> parsedExpressions;

        public PlotForm()
        {
            InitializeComponent();
            InitializePlotView();

            this.parseProperties = new ParseProperties();
            this.tokenTreeBuilder = new TokenTreeBuilder(parseProperties);
            this.syntaxTreeBuilder = new SyntaxTreeBuilder(parseProperties);
            this.parsedExpressions = new List<OpenXMLExpression>();
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

            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
            Body docBody = doc.MainDocumentPart.Document.Body;
            List<OfficeMath> mathExpressions = new List<OfficeMath>(docBody.Descendants<OfficeMath>());

            parsedExpressions.Clear();
            foreach (var expression in mathExpressions)
            {
                try
                {
                    var tokenTree = tokenTreeBuilder.build(expression);
                    var syntaxTree = syntaxTreeBuilder.Build(tokenTree);
                    parsedExpressions.Add(new OpenXMLExpression(expression, syntaxTree));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void openFileMenuItem_Click(object sender, EventArgs e)
        {
            if (openOXMLFileDialog.ShowDialog() == DialogResult.OK
                && !string.IsNullOrEmpty(openOXMLFileDialog.FileName))
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
                    string fileName = Path.GetFileName(openOXMLFileDialog.FileName);
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
