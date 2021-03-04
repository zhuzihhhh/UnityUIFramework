using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class OddsGroup<T> : IEnumerable<Odds<T>> {
    private List<Odds<T>> group = new List<Odds<T>>();

    public OddsGroup(IEnumerable<Odds<T>> items = null) {
        if (items == null) {
            return;
        }

        group.AddRange(items);
    }

    public OddsGroup<T> Add(Odds<T> item) {
        @group.Add(item);
        return this;
    }

    public OddsGroup<T> Add(T t, float probability) {
        Odds<T> item = Odds<T>.Create(t, probability);
        return this.Add(item);
    }

    /// <summary>
    /// 按照每个元素的概率，只随机出最多一个，如果每个元素的概率都很低，会什么也得不到
    /// </summary>
    /// <returns></returns>
    public Odds<T> GetOneSuccessOrNull() {
        return @group.Where(i => i.IsSuccess()).ToList().Random();
    }

    /// <summary>
    /// 只随机出一个，且一定会出一个
    /// 解释：圆桌理论，加入多个随机对象的概率总和达到或超过100%，那么一次随机一定会命中且只命中一个随机项目。
    /// 但是，用所有随机项构建“圆桌”比较麻烦，这个算法的本质其实是保证所有随机项目仍然按照其概率进行测试，
    /// 最终保证有一个命中，那么只要按照每个随机项原本的概率进行测试，如果能成功，说明按照其概率成功了，
    /// 当成功结果不止一个时，直接在结果中随机挑选一个，如果全都没有命中，就在所有参与项目中随机挑选一个。
    /// </summary>
    /// <returns></returns>
    public Odds<T> GetOneSuccess() {
        return GetOneSuccessOrNull() ?? @group.Random();
    }

    /// <summary>
    /// 每个元素按照自身几率测试，如果命中则都返回，如果没有命中则返回0个
    /// </summary>
    /// <returns></returns>
    public List<Odds<T>> GetAllSuccess() {
        return @group.Where(i => i.IsSuccess()).ToList();
    }

    /// <summary>
    /// 随机返回N个，此时将忽略几率，完全随机，并返回指定个
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public List<Odds<T>> GetN(int i) {
        if (i >= @group.Count()) {
            return @group.ToList(); //不要把内部对象引用通过方法传出去，尤其是集合类，那就给了外部修改它的漏洞。传一个clone出去，就算修改了也不会有别的副作用
        }

        HashSet<Odds<T>> set = new HashSet<Odds<T>>();
        while (set.Count() < i) {
            set.Add(@group.Random());
        }

        return set.ToList();
    }

    public OddsGroup<T> Remove(Odds<T> item) {
        @group.Remove(item);
        return this;
    }

    public IEnumerator<Odds<T>> GetEnumerator() {
        return group.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}