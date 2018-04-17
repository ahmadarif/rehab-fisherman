using System;

public class LinearRegression {

    public double[] X;
    public double[] Y;
    public double a;
    public double b;
    public double R2;

    public LinearRegression(double[] x, double[] y) {
        X = x;
        Y = y;
    }

    public void process() {
        double n, sumX, sumY, sumX2, sumY2, sumXY;

        n = X.Length;
        sumX = sumY = sumX2 = sumY2 = sumXY = 0;

        // hitung sigma masing-masing nilai
        for (int i = 0; i < n; i++) {
            sumX += X[i];
            sumY += Y[i];
            sumX2 += Math.Pow(X[i], 2);
            sumY2 += Math.Pow(Y[i], 2);
            sumXY += X[i] * Y[i];
        }

        a = ((sumY * sumX2) - (sumX * sumXY)) / ((n * sumX2) - Math.Pow(sumX, 2));
        b = ((n * sumXY) - (sumX * sumY)) / ((n * sumX2) - Math.Pow(sumX, 2));
        R2 = Math.Pow((n * sumXY) - (sumX * sumY), 2) / (((n * sumX2) - Math.Pow(sumX, 2)) * ((n * sumY2) - Math.Pow(sumY, 2)));
        R2 = Math.Round(R2, 2, MidpointRounding.AwayFromZero);
    }

    public double predict(double x) {
        return Math.Round(a + (b * x), 2, MidpointRounding.AwayFromZero);
    }

}