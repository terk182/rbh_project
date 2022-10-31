using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Response;

namespace BL.Utilities
{
    public class FireBaseUtility
    {
        public static string getValue(string path)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "uRn5RKAx3Z8DriwTEkP4g0PY6bx4KlJan8XVJKcg",
                BasePath = "https://fir-Gogojii.firebaseio.com/"
            };
            IFirebaseClient client = new FirebaseClient(config);
            FirebaseResponse response = client.Get(path);
            return response.ResultAs<string>(); //The response will contain the data being retreived
        }
    }
}
