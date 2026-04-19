using Microsoft.AspNetCore.Mvc;
using QuanLyChoThuePhongTro.Services;
using QuanLyChoThuePhongTro.Repositories;
using System.Threading.Tasks;
using System.Linq;

namespace QuanLyChoThuePhongTro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRoomService _roomService;
        private readonly IRentalContractRepository _contractRepository;
        private readonly IPaymentRepository _paymentRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IRoomService roomService,
            IRentalContractRepository contractRepository,
            IPaymentRepository paymentRepository)
        {
            _logger = logger;
            _roomService = roomService;
            _contractRepository = contractRepository;
            _paymentRepository = paymentRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (userRole != "Admin" && userRole != "Landlord")
            {
                return Forbid();
            }

            IEnumerable<Models.Room> rooms;
            IEnumerable<Models.RentalContract> contracts;

            if (userRole == "Landlord")
            {
                rooms = await _roomService.GetRoomsByOwnerAsync(userId.Value);
                contracts = await _contractRepository.GetByLandlordAsync(userId.Value);
            }
            else
            {
                rooms = await _roomService.GetAllRoomsAsync();
                contracts = await _contractRepository.GetAllAsync();
            }

            ViewBag.TotalRooms = rooms.Count();
            ViewBag.TotalContracts = contracts.Count();

            var activeContracts = contracts.Where(c => c.Status == "Active");
            ViewBag.ActiveContracts = activeContracts.Count();

            // Doanh thu th?c t? = t?ng các thanh toán Completed
            var payments = await _paymentRepository.GetAllAsync();
            var completedPayments = payments.Where(p => p.Status == "Completed");

            if (userRole == "Landlord")
            {
                completedPayments = completedPayments.Where(p => p.Contract != null && p.Contract.LandlordId == userId.Value);
            }

            ViewBag.TotalRevenue = completedPayments.Sum(p => p.Amount);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
