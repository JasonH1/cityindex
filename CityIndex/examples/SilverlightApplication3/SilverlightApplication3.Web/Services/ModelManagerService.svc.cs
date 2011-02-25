using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;

using CIAPI;
using CIAPI.DTO;
using CIAPI.Streaming;
using CIAPI.Rpc;


namespace SilverlightApplication3.Web
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ModelManagerService
    {
        private static readonly Uri RPC_URI = new Uri("https://ciapipreprod.cityindextest9.co.uk/tradingapi");
        private static readonly Uri STREAMING_URI = new Uri("https://pushpreprod.cityindextest9.co.uk/CITYINDEXSTREAMING");
        private const string USERNAME = "DM032299";
        private const string PASSWORD = "password";
        public CIAPI.Rpc.Client _ctx = null;
        public StreamingClient.IStreamingClient _client = null;

        private void AutoLogin()
        {
            if (_ctx == null)
            {
                _ctx = new CIAPI.Rpc.Client(RPC_URI);
                _ctx.LogIn(USERNAME, PASSWORD);
            }
        }

        [OperationContract]
        public List<NewsHeadline> DoWork()
        {
            // Add your operation implementation here
            List<NewsHeadline> news = new List<NewsHeadline>();

            try
            {

                AutoLogin();
                ListNewsHeadlinesResponseDTO newsHeadlines = _ctx.ListNewsHeadlines("UK", 10);
                //NewsDTO[] newsDTO = news.Headlines;
                //do something with the news
                foreach (NewsDTO nw in newsHeadlines.Headlines)
                {
                    NewsHeadline headline = new NewsHeadline();
                    headline.StoryID = nw.StoryId;
                    headline.PublishDate = nw.PublishDate;
                    headline.StoryHeadLine = nw.Headline;
                    news.Add(headline);
                }
                _ctx.LogOut();
            }
            catch (Exception err)
            {
                // raise excpetion here if we require more info...
                NewsHeadline headline = new NewsHeadline();
                headline.StoryID = 0;
                headline.StoryHeadLine = err.Message;
                news.Add(headline);                
                return news;
            }
            return news;
        }

        [DataContract]
        public class NewsHeadline
        {
            [DataMember]
            public Int32 StoryID { get; set; }
            [DataMember]
            public DateTime PublishDate { get; set; }
            [DataMember]
            public String StoryHeadLine { get; set; }
        }

    }
}
