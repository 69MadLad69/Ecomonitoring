using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomonitoring.RisksCalasses.NonCarcinogenicRisk
{
    public class NonCarcinogenicRisk
    {
        public int riskId;
        public int pollution_Id;
        public double HQ;
        public NonCarcinogenicRisk(int riskId, int pollution_Id, double HQ)
        {
            this.riskId = riskId;
            this.pollution_Id = pollution_Id;
            this.HQ = HQ;
        }
    }
}
