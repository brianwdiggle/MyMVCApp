using System;
using System.Web;
using MyMVCAppCS;

[Serializable]
public sealed class SessionSingleton
{


    private const string SESSION_SINGLETON_NAME = "Singleton_502E69E5-668B-E011-951F-00155DF26207";

    private SessionSingleton()
    {

    }

    public static SessionSingleton Current
    {
        get
        {
            if (HttpContext.Current.Session[SESSION_SINGLETON_NAME] == null)
            {
                HttpContext.Current.Session[SESSION_SINGLETON_NAME] = new SessionSingleton();
            }

            return HttpContext.Current.Session[SESSION_SINGLETON_NAME] as SessionSingleton;
        }
    }

    /// <summary>
    /// Session Variable indicating whether I'm at work (no images displayed) or home (images displayed)
    /// </summary>
    public string UsageLocation { get; set; }

    /// <summary>
    /// Connection string driven from web.config app key DataTierTarget
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Driven from web.config app key DataTierTarget
    /// </summary>
    public string DataTierTarget { get; set; }
}
