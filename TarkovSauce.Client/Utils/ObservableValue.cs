namespace TarkovSauce.Client.Utils
{
    public class ObservableValue<T>(T? defaultValue)
    {
        public event Action<T?>? OnValueChanged;
        private T? _value = defaultValue;
        public T? Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    OnValueChanged?.Invoke(value);
                }
            }
        }
    }
}
