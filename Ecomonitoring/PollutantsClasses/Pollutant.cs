using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomonitoring
{
    public class Pollutant
    {
        public int Pollutant_Id;
        public String name;
        public String activity;
        public String ownership;
        public String address;
        public double Kf;
        public double Knas;
        public Pollutant(int Pollutant_Id, string name, string activity, string ownership, string address,
                         double Kf, double Knas)
        {
            this.Pollutant_Id = Pollutant_Id;
            this.name = name;
            this.activity = activity;
            this.ownership = ownership;
            this.address = address;
            this.Kf = Kf;
            this.Knas = Knas;
        }
    }
}
