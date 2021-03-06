using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class UtilsTest : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        int sum = 100;
        int length = 10;
        int length2 = 7;
        int length3 = 100;
        int length4 = 120;

        int length5 = 1;

        var r1 = MathTools.GetArrayBySum(sum, length);
        var r2 = MathTools.GetArrayBySum(sum, length2);
        var r3 = MathTools.GetArrayBySum(sum, length3);
        var r4 = MathTools.GetArrayBySum(sum, length4);
        var r5 = MathTools.GetArrayBySum(sum, length5);
        Print(r1);
        Print(r2);
        Print(r3);
        Print(r4);
        Print(r5);
    }
    

    void Print(int[] arr) {
        for (int i = 0; i < arr.Length; i++) {
            print(arr[i]);
        }
        print("----------");
    }
}