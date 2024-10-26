using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayPal.Api;
using Microsoft.Extensions.Configuration;

namespace healthycannab.Services
{
    public class PayPalService
    {
        private readonly APIContext _apiContext;

        public APIContext ApiContext => _apiContext;

        public PayPalService(IConfiguration configuration)
        {
            // Configura el contexto de la API con tus credenciales de PayPal
            var config = configuration.GetSection("PayPal");
            _apiContext = new APIContext(new OAuthTokenCredential(config["ClientId"], config["ClientSecret"]).GetAccessToken())
            {
                Config = new Dictionary<string, string>
                {
                    { "mode", "sandbox" } // "live" o "sandbox"
                }
            };
        }

        public Payment CreatePayment(decimal total, string currency, string returnUrl, string cancelUrl)
        {
            // Crea un objeto de pago
            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        description = "Compra de productos",
                        invoice_number = Guid.NewGuid().ToString(), // Identificador único de la transacción
                        amount = new Amount
                        {
                            currency = currency,
                            total = total.ToString("F2") // Asegúrate de que sea un string con dos decimales
                        }
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    cancel_url = cancelUrl,
                    return_url = returnUrl
                }
            };

            return payment.Create(_apiContext);
        }
    }
}



