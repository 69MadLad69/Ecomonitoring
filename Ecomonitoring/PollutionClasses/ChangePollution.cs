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
    public partial class ChangePollution : Form
    {
        Form1 mainForm;
        DataGridViewRow selectedRow;
        public ChangePollution(Form1 mainForm, DataGridViewRow selectedRow)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.selectedRow = selectedRow;

            PollutantName.Text = CheckForNull(selectedRow.Cells[1].Value);
            SubstanceName.Text = CheckForNull(selectedRow.Cells[2].Value);            
            PollutionCa.Text = CheckForNull(selectedRow.Cells[3].Value);
            PollutionRoi.Text = CheckForNull(selectedRow.Cells[4].Value);
            PollutionT.Text = CheckForNull(selectedRow.Cells[5].Value);
            PollutionAmmount.Text = CheckForNull(selectedRow.Cells[6].Value);
            PollutionYear.Text = CheckForNull(selectedRow.Cells[11].Value);

            foreach (Pollutant pol in mainForm.pollutantRepository.Pollutants) {
                PollutantName.Items.Add(pol.name);
            }

            foreach (Substance sub in mainForm.substanceRepository.Substances) { 
                SubstanceName.Items.Add(sub.name);
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

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            if (PollutantName.Text != "" && SubstanceName.Text != "")
            {
                if (mainForm.isDoubleParsable(PollutionAmmount.Text) && mainForm.isDoubleParsable(PollutionCa.Text)
                    && mainForm.isDoubleParsable(PollutionRoi.Text) && mainForm.isDoubleParsable(PollutionT.Text))
                {
                    int polRepPollutantId = mainForm.pollutantRepository.FindPollutantId(PollutantName.Text);
                    int polRepSubId = mainForm.substanceRepository.FindSubstanceId(SubstanceName.Text);

                    double Ca = double.Parse(PollutionCa.Text, NumberStyles.Any);
                    double roi = double.Parse(PollutionRoi.Text, NumberStyles.Any);
                    double T = double.Parse(PollutionT.Text, NumberStyles.Any);
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

                    double pollutionAmmount = double.Parse(PollutionAmmount.Text, NumberStyles.Any);

                    double taxYearAmmount = pollutionAmmount * polRepSub.taxAmmount;

                    selectedRow.Cells[1].Value = PollutantName.Text;
                    selectedRow.Cells[2].Value = SubstanceName.Text;
                    selectedRow.Cells[3].Value = PollutionCa.Text;
                    selectedRow.Cells[4].Value = PollutionRoi.Text;
                    selectedRow.Cells[5].Value = PollutionT.Text;
                    selectedRow.Cells[6].Value = PollutionAmmount.Text;
                    selectedRow.Cells[7].Value = excessiveMass.ToString();
                    selectedRow.Cells[8].Value = compensationAmmount.ToString();
                    selectedRow.Cells[10].Value = taxYearAmmount.ToString();
                    selectedRow.Cells[11].Value = PollutionYear.Text;

                    mainForm.PollutionTable.Refresh();
                    mainForm.UpdatePollutions();
                    mainForm.pollutionReportRepository.UpdatePollutionReportsInDB();


                    mainForm.UpdateCRTable();
                    mainForm.UpdateNCRTable();

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
