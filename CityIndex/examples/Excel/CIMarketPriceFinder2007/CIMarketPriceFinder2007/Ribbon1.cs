using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;
using System.IO;

using CIAPI;
using CIAPI.DTO;
using CIAPI.Streaming;
using CIAPI.Rpc;

using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Tools.Ribbon;

namespace CIMarketPriceFinder2007
{
    public partial class CIExcelRibbon
    {
        private static readonly Uri RPC_URI = new Uri("https://ciapipreprod.cityindextest9.co.uk/tradingapi");
        private static readonly Uri STREAMING_URI = new Uri("https://pushpreprod.cityindextest9.co.uk/CITYINDEXSTREAMING");
        public CIAPI.Rpc.Client _ctx = null;

        private Boolean _IsLoggedIn = false;

        public Dictionary<int, MarketPattern> _PatternSearch = new Dictionary<int, MarketPattern>();
        

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            editBoxMarketId.Text = "71442";
            editBoxInterval.Text = "DAY";
            editBoxNo.Text = "30";
            lblStatus.Label = "Not logged in";
            btnLogin.Label = "[Log in]";
            editBoxMethod.Text = "1";
        }
        private void Logout()
        {
            var gate = new AutoResetEvent(false);

            try
            {
                _ctx.BeginLogOut(result =>
                {                
                    String msg = "Logout success!";
                    lblStatus.Label = "Logged out.";
                    btnLogin.Label = "[Log in]";
                    _IsLoggedIn = false;
                    MessageBox.Show(msg);                
                    gate.Set();
                }, null);
            }
            catch (CityIndex.JsonClient.ApiException err)
            {
                String msg = "Logout failed! " + err.Message;
                MessageBox.Show(msg);
                gate.Set();
            }
            gate.WaitOne();
        }

        private void EditPattern()
        {
            MarketPatternFinder fMarketPatternFinder = new MarketPatternFinder(_PatternSearch);
            Application.Run(fMarketPatternFinder);

            _PatternSearch = fMarketPatternFinder.getMarketPatten();
        }    

        private void Login()
        {
            
            if (!_IsLoggedIn)
            {
                if (_ctx == null)
                {
                    _ctx = new CIAPI.Rpc.Client(RPC_URI);
                }

                frmLogin fLogin = new frmLogin();

                Application.Run(fLogin);

                if (fLogin.GetLogin)
                {
                    var gate = new AutoResetEvent(false);

                    String username = fLogin.Username;
                    String password = fLogin.Password;

                    _ctx.BeginLogIn(username, password, a =>
                    {
                        try
                        {
                            _ctx.EndLogIn(a);
                            _IsLoggedIn = true;
                            String msg = "Login success!";
                            lblStatus.Label = "Logged in.";
                            btnLogin.Label = "[Log out]";
                            MessageBox.Show(msg);
                        }
                        catch (CityIndex.JsonClient.ApiException err)
                        {
                            String msg = "Login failed incorrect username/password! " + err.Message;
                            MessageBox.Show(msg);
                            _IsLoggedIn = false;
                        }
                        gate.Set();
                    }, null);
                    gate.WaitOne();
                }
                else
                {
                    String msg = "Login has been cancelled!";
                    lblStatus.Label = "Logged cancelled.";                    
                    MessageBox.Show(msg);
                }
            }            
        }

        public PriceBarDTO[] GetPriceBars(String marketid, String interval, String no)
        {
            GetPriceBarResponseDTO priceBars = _ctx.GetPriceBars(marketid, interval, 1, no);
            return priceBars.PriceBars;
        }

