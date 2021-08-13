using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.DotNetBar.Charts;
using DevComponents.DotNetBar.Charts.Style;

namespace S10Cuantificacion.Cantidades
{
    public partial class FrmVerElementos : Form
    {

        #region Private Data

        private Timer _Timer;

        private NwData[] _NwData = new NwData[] {
            new NwData("Armazón estructural", 16),
            new NwData("Bandeja de cables", 40),
            new NwData("Barandillas", 19),
            new NwData("Cimentación estructural", 83),
            new NwData("Condiciones de contorno", 30),
            new NwData("Conductos", 18),
            new NwData("Conductos flexibles", 22),
            new NwData("Conexiones estructurales", 77),
            new NwData("Cubiertas", 122),
            new NwData("Elementos de detalle", 19),
            new NwData("Emplazamiento", 28),
            new NwData("Equipos especializados", 28),
            new NwData("Escaleras", 6),
            new NwData("Mallazo de refuerzo estructural", 7),
            new NwData("Modelos genericos", 13),
            new NwData("Montantes de muro cortina", 23),
            new NwData("Muros", 37),
            new NwData("Paneles de muro cortina", 18),
            new NwData("Patrón", 56),
            new NwData("Perfiles", 122),
            new NwData("Pilares estructurales", 46),
            new NwData("Rampas", 46),
            new NwData("Sistemas de conductos", 46),
            new NwData("Sistemas de muro cortina", 46),
            new NwData("Sistemas de tuberías", 46),
            new NwData("Sistemas de vigas estructurales", 46),
            new NwData("Suelos", 46),
            new NwData("Símbolos de anotación", 46),
            new NwData("Techos", 46),
            new NwData("Tuberías", 46),
            new NwData("Tuberías flexibles", 46),
            new NwData("Tubos", 46),
            new NwData("Vículos analíticos", 46),
            new NwData("Áreas de mallazo estructural", 46),
        };

        #endregion
        public FrmVerElementos()
        {
            InitializeComponent();
            

        }



        #region InitializeChart

        /// <summary>
        /// Initializes our chart.
        /// </summary>
        private void InitializeChart()
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            AddSeries(pieChart);
        }

        #region AddSeries

        /// <summary>
        /// Creates a new series from the given data.
        /// </summary>
        /// <param name="chartXy"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="color"></param>
        private void AddSeries(PieChart pieChart)
        {
            PieSeries series = new PieSeries("NwPieSeries");

            // Set our format string to display the country name (X), followed
            // by the 'angular' percentage of the pie (VAL or V format code).

            string format = "{x} - {v:0.0#%}";

            // Iterate through each data element, creating and adding a new
            // PieSeriesPoint for each respective entry.

            foreach (NwData data in _NwData)
            {
                PieSeriesPoint psp = new PieSeriesPoint(data.Country, data.Count);

                psp.LegendText = format;

                series.SeriesPoints.Add(psp);
            }

            // Get a reference to the control generated "Other" slice, and
            // establish some defaults for its dislpay and behavior.

            PieSeriesPoint opsp = series.GetOtherPieSeriesPoint();

            // Legend related.

            opsp.LegendText = format;
            opsp.LegendItem.ChartLegendItemVisualStyles.Default.TextColor = Color.Green;

            // Setup some Outer label styles for the 'Default' state.

            SliceOuterLabelVisualStyle ostyle =
                opsp.SliceVisualStyles.Default.SliceOuterLabelStyle;

            ostyle.Border.LineWidth = 1;
            ostyle.Border.LinePattern = LinePattern.Dash;
            ostyle.Border.LineColor = Color.DarkGreen;
            ostyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(4);

            ostyle.TextColor = Color.White;
            ostyle.Background = new Background(Color.Green);

            // Setup some Outer label styles for the 'MouseOver' state.

            ostyle = opsp.SliceVisualStyles.MouseOver.SliceOuterLabelStyle;

            ostyle.TextColor = Color.Black;
            ostyle.Background = new Background(Color.GreenYellow);

            // Add the series to the chart.

            pieChart.ChartSeries.Add(series);
        }

