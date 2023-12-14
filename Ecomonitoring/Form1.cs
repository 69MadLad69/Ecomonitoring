using Ecomonitoring.PollutionClasses;
using Ecomonitoring.Repositories;
using Ecomonitoring.RisksCalasses.CarcinogenicRisk;
using Ecomonitoring.RisksCalasses.NonCarcinogenicRisk;
using Ecomonitoring.SubstancesClasses;
using MySql.Data.MySqlClient;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Ecomonitoring
{
    public partial class Form1 : Form
    {
        public static string connectionString = "Server = 127.0.0.1; Database = ecomonitoring; User Id = root; Password = bober2467;";
        public PollutantRepository pollutantRepository = new PollutantRepository(connectionString);
        public SubstanceRepository substanceRepository = new SubstanceRepository(connectionString);
        public PollutionReportRepository pollutionReportRepository = new PollutionReportRepository(connectionString);
        public CarcinogenicRiskRepository CRRepository = new CarcinogenicRiskRepository(connectionString);
        public NonCarcinogenicRiskRepository NCRrepository = new NonCarcinogenicRiskRepository(connectionString);

        public Form1()
        {
            InitializeComponent();

            foreach (DataGridViewColumn column in PollutantTable.Columns)
            {
                PollutantSearchType.Items.Add(column.Name);
            }

            foreach (DataGridViewColumn column in SubstanceTable.Columns) {
                SubstanceSearchType.Items.Add(column.Name);
            }

            pollutantRepository.LoadPollutantsFromDatabase();
            UpdatePollutantTable();

            substanceRepository.LoadSubstancesFromDatabase();
            UpdateSubstanesTable();

            pollutionReportRepository.LoadPollutionReportsFromDatabase();
            UpdatePollutionTable();

            CRRepository.LoadCarcinogenicRisksFromDatabase();
            UpdateCRTable();

            NCRrepository.LoadNonCarcinogenicRisksFromDatabase();
            UpdateNCRTable();

            CountSums();
        }

        internal Excel_Reader Excel_Reader
        {
            get => default;
            set
            {
            }
        }

        // ПІДПРИЄМСТВА
        private void ReadPollutantExcelButton_Click(object sender, EventArgs e)
        {

            openFileDialog1.DefaultExt = "Excel";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<int> unParsableRows = new List<int>();
                List<List<String>> rows = Excel_Reader.Read(openFileDialog1.FileName);

                foreach (List<String> row in rows)
                {
                    if (isIntParsable(row[0]) && isDoubleParsable(row[5]) && isDoubleParsable(row[6]))
                    {
                        if (isPollutantUnique(row[1], int.Parse(row[0])))
                        {
                            pollutantRepository.Pollutants.Add(new Pollutant(int.Parse(row[0]), row[1], row[2], row[3], row[4], double.Parse(row[5]), double.Parse(row[6])));
                            pollutantRepository.AddNewPollutantToDB(new Pollutant(int.Parse(row[0]), row[1], row[2], row[3], row[4], double.Parse(row[5]), double.Parse(row[6])));
                        }
                        else
                        {
                            int rowIndex = rows.FindIndex(r => r == row);
                            unParsableRows.Add(rowIndex + 1);
                        }
                    }
                    else
                    {
                        int rowIndex = rows.FindIndex(r => r == row);
                        unParsableRows.Add(rowIndex + 1);
                    }
                }

                if (unParsableRows.Count != 0)
                {
                    String unParsableIds = "";
                    foreach (int id in unParsableRows)
                    {
                        unParsableIds += id.ToString() + "; ";
                    }
                    MessageBox.Show("Деякі з рядків не відповідають умовам: " + unParsableIds, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                UpdatePollutantTable();
            }
        }

        public bool isPollutantUnique(string name, int id)
        {
            foreach (Pollutant pol in pollutantRepository.Pollutants)
            {
                if (pol.Pollutant_Id == id || pol.name.Equals(name))
                {
                    return false;
                }
            }
            return true;
        }

        public bool isPollutantNameUnique(string name, int id)
        {
            foreach (Pollutant pol in pollutantRepository.Pollutants)
            {
                if (pol.Pollutant_Id != id && pol.name.Equals(name))
                {
                    return false;
                }
            }
            return true;
        }

        private void DeletePollutantButton_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = PollutantTable.SelectedRows[0];
            int Pollutant_Id = (int)selectedRow.Cells[0].Value;
            DialogResult result = MessageBox.Show("Чи ви впевнені в видаленні рядка #" + Pollutant_Id + "?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                foreach (Pollutant pol in pollutantRepository.Pollutants)
                {
                    if (pol.Pollutant_Id == Pollutant_Id)
                    {
                        pollutantRepository.Pollutants.Remove(pol);
                        pollutantRepository.DeletePollutantFromDB(pol);

                        foreach (Pollution_Report polRep in pollutionReportRepository.findPollutionRepotsWithPolId(Pollutant_Id))
                        {
                            foreach (NonCarcinogenicRisk NCR in NCRrepository.findNCRsWithPollutionID(polRep.Pollution_Id))
                            {
                                NCRrepository.NonCarcinogenicRisks.Remove(NCR);
                                NCRrepository.DeleteNonCarcinogenicRiskFromDB(NCR);
                            }

                            foreach (CarcinogenicRisk CR in CRRepository.findCRsWithPollutionID(polRep.Pollution_Id))
                            {
                                CRRepository.CarcinogenicRisks.Remove(CR);
                                CRRepository.DeleteCarcinogenicRiskFromDB(CR.riskId);
                            }

                            pollutionReportRepository.PollutionReports.Remove(polRep);
                            pollutionReportRepository.DeletePollutionReportFromDB(polRep);
                        }

                        UpdatePollutionTable();
                        UpdateNCRTable();
                        UpdateCRTable();

                        break;
                    }
                }
                PollutantTable.Rows.Remove(selectedRow);
            }
        }

        private void AddPollutantButton_Click(object sender, EventArgs e)
        {
            AddPollutant adder = new AddPollutant(this);
            adder.Show();
        }

        public void UpdatePollutants()
        {
            List<Pollutant> newPollutants = new List<Pollutant>();
            foreach (DataGridViewRow row in PollutantTable.Rows)
            {
                Pollutant rowPol = new Pollutant(
                    (int)row.Cells[0].Value, row.Cells[1].Value.ToString(), 
                    row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString(), 
                    row.Cells[4].Value.ToString(), (double) row.Cells[5].Value, (double) row.Cells[6].Value);
                newPollutants.Add(rowPol);
            }
            pollutantRepository.Pollutants = newPollutants;
        }

        public void UpdatePollutantTable()
        {
            PollutantTable.Rows.Clear();
            foreach (Pollutant pol in pollutantRepository.Pollutants)
            {
                PollutantTable.Rows.Add(pol.Pollutant_Id, pol.name, pol.activity, pol.ownership, pol.address, pol.Kf, pol.Knas);
            }

        }

        private void ChangePollutantButton_Click(object sender, EventArgs e)
        {
            if (PollutantTable.SelectedRows.Count != 0)
            {
                DataGridViewRow selectedRow = PollutantTable.SelectedRows[0];
                ChangePollutant changeForm = new ChangePollutant(this, selectedRow);
                changeForm.Show();
            }
            else
            {
                MessageBox.Show("Будь ласка, оберіть заповнений рядок", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool isIntParsable(String val)
        {
            try
            {
                int num = int.Parse(val);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool isDoubleParsable(String val)
        {
            try
            {
                double num = double.Parse(val, NumberStyles.Any, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool isDoubleAddParsable(String val)
        {
            try
            {
                double num = double.Parse(val, NumberStyles.Any, CultureInfo.InvariantCulture);

                // Перевірка, чи в рядку є крапка як роздільник дробової частини
                if (val.Contains("."))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SearchPollutantButton_Click(object sender, EventArgs e)
        {
            RemoveRowsWithoutMatchingValue(PollutantTable, PollutantSearchType.Text, PollutantSearch.Text);
        }

        public static void RemoveRowsWithoutMatchingValue(DataGridView dataGridView, string columnName, string searchTerm)
        {
            if (!dataGridView.Columns.Contains(columnName))
            {
                MessageBox.Show($"Колонка з ім'ям {columnName} не знайдена.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var rowVisibility = dataGridView.Rows.Cast<DataGridViewRow>().ToDictionary(row => row, row => row.Visible);

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                DataGridViewCell cell = row.Cells[columnName];

                if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchTerm.ToLower()))
                {
                    row.Visible = true;
                }
                else
                {
                    row.Visible = false;
                }
            }
        }

        private void ShowAllPollutantsButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in PollutantTable.Rows)
            {
                row.Visible = true;
            }
        }


        // РЕЧОВИНИ


        private void ReadExcel_Substances_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "Excel";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<int> unParsableRows = new List<int>();
                List<List<String>> rows = Excel_Reader.Read(openFileDialog1.FileName);

                foreach (List<String> row in rows)
                {
                    if (isIntParsable(row[0]) 
                        && isIntParsable(row[5]) && isDoubleParsable(row[2]) 
                        && isDoubleParsable(row[3]) && isDoubleParsable(row[4])
                        && isDoubleParsable(row[7]) && isDoubleParsable(row[8]) && isDoubleParsable(row[9]))
                    {
                        if (isSubstanceUnique(row[1], int.Parse(row[0])))
                        {
                            Substance newSub = new Substance(int.Parse(row[0]), row[1], double.Parse(row[2]), 
                                                             double.Parse(row[3]), double.Parse(row[4]), int.Parse(row[5]), 
                                                             row[6], double.Parse(row[7]), double.Parse(row[8]), row[10], double.Parse(row[9]));
                           
                            substanceRepository.Substances.Add(newSub);
                            substanceRepository.AddNewSubstanceToDB(newSub);
                        }
                        else
                        {
                            int rowIndex = rows.FindIndex(r => r == row);
                            unParsableRows.Add(rowIndex + 1);
                        }
                    }
                    else
                    {
                        int rowIndex = rows.FindIndex(r => r == row);
                        unParsableRows.Add(rowIndex + 1);
                    }
                }

                if (unParsableRows.Count != 0)
                {
                    String unParsableIds = "";
                    foreach (int id in unParsableRows)
                    {
                        unParsableIds += id.ToString() + "; ";
                    }
                    MessageBox.Show("Деякі з рядків не відповідають умовам: " + unParsableIds, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                UpdateSubstanesTable();
            }

        }

        public void UpdateSubstanesTable()
        {
            SubstanceTable.Rows.Clear();
            foreach (Substance sub in substanceRepository.Substances)
            {
                SubstanceTable.Rows.Add(sub.Substance_Id, sub.name, sub.TLK, sub.RfC, sub.SF, sub.danger_class, sub.roNorm, sub.qv, sub.taxType, sub.taxAmmount);
            }
        }

        /*
        public void UpdateSubstances()
        {
            List<Substance> newSubstances = new List<Substance>();
            foreach (DataGridViewRow row in SubstanceTable.Rows)
            {
                Substance rowSub = new Substance(
                    (int)row.Cells[0].Value, 
                    row.Cells[1].Value.ToString(), 
                    double.Parse(row.Cells[2].Value.ToString()), 
                    double.Parse(row.Cells[3].Value.ToString()),
                    double.Parse(row.Cells[4].Value.ToString()),
                    (int)row.Cells[5].Value,
                    row.Cells[6].Value.ToString());

                newSubstances.Add(rowSub);
            }
            substanceRepository.Substances = newSubstances;
        }
        */

        public bool isSubstanceUnique(string name, int id)
        {
            foreach (Substance sub in substanceRepository.Substances)
            {
                if (sub.Substance_Id == id || sub.name.Equals(name))
                {
                    return false;
                }
            }
            return true;
        }

        public bool isSubstanceNameUnique(string name, int id)
        {
            foreach (Substance sub in substanceRepository.Substances)
            {
                if (sub.Substance_Id != id && sub.name.Equals(name))
                {
                    return false;
                }
            }
            return true;
        }

        private void DeleteSubstanceButton_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = SubstanceTable.SelectedRows[0];
            int Substance_Id = (int)selectedRow.Cells[0].Value;
            DialogResult result = MessageBox.Show("Чи ви впевнені в видаленні рядка #" + Substance_Id + "?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                foreach (Substance sub in substanceRepository.Substances)
                {
                    if (sub.Substance_Id == Substance_Id)
                    {
                        substanceRepository.Substances.Remove(sub);
                        substanceRepository.DeleteSubstanceFromDB(sub);

                        foreach (Pollution_Report polRep in pollutionReportRepository.findPollutionReportsWithSubId(Substance_Id))
                        {
                            foreach (NonCarcinogenicRisk NCR in NCRrepository.findNCRsWithPollutionID(polRep.Pollution_Id))
                            {
                                NCRrepository.NonCarcinogenicRisks.Remove(NCR);
                                NCRrepository.DeleteNonCarcinogenicRiskFromDB(NCR);
                            }

                            foreach (CarcinogenicRisk CR in CRRepository.findCRsWithPollutionID(polRep.Pollution_Id))
                            {
                                CRRepository.CarcinogenicRisks.Remove(CR);
                                CRRepository.DeleteCarcinogenicRiskFromDB(CR.riskId);
                            }

                            pollutionReportRepository.PollutionReports.Remove(polRep);
                            pollutionReportRepository.DeletePollutionReportFromDB(polRep);
                        }

                        UpdatePollutionTable();
                        UpdateNCRTable();
                        UpdateCRTable();

                        break;
                    }
                }
                SubstanceTable.Rows.Remove(selectedRow);
            }
        }

        private void ChangeSubstanceButton_Click(object sender, EventArgs e)
        {
            if (SubstanceTable.SelectedRows.Count != 0)
            {
                DataGridViewRow selectedRow = SubstanceTable.SelectedRows[0];
                ChangeSubstance changeForm = new ChangeSubstance(this, selectedRow);
                changeForm.Show();
            }
            else
            {
                MessageBox.Show("Будь ласка, оберіть заповнений рядок", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddSubstanceButton_Click(object sender, EventArgs e)
        {
            AddSubstance adder = new AddSubstance(this);
            adder.Show();
        }

        private void SearchSubstanceButton_Click(object sender, EventArgs e)
        {
            RemoveRowsWithoutMatchingValue(SubstanceTable, SubstanceSearchType.Text, SubstanceSearch.Text);
        }

        private void ShowAllSubstancesButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in SubstanceTable.Rows)
            {
                row.Visible = true;
            }
        }


        // ЗАБРУДНЕННЯ


        private void ReadPollutionsExcel_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "Excel";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<int> unParsableRows = new List<int>();
                List<List<String>> rows = Excel_Reader.Read(openFileDialog1.FileName);

                foreach (List<String> row in rows)
                {
                    if (!pollutantRepository.PollutantExists(row[1]) || !substanceRepository.SubstanceExists(row[2]))
                    {
                        int rowIndex = rows.FindIndex(r => r == row);
                        unParsableRows.Add(rowIndex + 1);
                    }
                    else {
                        if (isIntParsable(row[0]) && isDoubleParsable(row[3]) && isDoubleParsable(row[4]) && isDoubleParsable(row[6]) && isDoubleParsable(row[7]))
                        {
                            int polutantId = pollutantRepository.FindPollutantId(row[1]);
                            int substanceId = substanceRepository.FindSubstanceId(row[2]);

                            if (polutantId != -1 && substanceId != -1)
                            {
                                double Ca = double.Parse(row[4]);
                                double roi = double.Parse(row[7]);
                                double T = double.Parse(row[6]);
                                Substance polRepSub = substanceRepository.GetSubstance(substanceId);
                                Pollutant polRepPollutant = pollutantRepository.GetPollutant(polutantId);

                                double excessiveMass = 3.6 * Math.Pow(10, -6) * (roi - polRepSub.roNorm) * polRepSub.qv * T;

                                if (excessiveMass < 0) {
                                    excessiveMass = 0;
                                }

                                double Ai = 1 / polRepSub.TLK;
                                double Kt = polRepPollutant.Knas * polRepPollutant.Kf;
                                double Kzi = Ca / polRepSub.TLK;
                                double P = double.Parse(MinPaymentAmmount.Text);

                                double compensationAmmount = excessiveMass * 1.1 * P * Ai * Kt * Kzi;

                                double pollutinAmmount = double.Parse(row[3]);

                                double taxYearAmmount = pollutinAmmount * polRepSub.taxAmmount;

                                Pollution_Report report = 
                                    new Pollution_Report(int.Parse(row[0]), polutantId, substanceId, pollutinAmmount, 
                                    Ca, row[5], T, excessiveMass, compensationAmmount, taxYearAmmount, roi);
                                
                                if (isPollutionUnique(report))
                                {
                                    pollutionReportRepository.PollutionReports.Add(report);
                                    pollutionReportRepository.AddNewPollutionReportToDB(report);
                                }
                                else
                                {
                                    int rowIndex = rows.FindIndex(r => r == row);
                                    unParsableRows.Add(rowIndex + 1);
                                }
                            }
                            else 
                            {
                                int rowIndex = rows.FindIndex(r => r == row);
                                unParsableRows.Add(rowIndex + 1);
                            }
                        }
                        else
                        {
                            int rowIndex = rows.FindIndex(r => r == row);
                            unParsableRows.Add(rowIndex + 1);
                        }
                    }
                }

                if (unParsableRows.Count != 0)
                {
                    String unParsableIds = "";
                    foreach (int id in unParsableRows)
                    {
                        unParsableIds += id.ToString() + "; ";
                    }
                    MessageBox.Show("Деякі з рядків не відповідають умовам: " + unParsableIds, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                UpdatePollutionTable();
            }
        }

        private void PollutionTable_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            CountSums();
        }

        private void PollutionTable_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CountSums();
        }

        public void UpdatePollutionTable()
        {
            PollutionTable.Rows.Clear();
            foreach (Pollution_Report polRep in pollutionReportRepository.PollutionReports)
            {
                string PollutantName = pollutantRepository.GetPollutant(polRep.Pollutant_Id).name;
                string SubstanceName = substanceRepository.GetSubstance(polRep.Substance_Id).name;
                string TaxType = substanceRepository.GetSubstance(polRep.Substance_Id).taxType;
                PollutionTable.Rows.Add(polRep.Pollution_Id, PollutantName, SubstanceName, polRep.CA, polRep.roi, polRep.T, polRep.ammount, polRep.excessiveMass, polRep.compensationAmmount, TaxType, polRep.taxYearAmmount, polRep.report_year);
            }
        }

        private void ChangeMinPayment_Click(object sender, EventArgs e)
        {
            if (isDoubleParsable(MinPaymentAmmount.Text))
            {
                double P = double.Parse(MinPaymentAmmount.Text, NumberStyles.Any);

                foreach (Pollution_Report polRep in pollutionReportRepository.PollutionReports)
                {
                    if (polRep.report_year.Equals(PaymentYear.Text))
                    {
                        Substance polRepSub = substanceRepository.GetSubstance(polRep.Substance_Id);
                        Pollutant polRepPollutant = pollutantRepository.GetPollutant(polRep.Pollutant_Id);

                        double Ai = 1 / polRepSub.TLK;
                        double Kt = polRepPollutant.Knas * polRepPollutant.Kf;
                        double Kzi = polRep.CA / polRepSub.TLK;

                        polRep.compensationAmmount = polRep.excessiveMass * 1.1 * P * Ai * Kt * Kzi;
                    }
                }

                UpdatePollutionTable();
            }
            else {
                MessageBox.Show("Мінімальна заробітня плата має бути числами!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void CountSums() {
            double compensationSum = 0;
            double taxSum = 0;

            foreach (DataGridViewRow row in PollutionTable.Rows)
            {
                if (row.Visible == true)
                {
                    compensationSum += double.Parse(row.Cells[8].Value.ToString());
                    taxSum += double.Parse(row.Cells[10].Value.ToString());
                }
            }

            CompensationSumText.Text = compensationSum.ToString("#,##0.#####", CultureInfo.InvariantCulture);
            TaxSumText.Text = taxSum.ToString("#,##0.#####", CultureInfo.InvariantCulture);
        }

        public void UpdatePollutions()
        {
            List<Pollution_Report> newPollutions = new List<Pollution_Report>();
            foreach (DataGridViewRow row in PollutionTable.Rows)
            {
                int PollutionId = (int) row.Cells[0].Value;
                int PollutantId = pollutantRepository.FindPollutantId(row.Cells[1].Value.ToString());
                int SubstanceId = substanceRepository.FindSubstanceId(row.Cells[2].Value.ToString());
                double Ca = double.Parse(row.Cells[3].Value.ToString());
                double roi = double.Parse(row.Cells[4].Value.ToString());
                double T = double.Parse(row.Cells[5].Value.ToString());
                double ammount = double.Parse(row.Cells[6].Value.ToString());
                double excessiveMass = double.Parse(row.Cells[7].Value.ToString());
                double compensation = double.Parse(row.Cells[8].Value.ToString());
                string taxType = row.Cells[9].Value.ToString();
                double tax = double.Parse(row.Cells[10].Value.ToString());
                string pollutionYear = row.Cells[11].Value.ToString();

                Pollution_Report rowPolRep = 
                    new Pollution_Report(PollutionId, PollutantId, SubstanceId, ammount, Ca, 
                                        pollutionYear, T, excessiveMass, compensation, tax, roi);

                newPollutions.Add(rowPolRep);
            }
            pollutionReportRepository.PollutionReports = newPollutions;
        }

        public bool isPollutionUnique(Pollution_Report report) {
            foreach (Pollution_Report rep in pollutionReportRepository.PollutionReports) {
                if (rep.Pollution_Id == report.Pollution_Id || report.Pollutant_Id == rep.Pollutant_Id && report.Substance_Id == rep.Substance_Id && report.report_year == rep.report_year) {
                    return false;
                }
            }
            return true;
        }

        private void DeletePollution_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = PollutionTable.SelectedRows[0];
            int Pollution_Id = (int)selectedRow.Cells[0].Value;
            DialogResult result = MessageBox.Show("Чи ви впевнені в видаленні рядка #" + Pollution_Id + "?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                foreach (Pollution_Report polRep in pollutionReportRepository.PollutionReports)
                {
                    if (polRep.Pollution_Id == Pollution_Id)
                    {
                        pollutionReportRepository.PollutionReports.Remove(polRep);
                        pollutionReportRepository.DeletePollutionReportFromDB(polRep);

                        foreach (NonCarcinogenicRisk NCR in NCRrepository.findNCRsWithPollutionID(polRep.Pollution_Id))
                        {
                            NCRrepository.NonCarcinogenicRisks.Remove(NCR);
                            NCRrepository.DeleteNonCarcinogenicRiskFromDB(NCR);
                        }

                        foreach (CarcinogenicRisk CR in CRRepository.findCRsWithPollutionID(polRep.Pollution_Id))
                        {
                            CRRepository.CarcinogenicRisks.Remove(CR);
                            CRRepository.DeleteCarcinogenicRiskFromDB(CR.riskId);
                        }

                        UpdateCRTable();

                        UpdateNCRTable();

                        break;
                    }
                }

                PollutionTable.Rows.Remove(selectedRow);
            }
        }

        private void ChangePollution_Click(object sender, EventArgs e)
        {
            if (PollutionTable.SelectedRows.Count != 0)
            {
                DataGridViewRow selectedRow = PollutionTable.SelectedRows[0];
                ChangePollution changeForm = new ChangePollution(this, selectedRow);
                changeForm.Show();
            }
            else
            {
                MessageBox.Show("Будь ласка, оберіть заповнений рядок", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PollutionTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CountSums();
        }

        private void AddPollution_Click(object sender, EventArgs e)
        {
            AddPollution adder = new AddPollution(this);
            adder.Show();
        }

        private void PolSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
           switch (PolSearchType.SelectedIndex) { 
            
                    case 0:
                    {
                        PolSearch.Items.Clear();
                        foreach (Pollutant pol in pollutantRepository.Pollutants)
                        {
                            PolSearch.Items.Add(pol.name);
                        }
                        PolSearch.Text = PolSearch.Items[0].ToString();
                        break;
                    }

                    case 1: 
                    {
                        PolSearch.Items.Clear();
                        foreach (Substance sub in substanceRepository.Substances)
                        {
                            PolSearch.Items.Add(sub.name);
                        }
                        PolSearch.Text = PolSearch.Items[0].ToString();
                        break;
                    }

                    case 2:
                    {
                        PolSearch.Items.Clear();
                        String[] years = {"2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019", "2020", "2021", "2022", "2023"};

                        foreach (String year in years) {
                            PolSearch.Items.Add(year);
                        }
                        PolSearch.Text = PolSearch.Items[0].ToString();
                        break;
                    }
            }
        }

        private void ShowAllPollutionsButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in PollutionTable.Rows)
            {
                row.Visible = true;
            }

            CountSums();
        }

        private void SearchPollutionButton_Click(object sender, EventArgs e)
        {
            RemoveRowsWithoutMatchingValue(PollutionTable, PolSearchType.Text, PolSearch.Text);

            CountSums();
        }


        // Канцерогенні ризики


        private void ReadExcelCR_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "Excel";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<int> unParsableRows = new List<int>();
                List<List<String>> rows = Excel_Reader.Read(openFileDialog1.FileName);

                foreach (List<String> row in rows)
                {
                    if (!isIntParsable(row[0]) 
                        && !isIntParsable(row[1]) 
                        && !isDoubleParsable(row[3]) 
                        && !isDoubleParsable(row[4])
                        && !isDoubleParsable(row[5])
                        && !isDoubleParsable(row[6])
                        && !isDoubleParsable(row[7])
                        && !isDoubleParsable(row[8]))
                    {
                        int rowIndex = rows.FindIndex(r => r == row);
                        unParsableRows.Add(rowIndex + 1);
                    }
                    else
                    {
                        if (pollutionReportRepository.GetPollution_Report(int.Parse(row[0])) != null)
                        {
                            int CR_Id = int.Parse(row[0]);
                            int CR_PollutionId = int.Parse(row[1]);
                            int SubstanceID = pollutionReportRepository.GetPollution_Report(CR_PollutionId).Substance_Id;


                            double CA = pollutionReportRepository.GetPollution_Report(CR_PollutionId).CA;
                            double CH = 1.0 * CA;
                            double TimeOut = double.Parse(row[2], NumberStyles.Any);
                            double TimeIn = double.Parse(row[3], NumberStyles.Any);
                            double Vout = double.Parse(row[4], NumberStyles.Any);
                            double Vin = double.Parse(row[5], NumberStyles.Any);
                            double EF = double.Parse(row[6], NumberStyles.Any);
                            double ED = double.Parse(row[7], NumberStyles.Any);
                            double BW = double.Parse(row[8], NumberStyles.Any);

                            double LADD = ((CA * TimeOut * Vout) + (CH * TimeIn * Vin)) * EF * ED / (BW * 70 * 365);
                            double SF = substanceRepository.GetSubstance(SubstanceID).SF;
                            double CR = LADD * SF  * Math.Pow(10, 4);

                            CarcinogenicRisk NewCR = new CarcinogenicRisk(
                                CR_Id, CR_PollutionId, TimeOut, TimeIn, Vout, Vin, EF, ED,
                                BW, LADD, CR);

                            if (isCRUnique(NewCR))
                            {
                                CRRepository.CarcinogenicRisks.Add(NewCR);
                                CRRepository.AddNewCarcinogenicRiskToDB(NewCR);
                            }
                            else
                            {
                                int rowIndex = rows.FindIndex(r => r == row);
                                unParsableRows.Add(rowIndex + 1);
                            }
                        }
                        else
                        {
                            int rowIndex = rows.FindIndex(r => r == row);
                            unParsableRows.Add(rowIndex + 1);
                        }
                    }
                }

                if (unParsableRows.Count != 0)
                {
                    String unParsableIds = "";
                    foreach (int id in unParsableRows)
                    {
                        unParsableIds += id.ToString() + "; ";
                    }
                    MessageBox.Show("Деякі з рядків не відповідають умовам: " + unParsableIds, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                UpdateCRTable();
            }
        }

        private void AddCR_Click(object sender, EventArgs e)
        {
            AddCR adder = new AddCR(this);
            adder.Show();
        }

        public void UpdateCRTable()
        {
            CRtable.Rows.Clear();
            foreach (CarcinogenicRisk CR in CRRepository.CarcinogenicRisks)
            {
                string PollutantName = pollutantRepository.GetPollutant(pollutionReportRepository.GetPollution_Report(CR.pollution_Id).Pollutant_Id).name;
                string SubstanceName = substanceRepository.GetSubstance(pollutionReportRepository.GetPollution_Report(CR.pollution_Id).Substance_Id).name;

                string RiskLevel = "";

                if (CR.CR * Math.Pow(10, -4) > 1 * Math.Pow(10, -3))
                {
                    RiskLevel = "Високий";
                }
                
                if (CR.CR * Math.Pow(10, -4) < 1 * Math.Pow(10, -3) && CR.CR * Math.Pow(10, -4) > 1 * Math.Pow(10, -4)) {
                    RiskLevel = "Середній";
                }

                if (CR.CR * Math.Pow(10, -4) < 1 * Math.Pow(10, -4) && CR.CR * Math.Pow(10, -4) > 1 * Math.Pow(10, -6))
                {
                    RiskLevel = "Низький";
                }

                if (CR.CR < 1 * Math.Pow(10, -6))
                {
                    RiskLevel = "Мінімальний";
                }

                string RiskYear = pollutionReportRepository.GetPollution_Report(CR.pollution_Id).report_year;

                CRtable.Rows.Add(CR.riskId, PollutantName, SubstanceName, CR.CR, RiskLevel, RiskYear);
            }
        }

        public bool isCRUnique(CarcinogenicRisk risk)
        {
            foreach (CarcinogenicRisk r in CRRepository.CarcinogenicRisks)
            {
                if (r.riskId == risk.riskId
                    ||
                    !isCRDataUnique(risk))
                {
                    return false;
                }
            }
            return true;
        }

        public bool isCRDataUnique(CarcinogenicRisk risk)
        {
            foreach (CarcinogenicRisk r in CRRepository.CarcinogenicRisks)
            {
                if (r.pollution_Id == risk.pollution_Id
                    && r.TimeOut == risk.TimeOut
                    && r.TimeIn == risk.TimeIn
                    && r.Vout == risk.Vout
                    && r.Vin == risk.Vin
                    && r.EF == risk.EF
                    && r.ED == risk.ED
                    && r.BodyWeight == risk.BodyWeight)
                {
                    return false;
                }
            }
            return true;
        }

        private void ChangeCR_Click(object sender, EventArgs e)
        {
            if (CRtable.SelectedRows.Count != 0)
            {
                DataGridViewRow selectedRow = CRtable.SelectedRows[0];
                ChangeCR changeForm = new ChangeCR(this, selectedRow);
                changeForm.Show();
            }
            else
            {
                MessageBox.Show("Будь ласка, оберіть заповнений рядок", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DelCR_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = CRtable.SelectedRows[0];
            int CR_Id = (int)selectedRow.Cells[0].Value;
            DialogResult result = MessageBox.Show("Чи ви впевнені в видаленні рядка #" + CR_Id + "?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                foreach (CarcinogenicRisk CR in CRRepository.CarcinogenicRisks)
                {
                    if (CR.riskId == CR_Id)
                    {
                        CRRepository.CarcinogenicRisks.Remove(CR);
                        CRRepository.DeleteCarcinogenicRiskFromDB(CR.riskId);
                        break;
                    }
                }
                CRtable.Rows.Remove(selectedRow);
            }
        }

        private void FindCR_Click(object sender, EventArgs e)
        {
            RemoveRowsWithoutMatchingValue(CRtable, SearchTypeCR.Text, SearchTextCR.Text);
        }

        private void ShowAllCR_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in CRtable.Rows)
            {
                row.Visible = true;
            }
        }

        private void SearchTypeCR_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SearchTypeCR.SelectedIndex)
            {

                case 0:
                    {
                        SearchTextCR.Items.Clear();
                        foreach (Pollutant pol in pollutantRepository.Pollutants)
                        {
                            SearchTextCR.Items.Add(pol.name);
                        }
                        SearchTextCR.Text = SearchTextCR.Items[0].ToString();
                        break;
                    }

                case 1:
                    {
                        SearchTextCR.Items.Clear();
                        foreach (Substance sub in substanceRepository.Substances)
                        {
                            SearchTextCR.Items.Add(sub.name);
                        }
                        SearchTextCR.Text = SearchTextCR.Items[0].ToString();
                        break;
                    }

                case 2:
                    {
                        SearchTextCR.Items.Clear();
                        String[] years = { "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019", "2020", "2021", "2022", "2023" };

                        foreach (String year in years)
                        {
                            SearchTextCR.Items.Add(year);
                        }
                        SearchTextCR.Text = SearchTextCR.Items[0].ToString();
                        break;
                    }
            }
        }


        // Неканцерогенні ризики


        private void AddNCR_Click(object sender, EventArgs e)
        {
            AddNCR adder = new AddNCR(this);
            adder.Show();
        }


        public void UpdateNCRTable()
        {
            NCRtable.Rows.Clear();
            foreach (NonCarcinogenicRisk NCR in NCRrepository.NonCarcinogenicRisks)
            {
                string PollutantName = pollutantRepository.GetPollutant(pollutionReportRepository.GetPollution_Report(NCR.pollution_Id).Pollutant_Id).name;
                string SubstanceName = substanceRepository.GetSubstance(pollutionReportRepository.GetPollution_Report(NCR.pollution_Id).Substance_Id).name;

                string RiskLevel = "";

                if (NCR.HQ < 1) {
                    RiskLevel = "Зневажливо малий";
                }

                if (NCR.HQ == 1)
                {
                    RiskLevel = "Потребує термінових заходів";
                }

                if (NCR.HQ > 1)
                {
                    RiskLevel = "Імовірність розвитку шкідливих ефектів";
                }

                string RiskYear = pollutionReportRepository.GetPollution_Report(NCR.pollution_Id).report_year;

                NCRtable.Rows.Add(NCR.riskId, PollutantName, SubstanceName, NCR.HQ, RiskLevel, RiskYear);
            }
        }

        public bool isNCRUnique(NonCarcinogenicRisk risk)
        {
            foreach (NonCarcinogenicRisk r in NCRrepository.NonCarcinogenicRisks)
            {
                if (r.riskId == risk.riskId || r.pollution_Id == risk.pollution_Id)
                {
                    return false;
                }
            }
            return true;
        }

        private void DelNCR_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = NCRtable.SelectedRows[0];
            int NCR_Id = (int)selectedRow.Cells[0].Value;
            DialogResult result = MessageBox.Show("Чи ви впевнені в видаленні рядка #" + NCR_Id + "?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                foreach (NonCarcinogenicRisk NCR in NCRrepository.NonCarcinogenicRisks)
                {
                    if (NCR.riskId == NCR_Id)
                    {
                        NCRrepository.NonCarcinogenicRisks.Remove(NCR);
                        NCRrepository.DeleteNonCarcinogenicRiskFromDB(NCR);
                        break;
                    }
                }
                NCRtable.Rows.Remove(selectedRow);
            }
        }

        private void ChangeNCR_Click(object sender, EventArgs e)
        {
            if (NCRtable.SelectedRows.Count != 0)
            {
                DataGridViewRow selectedRow = NCRtable.SelectedRows[0];
                ChangeNCR changeForm = new ChangeNCR(this, selectedRow);
                changeForm.Show();
            }
            else
            {
                MessageBox.Show("Будь ласка, оберіть заповнений рядок", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReadExcelNCR_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "Excel";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<int> unParsableRows = new List<int>();
                List<List<String>> rows = Excel_Reader.Read(openFileDialog1.FileName);

                foreach (List<String> row in rows)
                {
                    if (!isIntParsable(row[0])
                        && !isIntParsable(row[1]))
                    {
                        int rowIndex = rows.FindIndex(r => r == row);
                        unParsableRows.Add(rowIndex + 1);
                    }
                    else
                    {
                        if (pollutionReportRepository.GetPollution_Report(int.Parse(row[0])) != null)
                        {
                            int NCR_Id = int.Parse(row[0]);
                            int NCR_PollutionId = int.Parse(row[1]);
                            int SubstanceID = pollutionReportRepository.GetPollution_Report(NCR_PollutionId).Substance_Id;


                            double CA = pollutionReportRepository.GetPollution_Report(NCR_PollutionId).CA;
                            double RfC = substanceRepository.GetSubstance(SubstanceID).RfC;

                            double HQ = CA * RfC;

                            NonCarcinogenicRisk NCR = new NonCarcinogenicRisk(NCR_Id, NCR_PollutionId, HQ);

                            if (isNCRUnique(NCR))
                            {
                                NCRrepository.NonCarcinogenicRisks.Add(NCR);
                                NCRrepository.AddNewNonCarcinogenicRiskToDB(NCR);
                                UpdateNCRTable();
                            }
                            else
                            {
                                int rowIndex = rows.FindIndex(r => r == row);
                                unParsableRows.Add(rowIndex + 1);
                            }
                        }
                        else
                        {
                            int rowIndex = rows.FindIndex(r => r == row);
                            unParsableRows.Add(rowIndex + 1);
                        }
                    }
                }

                if (unParsableRows.Count != 0)
                {
                    String unParsableIds = "";
                    foreach (int id in unParsableRows)
                    {
                        unParsableIds += id.ToString() + "; ";
                    }
                    MessageBox.Show("Деякі з рядків не відповідають умовам: " + unParsableIds, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                UpdateNCRTable();
            }
        }

        private void AddAllCRP_Click(object sender, EventArgs e)
        {
            foreach (Pollution_Report polRep in pollutionReportRepository.PollutionReports)
            {
                int NCR_Id = 1;

                if (NCRrepository.NonCarcinogenicRisks.Count != 0)
                {
                    NCR_Id = NCRrepository.NonCarcinogenicRisks[NCRrepository.NonCarcinogenicRisks.Count - 1].riskId + 1;
                }

                int NCR_PollutionId = polRep.Pollution_Id;
                int SubstanceID = polRep.Substance_Id;

                if (substanceRepository.GetSubstance(SubstanceID).cancerogenic == 0) {
                    double CA = polRep.CA;
                    double RfC = substanceRepository.GetSubstance(SubstanceID).RfC;

                    double HQ = CA * RfC;

                    NonCarcinogenicRisk NCR = new NonCarcinogenicRisk(NCR_Id, NCR_PollutionId, HQ);

                    if (isNCRUnique(NCR))
                    {
                        NCRrepository.NonCarcinogenicRisks.Add(NCR);
                        NCRrepository.AddNewNonCarcinogenicRiskToDB(NCR);
                        UpdateNCRTable();
                    }
                }
            }
        }

        private void SearchTypeNCR_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SearchTypeNCR.SelectedIndex)
            {

                case 0:
                    {
                        SearchTextNCR.Items.Clear();
                        foreach (Pollutant pol in pollutantRepository.Pollutants)
                        {
                            SearchTextNCR.Items.Add(pol.name);
                        }
                        SearchTextNCR.Text = SearchTextNCR.Items[0].ToString();
                        break;
                    }

                case 1:
                    {
                        SearchTextNCR.Items.Clear();
                        foreach (Substance sub in substanceRepository.Substances)
                        {
                            SearchTextNCR.Items.Add(sub.name);
                        }
                        SearchTextNCR.Text = SearchTextNCR.Items[0].ToString();
                        break;
                    }

                case 2:
                    {
                        SearchTextNCR.Items.Clear();
                        String[] years = { "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019", "2020", "2021", "2022", "2023" };

                        foreach (String year in years)
                        {
                            SearchTextNCR.Items.Add(year);
                        }
                        SearchTextNCR.Text = SearchTextNCR.Items[0].ToString();
                        break;
                    }
            }
        }

        private void SearchNCR_Click(object sender, EventArgs e)
        {
            RemoveRowsWithoutMatchingValue(NCRtable, SearchTypeNCR.Text, SearchTextNCR.Text);
        }

        private void ShowAllNCR_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in NCRtable.Rows)
            {
                row.Visible = true;
            }
        }
    }
}
