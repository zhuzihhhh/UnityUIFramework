using System;
using UnityEngine;


/*
enum和int之间的转换可以直接强制转换类型；判断一个int值是否在枚举的定义内：Enum.IsDefined(typeof(ENUM_TYPE), intval);
enum和string之间的转换需要用到Enum.GetName 和 Enum.Parse；单个枚举值可以简单的ToString就能得到他的名字
enum可以声明带[Flags] attribute，编程一个flags枚举，每个枚举变量可以是多个enum字段的combine，
可以使用&来判断你的变量是否包含某个枚举字段，可以用|=来添加字段，用^=来删除字段。
 */

#region enum type Samples

//enum底层表示必须是整数，基本上enum的底层值等价于int
public enum Colors {
    Red,
    Green,
    Blue,
    Yellow
};

//默认枚举值从0开始自动递增
public enum BorderSide {
    Left, //0
    Right, //1
    Top, //2
    Bottom //3
};

//可以强制指定每一个enum member 的值
public enum BorderSide_v2 {
    Left = 1,
    Right = 2,
    Top = 10,
    Bottom = 11
}

//可以选择指定个别enum member的值，未指定的值将按照之前最近的明确指定值递增
public enum BorderSide_v1 {
    Left = 1,
    Right, //2
    Top = 10,
    Bottom //11
}

//Flags enum:支持combine的枚举：必须明确指定每个枚举值，是2的n次方
//Flags enum表示一个单独的枚举变量可以同时拥有枚举定义的多个值，
//可以用来定义一组同时可以打开多个的选项，
[Flags]
public enum BorderSides {
    None = 0,
    Left = 1,
    Right = 2,
    Top = 4,
    Bottom = 8
}

#endregion

public class EnumUtils : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        int right_v1 = (int) BorderSide_v1.Right;
        int bottom_v1 = (int) BorderSide_v1.Bottom;
        Debug.Log($"right_v1:{right_v1}");
        Debug.Log($"bottom_v1:{bottom_v1}");


        BorderSides leftRight = BorderSides.Left | BorderSides.Right;

        if ((leftRight & BorderSides.Left) != 0)
            Debug.Log("leftRight 包含 Left");

        string formatted = leftRight.ToString(); //声明为Flags，打印结果为：Left, Right
        Debug.Log(formatted);

        BorderSides side = (BorderSides) 12345;
        bool isDefined = Enum.IsDefined(typeof(BorderSides), side);
        Debug.Log($" {side} is defined? {isDefined}");

        var name = Enum.GetName(typeof(BorderSides), BorderSides.Bottom);
        Debug.Log(name);
        var names = Enum.GetNames(typeof(BorderSides));
        Debug.Log(string.Concat(names));


        Colors r = (Colors) Enum.Parse(typeof(Colors), "Red");
        if (r == Colors.Red) {
            Debug.Log("Success");
        }

        Debug.Log(BorderSides.Bottom.ToString());
    }
}