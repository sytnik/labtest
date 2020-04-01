using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LabTest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LabTest.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() =>
            View(new SimpleViewModel(SimpleMock.Employees.Select(e =>
                    new SimpleViewItem(e.Id, e.Name, e.Surname, Enum.GetName(typeof(Position), e.Position)))
                .OrderBy(e => e.Id).ToList()));

        public IActionResult AddEmployee()
        {
            Positions2View();
            return View();
        }

        [HttpPost]
        public IActionResult AddEmployee(Employee employee)
        {
            if (!ModelState.IsValid) return View();
            employee.Id = SimpleMock.Employees.Max(e => e.Id) + 1;
            SimpleMock.Employees.Add(employee);
            return RedirectToAction("Index");
        }

        public IActionResult EditEmployee(int id)
        {
            Positions2View();
            return SimpleMock.Employees.Any(e => e.Id == id)
                ? View(SimpleMock.Employees.First(e => e.Id == id))
                : (IActionResult) RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditEmployee(Employee employee)
        {
            if (!ModelState.IsValid) return View();
            SimpleMock.Employees[SimpleMock.Employees.FindIndex(e => e.Id == employee.Id)] = employee;
            return RedirectToAction("Index");
        }

        private void Positions2View()
        {
            var list = new List<SelectListItem>();
            for (var i = 0; i < Enum.GetNames(typeof(Position)).Length; i++)
                list.Add(new SelectListItem {Text = Enum.GetName(typeof(Position), i), Value = i.ToString()});
            ViewBag.Positions = new SelectList(list, "Value", "Text");
        }

        public IActionResult RemoveEmployee(int id)
        {
            if (SimpleMock.Employees.All(e => e.Id != id)) return RedirectToAction("Index");
            SimpleMock.Employees.Remove(SimpleMock.Employees.First(e => e.Id == id));
            return RedirectToAction("Index");
        }

        public IActionResult Login() =>
            User.IsInRole("Admin") ? (IActionResult) RedirectToAction("Index") : View();

        [HttpPost]
        public async Task<IActionResult> Login(Auth auth)
        {
            if (!ModelState.IsValid) return View();
            var user = UserMock.Users.FirstOrDefault(u =>
                u.Login == auth.Login && u.Password == auth.Password);
            if (user == null) return View();
            await Authenticate(user);
            return RedirectToAction("Index");
        }

        private async Task Authenticate(User user)
        {
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                    },
                    "ApplicationCookie",
                    ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType))
            );
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }
    }
}