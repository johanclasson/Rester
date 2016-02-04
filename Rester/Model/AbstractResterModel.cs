using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Rester.Service;

namespace Rester.Model
{
    internal class SomethingIsChangedMessage { }

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

        protected void SetAndSave<T>(string propertyName, ref T field, T newValue)
        {
            Set(propertyName, ref field, newValue);
            Messenger.Default.Send(new SomethingIsChangedMessage());
        }
    }
}