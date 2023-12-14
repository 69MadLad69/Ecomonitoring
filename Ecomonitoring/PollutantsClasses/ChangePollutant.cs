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

namespace Ecomonitoring
{
    public partial class ChangePollutant : Form
    {
        Form1 mainForm;
        DataGridViewRow selectedRow;
        public ChangePollutant(Form1 mainForm, DataGridViewRow selectedRow)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.selectedRow = selectedRow;
            PollutantName.Text = CheckForNull(selectedRow.Cells[1].Value);
            PollutantActivity.Text = CheckForNull(selectedRow.Cells[2].Value);
            PollutantOwnership.Text = CheckForNull(selectedRow.Cells[3].Value);
            PollutantAddress.Text = CheckForNull(selectedRow.Cells[4].Value);

            switch((double) selectedRow.Cells[5].Value) 
            {
                case 1.0: 
                    {
                        CityType.SelectedIndex = 0;
                        break;
                    }

                case 1.25:
                    {
                        CityType.SelectedIndex = 1;
                        break;
                    }

                case 1.65:
                    {
                        CityType.SelectedIndex = 2;
                        break;
                    }
            }

            switch ((double) selectedRow.Cells[6].Value)
            {
                case 1.0:
                    {
                        CityPop.SelectedIndex = 0;
                        break;
                    }

                case 1.20:
                    {
                        CityPop.SelectedIndex = 1;
                        break;
                    }

                case 1.35:
                    {
                        CityPop.SelectedIndex = 2;
                        break;
                    }

                case 1.55:
                    {
                        CityPop.SelectedIndex = 3;
                        break;
                    }

                case 1.80:
                    {
                        CityPop.SelectedIndex = 4;
                        break;
                    }
            }

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            if (PollutantName.Text != "" && mainForm.isPollutantNameUnique(PollutantName.Text, int.Parse(selectedRow.Cells[0].Value.ToString())) )
            {
                if (mainForm.isDoubleParsable(mainForm.MinPaymentAmmount.Text))
                {
                    selectedRow.Cells[1].Value = PollutantName.Text;
                    selectedRow.Cells[2].Value = PollutantActivity.Text;
                    selectedRow.Cells[3].Value = PollutantOwnership.Text;
                    selectedRow.Cells[4].Value = PollutantAddress.Text;

                    double Kf = 1.00;
                    double Knas = 1.00;

                    switch (CityPop.SelectedIndex)
                    {

                        case 0:
                            {
                                Knas = 1.00;
                                break;
                            }

                        case 1:
                            {
                                Knas = 1.20;
                                break;
                            }

                        case 2:
                            {
                                Knas = 1.35;
                                break;
                            }

                        case 3:
                            {
                                Knas = 1.55;
                                break;
                            }

                        case 4:
                            {
                                Knas = 1.80;
                                break;
                            }
                    }

                    switch (CityType.SelectedIndex)
                    {

                        case 0:
                            {
                                Kf = 1.00;
                                break;
                            }

                        case 1:
                            {
                                Kf = 1.25;
                                break;
                            }

                        case 2:
                            {
                                Kf = 1.65;
                                break;
                            }
                    }

                    selectedRow.Cells[5].Value = Kf;
                    selectedRow.Cells[6].Value = Knas;

                    mainForm.PollutantTable.Refresh();
                    mainForm.UpdatePollutants();
                    mainForm.pollutantRepository.UpdatePollutantsDB();

                    double P = double.Parse(mainForm.MinPaymentAmmount.Text, NumberStyles.Any);

                    foreach (Pollution_Report polRep in mainForm.pollutionReportRepository.PollutionReports)
                    {
                        if (polRep.Pollutant_Id == (int) selectedRow.Cells[0].Value)
                        {
                            Substance polRepSub = mainForm.substanceRepository.GetSubstance(polRep.Substance_Id);
                            Pollutant polRepPollutant = mainForm.pollutantRepository.GetPollutant(polRep.Pollutant_Id);

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
                MessageBox.Show("Назва речовини має бути унікальною та не бути пустою!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public static string CheckForNull(object value)
        {
            return value != null ? value.ToString() : "";
        }
    }
}
