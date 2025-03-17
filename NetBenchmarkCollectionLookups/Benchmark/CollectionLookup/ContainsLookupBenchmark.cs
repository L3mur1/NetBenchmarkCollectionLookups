using BenchmarkDotNet.Attributes;

namespace NetBenchmarkCollectionLookups.Benchmark.CollectionLookup
{
    [MemoryDiagnoser]
    public class ContainsLookupBenchmark
    {
        private const int NumberOfItemsToFind = 1000;
        private const int TotalNumberOfItems = 10000;

        private List<Identity> identityList = [];
        private Dictionary<Guid, Identity> precomputedDictionary = [];
        private HashSet<Guid> precomputedHashSet = [];
        private List<Identity> searchTargets = [];

        [Benchmark]
        public int DictionaryContainsKeyMultiple()
        {
            var dictionary = identityList.ToDictionary(x => x.Id);
            int found = 0;

            foreach (var target in searchTargets)
            {
                if (dictionary.ContainsKey(target.Id))
                {
                    found++;
                }
            }
            return found;
        }

        [Benchmark]
        public int DictionaryTryGetValueMultiple()
        {
            var dictionary = identityList.ToDictionary(x => x.Id);
            int found = 0;

            foreach (var target in searchTargets)
            {
                if (dictionary.TryGetValue(target.Id, out _))
                {
                    found++;
                }
            }
            return found;
        }

        [Benchmark]
        public int HashSetContainsKeyMultiple()
        {
            var hashSet = identityList.Select(x => x.Id).ToHashSet();
            int found = 0;

            foreach (var target in searchTargets)
            {
                if (hashSet.Contains(target.Id))
                {
                    found++;
                }
            }
            return found;
        }

        [Benchmark]
        public int ListContainsMultiple()
        {
            int found = 0;

            foreach (var target in searchTargets)
            {
                if (identityList.Any(i => i.Id == target.Id)) // Ensure it compares by Id, not reference
                {
                    found++;
                }
            }
            return found;
        }

        [Benchmark]
        public int ListFirstOrDefaultMultiple()
        {
            int found = 0;

            foreach (var target in searchTargets)
            {
                if (identityList.FirstOrDefault(i => i.Id == target.Id) is not null)
                {
                    found++;
                }
            }
            return found;
        }

        [Benchmark]
        public int Precomputed_DictionaryContainsKeyMultiple()
        {
            int found = 0;

            foreach (var target in searchTargets)
            {
                if (precomputedDictionary.ContainsKey(target.Id))
                {
                    found++;
                }
            }
            return found;
        }

        [Benchmark]
        public int Precomputed_HashSetContainsMultiple()
        {
            int found = 0;

            foreach (var target in searchTargets)
            {
                if (precomputedHashSet.Contains(target.Id))
                {
                    found++;
                }
            }
            return found;
        }

        [GlobalSetup]
        public void Setup()
        {
            identityList = [.. Identity.Generate(TotalNumberOfItems)];
            precomputedHashSet = [.. identityList.Select(i => i.Id)];
            precomputedDictionary = identityList.ToDictionary(x => x.Id);
            searchTargets = [.. identityList.OrderBy(_ => Guid.NewGuid()).Take(NumberOfItemsToFind)];
        }
    }
}