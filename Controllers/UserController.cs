using Microsoft.AspNetCore.Mvc;
using QuanLyChoThuePhongTro.Models;
using QuanLyChoThuePhongTro.Repositories;
using System.Threading.Tasks;

namespace QuanLyChoThuePhongTro.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // ================= ADMIN MANAGEMENT =================

        // GET: User (Admin only)
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Admin")
            {
                return Forbid();
            }

            var users = await _userRepository.GetAllAsync();
            return View(users);
        }

        // GET: User/Edit/5 (Admin only)
        public async Task<IActionResult> Edit(int id)
        {
            var adminRole = HttpContext.Session.GetString("Role");
            if (adminRole != "Admin")
            {
                return Forbid();
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            var adminRole = HttpContext.Session.GetString("Role");
            if (adminRole != "Admin")
            {
                return Forbid();
            }

            if (id != user.Id)
            {
                return NotFound();
            }

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) return NotFound();

            // Admin updates role and status
            existingUser.Role = user.Role;
            existingUser.IsActive = user.IsActive;
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Address = user.Address;
            existingUser.UpdatedDate = DateTime.UtcNow;

            await _userRepository.UpdateAsync(existingUser);
            return RedirectToAction(nameof(Index));
        }

        // GET: User/Delete/5 (Admin only)
        public async Task<IActionResult> Delete(int id)
        {
            var adminRole = HttpContext.Session.GetString("Role");
            if (adminRole != "Admin")
            {
                return Forbid();
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent admin from deleting themselves
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (id == currentUserId)
            {
                TempData["Error"] = "Bạn không thể tự xóa tài khoản của chính mình.";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminRole = HttpContext.Session.GetString("Role");
            if (adminRole != "Admin")
            {
                return Forbid();
            }

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (id == currentUserId)
            {
                return BadRequest("Cannot delete yourself");
            }

            await _userRepository.DeleteAsync(id);
            TempData["Success"] = "Đã xóa người dùng thành công.";
            return RedirectToAction(nameof(Index));
        }

        // ================= USER PROFILE =================

        // GET: User/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _userRepository.GetByIdAsync(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(User user)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue || userId.Value != user.Id)
            {
                return Forbid();
            }

            var existingUser = await _userRepository.GetByIdAsync(userId.Value);
            if (existingUser == null) return NotFound();

            // User updates their own info (cannot change role or status via profile)
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Address = user.Address;
            existingUser.UpdatedDate = DateTime.UtcNow;

            await _userRepository.UpdateAsync(existingUser);
            
            TempData["Success"] = "Cập nhật thông tin cá nhân thành công!";
            return View(existingUser);
        }
    }
}
