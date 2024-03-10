using Microsoft.AspNetCore.Components;
using TarkovSauce.Client.Services;

namespace TarkovSauce.Client.Components.Layout
{
    public partial class NavMenu
    {
        [Inject]
        public StateContainer StateContainer { get; set; } = default!;

        private void ChangeState(State state)
        {
            if (StateContainer.State.Value == state) return;
            StateContainer.State.Value = state;
        }
    }
}
