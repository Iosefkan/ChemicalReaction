using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using ScottPlot;
using Ode = MathNet.Numerics.OdeSolvers;

namespace ChemicalReactioni
{
    internal static class Get
    {
        public static (List<double>, List<double>, List<double>, List<double>) GetConcentrationsKutta(Reaction reaction, double[] time)
        {
            List<double> y1 = new();
            List<double> y2 = new();
            List<double> y3 = new();
            List<double> y4 = new();
            foreach (var t in time)
            {
                y1.Add(reaction.Ca);
                reaction.Ca = NumericalMethods.RungeKutta(0, reaction.Ca, t, (time[1] - time[0]), reaction.MatBalanceComponentA);
                y2.Add(reaction.Cb);
                reaction.Cb = NumericalMethods.RungeKutta(0, reaction.Cb, t, (time[1] - time[0]), reaction.MatBalanceComponentB);
                y3.Add(reaction.Cc);
                reaction.Cc = NumericalMethods.RungeKutta(0, reaction.Cc, t, (time[1] - time[0]), reaction.MatBalanceComponentC);
                y4.Add(reaction.Cd);
                reaction.Cd = NumericalMethods.RungeKutta(0, reaction.Cd, t, (time[1] - time[0]), reaction.MatBalanceComponentD);

            }
            return (y1, y2, y3, y4);
        }
        public static (List<double>, List<double>, List<double>, List<double>) GetConcentrationsEuler(Reaction reaction, double[] time)
        {
            List<double> y1 = new();
            List<double> y2 = new();
            List<double> y3 = new();
            List<double> y4 = new();
            y1.Add(reaction.Ca);
            y2.Add(reaction.Cb);
            y3.Add(reaction.Cc);
            y4.Add(reaction.Cd);

            foreach (var t in time)
            {

                reaction.Ca = NumericalMethods.Euler(0, reaction.Ca, 0.5, t, reaction.MatBalanceComponentA);
                y1.Add(reaction.Ca);

                reaction.Cb = NumericalMethods.Euler(0, reaction.Cb, 0.5, t, reaction.MatBalanceComponentB);
                y2.Add(reaction.Cb);

                reaction.Cc = NumericalMethods.Euler(0, reaction.Cc, 0.5, t, reaction.MatBalanceComponentC);
                y3.Add(reaction.Cc);

                reaction.Cd = NumericalMethods.Euler(0, reaction.Cd, 0.5, t, reaction.MatBalanceComponentD);
                y4.Add(reaction.Cd);

            }
            return (y1, y2, y3, y4);
        }
        public static (double[], double[], double[], double[], double[]) GetConcentrationsMathNet(Reaction reaction, double fromTime, int dotsCount)
        {
            double[] y0arr = new double[] { reaction.Ca, reaction.Cb, reaction.Cc, reaction.Cd };
            Vector<double> y0 = Vector<double>.Build.DenseOfArray(y0arr);
            Func<double, Vector<double>, Vector<double>> odeSystem = (t, Z) =>
            {
                double[] A = Z.ToArray();
                double Ca = A[0];
                double Cb = A[1];
                double Cc = A[2];
                double Cd = A[3];
                reaction.Ca = Ca;
                reaction.Cb = Cb;
                reaction.Cc = Cc;
                reaction.Cd = Cd;
                return Vector<double>.Build.Dense(new[] {
                    reaction.MatBalanceComponentA(t, reaction.Ca),
                reaction.MatBalanceComponentB(t, reaction.Cb),
                reaction.MatBalanceComponentC(t, reaction.Cc),
                reaction.MatBalanceComponentD(t, reaction.Cd)
                });

            };
            var res = Ode.RungeKutta.FourthOrder(y0, fromTime, reaction.theta, dotsCount, odeSystem);
            
            List<double> y1 = new();
            List<double> y2 = new();
            List<double> y3 = new();
            List<double> y4 = new();
            for (int i = 0; i < dotsCount; i++)
            {
                var temp = res[i].ToList();
                y1.Add(temp[0]);
                y2.Add(temp[1]);
                y3.Add(temp[2]);
                y4.Add(temp[3]);
            }
            var time = DataGen.Consecutive(dotsCount, (reaction.theta - fromTime) / dotsCount, fromTime);
            return (time, y1.ToArray(), y2.ToArray(), y3.ToArray(), y4.ToArray());
        }
    }
}
