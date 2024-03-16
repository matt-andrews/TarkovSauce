using TarkovSauce.MapTools;

namespace TarkovSauce.Tests.MapTools
{
    internal class MapToolsTests
    {
        private readonly IMapTools _mapTools;
        public MapToolsTests()
        {
            _mapTools =
                new ServiceCollection()
                    .AddMapTools()
                    .AddMap(options =>
                    {
                        options.Name = "Customs";
                        options.BaseImage = "fake/path.png";
                        options.AddAnchor(new GameCoord(675.8f, 8.1f, 126.0f), new MapCoord(48, 912, 0));
                        options.AddAnchor(new GameCoord(-328.8f, 3.0f, -236.0f), new MapCoord(2239, 139, 0));
                    });
        }
        [TestCase(656.5f, 2.6f, -167.1f,/**/ 90, 286, 0)]
        [TestCase(401.5f, 14.8f, 214.7f,/**/ 646, 1101, 0)]
        [TestCase(182.2f, -0.2f, 214.8f,/**/ 1125, 1102, 0)]
        [TestCase(207.3f, 5.6f, -238.6f,/**/ 1070, 133, 0)]
        [TestCase(-325.4f, 0.8f, -68.4f,/**/ 2232, 497, 0)]
        [TestCase(-142.3f, 1.9f, 47.7f, /**/ 1832, 745, 0)]
        [TestCase(-340.2f, 1.2f, -84.7f,/**/ 2264, 462, 0)]
        public void TestCustomsGetPos(float inX, float inY, float inZ, int outX, int outY, int outZ)
        {
            var map = _mapTools.GetMap("Customs");
            var mapPos = map.GetPos(new GameCoord(inX, inY, inZ));
            Console.WriteLine(mapPos);
            Assert.That(mapPos, Is.EqualTo(new MapCoord(outX, outY, outZ)));
        }
    }
}
