using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace GH.DD.Cache.Tests
{
    public class MemoryCacheTest
    {
        private MemoryCache _cache;
        private const string FirstCacheKey = "firstCacheKey";
        private const string SecondCacheKey = "secondCacheKey";
        private IReadOnlyDictionary<int, IList<int>> _firstValue = new Dictionary<int, IList<int>>
        {
            { 0, new List<int> {1,2,3}},
            { 1, new List<int> {1,2,3}},
            { 2, new List<int> {1,2,3}},
            { 3, new List<int> {1,2,3}},
            { 4, new List<int> {1,2,3}},
        }; 
        private IReadOnlyDictionary<int, IList<int>> _secondValue = new Dictionary<int, IList<int>>
        {
            { 0, new List<int> {1,2,3,4,5}},
            { 1, new List<int> {1,2,3,4,5}},
            { 2, new List<int> {1,2,3,4,5}},
            { 3, new List<int> {1,2,3,4,5}},
            { 4, new List<int> {1,2,3,4,5}},
            { 5, new List<int> {1,2,3,4,5}},
            { 6, new List<int> {1,2,3,4,5}},
            { 7, new List<int> {1,2,3,4,5}},
            { 8, new List<int> {1,2,3,4,5}},
            { 9, new List<int> {1,2,3,4,5}},
        }; 
        
        [SetUp]
        public void Setup()
        {
            _cache = new MemoryCache();
        }

        [Test]
        public void Test1()
        {
            IReadOnlyDictionary<int, IList<int>> firstValue = new Dictionary<int, IList<int>>
            {
                { 0, new List<int> {1,2,3}},
                { 1, new List<int> {1,2,3}},
                { 2, new List<int> {1,2,3}},
                { 3, new List<int> {1,2,3}},
                { 4, new List<int> {1,2,3}},
            }; 
            _cache.Set(FirstCacheKey, firstValue, new TimeSpan(0,0,0,10));

            var t1 = Task.Run(() => Test1_Thread());
            var t2 = Task.Run(() => Test1_Thread());
            var t3 = Task.Run(() => Test1_Thread());

            Task.WaitAll(t1, t2, t3);
            
            Assert.Pass();
        }

        private void Test1_Thread()
        {
            Thread.Sleep(100);
            if (!_cache.TryGet(FirstCacheKey, out var value))
                Assert.Fail($"Value {FirstCacheKey} not found in cache");

            for (int i = 1; i < 50; i++)
            {
                Thread.Sleep(100);
                
                value.Should().BeEquivalentTo(_firstValue, $"Value in cache not equivalent to FirstValue");
            }
        }

        private void UpdateCache(ICacheEntry entry)
        {
            Thread.Sleep(new TimeSpan(0,0,0,1));
            
            IReadOnlyDictionary<int, IList<int>> newValue = new Dictionary<int, IList<int>>
            {
                { 0, new List<int> {1,2,3,4,5}},
                { 1, new List<int> {1,2,3,4,5}},
                { 2, new List<int> {1,2,3,4,5}},
                { 3, new List<int> {1,2,3,4,5}},
                { 4, new List<int> {1,2,3,4,5}},
                { 5, new List<int> {1,2,3,4,5}},
                { 6, new List<int> {1,2,3,4,5}},
                { 7, new List<int> {1,2,3,4,5}},
                { 8, new List<int> {1,2,3,4,5}},
                { 9, new List<int> {1,2,3,4,5}},
            }; 
            
            entry.UpdateValue(newValue);
        }
    }
}