namespace xmas
{
    public static class FunctionalExtensions
    {
        public static TOut Bind<TIn, TOut>(this TIn @this, Func<TIn, TOut> f) => f(@this);
    }
}
