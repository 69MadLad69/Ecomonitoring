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
    public partial class AddSubstance : Form
    {
        Form1 mainForm;
        public AddSubstance(Form1 mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void TextBox_MouseHover(object sender, EventArgs e)
        {
            ((TextBox)sender).ForeColor = Color.FromArgb(42, 44, 54);
            ((TextBox)sender).BackColor = Color.FromArgb(155, 179, 205);
        }

        private void TextBox_Click(object sender, EventArgs e)
        {
            ((TextBox)sender).ForeColor = Color.FromArgb(42, 44, 54);
            ((TextBox)sender).BackColor = Color.FromArgb(155, 179, 205);
        }

        private void TextBox_MouseLeave(object sender, EventArgs e)
        {
            ((TextBox)sender).ForeColor = Color.FromArgb(155, 179, 205);
            ((TextBox)sender).BackColor = Color.FromArgb(42, 44, 54);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            int newSubstanceId = 1;
            
            if (mainForm.substanceRepository.Substances.Count != 0)
            {
                newSubstanceId = mainForm.substanceRepository.Substances[mainForm.substanceRepository.Substances.Count - 1].Substance_Id + 1;
            }

            if (mainForm.isSubstanceUnique(NewSubstanceName.Text, newSubstanceId) && NewSubstanceName.Text != "")
            {
                if (mainForm.isDoubleParsable(NewSubstanceTLK.Text) && mainForm.isDoubleParsable(NewSubstanceRfC.Text) 
                    && mainForm.isDoubleParsable(NewSubstanceSF.Text) && mainForm.isDoubleParsable(NewSubstanceRfC.Text)
                    && mainForm.isDoubleParsable(NewSubstanceQv.Text) && mainForm.isDoubleParsable(NewSubstanceRoNorm.Text)
                    && mainForm.isDoubleParsable(NewSubstanceTaxAmmount.Text))
                {
                    mainForm.substanceRepository.AddNewSubstanceToDB(
                        new Substance(newSubstanceId, NewSubstanceName.Text,
                        double.Parse(NewSubstanceTLK.Text, NumberStyles.Any),
                        double.Parse(NewSubstanceRfC.Text, NumberStyles.Any),
                        double.Parse(NewSubstanceSF.Text, NumberStyles.Any),
                        NewSubstanceCancerogenic.SelectedIndex, DangerClass.Text,
                        double.Parse(NewSubstanceRoNorm.Text, NumberStyles.Any),
                        double.Parse(NewSubstanceQv.Text, NumberStyles.Any),
                        NewSubstanceTaxType.Text, double.Parse(NewSubstanceTaxAmmount.Text, NumberStyles.Any))
                    );

                    mainForm.substanceRepository.LoadSubstancesFromDatabase();
                    mainForm.UpdateSubstanesTable();
                    this.Close();
                }
                else {
                    MessageBox.Show("Записи в полях з ручним заповненням мають бути числами!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Назва речовини має бути унікальною та не бути пустою!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
