﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Linq;
using Re.Common;
using Re.Model;

namespace Re.ViewModels
{
    public class ResultViewModel:INotifyPropertyChanged
    {
        private bool _isLoading = false;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");
            }
        }

        public ResultViewModel()
        {
            this.WebResultCollection = new ObservableCollection<WebResult>();
        }

        public ObservableCollection<WebResult> WebResultCollection
        {
            get;
            private set;
        }

        private int CountPanels(int resultNumber)
        {
            return (int)Math.Ceiling(resultNumber / 4.0d);
        }

        public void FetchSearchResults(HttpWebRequest queryRequest)
        {
            this.WebResultCollection.Clear();

            IsLoading = true;

            queryRequest.BeginGetResponse(new AsyncCallback(ReadCallback), queryRequest);
        }

        private void ReadCallback(IAsyncResult asyncResult)
        {
            // General info we need to parse the XML later
            XNamespace xmlns = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            XNamespace xmlnsm = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

            // Gets state info
            HttpWebRequest requestState = (HttpWebRequest)asyncResult.AsyncState;
            HttpWebResponse queryResponse = (HttpWebResponse)requestState.EndGetResponse(asyncResult);

            Stream resultStream;

            string xmlHolder = "";

            try
            {
                // First we need to get the response stream
                resultStream = queryResponse.GetResponseStream();

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

                results.ToList();

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in results)
                    {
                        this.WebResultCollection.Add(item);
                    }
                    IsLoading = false;
                });

            }
            catch (Exception e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Network error occured " + e.Message);
                });
            }
        }

        public string RemoveStopWords(string input)
        {
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
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
