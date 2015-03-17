using EasyClustering;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLustering.Test
{
    public static class MockData
    {
        public static ItemCollection GenerateMock(int nbItem)
        {
            var collection = new ItemCollection();
            Random rand = new Random();
            for (int i = 0; i < nbItem; i++)
            {
                collection.Add(new ItemObjet()
                {
                    item = "item " + i,
                    Location = new Location()
                    {
                        Latitude = rand.NextDouble() * 180 - 90,
                        Longitude = rand.NextDouble() * 360 - 180
                    }
                });
            }

            return collection;
        }
    }
}
