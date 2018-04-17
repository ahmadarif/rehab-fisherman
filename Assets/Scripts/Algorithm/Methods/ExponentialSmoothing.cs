using System;
using UnityEngine;

public class ExponentialSmoothing {
    private double[] data;
    private double alpha;

    public ExponentialSmoothing(double[] data, double alpha) {
        this.data = data;
        this.alpha = alpha;
    }

    public double SES() {
        double[] f = new double[data.Length];
        f[0] = data[0];

//        Debug.Log(String.Format("S1 = %.2f", f[0]));
        for (int i = 1; i < data.Length; i++) {
            f[i] = (alpha * data[i]) + ((1 - alpha) * f[i - 1]);
//            Debug.Log(String.Format("S%d = %.2f(%.2f) + %.2f(%.2f) = %.2f", (i + 1), alpha, data[i], (1 - alpha), f[i - 1], f[i]));
        }

        return Math.Round((alpha * data[data.Length - 1]) + ((1 - alpha) * f[data.Length - 1]), 2, MidpointRounding.AwayFromZero);
    }

    public double DES() {
        double[] s1 = new double[data.Length];
        double[] s2 = new double[data.Length];
        double[] a = new double[data.Length];
        double[] b = new double[data.Length];
        s1[0] = s2[0] = data[0];

//        Debug.Log("S't = (Alpha * Xt) + ((1 - Alpha) S't-1)");
//        Debug.Log(String.Format("S'1 = %.2f", s1[0]));
        for (int i = 1; i < data.Length; i++) {
            s1[i] = (alpha * data[i]) + ((1 - alpha) * s1[i - 1]);
//            Debug.Log(String.Format("S'%d = %.2f(%.2f) + %.2f(%.2f) = %.2f", (i + 1), alpha, data[i], (1 - alpha), s1[i - 1], s1[i]));
        }

//        Debug.Log("S\"t = (Alpha * S't) + ((1 - Alpha) * S\"t-1)");
//        Debug.Log(String.Format("S\"1 = %.2f", s2[0]));
        for (int i = 1; i < data.Length; i++) {
            s2[i] = (alpha * s1[i]) + ((1 - alpha) * s2[i - 1]);
//            Debug.Log(String.Format("S\"%d = %.2f(%.2f) + %.2f(%.2f) = %.2f", (i + 1), alpha, s1[i], (1 - alpha), s2[i - 1], s2[i]));
        }

//        Debug.Log("at = 2S't - S\"t");
        for (int i = 0; i < data.Length; i++) {
            a[i] = (2 * s1[i]) - s2[i];
//            Debug.Log(String.Format("a%d = 2(%.2f) - %.2f = %.2f", (i + 1), s1[i], s2[i], a[i]));
        }

//        Debug.Log("bt = (Alpha / (1 - Alpha)) * (S't - S\"t)");
        for (int i = 0; i < data.Length; i++) {
            b[i] = (alpha / (1 - alpha)) * (s1[i] - s2[i]);
//            Debug.Log(String.Format("b%d = (%.2f / %.2f) * (%.2f - %.2f) = %.2f", (i + 1), alpha, (1 - alpha), s1[i], s2[i], b[i]));
        }

        return Math.Round((a[data.Length - 1] + b[data.Length - 1]), 2, MidpointRounding.AwayFromZero);;
    }
}