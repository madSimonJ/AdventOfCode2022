namespace xmas
{
    public static class FunctionalExtensions
    {
        public static TOut Bind<TIn, TOut>(this TIn @this, Func<TIn, TOut> f) => f(@this);

        public static IEnumerable<TOut> BindSelect<TIn, TOut>(this IEnumerable<TIn> @this,
            Func<IEnumerable<TIn>, TIn, TOut> f) =>
            @this.ToArray().Bind(x => x.Select(y => f(x, y)));

        public static T Tap<T>(this T @this, Action<T> a)
        {
            a(@this);
            return @this;
        }
    }
}
