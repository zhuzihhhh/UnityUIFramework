using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosureAndFuncVariableTest : MonoBehaviour {
    //使用函数变量，而不是类方法，函数变量是一个变量，他保存的函数随时可以改变，
    //每个实例的函数变量可以各自指向不同的函数 ；
    //而一个类型的方法在声明后不可以改变，每个实例的方法都完全相同。
    public Func<int, int, int> IntFunc;

    // Start is called before the first frame update
    void Start() {
        //函数变量必须在一个构造函数或者init方法中初始化自己，而类方法只要在类声明中给出过定义即可
        IntFunc = GetIntFunc(1000);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.A)) {
            Debug.Log($"{IntFunc(4, 5)}");
        }
        else if (Input.GetKeyUp(KeyCode.R)) {
            //可以在适当的时候修改一个函数变量指向的函数，改变其行为，
            //通过闭包，你的函数可以拥有状态，但是这个状态可以被限制为被函数访问，带来便利的同时没有带来副作用，很强大了。
            //如果只能从函数内部访问context，那么如果要重置函数内部的context就比较麻烦，这时候解决办法是完全构造一个新的函数赋值给函数变量。
            IntFunc = GetIntFunc(1000);
        }
    }


    //每个函数变量都是一个具体的变量，所以类的每个实例的函数便令可以指向不同的函数
    //利用闭包，每个函数可以拥有只有自己可见的上下文变量。
    public static Func<int, int, int> GetIntFunc(int startValue) {
        int sum = startValue;
        return (a, b) => {
            int product = a * b;
            sum += product;
            return sum;
        };
    }
}