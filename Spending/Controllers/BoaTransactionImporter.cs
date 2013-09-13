using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using HtmlAgilityPack;
using Spending.Models;

using HtmlAgilityDoc = HtmlAgilityPack.HtmlDocument;

namespace Spending
{
	public class BoaTransactionImporter
	{
		public static List<BoaTransaction> Import(
			string userName,
			string password,
			Dictionary<string, string> answers,
			bool negate,
			DateTime startDate,
			DateTime endDate)
		{
			var baseUrl = "https://secure.bankofamerica.com";

			CookieContainer cookies = new CookieContainer();
			HttpWebResponse response;
			HtmlAgilityDoc htmlDocument;
			XPathNavigator navigator;
			string csrfToken;

			//
			// Get and parse user name page
			//

			response = Request(baseUrl + "/login/sign-in/signOnScreen.go", cookies, null);

			htmlDocument = GetHtmlDocument(response);
			csrfToken = htmlDocument.GetElementbyId("csrfTokenHidden").Attributes["value"].Value;

			//
			// Submit user name
			//

			response = Request(baseUrl + "/login/sign-in/internal/entry/signOn.go", cookies, new PostParam[]
			{
				new PostParam("csrfTokenHidden", csrfToken),
				new PostParam("lpOlbResetErrorCounter", 0),
				new PostParam("lpPasscodeErrorCounter", 0),
				new PostParam("onlineId", userName)
			});

			//
			// Get and parse challenge page
			//

			response = Request(baseUrl + "/login/sign-in/signOn.go", cookies, null);

			htmlDocument = GetHtmlDocument(response);
			csrfToken = htmlDocument.GetElementbyId("csrfTokenHidden").Attributes["value"].Value;

			navigator = htmlDocument.DocumentNode.CreateNavigator();
			var challenge = navigator.SelectSingleNode("//label[@for='tlpvt-challenge-answer']").Value.Trim();
			var answer = answers.ContainsKey(challenge) ? answers[challenge] : string.Empty;

			//
			// Submit challenge answer / get and parse password page
			//

			response = Request(baseUrl + "/login/sign-in/validateChallengeAnswer.go", cookies, new PostParam[]
			{
				new PostParam("csrfTokenHidden", csrfToken),
				new PostParam("challengeQuestionAnswer", answer)
			});

			htmlDocument = GetHtmlDocument(response);
			csrfToken = htmlDocument.GetElementbyId("csrfTokenHidden").Attributes["value"].Value;

			//
			// Submit password / get accounts overview page
			//

			response = Request(baseUrl + "/login/sign-in/validatePassword.go", cookies, new PostParam[]
			{
				new PostParam("csrfTokenHidden", csrfToken),
				new PostParam("lpOlbResetErrorCounter", 0),
				new PostParam("lpPasscodeErrorCounter", 0),
				new PostParam("password", password)
			});

			//
			// Go to Visa details page
			//

			var accountDetailsUrlBase = "/myaccounts/brain/redirect.go?source=overview&target=acctDetails&adx=";
			var visaAdx = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
			response = Request(baseUrl + accountDetailsUrlBase + visaAdx, cookies, null);

			htmlDocument = GetHtmlDocument(response);

			var datePrefix = "goto_transactions_top_for_this_date_";
			var statementLinks = htmlDocument.DocumentNode.SelectNodes(string.Format("//a[starts-with(@name, '{0}')]", datePrefix));

			var statements = new List<BoaStatement>();
			DateTime stmtStartDate = DateTime.MinValue;

			foreach (var statementLink in statementLinks.Reverse())
			{
				var dateStr = statementLink.Attributes["name"].Value.Substring(datePrefix.Length);
				var stmtEndDate = DateTime.Parse(dateStr.Replace("_", "/"));

				if (startDate <= stmtEndDate && endDate >= stmtStartDate)
				{
					statements.Add(new BoaStatement(
						statementLink.Attributes["rel"].Value.Replace("&amp;", "&"), stmtStartDate, stmtEndDate));
				}

				stmtStartDate = stmtEndDate.AddDays(1);
			}

			var transactions = new List<BoaTransaction>();

			if (endDate >= stmtStartDate)
			{
				transactions.AddRange(GetTransactions(htmlDocument, negate, startDate, endDate));
			}

			foreach (var statement in ((IEnumerable<BoaStatement>)statements).Reverse())
			{
				response = Request(baseUrl + statement.Url, cookies, null);

				htmlDocument = GetHtmlDocument(response);
				transactions.AddRange(GetTransactions(htmlDocument, negate, startDate, endDate));
			}

			return transactions;
		}

