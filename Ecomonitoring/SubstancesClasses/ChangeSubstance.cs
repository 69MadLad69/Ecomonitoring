using Ecomonitoring.Repositories;
using MySqlX.XDevAPI.Relational;
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

namespace Ecomonitoring.SubstancesClasses
{
    public partial class ChangeSubstance : Form
    {
        Form1 mainForm;
        DataGridViewRow selectedRow;
        Substance selectedSub;
        public ChangeSubstance(Form1 mainForm, DataGridViewRow selectedRow)
        {
            InitializeComponent();

            this.mainForm = mainForm;
            this.selectedRow = selectedRow;

            selectedSub = mainForm.substanceRepository.GetSubstance((int)selectedRow.Cells[0].Value);

            SubstanceName.Text = selectedSub.name;
            SubstanceTLK.Text = selectedSub.TLK.ToString();
            SubstanceRfC.Text = selectedSub.RfC.ToString();
            SubstanceSF.Text = selectedSub.SF.ToString();
            SubstanceRoNorm.Text = selectedSub.roNorm.ToString();
            SubstanceQv.Text = selectedSub.qv.ToString();
            SubstanceCancerogenic.SelectedIndex = selectedSub.cancerogenic;
            DangerClass.Text = selectedSub.danger_class;
            SubstanceTaxType.Text = selectedSub.taxType.ToString();
            SubstanceTaxAmmount.Text = selectedSub.taxAmmount.ToString();

        }

        public static string CheckForNull(object value)
        {
            return value != null ? value.ToString() : "";
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            if (SubstanceName.Text != "" && mainForm.isSubstanceNameUnique(SubstanceName.Text, int.Parse(selectedRow.Cells[0].Value.ToString())))
            {
                if (mainForm.isDoubleParsable(SubstanceTLK.Text) && mainForm.isDoubleParsable(SubstanceRfC.Text)
                    && mainForm.isDoubleParsable(SubstanceSF.Text) && mainForm.isDoubleParsable(SubstanceRfC.Text)
                    && mainForm.isDoubleParsable(SubstanceQv.Text) && mainForm.isDoubleParsable(SubstanceRoNorm.Text)
                    && mainForm.isDoubleParsable(SubstanceTaxAmmount.Text))
                {
                    if (mainForm.isDoubleParsable(mainForm.MinPaymentAmmount.Text))
                    {
                        int selectedSubId = (int)selectedRow.Cells[0].Value;

                        mainForm.substanceRepository.GetSubstance(selectedSubId).name = SubstanceName.Text;
                        mainForm.substanceRepository.GetSubstance(selectedSubId).TLK = double.Parse(SubstanceTLK.Text, NumberStyles.Any);
                        mainForm.substanceRepository.GetSubstance(selectedSubId).roNorm = double.Parse(SubstanceRoNorm.Text, NumberStyles.Any);
                        mainForm.substanceRepository.GetSubstance(selectedSubId).qv = double.Parse(SubstanceQv.Text, NumberStyles.Any);
                        mainForm.substanceRepository.GetSubstance(selectedSubId).RfC = double.Parse(SubstanceRfC.Text, NumberStyles.Any);
                        mainForm.substanceRepository.GetSubstance(selectedSubId).SF = double.Parse(SubstanceSF.Text, NumberStyles.Any);
                        mainForm.substanceRepository.GetSubstance(selectedSubId).cancerogenic = SubstanceCancerogenic.SelectedIndex;
                        mainForm.substanceRepository.GetSubstance(selectedSubId).danger_class = DangerClass.Text;
                        mainForm.substanceRepository.GetSubstance(selectedSubId).taxAmmount = double.Parse(SubstanceTaxAmmount.Text, NumberStyles.Any);
                        mainForm.substanceRepository.GetSubstance(selectedSubId).taxType = SubstanceTaxType.Text;

                        mainForm.substanceRepository.UpdateSubstancesDB();
                        mainForm.UpdateSubstanesTable();

                        double P = double.Parse(mainForm.MinPaymentAmmount.Text, NumberStyles.Any);

                        foreach (Pollution_Report polRep in mainForm.pollutionReportRepository.PollutionReports)
                        {
                            if (polRep.Substance_Id == (int)selectedRow.Cells[0].Value)
                            {
                                Substance polRepSub = mainForm.substanceRepository.GetSubstance(polRep.Substance_Id);
                                Pollutant polRepPollutant = mainForm.pollutantRepository.GetPollutant(polRep.Pollutant_Id);

                                double excessiveMass = 3.6 * Math.Pow(10, -6) * (polRep.roi - polRepSub.roNorm) * polRepSub.qv * polRep.T;

                                if (excessiveMass < 0)
                                {
                                    excessiveMass = 0;
                                }

                                polRep.excessiveMass = excessiveMass;

                                double Ai = 1 / polRepSub.TLK;
                                double Kt = polRepPollutant.Knas * polRepPollutant.Kf;
                                double Kzi = polRep.CA / polRepSub.TLK;

                                polRep.compensationAmmount = polRep.excessiveMass * 1.1 * P * Ai * Kt * Kzi;
                            }
                        }

                        mainForm.UpdatePollutionTable();
                        mainForm.UpdateCRTable();
                        mainForm.UpdateNCRTable();

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Мінімальна заробітня плата має бути числами!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Записи в полях з ручним заповненням мають бути числами!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else {
                MessageBox.Show("Назва речовини має бути унікальною та не бути пустою!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
