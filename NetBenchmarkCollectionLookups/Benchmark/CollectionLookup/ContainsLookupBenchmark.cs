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

        // Case 1: Dictionary Lookup (Created on the Fly)
        // - This benchmark creates a dictionary every time before checking for keys.
        // - This is inefficient for repeated lookups, as dictionary creation takes time.
        [Benchmark]
        public int Contains_Multiple_Dictionary()
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

        // Case 2: Dictionary Lookup (Precomputed)
        // - Precomputing a dictionary is useful if you need to check for existence multiple times.
        // - This avoids the cost of re-creating the dictionary every time.
        [Benchmark]
        public int Contains_Multiple_Dictionary_Precomputed()
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

        // Case 3: HashSet Lookup (Created on the Fly)
        // - HashSets provide **O(1) average-time complexity** for lookups.
        // - However, creating a HashSet from a List takes extra time.
        [Benchmark]
        public int Contains_Multiple_HashSet()
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

        // Case 4: HashSet Lookup (Precomputed)
        // - Precomputing a HashSet is beneficial if you need to do multiple lookups.
        // - This avoids the cost of re-creating the HashSet.
        [Benchmark]
        public int Contains_Multiple_HashSet_Precomputed()
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

        // Case 5: List Lookup Using Any()
        // - This performs an O(n) linear search for each lookup.
        // - Inefficient for large datasets or repeated lookups.
        [Benchmark]
        public int Contains_Multiple_List_Any()
        {
            int found = 0;

            foreach (var target in searchTargets)
            {
                if (identityList.Any(i => i.Id == target.Id)) // Linear search!
                {
                    found++;
                }
            }
            return found;
        }

        // Case 6: Dictionary Lookup Using TryGetValue (Created on the Fly)
        // - Using `TryGetValue()` is more efficient than `ContainsKey() + dictionary[target.Id]`.
        // - However, we still re-create the dictionary every time, making it less efficient overall.
        [Benchmark]
        public int Select_Multiple_Dictionary()
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

        // Case 7: List Lookup Using FirstOrDefault()
        // - This is functionally similar to `Any()`, but it also retrieves the object if found.
        // - Like Any(), this performs a **linear search** (O(n) for each lookup).
        [Benchmark]
        public int Select_Multiple_List_FirstOrDefault()
        {
            int found = 0;

            foreach (var target in searchTargets)
            {
                if (identityList.FirstOrDefault(i => i.Id == target.Id) is not null) // Linear search!
                {
                    found++;
                }
            }
            return found;
        }

        // Global setup: Precompute collections for benchmarks that use them.
        // - This ensures a fair comparison between "precomputed" and "on-the-fly" versions.
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