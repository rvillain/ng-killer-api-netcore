using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NgKillerApiCore;
using NgKillerApiCore.DAL;
namespace WebPush
{
    public class WebPushService
    {
        private KillerContext _context;
        private IConfiguration _configuration;
        private WebPushClient _webPushClient;
        private ILogger<WebPushService> _logger;
        public WebPushService(KillerContext context, IConfiguration configuration, WebPushClient webPushClient, ILogger<WebPushService> logger)
        {
            _context = context;
            _configuration = configuration;
            _webPushClient = webPushClient;
            _logger = logger;
        }

        public void SendPushNotification(string agentId, string requestType)
        {
            string title = "Killer";
            string message = "Killer Notif";
            switch (requestType)
            {
                case Constantes.REQUEST_TYPE_ASK_KILL:
                    message = "On vous a tué";
                    break;
                case Constantes.REQUEST_TYPE_ASK_UNMASK:
                    message = "On vous a démasqué";
                    break;
                case Constantes.ACTTION_TYPE_GAME_STARTED:
                    message = "La partie est lancée";
                    break;
            }

            var payload = string.Format("{{\"title\": \"{0}\", \"message\": \"{1}\", \"agentId\":\"{2}\"}}", title, message, agentId);
            var device = _context.Devices.Where(m => m.Name == agentId).OrderByDescending(m => m.Id).FirstOrDefault();
            if (device != null)
            {
                try
                {
                    string vapidPublicKey = _configuration.GetSection("VapidKeys")["PublicKey"];
                    string vapidPrivateKey = _configuration.GetSection("VapidKeys")["PrivateKey"];

                    var pushSubscription = new PushSubscription(device.PushEndpoint, device.PushP256DH, device.PushAuth);
                    var vapidDetails = new VapidDetails("mailto:example@example.com", vapidPublicKey, vapidPrivateKey);

                    _webPushClient.SendNotification(pushSubscription, payload, vapidDetails);
                }
                catch(Exception ex){
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void Broadcast(string message)
        {
            var devices = _context.Devices.ToList();
            foreach (var device in devices)
            {
                try
                {
                    string vapidPublicKey = _configuration.GetSection("VapidKeys")["PublicKey"];
                    string vapidPrivateKey = _configuration.GetSection("VapidKeys")["PrivateKey"];

                    var pushSubscription = new PushSubscription(device.PushEndpoint, device.PushP256DH, device.PushAuth);
                    var vapidDetails = new VapidDetails("mailto:example@example.com", vapidPublicKey, vapidPrivateKey);

                    _webPushClient.SendNotification(pushSubscription, message, vapidDetails);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.StackTrace, ex);
                }

            }
        }
    }
}