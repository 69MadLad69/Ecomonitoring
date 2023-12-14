using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ecomonitoring.RisksCalasses.NonCarcinogenicRisk
{
    public partial class ChangeNCR : Form
    {
        Form1 mainForm;
        DataGridViewRow selectedRow;
        public ChangeNCR(Form1 mainForm, DataGridViewRow selectedRow)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.selectedRow = selectedRow;

            NonCarcinogenicRisk OldNCR = mainForm.NCRrepository.GetNCR((int)selectedRow.Cells[0].Value);
            NCR_PollutionId.Text = OldNCR.pollution_Id.ToString();
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            if (mainForm.isIntParsable(NCR_PollutionId.Text))
            {
                int PollutionId = int.Parse(NCR_PollutionId.Text);
                if (mainForm.pollutionReportRepository.GetPollution_Report(PollutionId) != null)
                {
                    int SubstanceID = mainForm.pollutionReportRepository.GetPollution_Report(PollutionId).Substance_Id;

                    if (mainForm.substanceRepository.GetSubstance(SubstanceID).cancerogenic == 1)
                    {
                        MessageBox.Show("Речовина в обраному забрудненні є канцерогенною!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        double CA = mainForm.pollutionReportRepository.GetPollution_Report(PollutionId).CA;
                        double RfC = mainForm.substanceRepository.GetSubstance(SubstanceID).RfC;

                        double HQ = CA * RfC;

                        NonCarcinogenicRisk NCR = new NonCarcinogenicRisk((int)selectedRow.Cells[0].Value, PollutionId, HQ);

                        if (mainForm.isNCRUnique(NCR))
                        {
                            mainForm.NCRrepository.UpdateNonCarcinogenicRiskInDB(NCR);
                            mainForm.NCRrepository.LoadNonCarcinogenicRisksFromDatabase();
                            mainForm.UpdateNCRTable();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Ризик з наданим ID чи з наданим ID Забруднення вже існує в БД!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("ID Забруднення має бути цілим числом!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
