using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Re.Common;
using Re.Model;
using Re.ViewModels;

namespace Re
{

    public partial class Results : PhoneApplicationPage
    {

        ResultViewModel _viewModel;
        static List<Keyword> queryList = new List<Keyword>();
        const int BUFFER_SIZE = 1024;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private string _searchTerm = "";

        public Results()
        {
            InitializeComponent();
            _viewModel = (ResultViewModel)Resources["viewModel"];
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var progressIndicator = SystemTray.ProgressIndicator;
            if (progressIndicator != null)
            {
                return;
            }

            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            Binding binding = new Binding("IsLoading") { Source = _viewModel };
            BindingOperations.SetBinding(
                progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") { Source = _viewModel };
            BindingOperations.SetBinding(
                progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);

            progressIndicator.Text = "Loading search results";

        }

        private void panoramaControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string query = NavigationContext.QueryString["query"];

            // This only gets called when navigated to from the main page
            // To ensure that users aren't searching for weird things that they don't expect, we need to clear the list
            queryList.Clear();

            foreach (string word in query.Split(' '))
            {
                queryList.Add(new Keyword(word, 2));
            }

            // Reset the query because we're going to modify it
            query = "";

            foreach (Keyword k in queryList.Take(7))
            {
                query += k.Word + " ";
            }

            _searchTerm = Uri.EscapeDataString(query);

            FetchWebResults();
            // gvwQuery.ItemsSource = queryList.Take(7); // This is the databinding for the query text
        }

        public void FetchWebResults()//string strKeyWord, object control)
        {
            String strBingAppID = "xmGJ+CFOgZtjSJhZWOb0QnxTa3KBu3LFB1I8Fd+pQPU=";
            var byteArray = Encoding.UTF8.GetBytes(strBingAppID + ":" + strBingAppID);
<<<<<<< HEAD
=======

>>>>>>> Clobber all of the things.
            // Everything in mobile is a URI, so we make a URI
            Uri uri = new Uri("https://api.datamarket.azure.com/Bing/Search/v1/Web?Query=%27" + _searchTerm + "%27&$top=40");
            string _cred = String.Format("{0} {1}", "Basic", Convert.ToBase64String(byteArray));

            HttpWebRequest queryRequest = (HttpWebRequest)WebRequest.Create(uri);
            queryRequest.Headers[HttpRequestHeader.Authorization] = _cred;
<<<<<<< HEAD
            RequestState requestState = new RequestState();
            requestState.request = queryRequest;

            queryRequest.BeginGetResponse(new AsyncCallback(RespCallBack), requestState);

=======

            this._viewModel.FetchSearchResults(queryRequest);
>>>>>>> Clobber all of the things.
        }

        public string RemoveStopWords(string input)
        {
<<<<<<< HEAD

            XNamespace xmlns = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            XNamespace xmlnsm = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

            // Gets state info
            RequestState requestState = (RequestState)asynchronousResult.AsyncState;
            HttpWebRequest queryRequest = (HttpWebRequest)requestState.request;

            // Ends the async state
            requestState.response = (HttpWebResponse)queryRequest.EndGetResponse(asynchronousResult);

            Stream resultStream ;

            string xmlHolder = "";

            try
            {
                // First we need to get the response stream
                resultStream = requestState.response.GetResponseStream();

                // Then we load the XML
                XDocument doc = XDocument.Load(resultStream);

                // Parse the xml
                xmlHolder = doc.ToString();

                IEnumerable<WebResult> results = from r in doc.Descendants(xmlnsm + "properties")
                                                 select new WebResult
                                                 {
                                                     Id = r.Element(xmlns + "ID").Value,
                                                     Title = r.Element(xmlns + "Title").Value,
                                                     Description = r.Element(xmlns + "Description").Value,
                                                     DisplayUrl = r.Element(xmlns + "DisplayUrl").Value,
                                                     Url = r.Element(xmlns + "Url").Value
                                                 };

                results = results.ToList();
            }
            catch (WebException e)
            {
                // Do nothing
                return;
            }
        }

