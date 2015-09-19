using System.ComponentModel;
using System.Runtime.CompilerServices;
using AnimeXdcc.Wpf.Annotations;

namespace AnimeXdcc.Wpf.Infrastructure.Bindable
{
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SetProperty<T>(ref T member, T value, [CallerMemberName] string propertyName = default(string))
        {
            if (Equals(member, value))
            {
                return;
            }

            member = value;
            OnPropertyChanged(propertyName);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}