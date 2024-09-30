using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Models.ViewModels;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Services.IService;
using DrinkCatalog.Utility;
using Microsoft.AspNetCore.Mvc;

namespace DrinkCatalog.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public IActionResult Index()
        {
            var paymentVM = _paymentService.PreparePaymentViewModel();
            return View(paymentVM);
        }

        [HttpPost]
        public IActionResult ProcessPayment(PaymentViewModel paymentVM, string coinCounts)
        {
            var coinCountsDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(coinCounts);
            paymentVM = _paymentService.ProcessPayment(paymentVM, coinCountsDict);

            if (paymentVM.Change == null)
            {
                return View("Index", paymentVM);
            }

            HttpContext.Session.SetInt32(StaticDetails.SessionCart, 0);
            return View("Success", paymentVM);
        }
    }
}