        #endregion

        #endregion

        #region InitializeComboItems

        /// <summary>
        /// Initializes our ComboBox items and establishes the
        /// appropriate defaults for them.
        /// </summary>
        private void InitializeComboItems()
        {
            cbxPalette.Items.AddRange(Enum.GetNames(typeof(PaletteGroup)));
            cbxPalette.Items.Remove("Custom");
            cbxPalette.SelectedItem = "NotSet";

            intMaxSlices.Value = 100;
            intMinPercent.Value = 0;

            sliderInnerRadius.Value = 40;
            sliderOuterRadius.Value = 70;
        }

        #endregion

        #region chartControl1_PieSelectionChanged

        /// <summary>
        /// Handles PieSelectionChanged events.
        /// 
        /// We will use this event to display the user selections in the
        /// inner Pie center area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chartControl1_PieSelectionChanged(object sender, PieSelectionChangedEventArgs e)
        {
            PieChart pieChart = e.PieChart;

            _Timer.Stop();

            // Get a list of the currently selected items, and set the
            // center label to their concatenated value.

            List<PieSeriesPoint> list = pieChart.GetSelectedPoints();

            if (list != null && list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (PieSeriesPoint psp in list)
                    sb.AppendLine((string)psp.ValueX);

                pieChart.CenterLabel = sb.ToString();
                TextBoxX1.Text = sb.ToString();
            }
            else
            {
                // Nothing selected. Show that fact, but do so only
                // for a a short period of time (1-1/2 seconds).
                TextBoxX1.Text = "";
                pieChart.CenterLabel = "Nothing selected";

                _Timer.Start();
            }
        }

        #endregion

        #region sliderInnerRadius_ValueChanged

        /// <summary>
        /// Handles changes to the chart's InnerRadius.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliderInnerRadius_ValueChanged(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            // If the chart's PieRadiusScale property is set to Percentage, or
            // the radius is less than 1, then the value will be taken as a
            // relative percentage of the content size of the chart (which will
            // scale accordingly as the chart grows or shrinks in size).

            // If the chart's PieRadiusScale property is set to Pixel, or the radius is
            // greater than 1, then the value will be taken as an absolute pixel value
            // (which will remain constant when the chart size shrinks or grows).

            // In this example we are specifying the radius as a
            // percentage, with a limit of 95%.

            double radius = (double)sliderInnerRadius.Value / 100;

