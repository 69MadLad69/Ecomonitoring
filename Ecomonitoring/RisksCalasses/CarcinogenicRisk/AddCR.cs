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
    public partial class AddCR : Form
    {
        Form1 mainForm;
        public AddCR(Form1 mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            int newCRId = 1;

            if (mainForm.CRRepository.CarcinogenicRisks.Count != 0)
            {
                newCRId = mainForm.CRRepository.CarcinogenicRisks[mainForm.CRRepository.CarcinogenicRisks.Count - 1].riskId + 1;
            }

            if (NewCR_PollutionId.Text != "" && mainForm.isIntParsable(NewCR_PollutionId.Text))
            {
                if (mainForm.isDoubleParsable(NewCR_TimeOut.Text) 
                    && mainForm.isDoubleParsable(NewCR_TimeIn.Text)
                    && mainForm.isDoubleParsable(NewCR_Vout.Text)
                    && mainForm.isDoubleParsable(NewCR_Vin.Text)
                    && mainForm.isDoubleParsable(NewCR_EF.Text)
                    && mainForm.isDoubleParsable(NewCR_ED.Text)
                    && mainForm.isDoubleParsable(NewCR_BW.Text))
                {

                    if (mainForm.pollutionReportRepository.GetPollution_Report(int.Parse(NewCR_PollutionId.Text)) != null) {

                        int SubstanceID = mainForm.pollutionReportRepository.GetPollution_Report(int.Parse(NewCR_PollutionId.Text)).Substance_Id;

                        if (mainForm.substanceRepository.GetSubstance(SubstanceID).cancerogenic == 0)
                        {
                            MessageBox.Show("Речовина в обраному забрудненні не є канцерогенною!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            double CA = mainForm.pollutionReportRepository.GetPollution_Report(int.Parse(NewCR_PollutionId.Text)).CA;
                            double CH = 1.0 * CA;
                            double TimeOut = double.Parse(NewCR_TimeOut.Text, NumberStyles.Any);
                            double TimeIn = double.Parse(NewCR_TimeIn.Text, NumberStyles.Any);
                            double Vout = double.Parse(NewCR_Vout.Text, NumberStyles.Any);
                            double Vin = double.Parse(NewCR_Vin.Text, NumberStyles.Any);
                            double EF = double.Parse(NewCR_EF.Text, NumberStyles.Any);
                            double ED = double.Parse(NewCR_ED.Text, NumberStyles.Any);
                            double BW = double.Parse(NewCR_BW.Text, NumberStyles.Any);

                            double LADD = ((CA * TimeOut * Vout) + (CH * TimeIn * Vin)) * EF * ED / (BW * 70 * 365);
                            double SF = mainForm.substanceRepository.GetSubstance(SubstanceID).SF;
                            double CR = LADD * SF * Math.Pow(10, 4);

                            CarcinogenicRisk NewCR = new CarcinogenicRisk(
                                newCRId, int.Parse(NewCR_PollutionId.Text), TimeOut, TimeIn, Vout,
                                Vin, EF, ED, BW, LADD, CR);

                            if (mainForm.isCRUnique(NewCR))
                            {
                                mainForm.CRRepository.AddNewCarcinogenicRiskToDB(NewCR);
                                mainForm.CRRepository.LoadCarcinogenicRisksFromDatabase();
                                mainForm.UpdateCRTable();
                                this.Close();
                            }
                            else {
                                MessageBox.Show("Такий ризик вже існує в системі!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else {
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

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
