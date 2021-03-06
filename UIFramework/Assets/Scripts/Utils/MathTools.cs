using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public static class MathTools {
    /// <summary>
    /// 给出一个int值，给出一个数组长度值，返回一个数组，这个数组的和等于这个int值
    /// 如果长度为0，或者长度大于sum本身，都返回一个长度为一的数组，唯一的元素就是sum本身
    /// </summary>
    /// <param name="sum">给出的数组的和，</param>
    /// <param name="length">指定数组的长度</param>
    /// <returns></returns>
    public static int[] GetArrayBySum(int sum, int length) {
        if (length == 0 || sum <= length) return new[] {sum};

        int average = sum / length;
        int remainder = sum % length;

        int[] result = new int[length];
        for (var i = 0; i < result.Length; i++) {
            result[i] = average;
        }

        result[result.Length - 1] += remainder;
        return result;
    }
}