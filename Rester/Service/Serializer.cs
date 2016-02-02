using Rester.Model;

namespace Rester.Service
{
    internal interface ISerializer
    {
        void Serialize(ServiceConfiguration[] configurations);
    }

    internal class Serializer : ISerializer
    {
        public void Serialize(ServiceConfiguration[] configurations)
        {
        }
    }
}
