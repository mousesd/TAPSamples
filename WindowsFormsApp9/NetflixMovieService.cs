using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WindowsFormsApp9
{
    public class NetflixMovieService
    {
        private IEnumerable<NetflixMovie> GetMovieDataFromXDoc(XDocument doc)
        {
            XNamespace dRss = "http://www.w3.org/2005/Atom";
            XNamespace odata = "http://schemas.microsoft.com/ado/2007/08/dataservices";

            List<XElement> dox = doc.Descendants(dRss + "entry").ToList();
            List<NetflixMovie> movieList = new List<NetflixMovie>();
            foreach (XElement ele in dox)
            {
                XElement movieSummary = ele.Descendants(dRss + "summary").First();
                XElement movieName = ele.Descendants(odata + "Name").First();
                XElement boxArtUrl = ele.Descendants(odata + "MediumUrl").First();
                NetflixMovie tempMovie = new NetflixMovie()
                {
                    BoxArtUrl = boxArtUrl.Value,
                    Title = movieName.Value,
                    Summary = movieSummary.Value
                };
                movieList.Add(tempMovie);
            }
            return movieList;
        }

        public async Task<List<NetflixMovie>> GetMoviesAsync()
        {
            string requestUri = @"http://odata.netflix.com/Catalog/Titles?" +
                @"$filter=ReleaseYear le 1989 and ReleaseYear ge 1980 " +
                @"and AverageRating gt 4";

            using (var client = new HttpClient() { MaxResponseContentBufferSize = Int32.MaxValue })
            {
                try
                {
                    string content = await client.GetStringAsync(requestUri);
                    var xDoc = XDocument.Parse(content);
                    var movies = this.GetMovieDataFromXDoc(xDoc).ToList();
                    return movies;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
