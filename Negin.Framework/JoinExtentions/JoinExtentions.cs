
namespace Negin.Framework.JoinExtentions;

public static class JoinExtentions
{
    private static IEnumerable<TResult> RightOuterJoin<TLeft, TRight, TKey, TResult>(
        this IEnumerable<TLeft> leftItems,
        IEnumerable<TRight> rightItems,
        Func<TLeft, TKey> leftKeySelector,
        Func<TRight, TKey> rightKeySelector,
        Func<TLeft, TRight, TResult> resultSelector)
    {

        return from right in rightItems
               join left in leftItems on rightKeySelector(right) equals leftKeySelector(left) into temp
               from left in temp.DefaultIfEmpty()
               select resultSelector(left, right);
    }

    private static IEnumerable<TResult> RightAntiSemiJoin<TLeft, TRight, TKey, TResult>(
        this IEnumerable<TLeft> leftItems,
        IEnumerable<TRight> rightItems,
        Func<TLeft, TKey> leftKeySelector,
        Func<TRight, TKey> rightKeySelector,
        Func<TLeft, TRight, TResult> resultSelector)
    {

        var hashLK = new HashSet<TKey>(from l in leftItems select leftKeySelector(l));
        return rightItems.Where(r => !hashLK.Contains(rightKeySelector(r))).Select(r => resultSelector(default(TLeft), r));
    }

    public static IEnumerable<TResult> LeftOuterJoin<TLeft, TRight, TKey, TResult>(
    this IEnumerable<TLeft> leftItems,
    IEnumerable<TRight> rightItems,
    Func<TLeft, TKey> leftKeySelector,
    Func<TRight, TKey> rightKeySelector,
    Func<TLeft, TRight, TResult> resultSelector)
    {

        return from left in leftItems
               join right in rightItems on leftKeySelector(left) equals rightKeySelector(right) into temp
               from right in temp.DefaultIfEmpty()
               select resultSelector(left, right);
    }

    public static IEnumerable<TResult> FullOuterJoin<TLeft, TRight, TKey, TResult>(
       this IEnumerable<TLeft> leftItems,
       IEnumerable<TRight> rightItems,
       Func<TLeft, TKey> leftKeySelector,
       Func<TRight, TKey> rightKeySelector,
       Func<TLeft, TRight, TResult> resultSelector) where TLeft : class
    {

        return leftItems.LeftOuterJoin(rightItems, leftKeySelector, rightKeySelector, resultSelector).Concat(leftItems.RightAntiSemiJoin(rightItems, leftKeySelector, rightKeySelector, resultSelector));
    }

}
