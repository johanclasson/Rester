namespace Rester.Tests
{
    public abstract class DataFixture
    {
        protected ServiceConfigurationBuilder CreateBuilder()
        {
            return new ServiceConfigurationBuilder();
        }
    }
}