namespace ActivateAnything.Specs
{
    public class TestTimeDependencyAttribute : Xunit.FactAttribute 
    { 
        public TestTimeDependencyAttribute(){DisplayName="[TestTime Dependency]";}
        public TestTimeDependencyAttribute(string detail){DisplayName=$"[TestTime Dependency:{detail}]";}
    }
}

namespace Xunit
{
    public static class WIP 
    {
        public const string Next="WorkInProgress.Next"; 
        public const string Future="WorkInProgress.Future"; 
        public const string Maybe="WorkInProgress.Maybe"; 
    }
}
