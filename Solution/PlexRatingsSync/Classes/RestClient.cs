using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DS.PlexRatingsSync.Classes
{
  public class RestClient
  {
    public sealed class httpMethod
    {
      public const string Get = "GET";
      public const string Post = "POST";
      public const string Put = "PUT";
      public const string Delete = "DELETE";
    }

    public sealed class httpContentType
    {
      public const string Json = "application/json";
      public const string Text = "text/xml";
      public const string Form = "application/x-www-form-urlencoded";
      public const string CSV = "text/csv";
    }

    #region Private Members

    Dictionary<string, string> _Headers = new Dictionary<string, string>();

    Dictionary<string, string> _Parameters = new Dictionary<string, string>();

    Uri _Endpoint;

    string _Method = httpMethod.Get;

    string _ContentType = httpContentType.Json;

    string _RequestData;

    // The Accept header is a restricted header type, so we need to store this seperately
    string _AcceptHeader;

    // The Authorization header is a restricted header type, so we need to store this seperately
    string _AuthorizationBearer;

    string _AuthorizationBasic;

    #endregion

    #region Constructors

    public RestClient(Uri endpoint, string method = httpMethod.Get, string contentType = httpContentType.Json)
    {
      Clear();

      _Endpoint = endpoint;

      _Method = method;

      _ContentType = contentType;
    }

    #endregion

    #region Public Static Methods

    public static string Base64Encode(string plainText)
    {
      var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

      return Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
      var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

      return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    #endregion

    #region Fluent Interface

    public static RestClient Create(Uri endpoint, string method = httpMethod.Get, string contentType = httpContentType.Json)
    {
      return new RestClient(endpoint, method, contentType);
    }

    public RestClient Clear()
    {
      _Endpoint = null;

      _Method = httpMethod.Get;

      _ContentType = httpContentType.Json;

      _AcceptHeader = string.Empty;

      _AuthorizationBearer = string.Empty;

      _AuthorizationBasic = string.Empty;

      _RequestData = string.Empty;

      _Headers.Clear();

      _Parameters.Clear();

      return this;
    }

    public RestClient AcceptHeader(string acceptHeader)
    {
      _AcceptHeader = acceptHeader;

      return this;
    }

    public RestClient AuthorizationBearer(string bearerToken)
    {
      _AuthorizationBearer = bearerToken;

      return this;
    }

    public RestClient AuthorizationBasic(string username, string password)
    {
      _AuthorizationBasic = Base64Encode($"{username}:{password}");

      return this;
    }

    public RestClient RequestData(string requestData)
    {
      _RequestData = requestData;

      return this;
    }

    public RestClient AddHeader(string name, string value)
    {
      if (_Headers.ContainsKey(name))
        _Headers[name] = value;
      else
        _Headers.Add(name, value);

      return this;
    }

    public RestClient AddParameter(string name, string value)
    {
      if (_Parameters.ContainsKey(name))
        _Parameters[name] = value;
      else
        _Parameters.Add(name, value);

      return this;
    }

    /// <summary>
    /// Sends the request message and returns the response. This method will catch any non 200 responses and extracts the error information and returns it.
    /// </summary>
    /// <returns>The response string</returns>
    public string SendRequestWithExceptionResponse()
    {
      var responseValue = string.Empty;

      try
      {
        responseValue = SendRequest();
      }
      catch (WebException ex)
      {
        // Non 200 status codes will be caught here

        if (ex.Response != null)
        {
          using (var responseStream = ((HttpWebResponse)ex.Response).GetResponseStream())
          {
            if (responseStream != null)
            {
              using (var reader = new StreamReader(responseStream, true))
              {
                responseValue = reader.ReadToEnd();
              }
            }
          }
        }
        // Allows for error message to be returned if the exception message is populated.
        else if (!string.IsNullOrEmpty(ex.Message))
        {
          responseValue = ex.Message;
        }

        return responseValue;
      }
      catch (Exception)
      {
        throw;
      }

      return responseValue;
    }

    /// <summary>
    /// Sends the request message and returns the response. Any non 200 status codes will throw an error.
    /// It is preferred to use SendRequestWithExceptionResponse() which catches non 200 responses and extracts the error information.
    /// </summary>
    /// <returns>The response string</returns>
    public string SendRequest()
    {
      // Build the uri for the endpoint
      var endpoint = new UriBuilder(_Endpoint);

      // Fixed bug where multiple parameters weren't concatinated correctly.

      // Add any request parameters
      if (_Parameters.Count > 0)
      {
        var queryParams = new List<string>();

        foreach (var parameter in _Parameters)
        {
          queryParams.Add($"{parameter.Key}={parameter.Value}");
        }

        endpoint.Query = string.Join("&", queryParams);
      }

      var request = (HttpWebRequest)WebRequest.Create(endpoint.Uri);

      request.Method = _Method;

      if (!string.IsNullOrEmpty(_AcceptHeader))
        request.Accept = _AcceptHeader;

      if (!string.IsNullOrEmpty(_AuthorizationBearer))
        request.Headers[HttpRequestHeader.Authorization] = $"Bearer {_AuthorizationBearer}";

      if (!string.IsNullOrEmpty(_AuthorizationBasic))
        request.Headers[HttpRequestHeader.Authorization] = $"Basic {_AuthorizationBasic}";

      // Add any additional headers
      foreach (var header in _Headers)
      {
        if (!WebHeaderCollection.IsRestricted(header.Key))
          request.Headers.Add(header.Key, header.Value);
      }

      HttpWebResponse response = null;

      var responseValue = string.Empty;

      try
      {
        // Removed verb restrictions; if we have some request data, then we should just send it with the request.

        // Any request data to send with a post?
        if ((!string.IsNullOrEmpty(_RequestData)) && request.Method.ToUpperInvariant() == httpMethod.Post || request.Method.ToUpperInvariant() == httpMethod.Put)
        {
          request.ContentType = _ContentType;

          var bytes = Encoding.UTF8.GetBytes(_RequestData);

          request.ContentLength = bytes.Length;

          using (var writeStream = request.GetRequestStream())
          {
            writeStream.Write(bytes, 0, bytes.Length);
          }
        }

        response = (HttpWebResponse)request.GetResponse();

        using (var responseStream = response.GetResponseStream())
        {
          if (responseStream != null)
          {
            using (var reader = new StreamReader(responseStream, true))
            {
              responseValue = reader.ReadToEnd();
            }
          }
        }
      }
      catch (Exception)
      {
        // Non 200 status codes will be caught here
        throw;
      }
      finally
      {
        if (response != null)
        {
          ((IDisposable)response).Dispose();
        }
      }

      return responseValue;
    }

    #endregion
  }
}
