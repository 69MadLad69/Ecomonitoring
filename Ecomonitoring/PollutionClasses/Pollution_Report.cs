using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomonitoring
{
    public class Pollution_Report
    {
        public int Pollution_Id;
        public int Pollutant_Id;
        public int Substance_Id;
        public double ammount;
        public double CA;
        public double roi;
        public String report_year;
        public double T;
        public double excessiveMass = 0;
        public double compensationAmmount = 0;
        public double taxYearAmmount = 0;

        public Pollution_Report(int Pollution_Id, int Pollutant_Id, int Substance_Id, double ammount, double Ca, string report_year,
                                double T, double excessiveMass, double compensationAmmount, double taxYearAmmount, double roi)
        {
            this.Pollution_Id = Pollution_Id;
            this.Pollutant_Id = Pollutant_Id;
            this.Substance_Id = Substance_Id;
            this.ammount = ammount;
            this.CA = Ca;
            this.report_year = report_year;
            this.T = T;
            this.excessiveMass = excessiveMass;
            this.compensationAmmount = compensationAmmount;
            this.taxYearAmmount = taxYearAmmount;
            this.roi = roi;
        }
    }
}
