using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIMarketPriceFinder2007
{
    public class MarketPattern
    {
        MarketPatternFinder.PriceType _priceType = new MarketPatternFinder.PriceType();
        MarketPatternFinder.TickDirection _tickDirection = new MarketPatternFinder.TickDirection();

        public MarketPatternFinder.PriceType priceType 
        {
            get { return _priceType; }
            set { _priceType = value; }
        }

        public MarketPatternFinder.TickDirection tickDirection
        {
            get { return _tickDirection; }
            set { _tickDirection = value; }
        }
    }
}
