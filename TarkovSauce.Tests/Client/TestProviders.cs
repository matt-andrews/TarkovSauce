using NSubstitute;
using TarkovSauce.Client.Data.EventArgs;
using TarkovSauce.Client.Data.Providers;
using TarkovSauce.Client.HttpClients;

namespace TarkovSauce.Tests.Client
{
    internal class TestProviders
    {
        [Test]
        public void TestRawLogProviderEventTrigger()
        {
            bool isHit = false;
            var provider = new RawLogProvider();
            provider.OnStateChanged += () =>
            {
                isHit = true;
            };
            provider.AppendLog("hello");
            Assert.Multiple(() =>
            {
                Assert.That(isHit, Is.True);
                Assert.That(provider.RawLogs, Does.Contain("hello"));
            });
        }
        [Test]
        public async Task TestFleaSalesProvider_SoldEventTrigger()
        {
            var httpClient = Substitute.For<ITarkovDevHttpClient>();
            bool isHit = false;
            var provider = new FleaSalesProvider(httpClient);
            provider.OnStateChanged += () =>
            {
                isHit = true;
            };
            await provider.AppendSale(new FleaSoldMessageEventArgs());
            Assert.Multiple(() =>
            {
                Assert.That(isHit, Is.True);
                Assert.That(provider.Events, Has.Count.EqualTo(1));
            });
        }
        [Test]
        public async Task TestFleaSalesProvider_ExpiryEventTrigger()
        {
            var httpClient = Substitute.For<ITarkovDevHttpClient>();
            bool isHit = false;
            var provider = new FleaSalesProvider(httpClient);
            provider.OnStateChanged += () =>
            {
                isHit = true;
            };
            await provider.AppendExpiry(new FleaExpiredeMessageEventArgs()
            {
                Message = new()
                {
                    Items = new TarkovSauce.Client.Data.Models.MessageItems()
                    {
                        Data =
                            [
                                new TarkovSauce.Client.Data.Models.LoadoutItem()
                                {
                                    Id = "1"
                                }
                            ]
                    }
                }
            });
            Assert.Multiple(() =>
            {
                Assert.That(isHit, Is.True);
                Assert.That(provider.Events, Has.Count.EqualTo(1));
            });
        }
    }
}
