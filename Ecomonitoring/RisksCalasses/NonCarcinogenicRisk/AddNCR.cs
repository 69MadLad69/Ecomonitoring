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
    public partial class AddNCR : Form
    {
        Form1 mainForm;
        public AddNCR(Form1 mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            int NewNCRId = 1;

            if (mainForm.NCRrepository.NonCarcinogenicRisks.Count != 0)
            {
                NewNCRId = mainForm.NCRrepository.NonCarcinogenicRisks[mainForm.NCRrepository.NonCarcinogenicRisks.Count - 1].riskId + 1;
            }

            if (mainForm.isIntParsable(NewNCR_PollutionId.Text))
            {
                int PollutionId = int.Parse(NewNCR_PollutionId.Text);
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

                        NonCarcinogenicRisk NCR = new NonCarcinogenicRisk(NewNCRId, PollutionId, HQ);

                        if (mainForm.isNCRUnique(NCR))
                        {
                            mainForm.NCRrepository.AddNewNonCarcinogenicRiskToDB(NCR);
                            mainForm.NCRrepository.LoadNonCarcinogenicRisksFromDatabase();
                            mainForm.UpdateNCRTable();
                            this.Close();
                        }
                        else {
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
    }
}
