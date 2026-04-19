using Microsoft.AspNetCore.Mvc;
using QuanLyChoThuePhongTro.Models;
using QuanLyChoThuePhongTro.Repositories;
using System.Threading.Tasks;

namespace QuanLyChoThuePhongTro.Controllers
{
    public class RentalContractController : Controller
    {
        private readonly IRentalContractRepository _contractRepository;
        private readonly IRoomRepository _roomRepository;

        public RentalContractController(IRentalContractRepository contractRepository, IRoomRepository roomRepository)
        {
            _contractRepository = contractRepository;
            _roomRepository = roomRepository;
        }

        // GET: RentalContract
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            IEnumerable<RentalContract> contracts;

            if (userRole == "Landlord")
            {
                contracts = await _contractRepository.GetByLandlordAsync(userId.Value);
            }
            else if (userRole == "Tenant")
            {
                contracts = await _contractRepository.GetByTenantAsync(userId.Value);
            }
            else if (userRole == "Admin")
            {
                contracts = await _contractRepository.GetAllAsync();
            }
            else
            {
                return Forbid(); // tránh role lạ/null thấy toàn bộ
            }

            ViewBag.UserRole = userRole;
            ViewBag.UserId = userId.Value;
            return View(contracts);
        }

        // GET: RentalContract/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: RentalContract/Create/5
        public async Task<IActionResult> Create(int roomId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            if (!userId.HasValue || userRole != "Tenant")
            {
                return RedirectToAction("Login", "Auth");
            }

            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
            {
                return NotFound();
            }

            var contract = new RentalContract
            {
                RoomId = roomId,
                TenantId = userId.Value,
                LandlordId = room.OwnerId,
                MonthlyPrice = room.Price
            };

            return View(contract);
        }

        // POST: RentalContract/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RentalContract contract)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            if (!userId.HasValue || userRole != "Tenant")
            {
                return Forbid();
            }

            // Không tin dữ liệu LandlordId/TenantId/Price từ client
            var room = await _roomRepository.GetByIdAsync(contract.RoomId);
            if (room == null)
            {
                return NotFound();
            }

            contract.TenantId = userId.Value;
            contract.LandlordId = room.OwnerId;
            contract.MonthlyPrice = room.Price;
            contract.Status = "Pending";
            contract.CreatedDate = DateTime.UtcNow;

            contract.StartDate = DateTime.SpecifyKind(contract.StartDate, DateTimeKind.Utc);
            contract.EndDate = DateTime.SpecifyKind(contract.EndDate, DateTimeKind.Utc);

            await _contractRepository.AddAsync(contract);

