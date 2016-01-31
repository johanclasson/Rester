using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Rester.Service;

namespace Rester.Model
{
    internal abstract class AbstractResterModel : ObservableObject
    {
        public AbstractResterModel(bool editMode) : this()
        {
            _editMode = editMode;
        }

        public AbstractResterModel()
        {
            Messenger.Default.Register<EditModeChangedMessage>(this, message => EditMode = message.Content);
        }

        public bool EditMode { get { return _editMode; } private set { Set(nameof(EditMode), ref _editMode, value); } }
        private bool _editMode;
    }
}