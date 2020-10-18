using irf_6gy.Entities;
using irf_6gy.MnbServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace irf_6gy
{

    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();
        BindingList<string> Currencies = new BindingList<string>();
        public Form1()
        {
            InitializeComponent();
            getExchangeRates();
            RefreshData();
            comboBox1.Text = "EUR";
        }

        private void getExchangeRates()
        {
            var mnbService = new MNBArfolyamServiceSoapClient();
            var request = new GetCurrenciesRequestBody()
            {

            };
            var response = mnbService.GetCurrencies(request);
            var result = response.GetCurrenciesResult;
            var xml = new XmlDocument();
            xml.LoadXml(result);
            int sorszam = 0; //ha ide 3-at írnék, akkor a HUF-t be se olvassa
            while (sorszam+2<xml.DocumentElement.InnerText.Length) //data-nál alkalmazott foreach csak egy elemet számít, HUF után kiugrik a ciklusból, ez végigmegy a teljes listán, még ha nem is szép megoldás
            {
                string curr;
                curr = (xml.DocumentElement.InnerText[sorszam]).ToString()+(xml.DocumentElement.InnerText[sorszam+1]).ToString()+(xml.DocumentElement.InnerText[sorszam+2]).ToString();
                Currencies.Add(curr);
                sorszam+=3;
            }
            comboBox1.DataSource = Currencies;
        }

        private void RefreshData()
        {
            Rates.Clear();
            var mnbService = new MNBArfolyamServiceSoapClient();
            var request = new GetExchangeRatesRequestBody()
            {
                currencyNames = comboBox1.SelectedItem.ToString(),
                startDate = dateTimeStart.Value.ToString(),
                endDate = dateTimeEnd.Value.ToString()
            };
            var response = mnbService.GetExchangeRates(request);
            var result = response.GetExchangeRatesResult;
            newXml(result);
            dataGridView1.DataSource = Rates;
            newChart();
        }

        void newXml(string result)
        {
            var xml = new XmlDocument();
            xml.LoadXml(result);
            foreach (XmlElement element in xml.DocumentElement)
            {
                var childElement = (XmlElement)element.ChildNodes[0];
                if (childElement == null)
                    continue;
                var rate = new RateData();
                Rates.Add(rate);
                rate.Date = DateTime.Parse(element.GetAttribute("date"));
                rate.Currency = childElement.GetAttribute("curr");
                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0) rate.Value = value / unit;
            }
        }
        void newChart()
        {
            chartRateData.DataSource = Rates;

            var series = chartRateData.Series[0];
            series.ChartType = SeriesChartType.Line;
            series.XValueMember = "Date";
            series.YValueMembers = "Value";
            series.BorderWidth = 2;

            var legend = chartRateData.Legends[0];
            legend.Enabled = false;

            var chartArea = chartRateData.ChartAreas[0];
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisY.IsStartedFromZero = false;
        }

        private void dateTimeStart_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void dateTimeEnd_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