            return RedirectToAction(nameof(Index));
        }

        // POST: RentalContract/CreateAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax([FromBody] RentalContractCreateRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            if (!userId.HasValue || userRole != "Tenant")
            {
                return StatusCode(403, new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            if (request == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu gửi lên không hợp lệ." });
            }

            if (!DateTime.TryParse(request.StartDate, out var startDate) || !DateTime.TryParse(request.EndDate, out var endDate))
            {
                return BadRequest(new { success = false, message = "Ngày bắt đầu hoặc ngày kết thúc không hợp lệ." });
            }

            if (startDate.Date >= endDate.Date)
            {
                return BadRequest(new { success = false, message = "Ngày bắt đầu phải nhỏ hơn ngày kết thúc." });
            }

            var room = await _roomRepository.GetByIdAsync(request.RoomId);
            if (room == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy phòng cần tạo hợp đồng." });
            }

            var contract = new RentalContract
            {
                RoomId = request.RoomId,
                TenantId = userId.Value,
                LandlordId = room.OwnerId,
                MonthlyPrice = room.Price,
                Deposit = request.Deposit,
                StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc),
                TermsAndConditions = request.TermsAndConditions ?? string.Empty,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            };

            await _contractRepository.AddAsync(contract);

            return Ok(new
            {
                success = true,
                message = "Tạo hợp đồng thành công. Hợp đồng đang chờ chủ nhà xác nhận.",
                data = new
                {
                    contractId = contract.Id,
                    status = contract.Status,
                    contractDetailsUrl = Url.Action("Details", "RentalContract", new { id = contract.Id })
                }
            });
        }

        // GET: RentalContract/Edit/5  — Chỉ Landlord mới được sửa
        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            // Chỉ Landlord sở hữu hợp đồng mới được sửa
            if (userRole != "Landlord" || contract.LandlordId != userId)
            {
                return Forbid();
            }

            return View(contract);
        }

        // POST: RentalContract/Approve/5  — Chỉ Landlord
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            if (!userId.HasValue || userRole != "Landlord")
            {
                return Forbid();
            }

            // Lấy lại từ DB để tránh giả mạo dữ liệu
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            if (contract.LandlordId != userId.Value)
            {
                return Forbid();
            }

            if (contract.Status != "Pending")
            {
                TempData["Error"] = "Chỉ có thể xác nhận hợp đồng đang ở trạng thái chờ xác nhận.";
                return RedirectToAction(nameof(Index));
            }

            contract.Status = "Active";
            contract.UpdatedDate = DateTime.UtcNow;
            await _contractRepository.UpdateAsync(contract);

            // Cập nhật trạng thái phòng sang Đã thuê
            var room = await _roomRepository.GetByIdAsync(contract.RoomId);
            if (room != null)
            {
                room.Status = "Rented";
                room.UpdatedDate = DateTime.UtcNow;
                await _roomRepository.UpdateAsync(room);
            }

            TempData["Success"] = "Đã xác nhận hợp đồng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: RentalContract/ApproveAjax/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveAjax(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            if (!userId.HasValue || userRole != "Landlord")
            {
                return StatusCode(403, new { success = false, message = "Bạn không có quyền xác nhận hợp đồng." });
            }

            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy hợp đồng." });
            }

            if (contract.LandlordId != userId.Value)
            {
                return StatusCode(403, new { success = false, message = "Bạn không sở hữu hợp đồng này." });
            }

            if (contract.Status != "Pending")
            {
                return BadRequest(new { success = false, message = "Hợp đồng không ở trạng thái chờ xác nhận." });
            }

            contract.Status = "Active";
            contract.UpdatedDate = DateTime.UtcNow;
            await _contractRepository.UpdateAsync(contract);

            var room = await _roomRepository.GetByIdAsync(contract.RoomId);
            if (room != null)
            {
                room.Status = "Rented";
                room.UpdatedDate = DateTime.UtcNow;
                await _roomRepository.UpdateAsync(room);
            }

            return Ok(new
            {
                success = true,
                message = "Đã xác nhận hợp đồng thành công.",
                data = new { contractId = contract.Id, status = contract.Status, statusText = "Đang hoạt động", statusClass = "bg-success" }
            });
        }

        // POST: RentalContract/Reject/5  — Chỉ Landlord
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            if (!userId.HasValue || userRole != "Landlord")
            {
                return Forbid();
            }

            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            if (contract.LandlordId != userId.Value)
            {
                return Forbid();
            }

            if (contract.Status != "Pending")
            {
                TempData["Error"] = "Chỉ có thể từ chối hợp đồng đang ở trạng thái chờ xác nhận.";
                return RedirectToAction(nameof(Index));
            }

            contract.Status = "Terminated";
            contract.UpdatedDate = DateTime.UtcNow;
            await _contractRepository.UpdateAsync(contract);

            TempData["Success"] = "Đã từ chối hợp đồng.";
            return RedirectToAction(nameof(Index));
        }

        // POST: RentalContract/RejectAjax/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectAjax(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            if (!userId.HasValue || userRole != "Landlord")
            {
                return StatusCode(403, new { success = false, message = "Bạn không có quyền từ chối hợp đồng." });
            }

            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy hợp đồng." });
            }

            if (contract.LandlordId != userId.Value)
            {
                return StatusCode(403, new { success = false, message = "Bạn không sở hữu hợp đồng này." });
            }

            if (contract.Status != "Pending")
            {
                return BadRequest(new { success = false, message = "Hợp đồng không ở trạng thái chờ xác nhận." });
            }

            contract.Status = "Terminated";
            contract.UpdatedDate = DateTime.UtcNow;
            await _contractRepository.UpdateAsync(contract);

            return Ok(new
            {
                success = true,
                message = "Đã từ chối hợp đồng.",
                data = new { contractId = contract.Id, status = contract.Status, statusText = "Đã kết thúc", statusClass = "bg-secondary" }
            });
        }

        // POST: RentalContract/Edit/5  — Chỉ Landlord
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RentalContract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            // Lấy lại từ DB tránh giả mạo LandlordId từ form
            var existingContract = await _contractRepository.GetByIdAsync(id);
            if (existingContract == null) return NotFound();

            if (userRole != "Landlord" || existingContract.LandlordId != userId)
            {
                return Forbid();
            }

            // Cập nhật trực tiếp trên entity đã track để tránh lỗi duplicate tracking
            existingContract.MonthlyPrice = contract.MonthlyPrice;
            existingContract.Deposit = contract.Deposit;
            existingContract.StartDate = DateTime.SpecifyKind(contract.StartDate, DateTimeKind.Utc);
            existingContract.EndDate = DateTime.SpecifyKind(contract.EndDate, DateTimeKind.Utc);
            existingContract.Status = contract.Status;
            existingContract.TermsAndConditions = contract.TermsAndConditions;
            existingContract.UpdatedDate = DateTime.UtcNow;

            await _contractRepository.UpdateAsync(existingContract);

            return RedirectToAction(nameof(Index));
        }

        // GET: RentalContract/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (contract.LandlordId != userId)
            {
                return Forbid();
            }

            return View(contract);
        }

        // POST: RentalContract/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract != null)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (contract.LandlordId != userId)
                {
                    return Forbid();
                }

                // Nếu xóa hợp đồng đang hiệu lực, trả trạng thái phòng về Available
                var room = await _roomRepository.GetByIdAsync(contract.RoomId);
                if (room != null)
                {
                    room.Status = "Available";
                    await _roomRepository.UpdateAsync(room);
                }

                await _contractRepository.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

    public class RentalContractCreateRequest
    {
        public int RoomId { get; set; }
        public decimal Deposit { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string? TermsAndConditions { get; set; }
    }
    }
}
