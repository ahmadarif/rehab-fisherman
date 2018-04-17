using System;
using UnityEngine;

public class Angle
{

    /**
     * Menghitung besar sudut tengah dari 3 titik
     * Hasil ada besar sudut di titik B (antara BA dan BC)
     * @param a Vector3
     * @param b Vector3
     * @param c Vector3
     * @return double
     */
    public static double calculate(Vector3 a, Vector3 b, Vector3 c) {
        Vector3 ba, bc;
        double baAbs, bcAbs, cosB;

        ba = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        bc = new Vector3(c.x - b.x, c.y - b.y, c.z - b.z);

        baAbs = Math.Sqrt(Math.Pow(ba.x, 2) + Math.Pow(ba.y, 2) + Math.Pow(ba.z, 2));
        bcAbs = Math.Sqrt(Math.Pow(bc.x, 2) + Math.Pow(bc.y, 2) + Math.Pow(bc.z, 2));

        cosB = Mathf.Rad2Deg * Math.Acos(((ba.x * bc.x) + (ba.y * bc.y) + (ba.z * bc.z)) / (baAbs * bcAbs));
        return cosB;
    }

    /**
    * Melakukan pengecekan apakah 3 titik tersebut lurus
    * Yang ditengah harus titik b
    * @param a Vector3
    * @param b Vector3
    * @param c Vector3
    * @return boolean
    */
    public static bool isStraight(Vector3 a, Vector3 b, Vector3 c) {
        return calculate(a, b, c) == 180;
    }

    /**
     * Melakukan pengecekan apakah 3 titik tersebut lurus dengan nilai treshold
     * Yang ditengah harus titik b
     * @param a Vector3
     * @param b Vector3
     * @param c Vector3
     * @param treshold int
     * @return boolean
     */
    public static bool isStraight(Vector3 a, Vector3 b, Vector3 c, int treshold) {
        double resultDegrees = calculate(a, b, c);
        double minDegrees = 180 - treshold;
        double maxDegrees = 180 + treshold;
        return resultDegrees >= minDegrees && resultDegrees <= maxDegrees;
    }

}