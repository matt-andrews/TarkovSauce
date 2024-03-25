using Microsoft.AspNetCore.Components;
using TarkovSauce.Client.Data.Providers;
using TarkovSauce.Client.Services;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class FleaSales
    {
        [Inject]
        public StateContainer StateContainer { get; set; } = default!;
        [Inject]
        public IFleaSalesProvider FleaSalesProvider { get; set; } = default!;
        private long _runningTotal;
        protected override void OnInitialized()
        {
            StateContainer.IsLoading.Value = true;
            FleaSalesProvider.InitCache();
            CalcRunningTotal();
            StateContainer.IsLoading.Value = false;
            FleaSalesProvider.OnStateChanged = () =>
            {
                StateContainer.IsLoading.Value = true;
                CalcRunningTotal();
                InvokeAsync(StateHasChanged);

                StateContainer.IsLoading.Value = false;
            };
        }
        private void CalcRunningTotal()
        {
            _runningTotal = FleaSalesProvider.CachedEvents
                .Concat(FleaSalesProvider.Events)
                .Sum(item =>
                {
                    if (item.Currency == "₽")
                    {
                        return item.Reward;
                    }
                    else if (item.Currency == "€")
                    {
                        return item.Reward * 161;
                    }
                    else if (item.Currency == "$")
                    {
                        return item.Reward * 146;
                    }
                    return 0;
                });
        }
        private double CalculateRep()
        {
            return 0.20 + (_runningTotal / 50_000) * 0.01;
        }
        private long CalcNextRepMilestone()
        {
            double currentRep = CalculateRep();
            double nextRep = 0;
            if (currentRep < 0.2)
                nextRep = 0.2;
            else if (currentRep < 7)
                nextRep = 7;
            else if (currentRep < 30)
                nextRep = 30;
            else if (currentRep < 60)
                nextRep = 60;
            else if (currentRep < 100)
                nextRep = 100;
            else if (currentRep < 150)
                nextRep = 150;
            else if (currentRep < 1000)
                nextRep = 1000;
            return (int)Math.Round(((nextRep - currentRep) / 0.01) * 50_000);
        }
    }
}
