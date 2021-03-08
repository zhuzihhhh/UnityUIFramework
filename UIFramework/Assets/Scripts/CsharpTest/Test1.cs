using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class Test1 : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        TestTuple();
    }

    /// <summary>
    /// Tuple主要的用途是返回多个值。 总的来说很实用又不常用
    /// 如果有些只在当前上下文内使用的临时的几个属性集合，可以用tuple代替类型声明
    /// 可以声明每个item的类型，也可以让编译器推断
    /// 可以为每个item起名字，这只是个语法糖，编译之后这些信息会消失（语言本身没有这个概念）
    /// 两个tuple是否相等就是逐个比较item（item是值类型或引用类型都会按照其equals方法进行比较）
    /// 两个tuple是否类型相同就看元素数是否相同，类型顺序是否相同，相同类型的tuple可以相互赋值
    /// tuple支持deconstructor，其实是一种简写，声明几个变量并同时给他们赋值为tuple的items
    /// </summary>
    private void TestTuple() {
        //Allow compiler to infer the element types,也可以明确给出类型
        (string, int) bob = ("Bob Green", 33);

        string name = bob.Item1;
        int age = bob.Item2;

        // 明确给出类型及名字
        (string name, int age) Jim = ("Jim", 25);
        var n = Jim.name;
        var a = Jim.age;

        //让编译器推断类型，但是明确给出item的名字
        var tupleNew = (Name: "Tom", Age: 5);

        //注意，这里是在Deconstruct tuple， 等号左边其实是声明了两个变量，TomName 和 TomAge，并赋值为tupleNew的item1，item2
        //其关键特征是：这个看起来像声明tuple的表达式，最终并没没有声明以讹tuple对象！
        (string TomName, int TomAge) = tupleNew;

        (string, int) fakeBob = ("Bob Green", 33);

        if (fakeBob.Equals(bob)) {
            Debug.Log("equal");
        }


        (string, Transform) tran = ("parent", transform);
        var tran1 = ("parent", transform);
        if (tran.Equals(tran1)) {
            Debug.Log("reference type can be used ,and test equal ");
        }
    }

}