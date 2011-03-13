using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

using CIAPI;
using CIAPI.DTO;
using CIAPI.Streaming;
using CIAPI.Rpc;
using StreamingClient;

namespace CityIndexConsoleTest2
{
    class Program
    {
        private static readonly Uri RPC_URI = new Uri("https://ciapipreprod.cityindextest9.co.uk/tradingapi");
        //private static readonly Uri RPC_URI = new Uri("http://ciapipreprod.cityindextest9.co.uk/tradingapi");
        private static readonly Uri STREAMING_URI = new Uri("https://pushpreprod.cityindextest9.co.uk/CITYINDEXSTREAMING");
        //private static readonly Uri STREAMING_URI = new Uri("http://pushpreprod.cityindextest9.co.uk/CITYINDEXSTREAMING");
        private const string USERNAME = "DM032299";
        private const string PASSWORD = "password";
        public static StreamingClient.IStreamingClient _streamingClient = null;
        
        static void Main(string[] args)
        {
            // Test if input arguments were supplied:
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a numeric argument for number of tests.");
                return;
            }

            int noTests = 10;
            bool test = int.TryParse(args[0], out noTests);
            if (test == false)
            {
                Console.WriteLine("Please enter a numeric argument.");
                return;
            }


            Console.WriteLine("*** BEGIN TESTS ***");
            Console.WriteLine("No of trails: " + noTests);
            Console.WriteLine("*** BEGIN TESTS ***");

            CIAPI.Rpc.Client _ctx = null;
            int method = 1;

            List<int> errorList = new List<int>();

            _ctx = new CIAPI.Rpc.Client(RPC_URI);

            var gate = new AutoResetEvent(false);

            _ctx.BeginLogIn(USERNAME, PASSWORD, a =>
            {
                try
                {
                    Console.WriteLine("Logged in...");
                    _ctx.EndLogIn(a);
                }
                catch (CityIndex.JsonClient.ApiException err)
                {
                    Console.WriteLine("Login failed incorrect username/password! " + err.Message);
                }
                gate.Set();
            }, null);
            gate.WaitOne();

            //_ctx.LogIn(USERNAME, PASSWORD);

            String[] markets = new String[] { "99498", "99500", "99502", "99504", "99506", "99508", "99510", "99553", "99554", "99555" };
            String interval = "DAY";
            String no = "30";

            
            