		private static List<BoaTransaction> GetTransactions(HtmlDocument htmlDocument, bool negate, DateTime startDate, DateTime endDate)
		{
			var transactions = new List<BoaTransaction>();

			var transactionRows = htmlDocument.DocumentNode.SelectNodes("//table[@id='transactions']/tbody/tr");

			foreach (var row in transactionRows)
			{
				var transactionCells = row.SelectNodes("td");

				int i = 0;
				bool pending = false;
				DateTime date = DateTime.MaxValue;
				var description = string.Empty;
				decimal amount = 0;

				foreach (var cell in transactionCells)
				{
					switch (i)
					{
						case 0:
						{
							var text = string.Empty;

							foreach (var textNode in cell.SelectNodes("text()"))
							{
								text += textNode.InnerText;
							}

							text = text.Trim();

							if (text == "Pending")
							{
								pending = true;
							}
							else
							{
								date = DateTime.Parse(text.Trim());
							}

							break;
						}

						case 1:
						{
							var anchorNode = cell.SelectSingleNode("a");

							var text = string.Empty;

							foreach (var textNode in anchorNode.SelectNodes("text()"))
							{
								text += textNode.InnerText;
							}

							description = text.Trim();
							break;
						}

						case 3:
						{
							var text = cell.InnerText.Replace("$", string.Empty).Replace(",", string.Empty);
							amount = decimal.Parse(text);

							if (negate)
							{
								amount = -amount;
							}

							break;
						}
					}

					i++;
				}

				if (date >= startDate && date <= endDate)
				{
					transactions.Add(new BoaTransaction
					{
						Pending = pending,
						Date = date,
						Description = description,
						Amount = amount
					});
				}
			}

			return transactions;
		}

		private class PostParam
		{
			public PostParam(string name, object value)
			{
				this.Name = name;
				this.Value = value;
			}

			public string Name { get; set; }
			public object Value { get; set; }

			public override string ToString()
			{
				return this.Name + "=" + Uri.EscapeDataString(this.Value.ToString());
			}
		}

		private static HttpWebResponse Request(string url, CookieContainer cookies, IEnumerable<PostParam> postParams)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.UserAgent = "Mozilla/5.0 (Windows NT 6.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.57 ";
			request.ContentType = "application/x-www-form-urlencoded";
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
			request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
			request.CookieContainer = cookies;

			if (postParams != null)
			{
				request.Method = "POST";

				var sb = new StringBuilder();

				foreach (var param in postParams)
				{
					if (sb.Length > 0)
					{
						sb.Append('&');
					}

					sb.Append(param);
				}

				var bytes = System.Text.Encoding.ASCII.GetBytes(sb.ToString());
				var dataStream = request.GetRequestStream();
				dataStream.Write(bytes, 0, bytes.Length);
				dataStream.Close();
			}

			var response = (HttpWebResponse)request.GetResponse();
			ReadCookies(request, response, cookies);
			return response;
		}

		private static string GetHtml(HttpWebResponse response)
		{
			var streamReader = new StreamReader(response.GetResponseStream());
			var html = streamReader.ReadToEnd();
			streamReader.Close();
			return html;
		}

		private static List<byte> GetData(HttpWebResponse response)
		{
			var stream = response.GetResponseStream();

			var contentLength = int.Parse(response.Headers[HttpResponseHeader.ContentLength]);

			var data = new List<byte>();
			byte[] buffer = new byte[512];
			int amtRead;

			while (true)
			{
				amtRead = stream.Read(buffer, 0, buffer.Length);

				if (amtRead == 0)
				{
					break;
				}

				data.AddRange(buffer.Take(amtRead));
			}

			stream.Close();
			return data;
		}

		private static HtmlAgilityDoc GetHtmlDocument(HttpWebResponse response)
		{
			var html = GetHtml(response);

			var htmlDocument = new HtmlAgilityDoc();
			htmlDocument.LoadHtml(html);
			return htmlDocument;
		}

		private static void ReadCookies(HttpWebRequest request, HttpWebResponse response, CookieContainer cookies)
		{
			for (int i = 0; i < response.Headers.Count; i++)
			{
				string name = response.Headers.GetKey(i);
				string value = response.Headers.Get(i);

				if (name == "Set-Cookie")
				{
					value = Regex.Replace(value, "Expires=(...),", new MatchEvaluator(x => x.Value.Replace(',', '^')));
					var cookieStrs = value.Split(',');

					foreach (var cookieStr in cookieStrs)
					{
						var s = Regex.Replace(cookieStr, "Expires=(...)\\^", new MatchEvaluator(x => x.Value.Replace('^', ',')));
						var parts = s.Split(';');

						var cookie = new Cookie();

						foreach (var part in parts)
						{
							var trimmedPart = part.TrimStart();
							var partValue = part.Substring(part.IndexOf('=') + 1);

							if (trimmedPart.StartsWith("Expires=", StringComparison.CurrentCultureIgnoreCase))
							{
								try
								{
									cookie.Expires = DateTime.Parse(partValue);
								}
								catch
								{
								}
							}
							else if (trimmedPart.StartsWith("Path=", StringComparison.CurrentCultureIgnoreCase))
							{
								cookie.Path = partValue;
							}
							else if (trimmedPart.StartsWith("Domain=", StringComparison.CurrentCultureIgnoreCase))
							{
								cookie.Domain = partValue;
							}
							else if (trimmedPart.ToUpper() == "SECURE")
							{
								cookie.Secure = true;
							}
							else if (trimmedPart.ToUpper() == "HTTPONLY")
							{
								cookie.HttpOnly = true;
							}
							else if (trimmedPart != string.Empty)
							{
								cookie.Name = trimmedPart.Substring(0, part.IndexOf('='));
								cookie.Value = partValue;
							}
						}

						if (string.IsNullOrEmpty(cookie.Domain))
						{
							cookie.Domain = request.Host.Split(':')[0];
						}

						cookies.Add(cookie);
					}
				}
			}
		}
	}

	public class BoaStatement
	{
		public BoaStatement(string url, DateTime startDate, DateTime endDate)
		{
			this.Url = url;
			this.StartDate = startDate;
			this.EndDate = endDate;
		}

		public string Url { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}

	public class BoaTransaction
	{
		public bool Pending { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }
	}
}
