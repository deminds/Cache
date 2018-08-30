using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace GH.DD.Cache.Tests
{
    public class MemoryCacheTest
    {
        private const string FirstValueInCache = "FirstValue in cache";
        private const string SecondValueInCache = "SecondValue in cache";
        private const string NullValueInCache = "Null value in cache";
        private const string StartUpdateValue = "Start update value";
        private const string StopUpdateValue = "Stop update value";
        private const string StartGcMessage = "Start GC";
        private const string StopGcMessage = "Stop GC";
        
        private int tsNow;
        private MemoryCache _cache;
        private IReadOnlyDictionary<int, int> _firstValue;
        private IReadOnlyDictionary<int, int> _secondValue;

        private TimeSpan _updateCacheDelay;

        private List<TestLogElement> _log;

        private object _locker = new object();

        private TimeSpan _maxRequestTime = TimeSpan.MinValue;
        
        [SetUp]
        public void Setup()
        {
            _cache = new MemoryCache();
            tsNow = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            
            _firstValue = new Dictionary<int, int>
            {
                { 0, 1 },
                { 1, tsNow },
            }; 
            _secondValue = new Dictionary<int, int>
            {
                { 10, tsNow },
                { 11, 10 },
                { 12, 20 },
            }; 
            
            _log = new List<TestLogElement>();
        }

        [Test]
        [Order(1)]
        public void InfinityReadWithOneUpdate()
        {
            _log = new List<TestLogElement>();
            _maxRequestTime = TimeSpan.MinValue;
            
            IReadOnlyDictionary<int, int> firstValue = new Dictionary<int, int>
            {
                { 0, 1 },
                { 1, tsNow },
            };
            _updateCacheDelay = TimeSpan.FromMilliseconds(3000);
            
            _cache.Set("firstCacheKey", firstValue, 5, UpdateCache);
            
            var threads = RunThreads(TestThreadInfinityRead, 200);
            JoinThreads(threads);
            
            // Check log
            var message = $"Need handle check!!!\nMax Request Time: {_maxRequestTime.TotalMilliseconds}ms\n\n" +
                          string.Join("\n", _log.Select(e => e));
            var logCount = _log.Count;
            if (logCount < 4)
                Assert.Fail($"Log of events is not full\n\n{message}");
            
            if (_maxRequestTime > _updateCacheDelay/2)
                Assert.Fail($"Max request time to long. Maybe update cache is synchronous\n\n{message}");
            
            if (_log[0].Event != FirstValueInCache)
                Assert.Fail($"FirstValue must be in [0] element of log\n\n{message}");
            
            if (_log[1].Event != StartUpdateValue)
                Assert.Fail($"StartUpdate must be in [1] element of log\n\n{message}");

            if (_log[2].Event != FirstValueInCache)
                Assert.Fail($"FirstValue must be in [2] element of log\n\n{message}");
            
            if (_log[3].Event != StopUpdateValue)
                Assert.Fail($"StopUpdate must be in [3] element of log\n\n{message}");
            
            if (_log.Count(l => l.Event == StartUpdateValue) != 1)
                Assert.Fail($"Not one StartUpdate element in log\n\n{message}");
            
            if (_log.Count(l => l.Event == StopUpdateValue) != 1)
                Assert.Fail($"Not one StopUpdate element in log\n\n{message}");
            
            if (_log[logCount-1].Event != SecondValueInCache)
                Assert.Fail($"SecondValue must be int [last] element in log\n\n{message}");
            
            Assert.Pass(message);
        }
        
        [Test]
        [Order(2)]
        public void OneReadWithTwoUpdate()
        {
            _log = new List<TestLogElement>();
            _maxRequestTime = TimeSpan.MinValue;
            
            IReadOnlyDictionary<int, int> firstValue = new Dictionary<int, int>
            {
                { 0, 1 },
                { 1, tsNow },
            };
            _updateCacheDelay = TimeSpan.FromMilliseconds(3000);
            
            _cache.Set("firstCacheKey", firstValue, 5, UpdateCache);
            
            var threads = RunThreads(TestThreadOneRead, 200);
            Thread.Sleep(10500);
            
            AppendLog(StartGcMessage);
            
            GC.GetTotalMemory(false);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.GetTotalMemory(true);
            
            AppendLog(StopGcMessage);
            
            JoinThreads(threads);
            
            // Check log
            var message = $"Need handle check!!!\nMax Request Time: {_maxRequestTime.TotalMilliseconds}ms\n\n" +
                          $"{string.Join("\n", _log.Select(e => e))}";
            var logCount = _log.Count;
            if (logCount != 13)
                Assert.Fail($"Log of events is not full\n\n{message}");
            
            if (_maxRequestTime > _updateCacheDelay/2)
                Assert.Fail($"Max request time to long. Maybe update cache is synchronous\n\n{message}");
            
            if (_log[0].Event != FirstValueInCache)
                Assert.Fail($"FirstValue must be in [0] element of log\n\n{message}");
            
            if (_log[1].Event != StartUpdateValue)
                Assert.Fail($"StartUpdate must be in [1] element of log\n\n{message}");
            
            if (_log[2].Event != FirstValueInCache)
                Assert.Fail($"FirstValue must be in [2] element of log\n\n{message}");
            
            if (_log[3].Event != StopUpdateValue)
                Assert.Fail($"StopUpdate must be in [3] element of log\n\n{message}");
            
            if (_log[4].Event != FirstValueInCache)
                Assert.Fail($"FirstValue must be in [4] element of log\n\n{message}");
            
            if (_log[5].Event != StartGcMessage)
                Assert.Fail($"StartGC must be in [5] element of log\n\n{message}");
            
            if (_log[6].Event != FirstValueInCache)
                Assert.Fail($"FirstValue must be in [6] element of log\n\n{message}");
            
            if (_log[7].Event != StopGcMessage)
                Assert.Fail($"StopGC must be in [7] element of log\n\n{message}");
            
            if (_log[8].Event != FirstValueInCache)
                Assert.Fail($"FirstValue must be in [8] element of log\n\n{message}");
            
            if (_log[9].Event != StartUpdateValue)
                Assert.Fail($"StartUpdate must be in [9] element of log\n\n{message}");
            
            if (_log[10].Event != FirstValueInCache)
                Assert.Fail($"FirstValue must be in [10] element of log\n\n{message}");
            
            if (_log[11].Event != StopUpdateValue)
                Assert.Fail($"StopUpdate must be in [11] element of log\n\n{message}");
            
            if (_log[12].Event != FirstValueInCache)
                Assert.Fail($"FirstValue must be in [12] element of log\n\n{message}");
            
            Assert.Pass(message);
        }
        
        [Test]
        [Order(3)]
        public void InfinityReadWithoutUpdate()
        {
            _log = new List<TestLogElement>();
            _maxRequestTime = TimeSpan.MinValue;
            
            IReadOnlyDictionary<int, int> firstValue = new Dictionary<int, int>
            {
                { 0, 1 },
                { 1, tsNow },
            };
            
            _cache.Set("firstCacheKey", firstValue, 5);
            
            var threads = RunThreads(TestThreadInfinityRead, 200);
            JoinThreads(threads);
            
            // Check log
            var message = $"Need handle check!!!\nMax Request Time: {_maxRequestTime.TotalMilliseconds}ms\n\n" +
                          $"{string.Join("\n", _log.Select(e => e))}";
            var logCount = _log.Count;
            if (logCount != 2)
                Assert.Fail($"Log of events is not full\n\n{message}");
            
            if (_maxRequestTime > _updateCacheDelay/2)
                Assert.Fail($"Max request time to long. Maybe update cache is synchronous\n\n{message}");
            
            if (_log[0].Event != FirstValueInCache)
                Assert.Fail($"FirstValue must be in [0] element of log\n\n{message}");
            
            if (_log[1].Event != NullValueInCache)
                Assert.Fail($"Null must be in [1] element of log\n\n{message}");
            
            Assert.Pass(message);
        }
        
        private void TestThreadOneRead()
        {
            var timer = new Stopwatch();
            
            var threadName = Thread.CurrentThread.Name;
            timer.Restart();
            if (!_cache.TryGet("firstCacheKey", out IReadOnlyDictionary<int, int> value))
            {
                timer.Stop();
                SelectMaxReqiestTime(timer.Elapsed);
                AppendLog($"{threadName}. Not found value in cache");
                return;
            }
            timer.Stop();
            SelectMaxReqiestTime(timer.Elapsed);
            
            for (int i = 1; i < 3500; i++)
            {
                Thread.Sleep(5);

                timer.Restart();
                var differentValue = _cache.Get<IReadOnlyDictionary<int, int>>("firstCacheKey");
                timer.Stop();
                SelectMaxReqiestTime(timer.Elapsed);
                
                if (value == null)
                {
                    AppendLog($"{threadName}. Value in cache is null");
                    continue;
                }

                if (CompareCacheValues(_firstValue, value))
                {
                    AppendLog(FirstValueInCache);
                    continue;
                }
                
                if (CompareCacheValues(_secondValue, value))
                {
                    AppendLog(SecondValueInCache);
                    continue;
                }

                AppendLog($"Undefined element in cache. Element: {value}");
            }
        }

        private void TestThreadInfinityRead()
        {
            var timer = new Stopwatch();
            
            var threadName = Thread.CurrentThread.Name;
            timer.Restart();
            if (!_cache.TryGet("firstCacheKey", out IReadOnlyDictionary<int, int> value))
            {
                timer.Stop();
                SelectMaxReqiestTime(timer.Elapsed);
                AppendLog($"{threadName}. Not found value in cache");
                return;
            }
            timer.Stop();
            SelectMaxReqiestTime(timer.Elapsed);
            
            for (int i = 1; i < 2000; i++)
            {
                Thread.Sleep(5);

                timer.Restart();
                value = _cache.Get<IReadOnlyDictionary<int, int>>("firstCacheKey");
                timer.Stop();
                SelectMaxReqiestTime(timer.Elapsed);

                if (value == null)
                {
                    AppendLog(NullValueInCache);
                    continue;
                }

                if (CompareCacheValues(_firstValue, value))
                {
                    AppendLog(FirstValueInCache);
                    continue;
                }
                
                if (CompareCacheValues(_secondValue, value))
                {
                    AppendLog(SecondValueInCache);
                    continue;
                }

                AppendLog($"Undefined element in cache. Element: {value}");
            }
        }

        private object UpdateCache()
        {
            Dictionary<int, int> secondValue = new Dictionary<int, int>
            {
                { 10, tsNow },
                { 11, 10 },
                { 12, 20 },
            };
            AppendLog(StartUpdateValue);
            
            Thread.Sleep(_updateCacheDelay);
            
            AppendLog(StopUpdateValue);
            
            return secondValue;
        }

        private bool CompareCacheValues(IReadOnlyDictionary<int, int> reference, IReadOnlyDictionary<int, int> value)
        {
            if (value.Count != reference.Count)
                return false;

            foreach (var key in value.Keys)
            {
                if (!reference.ContainsKey(key))
                    return false;

                if (value[key] != reference[key])
                    return false;
            }

            return true;
        }

        private IList<Thread> RunThreads(Action action, int count)
        {
            var threads = new List<Thread>();
            for (int i = 0; i < count; i++)
            {
                var thread = new Thread(() => action());
                thread.Name = "t_" + i;
                thread.Start();
                threads.Add(thread);
            }

            return threads;

        }

        private void JoinThreads(IList<Thread> threads)
        {
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
        
        private void AppendLog(string s)
        {
            lock (_locker)
            {
                var count = _log.Count;
                if (count == 0)
                {
                    _log.Add(new TestLogElement(s));
                    return;
                }
                
                if (_log[count-1].Event == s)
                {
                    _log[count-1].IncrementCount();
                    return;
                }
                
                _log.Add(new TestLogElement(s));
            }
        }

        private void SelectMaxReqiestTime(TimeSpan timeSpan)
        {
            lock (_locker)
            {
                if (_maxRequestTime < timeSpan)
                    _maxRequestTime = timeSpan;
            }
        }
    }
}