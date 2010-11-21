using System;
using System.Windows.Media;

namespace OpenStreetApp
{
    public class CloudeMadeTileSource : MultiScaleTileSource
    {
        public CloudeMadeTileSource()
            : base(0x8000000, 0x8000000, 256, 256, 0) //no idea why 0x8000000, but it works :)
        {
        }

        protected override void GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY, System.Collections.Generic.IList<object> tileImageLayerSources)
        {
            var zoom = tileLevel - 8; // No idea why this starts at 8 but... well.....
            if (zoom > 0 && zoom < 18)
                tileImageLayerSources.Add(new Uri("http://tile.cloudmade.com/" + CloudeMadeService.ApiKey + "/1/256/"
                    + zoom + "/" + tilePositionX + "/" + tilePositionY + ".png?token=" + CloudeMadeService.Token));
        }
    }
}