        public static void ReadCallBack(IAsyncResult asyncResult)
        {
            try
            {

                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                Stream responseStream = myRequestState.streamResponse;
                int read = responseStream.EndRead(asyncResult);
                // Read the HTML page and then do something with it
                if (read > 0)
                {
                    myRequestState.requestData.Append(Encoding.UTF8.GetString(myRequestState.BufferRead, 0, read));
                    IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                }
                else
                {
                    if (myRequestState.requestData.Length > 1)
                    {
                        string stringContent;
                        stringContent = myRequestState.requestData.ToString();
                        // do something with the response stream here
                    }

                    responseStream.Close();
                    allDone.Set();
                }

            }
            catch (WebException e)
            {
                // Need to handle the exception
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
=======
            // These are the words to remove to make the keywords more useful
            var stopWords = new HashSet<string> { "a", "about", "above", "above", "across", "after", "afterwards", "again", "against", "all",
                                        "almost", "alone", "along", "already", "also", "although", "always", "am", "among", "amongst",
                                        "amoungst", "amount", "an", "and", "another", "any", "anyhow", "anyone", "anything", "anyway",
                                        "anywhere", "are", "around", "as", "at", "back", "be", "became", "because", "become", "becomes",
                                        "becoming", "been", "before", "beforehand", "behind", "being", "below", "beside", "besides",
                                        "between", "beyond", "bill", "both", "bottom", "but", "by", "call", "can", "cannot", "cant", "co",
                                        "con", "could", "couldnt", "cry", "de", "describe", "detail", "do", "done", "down", "due", "during",
                                        "each", "eg", "eight", "either", "eleven", "else", "elsewhere", "empty", "enough", "etc", "even",
                                        "ever", "every", "everyone", "everything", "everywhere", "except", "few", "fifteen", "fify", "fill",
                                        "find", "fire", "first", "five", "for", "former", "formerly", "forty", "found", "four", "from", "front",
                                        "full", "further", "get", "give", "go", "had", "has", "hasnt", "have", "he", "hence", "her", "here",
                                        "hereafter", "hereby", "herein", "hereupon", "hers", "herself", "him", "himself", "his", "how", "however",
                                        "hundred", "ie", "if", "in", "inc", "indeed", "interest", "into", "is", "it", "its", "itself", "keep", "last",
                                        "latter", "latterly", "least", "less", "ltd", "made", "many", "may", "me", "meanwhile", "might", "mill", "mine",
                                        "more", "moreover", "most", "mostly", "move", "much", "must", "my", "myself", "name", "namely", "neither",
                                        "never", "nevertheless", "next", "nine", "no", "nobody", "none", "noone", "nor", "not", "nothing", "now",
                                        "nowhere", "of", "off", "often", "on", "once", "one", "only", "onto", "or", "other", "others", "otherwise",
                                        "our", "ours", "ourselves", "out", "over", "own", "part", "per", "perhaps", "please", "put", "rather", "re",
                                        "same", "see", "seem", "seemed", "seeming", "seems", "serious", "several", "she", "should", "show", "side",
                                        "since", "sincere", "six", "sixty", "so", "some", "somehow", "someone", "something", "sometime", "sometimes",
                                        "somewhere", "still", "such", "system", "take", "ten", "than", "that", "the", "their", "them", "themselves",
                                        "then", "thence", "there", "thereafter", "thereby", "therefore", "therein", "thereupon", "these", "they", "thickv",
                                        "thin", "third", "this", "those", "though", "three", "through", "throughout", "thru", "thus", "to", "together", "too",
                                        "top", "toward", "towards", "twelve", "twenty", "two", "un", "under", "until", "up", "upon", "us", "very", "via",
                                        "was", "we", "well", "were", "what", "whatever", "when", "whence", "whenever", "where", "whereafter", "whereas",
                                        "whereby", "wherein", "whereupon", "wherever", "whether", "which", "while", "whither", "who", "whoever", "whole",
                                        "whom", "whose", "why", "will", "with", "within", "without", "would", "yet", "you", "your", "yours", "yourself",
                                        "yourselves", "the" };

            //input = new string(input.Replace(',', ' ').ToCharArray().Where(c => !char.IsPunctuation(c)).ToArray());

            // Remove Punctuation
            input = Regex.Replace(input, "[^A-Za-z0-9]", " ").ToLower();

            // Remove all the stop words
            string output = string.Join(
                ", ",
                input
                    .Split(new[] { ' ', '\t', '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(word => !stopWords.Contains(word))
            );

            return output;
>>>>>>> Clobber all of the things.
        }

        /*
            string xml = await response.Content.ReadAsStringAsync();

            XDocument doc = XDocument.Parse(xml);

            IEnumerable<WebResult> results = from r in doc.Descendants(xmlnsm + "properties")
                                             select new WebResult
                                             {
                                                 Id = r.Element(xmlns + "ID").Value,
                                                 Title = r.Element(xmlns + "Title").Value,
                                                 Description = r.Element(xmlns + "Description").Value,
                                                 DisplayUrl = r.Element(xmlns + "DisplayUrl").Value,
                                                 Url = r.Element(xmlns + "Url").Value
                                             };

            results = results.ToList();

            Parallel.ForEach(results, async r =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    r.Keywords = await GetMetaTags(r);
                    (control as GridView).Items.Add(r);
                });
            });
            */

        /*foreach (WebResult result in results)
        {
            string html = await new HttpClient().GetStringAsync(result.Url);
            Match KeywordMatch = Regex.Match(html, "<meta name=\"keywords\" content=\"([^<]*)\">", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            result.Keywords = KeywordMatch.Groups[1].Value;
        }*/

        /*if (control is GridView)
        {
            // (control as GridView).ItemsSource = results;
        }*/

        public class WebResult
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string DisplayUrl { get; set; }
            public string Url { get; set; }
            public string TileColor
            {
                get
                {
                    string[] colorArray = { "FFA200FF", "FFFF0097", "FF00ABA9", "FF8CBF26", "FFA05000", "FFE671B8", "FFF09609", "FF1BA1E2", "FFE51400", "FF339933" };
                    Random random = new Random();
                    int num = random.Next(0, (colorArray.Length - 1));
                    return "#" + colorArray[num];
                }
            }
            public string Keywords { get; set; }
        }
    }
}