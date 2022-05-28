using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace MyCuisine.Web.Helpers
{
    public static class FirebaseHelper
    {
        public static FirebaseAuth GetFirebaseAuth(string firebaseAdminConfig)
        {
            if (string.IsNullOrWhiteSpace(firebaseAdminConfig)) throw new ArgumentNullException(nameof(firebaseAdminConfig));
            
            string instanceName = "Firebase";
            var firebaseApp = FirebaseApp.GetInstance(instanceName);
            if (firebaseApp == null)
            {
                try
                {
                    firebaseApp = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromJson(firebaseAdminConfig)
                    }, instanceName);
                }
                catch
                {
                    // if Instance was created in another thread
                    firebaseApp = FirebaseApp.GetInstance(instanceName);
                }
            }
            return FirebaseAuth.GetAuth(firebaseApp);
        }
    }
}
