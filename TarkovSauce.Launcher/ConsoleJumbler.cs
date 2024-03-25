namespace TarkovSauce.Launcher
{
    internal class ConsoleJumbler
    {
        public ConsoleJumbler()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;
        }

        private readonly string _jumblerChars = "ЀЁЂЃЄЅІЇЈЉЊЋЌЍЎЏАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        private Random _random = new();
        private int _cursorLeft = 0;
        private int _cursorTop = 0;
        public async Task Write(string message)
        {
            _random = new Random();

            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] != ' ')
                {
                    await PrintJumbledCharacter();
                }
                Console.Write(message[i]);
                _cursorLeft++;
            }

        }
        public async Task WriteLine(string message = "")
        {
            await Write(message);
            Console.Write(Environment.NewLine);
            _cursorTop++;
            _cursorLeft = 0;
        }

        public async Task Load(Func<Task> action)
        {
            bool finished = false;
            _ = Task.Run(async () =>
            {
                await action.Invoke();
                finished = true;
            });
            while (!finished)
            {
                await Write(".");
            }
            await WriteLine();
        }

        public async Task<T> Load<T>(Func<Task<T>> action)
            where T : class
        {
            T? result = default;
            _ = Task.Run(async () =>
            {
                result = await action.Invoke();
            });
            while (result == default)
            {
                await Write(".");
            }
            await WriteLine();
            return result;
        }

        private async Task PrintJumbledCharacter()
        {
            for (int i = 0; i < _random.Next(2, 5); i++)
            {
                Console.Write(GetChar());
                await Task.Delay(_random.Next(10, 20));
                if (_cursorLeft >= Console.BufferWidth)
                {
                    _cursorLeft = 0;
                    _cursorTop++;
                }
                Console.SetCursorPosition(_cursorLeft, _cursorTop);
            }
        }

        private char GetChar()
        {
            var rng = _random.Next(0, _jumblerChars.Length);
            return _jumblerChars[rng];
        }
    }
}
