using UnityEngine;
using System.Collections;
using System;

public class KalmanFilter {

    private double[] Zk;
    private double[] Xk;
    private double[] Pk;
    private double[] Kk;

    private bool isDebug = false;

    public KalmanFilter (double[] data, bool isDebug)
    {
        this.Zk = data;
        this.isDebug = isDebug;

        Xk = new double[data.Length];
        Pk = new double[data.Length];
        Kk = new double[data.Length];

    }

    public double process(double R)
    {
        double xl, pl;

        if (isDebug) Debug.Log(string.Format("# | val \t\t| xl \t\t| pl \t\t| K \t\t| x2 \t\t| p2"));

        for (int i = 0; i < Zk.Length; i++)
        {
            // Time Update
            if(i == 0)
            {
                Xk[i] = 0;          // estimasi
                Pk[i] = 1;          // kovaransi error
            } else
            {
                Xk[i] = Xk[i - 1];  // estimasi
                Pk[i] = Pk[i - 1];  // kovaransi error
            }

            // Save old data
            xl = Xk[i];
            pl = Pk[i];

            // Measurement Update
            Kk[i] = Pk[i] / (Pk[i] + R);
            Xk[i] = Xk[i] + Kk[i] * (Zk[i] - Xk[i]);
            Pk[i] = (1 - Kk[i]) * Pk[i];

            if (isDebug) Debug.Log(string.Format("{0} | {1} \t| {2} \t| {3} \t| {4} \t| {5} \t| {6} \n ", i + 1, Zk[i], xl, pl, Kk[i], Xk[i], Pk[i]));

        }

        return Xk[Zk.Length - 1];
    }

    
}
