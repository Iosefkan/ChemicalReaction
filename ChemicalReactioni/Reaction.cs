using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using ScottPlot.Drawing.Colormaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ode = MathNet.Numerics.OdeSolvers;

namespace ChemicalReactioni
{
    internal class Reaction
    {
        public double MatBalanceComponentA(double t, double Ca)
        {
            return (Q * (Cain - Ca) + V * (-r1 + r2 - (2 * r3))) / V;
        }
        public double MatBalanceComponentB(double t, double Cb)
        {
            return (Q * (-Cb) + V * (r1 - r2)) / V;
        }
        public double MatBalanceComponentC(double t, double Cc)
        {
            return (Q * (-Cc) + V * (r1 - r2)) / V;
        }
        public double MatBalanceComponentD(double t, double Cd)
        {
            return (Q * (-Cd) + V * r3) / V;
        }
        public double Ca { get; set; } = 0;
        public double Cb { get; set; } = 0;
        public double Cc { get; set; } = 0;
        public double Cd { get; set; } = 0;
        private static double R { get; } = 8.31;
        public double E1 { get; set; } = 0;
        public double E2 { get; set; } = 0;
        public double E3 { get; set; } = 0;
        public double k01 { get; set; } = 0;
        public double k02 { get; set; } = 0;
        public double k03 { get; set; } = 0;
        public double V { get; set; } = 0;
        public double Cain { get; set; } = 0;
        public double Q { get; set; } = 0;
        public double T { get; set; } = 0;
        public double k1 { 
            get
            {
                return k01 * Math.Pow(Math.E, (-E1) / (R * (T + 273)));
            } 
        }
        public double k2
        {
            get
            {
                return k02 * Math.Pow(Math.E, (-E2) / (R * (T + 273)));
            }
        }
        public double k3
        {
            get
            {
                return k03 * Math.Pow(Math.E, (-E3) / (R * (T + 273)));
            }
        }
        public double tau 
        { 
            get
            {
                return V / Q;
            } 
        }
        public double theta
        {
            get
            {
                return 3 * tau;
            }
        }
        public double r1
        {
            get
            {
                return k1 * Ca;
            }
        }
        public double r2
        {
            get
            {
                return k2 * Cb * Cc;
            }
        }
        public double r3
        {
            get
            {
                return k3 * Ca * Ca;
            }
        }
    }
    

}
