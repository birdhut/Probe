namespace Probe.Tests
{
    public class SimpleDependency : ISimpleDependency
    {
        public string DependencyTest()
        {
            return "DependencyWorks";
        }
    }
}
