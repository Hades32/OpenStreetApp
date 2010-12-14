using System;

namespace OpenStreetApp
{
    public class CloudeMadeTileSource : Microsoft.Phone.Controls.Maps.TileSource
    {
        public CloudeMadeTileSource()
        {
        }

        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            return new Uri("http://tile.cloudmade.com/" + CloudeMadeService.ApiKey + "/1/256/"
                    + zoomLevel + "/" + x + "/" + y + ".png?token=" + CloudeMadeService.Token);
        }
        /*
        protected override void GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY, System.Collections.Generic.IList<object> tileImageLayerSources)
        {
            var zoom = tileLevel - 8; // No idea why this starts at 8 but... well.....
            if (zoom > 0 && zoom < 18)
                tileImageLayerSources.Add();
        }*/

        public override string ToString()
        {
            return "Cloudemade Maps";
        }
    }
}
