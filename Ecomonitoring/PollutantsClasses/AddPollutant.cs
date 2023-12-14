using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ecomonitoring
{
    public partial class AddPollutant : Form
    {
        Form1 mainForm;
        public AddPollutant(Form1 mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private void NewPollutantName_MouseHover(object sender, EventArgs e)
        {
            ((TextBox)sender).ForeColor = Color.FromArgb(42, 44, 54);
            ((TextBox)sender).BackColor = Color.FromArgb(155, 179, 205);
        }

        private void NewPollutantName_Click(object sender, EventArgs e)
        {
            ((TextBox)sender).ForeColor = Color.FromArgb(42, 44, 54);
            ((TextBox)sender).BackColor = Color.FromArgb(155, 179, 205);
        }

        private void NewPollutantName_MouseLeave(object sender, EventArgs e)
        {
            ((TextBox)sender).ForeColor = Color.FromArgb(155, 179, 205);
            ((TextBox)sender).BackColor = Color.FromArgb(42, 44, 54);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            int newPollutantId = 1;
            if (mainForm.pollutantRepository.Pollutants.Count != 0) {
                newPollutantId = mainForm.pollutantRepository.Pollutants[mainForm.pollutantRepository.Pollutants.Count - 1].Pollutant_Id + 1;
            }

            if (mainForm.isPollutantUnique(NewPollutantName.Text, newPollutantId) && NewPollutantName.Text != "")
            {
                double Kf = 1.00;
                double Knas = 1.00;

                switch (CityPop.SelectedIndex) {

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

                mainForm.pollutantRepository.AddNewPollutantToDB(new Pollutant(newPollutantId, NewPollutantName.Text, NewPollutantActivity.Text, NewPollutantOwnership.Text, NewPollutantAddress.Text, Kf, Knas));
                mainForm.pollutantRepository.LoadPollutantsFromDatabase();
                mainForm.UpdatePollutantTable();
                this.Close();
            }
            else {
                MessageBox.Show("Назва підприємства має бути унікальною та не бути пустою!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