            pieChart.InnerRadius = Math.Min(radius, .95);
        }

        #endregion

        #region sliderOuterRadius_ValueChanged

        /// <summary>
        /// Handles changing the pie chart's Outer radius.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliderOuterRadius_ValueChanged(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            // If the chart's PieRadiusScale property is set to Percentage, or
            // the radius is less than 1, then the value will be taken as a
            // relative percentage of the content size of the chart (which will
            // scale accordingly as the chart grows or shrinks in size).

            // If the chart's PieRadiusScale property is set to Pixel, or the radius is
            // greater than 1, then the value will be taken as an absolute pixel value
            // (which will remain constant when the chart size shrinks or grows).

            // In this example we are specifying the radius as a
            // percentage, with a limit of 95%.

            double radius = (double)sliderOuterRadius.Value / 100;

            pieChart.OuterRadius = Math.Min(radius, .95);
        }

        #endregion

        #region sliderStartAngle_ValueChanged

        /// <summary>
        /// Handles changing the chart's starting angle. The default is 270.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliderStartAngle_ValueChanged(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            pieChart.SubSliceVisualLayout.StartAngle = sliderStartAngle.Value;
        }

        #endregion

        #region sliderSweepAngle_ValueChanged

        /// <summary>
        /// Handles changing the chart's Sweep Angle. Default is 360.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliderSweepAngle_ValueChanged(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            pieChart.SubSliceVisualLayout.SweepAngle = sliderSweepAngle.Value;
        }

        #endregion

        #region cbxCenterSlice_CheckedChanged

        /// <summary>
        /// Handles changing whether the first slice is centered on the
        /// StartAngle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxCenterSlice_CheckedChanged(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            pieChart.CenterFirstSlice =
                (cbxCenterSlice.Checked == true) ? Tbool.True : Tbool.False;
        }

        #endregion

        #region cbxClockwise_CheckedChanged

        /// <summary>
        /// Handles changing the SweepDirection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxClockwise_CheckedChanged(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            pieChart.SubSliceVisualLayout.SweepDirection =
                (cbxClockwise.Checked ? SweepDirection.Clockwise : SweepDirection.CounterClockwise);
        }

        #endregion

        #region cbxPalette_SelectedIndexChanged

        /// <summary>
        /// Handles changing the chart's Palette Group.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            pieChart.PaletteGroup = (PaletteGroup)
                Enum.Parse(typeof(PaletteGroup), (string)cbxPalette.SelectedItem);
        }

        #endregion

        #region cbxDragDetach_CheckedChanged

        /// <summary>
        /// Handles changing the DragDetach enable state.  With EnableDragDetach
        /// set to true, the user can click and drag individual slices away from
        /// the pie center.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDragDetach_CheckedChanged(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            pieChart.EnableDragDetach = (cbxDragDetach.Checked ? Tbool.True : Tbool.False);

            // If either DragDetach or ShiftExplode is enabled, then set the label visibility
            // to SliceDetach (ie only detached slices will have their associated labels displayed).
            // Otherwise, tell the chart to always display its labels.

            if (cbxDragDetach.Checked || cbxShiftExplode.Checked)
                pieChart.SubSliceVisualLayout.SliceLabelVisibility = SliceLabelVisibility.SliceDetach;
            else
                pieChart.SubSliceVisualLayout.SliceLabelVisibility = SliceLabelVisibility.Always;
        }

        #endregion

        #region cbxShiftExplode_CheckedChanged

        /// <summary>
        /// Handles changing the ShiftExplode state.  With EnableShiftDragExplode
        /// set to true, the user can click and drag on any slices to explode the
        /// pie from the pie center.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxShiftExplode_CheckedChanged(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            pieChart.EnableShiftDragExplode = cbxShiftExplode.Checked;

            // If either DragDetach or ShiftExplode is enabled, then set the label visibility
            // to SliceDetach (ie only detached slices will have their associated labels displayed).
            // Otherwise, tell the chart to always display its labels.

            if (cbxDragDetach.Checked || cbxShiftExplode.Checked)
                pieChart.SubSliceVisualLayout.SliceLabelVisibility = SliceLabelVisibility.SliceDetach;
            else
                pieChart.SubSliceVisualLayout.SliceLabelVisibility = SliceLabelVisibility.Always;
        }

        #endregion

        #region intMaxSlices_ValueChanged

        /// <summary>
        /// Handles changes to the number of MaxSlices.
        /// 
        /// MaxSlices value determines how many slices are presented in the pie.  If
        /// the 'Other' slice is visible, then the slices are placed in the 'Other'
        /// slice.  If not, then they are simply not displayed.  The slices are
        /// accumulated / counted as they are encounterd in the defined collection.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void intMaxSlices_ValueChanged(object sender, EventArgs e)
        {
            PieChart pieChart = (PieChart)chartControl1.ChartPanel.ChartContainers[0];
            PieSeries series = pieChart.ChartSeries[0];

            pieChart.SubSliceVisualLayout.MaxSlices = intMaxSlices.Value;

            // Only enable the 'ShowOther' checkbox if MinPercent has been set
            // to a non-zero value by the user.

            cbxShowOther.Enabled =
                (pieChart.SubSliceVisualLayout.MinPercent > 0 ||
                 pieChart.SubSliceVisualLayout.MaxSlices < series.SeriesPoints.Count);
        }

        #endregion

        #region intMinPercent_ValueChanged

        /// <summary>
        /// Handles changing the chart MinPercent value.
        /// 
        /// MinPercent is utilized only when the 'Other' slice is enabled.  If
        /// a slice value (relative angular value calculated with respect to the pie total),
        /// is less than the MinPercent, then it will be moved into the 'Other' slice.  If the
        /// 'Other' slice is not enabled, then the MinPercent value is ignored.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void intMinPercent_ValueChanged(object sender, EventArgs e)
        {
            PieChart pieChart = (PieChart)chartControl1.ChartPanel.ChartContainers[0];
            PieSeries series = pieChart.ChartSeries[0];

            pieChart.SubSliceVisualLayout.MinPercent = (double)intMinPercent.Value / 100;

            // Only enable the 'ShowOther' checkbox if MinPercent has been set
            // to a non-zero value by the user.

            cbxShowOther.Enabled =
                (pieChart.SubSliceVisualLayout.MinPercent > 0 ||
                 pieChart.SubSliceVisualLayout.MaxSlices < series.SeriesPoints.Count);
        }

        #endregion

        #region cbxShowOther_CheckedChanged

        /// <summary>
        /// Handles changing the ShowOtherSlice state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxShowOther_CheckedChanged(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            pieChart.ShowOtherSlice = (cbxShowOther.Checked ? Tbool.True : Tbool.False);
        }

        #endregion

        #region Timer_Tick

        /// <summary>
        /// Handles a simple delayed presentation of info to the
        /// user in the pie center label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Timer_Tick(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            pieChart.CenterLabel = null;
        }

        #endregion

        #region NwData Class def

        /// <summary>
        /// Simple data class.
        /// </summary>
        private class NwData
        {
            public string Country;
            public int Count;

            public NwData(string country, int count)
            {
                Country = country;
                Count = count;
            }
        }

        #endregion

        private void GrupoContenido_Click(object sender, EventArgs e)
        {

        }

        private void chartControl1_PieSelectionChanged_1(object sender, PieSelectionChangedEventArgs e)
        {
            
        }

        private void FrmVerElementos_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            timer1.Enabled = false;
            InitializeChart();
            this.sliderInnerRadius.ValueChanged += new System.EventHandler(this.sliderInnerRadius_ValueChanged);
            this.sliderStartAngle.ValueChanged += new System.EventHandler(this.sliderStartAngle_ValueChanged);
            this.sliderSweepAngle.ValueChanged += new System.EventHandler(this.sliderSweepAngle_ValueChanged);
            this.cbxCenterSlice.CheckedChanged += new System.EventHandler(this.cbxCenterSlice_CheckedChanged);
            this.cbxClockwise.CheckedChanged += new System.EventHandler(this.cbxClockwise_CheckedChanged);
            this.cbxPalette.SelectedIndexChanged += new System.EventHandler(this.cbxPalette_SelectedIndexChanged);
            this.cbxDragDetach.CheckedChanged += new System.EventHandler(this.cbxDragDetach_CheckedChanged);
            this.cbxShiftExplode.CheckedChanged += new System.EventHandler(this.cbxShiftExplode_CheckedChanged);
            this.intMaxSlices.ValueChanged += new System.EventHandler(this.intMaxSlices_ValueChanged);
            this.cbxShowOther.CheckedChanged += new System.EventHandler(this.cbxShowOther_CheckedChanged);
            this.sliderOuterRadius.ValueChanged += new System.EventHandler(this.sliderOuterRadius_ValueChanged);
            this.intMinPercent.ValueChanged += new System.EventHandler(this.intMinPercent_ValueChanged);


            InitializeComboItems();

            // Allocate and initialize a timer to use for displaying
            // temp information in the Pie Center area.

            _Timer = new Timer();

            _Timer.Interval = 1500;
            _Timer.Tick += Timer_Tick;

            // Hook the PieSelectionChanged event so that we can give the user
            // feedback on what items they have selected.

            chartControl1.PieSelectionChanged += chartControl1_PieSelectionChanged;
        }

        private void ButtonItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(AppDomain.CurrentDomain.BaseDirectory.ToString(), "");
        }

        private void ButtonItem18_Click(object sender, EventArgs e)
        {
            chromiumWebBrowser1.LoadUrl("www.google.com");
            chromiumWebBrowser1.Refresh();
            chromiumWebBrowser1.Text = "hola";
            MessageBox.Show("hecho", "");
        }
    }
}