        private void btnGetBars_Click(object sender, RibbonControlEventArgs e)
        {
            String marketId = editBoxMarketId.Text;
            String interval = editBoxInterval.Text;
            String no = editBoxNo.Text;

            if (!_IsLoggedIn)
            {
                Task[] logintask = new Task[]  
                {  
                    Task.Factory.StartNew(() => Login()),
                };
                Task.WaitAll(logintask);                                
            }

            if (!_IsLoggedIn)
            {
                return;
            }
            Microsoft.Office.Interop.Excel.Application excelObj;

            excelObj = (Microsoft.Office.Interop.Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
            
            Microsoft.Office.Interop.Excel.Workbook wb;

            wb = excelObj.ActiveWorkbook;

            Microsoft.Office.Interop.Excel.Worksheet ws;

            ws = excelObj.ActiveSheet;

            PriceBarDTO[] priceBars = null;

            Stopwatch watch = new Stopwatch();

            watch.Start();
            Task[] tasks = new Task[]  
            {  
                Task.Factory.StartNew(() => priceBars = GetPriceBars(marketId,interval,no)),  
            };

            Task.WaitAll(tasks);
            watch.Stop();

            int count = 1;

            if (priceBars != null)
            {
                ws.get_Range("A" + count, System.Type.Missing).Value = "DATE";
                ws.get_Range("B" + count, System.Type.Missing).Value = "OPEN";
                ws.get_Range("C" + count, System.Type.Missing).Value = "HIGH";
                ws.get_Range("D" + count, System.Type.Missing).Value = "LOW";
                ws.get_Range("E" + count, System.Type.Missing).Value = "CLOSE";
                count++;
                foreach (PriceBarDTO bar in priceBars)
                {
                    ws.get_Range("A" + count, System.Type.Missing).Value = bar.BarDate;
                    ws.get_Range("B" + count, System.Type.Missing).Value = bar.Open;
                    ws.get_Range("C" + count, System.Type.Missing).Value = bar.High;
                    ws.get_Range("D" + count, System.Type.Missing).Value = bar.Low;
                    ws.get_Range("E" + count, System.Type.Missing).Value = bar.Close;                    
                    count++;
                }
                excelObj.StatusBar = String.Format("Total time taken:{0} seconds, retrieved: {1}", watch.Elapsed.Seconds, priceBars.Count());
            }
            else
            {
                ws.get_Range("A" + count, System.Type.Missing).Value = "Error with requesting data...";
            }            
        }

        private void btnLogin_Click(object sender, RibbonControlEventArgs e)
        {
            if (_IsLoggedIn)
            {
                Task[] tasks = new Task[]  
                {  
                    Task.Factory.StartNew(() => Logout()),  
                };            
            }
            else
            {
                Task[] tasks = new Task[]  
                {  
                    Task.Factory.StartNew(() => Login()),  
                };            
            }            
        }

        private void btnSearchMarkets_Click(object sender, RibbonControlEventArgs e)
        {
            String marketId = editBoxMarketId.Text;
            String interval = editBoxInterval.Text;

            // The method to retrieve the data...  1) normal 2) Using tasks 3) parallel
            Int32 method = 0;

            if (_PatternSearch.Count < 2)
            {
                MessageBox.Show("Market Finder is only valid for pattern searching of 2 or more bars. Please click edit pattern to modify.");
                return;
            }

            try
            {
                method = Int32.Parse(editBoxMethod.Text);
                if (method < 1 || method > 3)
                {
                    MessageBox.Show("Method can only be integer values from 1 to 3. (Normal,Tasks,Parallel)");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Method can only be integer values from 1 to 3. (Normal,Tasks,Parallel)");
                return;
            }
            String no = editBoxNo.Text;
            String message = "";

            String[] markets = new String[] { "99498", "99500", "99502", "99504", "99506", "99508", "99510", "99553", "99554", "99555" };

            //We hardcode the market IDs for now since we cant list the markets yet currently.

            Dictionary<String, PriceBarDTO[]> priceBarResults = new Dictionary<String, PriceBarDTO[]>();

            //dictionary to hold our results.

            if (!_IsLoggedIn)
            {
                Task[] logintask = new Task[]  
                {  
                    Task.Factory.StartNew(() => Login()),
                };
                Task.WaitAll(logintask);
            }

            if (!_IsLoggedIn)
            {
                return;
            }
            Microsoft.Office.Interop.Excel.Application excelObj;

            excelObj = (Microsoft.Office.Interop.Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");

            Microsoft.Office.Interop.Excel.Workbook wb;

            wb = excelObj.ActiveWorkbook;

            Microsoft.Office.Interop.Excel.Worksheet ws;

            ws = excelObj.ActiveSheet;
            
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                switch (method)
                {
                    case 1:
                        // - Price bars via normal method...
                        foreach (String market in markets)
                        {
                            priceBarResults.Add(market, GetPriceBars(market, interval, no));
                        }    
        
                        break;
                    case 2:
                        // - Price bars via task method...
                        Task<Dictionary<String, PriceBarDTO[]>> taskWithFactoryAndState =

                           Task.Factory.StartNew<Dictionary<String, PriceBarDTO[]>>((stateObj) =>
                           {
                               Dictionary<String, PriceBarDTO[]> pricetasks = new Dictionary<String, PriceBarDTO[]>();
                               for (int i = 0; i < (int)stateObj; i++)
                               {
                                   pricetasks.Add(markets[i], GetPriceBars(markets[i], interval, no));
                               }
                               return pricetasks;
                           }, 10);

                        var gate = new AutoResetEvent(false);

                        try
                        {
                            Task.WaitAll(taskWithFactoryAndState);
                            //setup a continuation for task
                            taskWithFactoryAndState.ContinueWith((ant) =>
                            {
                                Dictionary<String, PriceBarDTO[]> result = ant.Result;
                                priceBarResults = result;
                                gate.Set();
                            });
                        }
                        catch (AggregateException aggEx)
                        {
                            gate.Set();
                            foreach (Exception ex in aggEx.InnerExceptions)
                            {
                                MessageBox.Show(string.Format("Caught exception '{0}'",
                                    ex.Message));
                            }
                        }

                        gate.WaitOne();
                        
                        break;
                    case 3:
                        // - Price bars via Parallel method...
                        Parallel.ForEach(
                            markets,
                            (n, loopState, index) =>
                            {
                                priceBarResults.Add(markets[index], GetPriceBars(markets[index], interval, no));
                            } //close lambda expression

                        ); //close method invocation
                        break;
                    case 4:
                        // not used currently....
                        break;
                    default:
                        break;
                }
            }
            catch (AggregateException err)
            {
                MessageBox.Show(String.Format("Parallel.ForEach has thrown an exception. THIS WAS NOT EXPECTED.\n{0}", err.Message));
            }
            watch.Stop();
            int timeWeb = watch.Elapsed.Seconds;

            int count = 0;
            watch.Start();

            int matchedBars = 0;
            foreach (KeyValuePair<String,PriceBarDTO[]> barKeyPair in priceBarResults)
            {
                PriceBarDTO[] bars = barKeyPair.Value;

                if (bars.Count() > 0)
                {
                    int max = bars.Count() - _PatternSearch.Count;
                    int matchesneeded = _PatternSearch.Count;
                    int matches = 0;
                    for (int i = 0; i < max; i++)
                    {
                        foreach (KeyValuePair<int, MarketPattern> KeyPair in _PatternSearch)
                        {
                            MarketPattern pattern = KeyPair.Value;
                            int index = KeyPair.Key;
                            switch (pattern.priceType)
                            {
                                case MarketPatternFinder.PriceType.Open:
                                    if( EvaluatePattern(pattern.tickDirection, bars[i + index].Open , bars[i + index - 1].Open)) 
                                        matches++;
                                    break;
                                case MarketPatternFinder.PriceType.High:
                                    if (EvaluatePattern(pattern.tickDirection, bars[i + index].High, bars[i + index - 1].High))
                                        matches++;
                                    break;
                                case MarketPatternFinder.PriceType.Low:
                                    if (EvaluatePattern(pattern.tickDirection, bars[i + index].Low, bars[i + index - 1].Low))
                                        matches++;
                                    break;
                                case MarketPatternFinder.PriceType.Close:
                                    if (EvaluatePattern(pattern.tickDirection, bars[i + index].Close, bars[i + index - 1].Close))
                                        matches++;
                                    break;
                            }
                        }

                        if (matches == matchesneeded)
                        {
                            message += "Matched: " + barKeyPair.Key + " (" + bars[i].BarDate + ") Close: "+ bars[i].Close + Environment.NewLine;
                            matchedBars++;
                        }

                        matches = 0;
                    }
                    count++;
                }
            }
            watch.Stop();
            int timeCalc = watch.Elapsed.Seconds;
            MessageBox.Show(message + Environment.NewLine + "CIAPS.CS retrieval time: " + timeWeb + " seconds. Calculation time " + timeCalc + " seconds. Matched bars: " + matchedBars);
        }

        private Boolean EvaluatePattern(MarketPatternFinder.TickDirection tickDirection, Decimal now, Decimal previous)
        {
            Boolean ret = false;
            switch (tickDirection)
            {
                case MarketPatternFinder.TickDirection.Up:
                    if (now > previous) return true;
                    break;
                case MarketPatternFinder.TickDirection.Down:
                    if (now < previous) return true;
                    break;
                case MarketPatternFinder.TickDirection.NoChange:
                    if (now == previous) return true;
                    break;
            }
            return ret;
        }

        private void btnEditPattern_Click(object sender, RibbonControlEventArgs e)
        {
            Task[] task = new Task[]  
                {  
                    Task.Factory.StartNew(() => EditPattern()),
                };
            Task.WaitAll(task);


        }        
    }
}