            for (int j = 1; j < noTests; j++)
            {
                Console.WriteLine("Initialising test no: " + j);
                Thread.Sleep(2000);

                for (int z = 0; z <= 2; z++)
                {
                
                    Dictionary<String, PriceBarDTO[]> priceBarResults = new Dictionary<String, PriceBarDTO[]>();
                    Dictionary<String, PriceBarDTO[]> priceBarResultsASync = new Dictionary<String, PriceBarDTO[]>();
                    method = z;
                    int errors = 0;
                    int timeout = 20;
                    int count = 0;
                    switch (method)
                    {
                        case 0:
                            Console.WriteLine("Begin Sync standard...");
                            Thread.Sleep(1000);
                            foreach (String market in markets)
                            {
                                GetPriceBarResponseDTO priceBars = _ctx.GetPriceBars(market, interval, 1, no);
                                Console.WriteLine("Received bar from sync call:" + market);
                                priceBarResults.Add(market, priceBars.PriceBars);
                            }
                            break;
                        case 1:
                            Console.WriteLine("Begin Sync Parallel ForEach...");
                            Thread.Sleep(1000);
                            Parallel.ForEach(
                                    markets,
                                    (n, loopState, index) =>
                                    {
                                        GetPriceBarResponseDTO priceBars = _ctx.GetPriceBars(markets[index], interval, 1, no);
                                        Console.WriteLine("Received bar from parallel sync call:" + markets[index]);
                                        priceBarResults.Add(markets[index], priceBars.PriceBars);
                                    } //close lambda expression
                                ); //close method invocation
                            break;
                        case 2:
                            // - Price bars via task method...
                            Console.WriteLine("Begin Sync task...");
                            Thread.Sleep(1000);                            
                            Task<Dictionary<String, PriceBarDTO[]>> taskWithFactoryAndState =

                               Task.Factory.StartNew<Dictionary<String, PriceBarDTO[]>>((stateObj) =>
                               {
                                   Dictionary<String, PriceBarDTO[]> pricetasks = new Dictionary<String, PriceBarDTO[]>();
                                   for (int i = 0; i < (int)stateObj; i++)
                                   {
                                       GetPriceBarResponseDTO priceBars = _ctx.GetPriceBars(markets[i], interval, 1, no);
                                       Console.WriteLine("Received bar from task sync call:" + markets[i]);
                                       priceBarResults.Add(markets[i], priceBars.PriceBars);
                                   }
                                   return pricetasks;
                               }, 10);

                            gate = new AutoResetEvent(false);

                            try
                            {
                                Task.WaitAll(taskWithFactoryAndState);
                                gate.Set();
                            }
                            catch (AggregateException aggEx)
                            {
                                gate.Set();
                                foreach (Exception ex in aggEx.InnerExceptions)
                                {
                                    Console.WriteLine(string.Format("Caught exception '{0}'",
                                        ex.Message));
                                }
                            }
                            gate.WaitOne();
                            break;
                    }            
                    foreach (String market in markets)
                    {
                        Console.WriteLine("Begin async call:" + market);
                        _ctx.BeginGetPriceBars(market,interval,1,no, pricebarResult =>
                        {                    
                            Console.WriteLine("Received bar from async call:" + market);
                            GetPriceBarResponseDTO pricebar = _ctx.EndGetPriceBars(pricebarResult);
                            priceBarResultsASync.Add(market, pricebar.PriceBars);                    
                        }, null);
                    }

                    while (count <= timeout)
                    {
                        Thread.Sleep(1000);
                        count++;
                        if (priceBarResults.Count == priceBarResultsASync.Count) break;
                        Console.WriteLine("Waiting for async... " + count + " seconds elapsed.");                        
                    }


                    if (priceBarResults.Count == priceBarResultsASync.Count)
                    {
                        Console.WriteLine("Sync and Async calls completed now checking...");
                        foreach (KeyValuePair<String, PriceBarDTO[]> KeyPairSync in priceBarResults)
                        {
                            String marketid = KeyPairSync.Key;
                            PriceBarDTO[] priceBarSync = KeyPairSync.Value;
                            PriceBarDTO[] priceBarAsync = priceBarResultsASync[marketid];
                            for (int i = 0; i < priceBarSync.Count(); i++)
                            {
                                // Lambda expression as executable code.
                                //Func<int, bool> deleg = i => i < 5;
                                Func<Decimal, Decimal, bool> myFunc = (x, y) => x == y;
                                bool result = myFunc(priceBarSync[i].Close, priceBarAsync[i].Close);
                                if (!result)
                                {
                                    Console.WriteLine("Sync and Async mismatch: Sync: " + priceBarSync[i].Close + " Async: " + priceBarAsync[i].Close);
                                    errors++;
                                }
                            }
                        }
                        Console.WriteLine("Sync and Async match test completed... There were: " + errors + " errors.");
                        errorList.Add(errors);
                        Thread.Sleep(2000);
                    } else if(priceBarResults.Count != priceBarResultsASync.Count) 
                    {
                        Console.WriteLine("Sync and Async match test completed... We timed out after " + count + " seconds.. Sync count: " + priceBarResults.Count + " Async count: " + priceBarResultsASync.Count);
                        errorList.Add(9999);
                    }
                }
            
            }

            Console.WriteLine("**** ERRORS ***");
            foreach (int error in errorList)
            {
                Console.Write(error + ",");
            }
            Console.WriteLine("**** END ERRORS ***");
            //String stop = Console.ReadLine();
        }
                
    }
}
