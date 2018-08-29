namespace GH.DD.Cache.Tests
{
    public class TestLogElement
    {
        public TestLogElement(string @event)
        {
            Event = @event;
            Count = 1;
        }

        public string Event { private set; get; }
        public int Count { private set; get; } = 0;

        public void IncrementCount()
        {
            Count += 1;
        }

        public override string ToString()
        {
            return $"Event: {Event}, Count: {Count}";
        }
    }
}