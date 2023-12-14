using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomonitoring.RisksCalasses.CarcinogenicRisk
{
    public class CarcinogenicRisk
    {
        public int riskId;
        public int pollution_Id;
        public double TimeOut;
        public double TimeIn;
        public double Vout;
        public double Vin;
        public double EF;
        public double ED;
        public double BodyWeight;
        public double AT = 70;
        public double LADD;
        public double CR;
        public CarcinogenicRisk(int riskId, int pollution_Id, double TimeOut, double TimeIn, 
                                double Vout, double Vin, double EF, double ED, double BodyWeight,
                                double LADD, double CR) 
        {
            this.riskId = riskId;
            this.pollution_Id = pollution_Id;
            this.TimeOut = TimeOut;
            this.TimeIn = TimeIn;
            this.Vout = Vout;
            this.Vin = Vin;
            this.EF = EF;
            this.ED = ED;
            this.BodyWeight = BodyWeight;
            this.LADD = LADD;
            this.CR = CR;
        }
    }
}
