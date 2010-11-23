using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenStreetApp
{
    public class VEArialTileSource : BaseVETileSource
    {
        public override string UriFormat
        {
            get { return "http://h{0}.ortho.tiles.virtualearth.net/tiles/h{1}.jpeg?g=203"; }
        }

        public override string ToString()
        {
            return "Virtual Earth Aerial";
        }
    }

    public class VERoadTileSource : BaseVETileSource
    {
        public override string UriFormat
        {
            get { return "http://r{0}.ortho.tiles.virtualearth.net/tiles/r{1}.png?g=203"; }
        }

        public override string ToString()
        {
            return "Virtual Earth Maps";
        }
    }

    public abstract class BaseVETileSource : MultiScaleTileSource
    {
        /// <summary>
        /// Setting of tile size and max image size.
        /// </summary>
        protected BaseVETileSource()
            : base(0x8000000, 0x8000000, 0x100, 0x100, 0)
        {
        }

        /// <summary>
        /// Format for map tile uri.
        /// </summary>
        public abstract string UriFormat { get; }

        /// <summary>
        /// Get tiles for <see cref="MultiScaleImage"   />.
        /// </summary>
        /// <param name="tileLevel">Tile Level</param>
        /// <param name="tilePositionX">Tile X position</param>
        /// <param name="tilePositionY">Tile Y position</param>
        /// <param name="tileImageLayerSources">Tile images repository</param>
        protected override void GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY,
                                              IList<object> tileImageLayerSources)
        {
            int zoom = tileLevel - 8;
            if (zoom > 0)
            {
                string QuadKey = TileXYToQuadKey(tilePositionX, tilePositionY, zoom);
                string veLink = string.Format(UriFormat,
                                              new object[] { QuadKey[QuadKey.Length - 1], QuadKey });
                var veUri = new Uri(veLink);
                tileImageLayerSources.Add(veUri);
            }
        }

        /// <summary>
        /// Converts tile XY coordinates into a QuadKey at a specified level of detail.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>A string containing the QuadKey.</returns>
        private static string TileXYToQuadKey(int tileX, int tileY, int levelOfDetail)
        {
            var quadKey = new StringBuilder();
            for (int i = levelOfDetail; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << (i - 1);
                if ((tileX & mask) != 0)
                {
                    digit++;
                }
                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }
                quadKey.Append(digit);
            }
            return quadKey.ToString();
        }
    }
}
