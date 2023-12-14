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

namespace Ecomonitoring.RisksCalasses.CarcinogenicRisk
{
    public partial class ChangeCR : Form
    {
        Form1 mainForm;
        DataGridViewRow selectedRow;
        public ChangeCR(Form1 mainForm, DataGridViewRow selectedRow)
        {
            InitializeComponent();
            this.mainForm = mainForm; this.selectedRow = selectedRow;

            CarcinogenicRisk OldCR = mainForm.CRRepository.GetCarcinogenicRisk((int)selectedRow.Cells[0].Value);

            CR_PollutionId.Text = OldCR.pollution_Id.ToString();
            CR_TimeOut.Text = OldCR.TimeOut.ToString();
            CR_TimeIn.Text = OldCR.TimeIn.ToString();
            CR_Vout.Text = OldCR.Vout.ToString();
            CR_Vin.Text = OldCR.Vin.ToString();
            CR_EF.Text = OldCR.EF.ToString();
            CR_ED.Text = OldCR.ED.ToString();
            CR_BW.Text = OldCR.BodyWeight.ToString();
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            if (CR_PollutionId.Text != "" && mainForm.isIntParsable(CR_PollutionId.Text))
            {
                if (mainForm.isDoubleParsable(CR_TimeOut.Text)
                    && mainForm.isDoubleParsable(CR_TimeIn.Text)
                    && mainForm.isDoubleParsable(CR_Vout.Text)
                    && mainForm.isDoubleParsable(CR_Vin.Text)
                    && mainForm.isDoubleParsable(CR_EF.Text)
                    && mainForm.isDoubleParsable(CR_ED.Text)
                    && mainForm.isDoubleParsable(CR_BW.Text))
                {

                    if (mainForm.pollutionReportRepository.GetPollution_Report(int.Parse(CR_PollutionId.Text)) != null)
                    {

                        int SubstanceID = mainForm.pollutionReportRepository.GetPollution_Report(int.Parse(CR_PollutionId.Text)).Substance_Id;

                        if (mainForm.substanceRepository.GetSubstance(SubstanceID).cancerogenic == 0)
                        {
                            MessageBox.Show("Речовина в обраному забрудненні не є канцерогенною!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            double CA = mainForm.pollutionReportRepository.GetPollution_Report(int.Parse(CR_PollutionId.Text)).CA;
                            double CH = 1.0 * CA;
                            double TimeOut = double.Parse(CR_TimeOut.Text, NumberStyles.Any);
                            double TimeIn = double.Parse(CR_TimeIn.Text, NumberStyles.Any);
                            double Vout = double.Parse(CR_Vout.Text, NumberStyles.Any);
                            double Vin = double.Parse(CR_Vin.Text, NumberStyles.Any);
                            double EF = double.Parse(CR_EF.Text, NumberStyles.Any);
                            double ED = double.Parse(CR_ED.Text, NumberStyles.Any);
                            double BW = double.Parse(CR_BW.Text, NumberStyles.Any);

                            double LADD = ((CA * TimeOut * Vout) + (CH * TimeIn * Vin)) * EF * ED / (BW * 70 * 365);
                            double SF = mainForm.substanceRepository.GetSubstance(SubstanceID).SF;
                            double CR = LADD * SF;

                            CarcinogenicRisk NewCR = new CarcinogenicRisk(
                                (int)selectedRow.Cells[0].Value,
                                int.Parse(CR_PollutionId.Text),
                                TimeOut,
                                TimeIn,
                                Vout,
                                Vin,
                                EF,
                                ED,
                                BW,
                                LADD,
                                CR);

                            if (mainForm.isCRDataUnique(NewCR))
                            {
                                mainForm.CRRepository.UpdateCarcinogenicRiskInDB(NewCR);
                                mainForm.CRRepository.LoadCarcinogenicRisksFromDatabase();
                                mainForm.UpdateCRTable();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Такий ризик вже існує в системі!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Забруднення з наданим ID немає в БД!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Усі записи мають бути числами!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("ID Забруднення має бути цілим числом та не бути пустим!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
