using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomonitoring
{
    public class Substance
    {
        public int Substance_Id;
        public String name;
        public double TLK;
        public double RfC;
        public double SF;
        public String danger_class;
        public int cancerogenic;
        public double roNorm;
        public double qv;
        public String taxType;
        public double taxAmmount;

        public Substance(int Substance_Id, string name, double TLK, double RfC, double SF, 
                            int cancerogenic, string danger_class, double roNorm, double qv,
                            string taxType, double taxAmmount)
        {
            this.Substance_Id = Substance_Id;
            this.name = name;
            this.TLK = TLK;
            this.RfC = RfC;
            this.SF = SF;
            this.cancerogenic = cancerogenic;
            this.danger_class = danger_class;
            this.roNorm = roNorm;
            this.qv = qv;
            this.taxType = taxType;
            this.taxAmmount = taxAmmount;
        }
    }
}
