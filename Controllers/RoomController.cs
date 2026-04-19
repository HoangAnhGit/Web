using Microsoft.AspNetCore.Mvc;
using QuanLyChoThuePhongTro.Models;
using QuanLyChoThuePhongTro.Services;
using QuanLyChoThuePhongTro.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyChoThuePhongTro.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IRoomRepository _roomRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RoomController(IRoomService roomService, IRoomRepository roomRepository, IWebHostEnvironment webHostEnvironment)
        {
            _roomService = roomService;
            _roomRepository = roomRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // Helper: lưu danh sách file ảnh và trả về chuỗi đường dẫn cách nhau bởi dấu phẩy
        private async Task<string?> SaveImagesAsync(IFormFileCollection files)
        {
            if (files == null || files.Count == 0) return null;

            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "rooms");
            Directory.CreateDirectory(uploadFolder);

            var savedPaths = new List<string>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(ext)) continue;

                    var uniqueName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadFolder, uniqueName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                    savedPaths.Add($"/images/rooms/{uniqueName}");
                }
            }
            return savedPaths.Count > 0 ? string.Join(",", savedPaths) : null;
        }

        // GET: Room
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            // Landlord chỉ thấy phòng của chính họ
            if (userId.HasValue && userRole == "Landlord")
            {
                var myRooms = await _roomService.GetRoomsByOwnerAsync(userId.Value);
                return View(myRooms);
            }

            // Tenant/Admin/public vẫn xem danh sách chung
            var rooms = await _roomService.GetAllRoomsAsync();
            return View(rooms);
        }

        // GET: Room/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            ViewBag.UserRole = HttpContext.Session.GetString("Role");
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");

            return View(room);
        }

        // GET: Room/Search
        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        // POST: Room/Search
        [HttpPost]
        public async Task<IActionResult> Search(RoomFilter filter)
        {
            if (filter == null)
            {
                return PartialView("_RoomList", new List<Room>());
            }

            if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue && filter.MinPrice > filter.MaxPrice)
            {
                ModelState.AddModelError("", "Giá tối thiểu không được lớn hơn giá tối đa.");
                return PartialView("_RoomList", new List<Room>());
            }

            if (filter.MinArea.HasValue && filter.MaxArea.HasValue && filter.MinArea > filter.MaxArea)
            {
                ModelState.AddModelError("", "Diện tích tối thiểu không được lớn hơn diện tích tối đa.");
                return PartialView("_RoomList", new List<Room>());
            }

            var rooms = await _roomService.SearchRoomsAsync(filter);
            return PartialView("_RoomList", rooms);
        }

        // GET: Room/Create
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            if (!userId.HasValue || userRole != "Landlord")
            {
                return Redirect("/Auth/Login");
            }

            return View();
        }

        // POST: Room/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room, IFormFileCollection roomImages)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");

            if (!userId.HasValue || userRole != "Landlord")
            {
                return Redirect("/Auth/Login");
            }

            if (!ModelState.IsValid)
            {
                return View(room);
            }

            room.OwnerId = userId.Value;
            room.CreatedDate = System.DateTime.UtcNow;

            var savedImages = await SaveImagesAsync(roomImages);
            if (savedImages != null)
            {
                room.ImageUrls = savedImages;
            }

            await _roomService.AddRoomAsync(room);
            return RedirectToAction(nameof(Index));
        }

        // GET: Room/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue || room.OwnerId != userId.Value)
            {
                return Forbid();
            }

            return View(room);
        }

        // POST: Room/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room room, IFormFileCollection roomImages, string? existingImageUrls)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return Redirect("/Auth/Login");
            }

            var existingRoom = await _roomService.GetRoomByIdAsync(id);
            if (existingRoom == null)
            {
                return NotFound();
            }

            if (existingRoom.OwnerId != userId.Value)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                room.ImageUrls = existingImageUrls;
                return View(room);
            }

            room.OwnerId = existingRoom.OwnerId;
            room.CreatedDate = existingRoom.CreatedDate;
            room.UpdatedDate = System.DateTime.UtcNow;

            var newImages = await SaveImagesAsync(roomImages);
            if (newImages != null)
            {
                room.ImageUrls = string.IsNullOrEmpty(existingImageUrls)
                    ? newImages
                    : existingImageUrls + "," + newImages;
            }
            else
            {
                room.ImageUrls = existingImageUrls;
            }

            await _roomService.UpdateRoomAsync(room);
            return RedirectToAction(nameof(Index));
        }

        // GET: Room/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (room.OwnerId != userId)
            {
                return Forbid();
            }

            return View(room);
        }

        // POST: Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room != null)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (room.OwnerId != userId)
                {
                    return Forbid();
                }

                await _roomService.DeleteRoomAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Room/DeleteAjax/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy phòng." });
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue || room.OwnerId != userId.Value)
            {
                return StatusCode(403, new { success = false, message = "Bạn không có quyền xóa phòng này." });
            }

            await _roomService.DeleteRoomAsync(id);

            return Ok(new { success = true, message = "Đã xóa phòng thành công.", roomId = id });
        }

    }
}
