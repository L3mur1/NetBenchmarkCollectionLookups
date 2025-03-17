namespace NetBenchmarkCollectionLookups
{
    public record Identity(Guid Id)
    {
        public static Identity Create() => new Identity(Guid.NewGuid());

        public static IEnumerable<Identity> Generate(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return Create();
            }
        }
    }
}