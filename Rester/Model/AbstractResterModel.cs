using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Rester.Service;

namespace Rester.Model
{
    public abstract class AbstractResterModel : ObservableObject
    {
        protected AbstractResterModel(bool editMode) : this()
        {
            _editMode = editMode;
        }

        protected AbstractResterModel()
        {
            Messenger.Default.Register<EditModeChangedMessage>(this, message => EditMode = message.Content);
        }

        public bool EditMode { get { return _editMode; } private set { Set(nameof(EditMode), ref _editMode, value); } }
        private bool _editMode;
    }
}