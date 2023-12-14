using Ecomonitoring.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ecomonitoring.PollutionClasses
{
    public partial class AddPollution : Form
    {
        Form1 mainForm;
        public AddPollution(Form1 mainForm)
        {
            InitializeComponent();

            this.mainForm = mainForm;

            foreach (Pollutant pol in mainForm.pollutantRepository.Pollutants)
            {
                NewPollutantName.Items.Add(pol.name);
            }

            foreach (Substance sub in mainForm.substanceRepository.Substances)
            {
                NewSubstanceName.Items.Add(sub.name);
            }
        }

        public static string CheckForNull(object value)
        {
            return value != null ? value.ToString() : "";
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            int newPollutionId = 1;

            if (mainForm.pollutionReportRepository.PollutionReports.Count != 0)
            {
                newPollutionId = mainForm.pollutionReportRepository.PollutionReports[mainForm.pollutionReportRepository.PollutionReports.Count - 1].Pollution_Id + 1;
            }

            if (NewPollutantName.Text != "" && NewSubstanceName.Text != "")
            {
                if (mainForm.isDoubleParsable(NewPollutionAmmount.Text) && mainForm.isDoubleParsable(NewPollutionCa.Text)
                    && mainForm.isDoubleParsable(NewPollutionRoi.Text) && mainForm.isDoubleParsable(NewPollutionT.Text))
                {
                    int polRepPollutantId = mainForm.pollutantRepository.FindPollutantId(NewPollutantName.Text);
                    int polRepSubId = mainForm.substanceRepository.FindSubstanceId(NewSubstanceName.Text);

                    double Ca = double.Parse(NewPollutionCa.Text, NumberStyles.Any);
                    double roi = double.Parse(NewPollutionRoi.Text, NumberStyles.Any);
                    double T = double.Parse(NewPollutionT.Text, NumberStyles.Any);
                    Substance polRepSub = mainForm.substanceRepository.GetSubstance(polRepSubId);
                    Pollutant polRepPollutant = mainForm.pollutantRepository.GetPollutant(polRepPollutantId);

                    double excessiveMass = 3.6 * Math.Pow(10, -6) * (roi - polRepSub.roNorm) * polRepSub.qv * T;

                    if (excessiveMass < 0)
                    {
                        excessiveMass = 0;
                    }

                    double Ai = 1 / polRepSub.TLK;
                    double Kt = polRepPollutant.Knas * polRepPollutant.Kf;
                    double Kzi = Ca / polRepSub.TLK;
                    double P = double.Parse(mainForm.MinPaymentAmmount.Text);

                    double compensationAmmount = excessiveMass * 1.1 * P * Ai * Kt * Kzi;

                    double pollutionAmmount = double.Parse(NewPollutionAmmount.Text, NumberStyles.Any);

                    double taxYearAmmount = pollutionAmmount * polRepSub.taxAmmount;

                    mainForm.pollutionReportRepository.AddNewPollutionReportToDB(
                        new Pollution_Report(
                            newPollutionId, polRepPollutantId, polRepSubId, pollutionAmmount, Ca, PollutionYear.Text,
                            T, excessiveMass, compensationAmmount, taxYearAmmount, roi));

                    mainForm.pollutionReportRepository.LoadPollutionReportsFromDatabase();
                    mainForm.UpdatePollutionTable();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Записи в полях з ручним вводом мають бути числами!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Назва речовини та підприємства не має бути пустою!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
