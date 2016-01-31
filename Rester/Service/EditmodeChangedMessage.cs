using GalaSoft.MvvmLight.Messaging;

namespace Rester.Service
{
    internal class EditModeChangedMessage : GenericMessage<bool>
    {
        public EditModeChangedMessage(bool content) : base(content)
        {
        }
    }
}