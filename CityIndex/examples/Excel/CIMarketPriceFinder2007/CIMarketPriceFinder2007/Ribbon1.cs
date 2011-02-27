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

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            editBoxMarketId.Text = "71442";
            editBoxInterval.Text = "MINUTE";
            editBoxNo.Text = "5";
            lblStatus.Label = "Not logged in";
            btnLogin.Label = "[Log in]";
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
            Task[] tasks = new Task[]  
            {  
                Task.Factory.StartNew(() => Login()),  
            };            
        }

    }
}
